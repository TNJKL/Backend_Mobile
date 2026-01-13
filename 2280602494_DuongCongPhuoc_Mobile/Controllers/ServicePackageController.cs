using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicePackageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServicePackageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ServicePackage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicePackage>>> GetServicePackages()
        {
            return await _context.ServicePackages
                .Include(sp => sp.ServicePackageItems)
                    .ThenInclude(i => i.GlobalMenuItem)
                .Where(sp => sp.IsActive)
                .ToListAsync();
        }

        // GET: api/ServicePackage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicePackage>> GetServicePackage(int id)
        {
            var servicePackage = await _context.ServicePackages
                .Include(sp => sp.ServicePackageItems)
                    .ThenInclude(i => i.GlobalMenuItem)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (servicePackage == null)
            {
                return NotFound();
            }

            return servicePackage;
        }

        // POST: api/ServicePackage
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServicePackage>> PostServicePackage(ServicePackage servicePackage)
        {
            // Populate value for Food items if not set
            foreach(var item in servicePackage.ServicePackageItems)
            {
                 if (item.ItemType == "Food" && item.ReferenceId.HasValue && item.CustomValue == 0)
                 {
                     var food = await _context.GlobalMenuItems.FindAsync(item.ReferenceId);
                     if (food != null) item.CustomValue = food.UnitPrice ?? 0;
                 }
            }

            _context.ServicePackages.Add(servicePackage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicePackage", new { id = servicePackage.Id }, servicePackage);
        }

        // PUT: api/ServicePackage/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutServicePackage(int id, ServicePackage servicePackage)
        {
            if (id != servicePackage.Id)
            {
                return BadRequest();
            }

            _context.Entry(servicePackage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicePackageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ServicePackage/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServicePackage(int id)
        {
            var servicePackage = await _context.ServicePackages.FindAsync(id);
            if (servicePackage == null)
            {
                return NotFound();
            }

            _context.ServicePackages.Remove(servicePackage);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // POST: api/ServicePackage/Apply/{packageId}/ToEvent/{eventId}
        [HttpPost("Apply/{packageId}/ToEvent/{eventId}")]
        public async Task<IActionResult> ApplyPackageToEvent(int packageId, int eventId, [FromQuery] int tableCount = 1)
        {
            var package = await _context.ServicePackages
                .Include(p => p.ServicePackageItems) 
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if (package == null)
                return NotFound("Package not found");

            var weddingEvent = await _context.Events
                .Include(e => e.Budgets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (weddingEvent == null)
                return NotFound("Event not found");

            Console.WriteLine($"[ApplyPackage] Processing PackageId: {packageId} for EventId: {eventId} with TableCount: {tableCount}");

            // Calculate Total Price
            // Formula: (TotalFood * tableCount) + TotalOtherServices
            decimal foodTotal = package.ServicePackageItems.Where(i => i.ItemType == "Food").Sum(i => i.CustomValue);
            decimal serviceTotal = package.ServicePackageItems.Where(i => i.ItemType != "Food").Sum(i => i.CustomValue);
            
            decimal finalPrice = (foodTotal * tableCount) + serviceTotal;
            Console.WriteLine($"[ApplyPackage] Calculated Price: {finalPrice} (Food: {foodTotal} * {tableCount} + Service: {serviceTotal})");

            // 1. Ensure Event has a Budget Category for Packages
            // Rename category to "Combo Trọn Gói" per user request
            var categoryName = "Combo Trọn Gói";
            // ERROR FIX: Use .FirstOrDefault() because Budgets is ICollection<Budget> (in-memory loaded)
            var packageBudget = weddingEvent.Budgets.FirstOrDefault(b => b.Category == categoryName);

            if (packageBudget == null)
            {
                Console.WriteLine("[ApplyPackage] Creating new Budget category: " + categoryName);
                packageBudget = new Budget
                {
                    EventId = eventId,
                    Category = categoryName,
                    BudgetedAmount = 0, 
                    ActualAmount = 0
                };
                _context.Budgets.Add(packageBudget);
                // Do NOT save here yet, let EF manage the transaction
            }
            else
            {
                Console.WriteLine($"[ApplyPackage] Found existing BudgetId: {packageBudget.Id}");
                
                // CRITICAL FIX 1: If the budget was previously soft-deleted (hidden), un-hide it
                if (packageBudget.IsHidden) 
                {
                    packageBudget.IsHidden = false;
                    Console.WriteLine("[ApplyPackage] Un-hiding existing budget.");
                }

                // CRITICAL FIX 2: Clear OLD expenses to avoid duplication (User request: "lấy đúng 1 lần thôi")
                // We need to fetch existing expenses first to delete them
                var existingExpenses = await _context.Expenses
                    .Where(e => e.BudgetId == packageBudget.Id && !e.IsHidden)
                    .ToListAsync();
                
                if (existingExpenses.Any())
                {
                     Console.WriteLine($"[ApplyPackage] Clearing {existingExpenses.Count} existing expenses.");
                    _context.Expenses.RemoveRange(existingExpenses);
                }

                // Reset amounts before re-calculating
                packageBudget.BudgetedAmount = 0;
                packageBudget.ActualAmount = 0;
            }
            
            // 2. Accumulate Budget
            packageBudget.BudgetedAmount += finalPrice;
            packageBudget.ActualAmount += finalPrice;

            // 3. Create SINGLE Expense for the Package
            var expense = new Expense
            {
                // BudgetId will be handled by navigation property
                EventId = eventId,
                Description = $"{package.Name} (x{tableCount} bàn)", 
                Amount = finalPrice, 
                ExpenseDate = DateTime.Now
            };
            
            packageBudget.Expenses.Add(expense);
            Console.WriteLine("[ApplyPackage] Added expense to budget. Saving changes...");

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã áp dụng gói {package.Name} thành công! Tổng: {finalPrice:N0}đ" });
        }


        private bool ServicePackageExists(int id)
        {
            return _context.ServicePackages.Any(e => e.Id == id);
        }
    }
}
