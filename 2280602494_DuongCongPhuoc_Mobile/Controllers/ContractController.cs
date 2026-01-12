using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/contract/event/{eventId}
        // Get all contracts (EventVendors) for a specific event
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetContractsByEvent(int eventId)
        {
            var contracts = await _context.EventVendors
                .Include(ev => ev.Vendor)
                .Where(ev => ev.EventId == eventId)
                .Select(ev => new
                {
                    ev.Id,
                    ev.EventId,
                    ev.VendorId,
                    VendorName = ev.Vendor.Name,
                    VendorType = ev.Vendor.VendorType,
                    ev.ServiceDescription,
                    ContractAmount = ev.ContractAmount ?? 0,
                    ev.Status,
                    // Calculate paid amount from milestones
                    PaidAmount = _context.PaymentMilestones
                        .Where(pm => pm.EventVendorId == ev.Id && pm.IsPaid)
                        .Sum(pm => pm.Amount),
                    // Remaining is ContractAmount - PaidAmount
                    RemainingAmount = (ev.ContractAmount ?? 0) - _context.PaymentMilestones
                        .Where(pm => pm.EventVendorId == ev.Id && pm.IsPaid)
                        .Sum(pm => pm.Amount)
                })
                .ToListAsync();

            return Ok(contracts);
        }

        // GET: api/contract/{contractId}/milestones
        [HttpGet("{contractId}/milestones")]
        public async Task<ActionResult<IEnumerable<PaymentMilestone>>> GetMilestones(int contractId)
        {
            return await _context.PaymentMilestones
                .Where(pm => pm.EventVendorId == contractId)
                .OrderBy(pm => pm.DueDate)
                .ToListAsync();
        }

        // POST: api/contract/milestone
        [HttpPost("milestone")]
        public async Task<ActionResult<PaymentMilestone>> CreateMilestone(PaymentMilestone milestone)
        {
            if (milestone.EventVendorId == 0) return BadRequest("Contract ID is required.");

            _context.PaymentMilestones.Add(milestone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMilestones), new { contractId = milestone.EventVendorId }, milestone);
        }

        // PUT: api/contract/milestone/{id}/pay
        [HttpPut("milestone/{id}/pay")]
        public async Task<IActionResult> PayMilestone(int id)
        {
            var milestone = await _context.PaymentMilestones
                .Include(pm => pm.EventVendor)
                .FirstOrDefaultAsync(pm => pm.Id == id);

            if (milestone == null) return NotFound("Milestone not found.");
            if (milestone.IsPaid) return BadRequest("Milestone already paid.");

            // 1. Mark as Paid
            milestone.IsPaid = true;
            milestone.PaidDate = DateTime.Now;

            // 2. Create Expense automatically
            var eventId = milestone.EventVendor.EventId;
            var expenseAmount = milestone.Amount;
            
            // Find or Create Budget Category "Hợp đồng & Dịch vụ"
            var budgetCategoryName = "Hợp đồng & Dịch vụ";
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.EventId == eventId && b.Category == budgetCategoryName);

            if (budget == null)
            {
                budget = new Budget
                {
                    EventId = eventId,
                    Category = budgetCategoryName,
                    BudgetedAmount = 0, // Customer can adjust later
                    ActualAmount = 0
                };
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync(); // Save to get BudgetId
            }

            // Update Budget Actual Amount
            budget.ActualAmount += expenseAmount;

            var expense = new Expense
            {
                EventId = eventId,
                BudgetId = budget.Id,
                VendorId = milestone.EventVendor.VendorId,
                Description = $"Thanh toán: {milestone.Name} ({milestone.EventVendor.Vendor?.Name ?? "Vendor"})",
                Amount = expenseAmount,
                ExpenseDate = DateTime.Now
                // PaymentMethod left empty or default
            };

            _context.Expenses.Add(expense);

            // 3. Update EventVendor Balance/Deposit if needed
            // This is optional if we rely purely on milestones calculation, 
            // but for backward compatibility let's update EventVendor.DepositAmount
            milestone.EventVendor.DepositAmount = (milestone.EventVendor.DepositAmount ?? 0) + expenseAmount;
            milestone.EventVendor.BalanceAmount = (milestone.EventVendor.ContractAmount ?? 0) - milestone.EventVendor.DepositAmount;


            await _context.SaveChangesAsync();

            return Ok(new { Message = "Payment recorded successfully", Milestone = milestone });
        }
        
        // DELETE: api/contract/milestone/{id}
        [HttpDelete("milestone/{id}")]
        public async Task<IActionResult> DeleteMilestone(int id)
        {
             var milestone = await _context.PaymentMilestones.FindAsync(id);
             if (milestone == null) return NotFound();
             
             if (milestone.IsPaid) return BadRequest("Cannot delete a paid milestone. Please unpay it first (feature pending) or delete the expense.");

             _context.PaymentMilestones.Remove(milestone);
             await _context.SaveChangesAsync();
             return NoContent();
        }
    }
}
