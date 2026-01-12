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
        public async Task<IActionResult> ApplyPackageToEvent(int packageId, int eventId)
        {
            var package = await _context.ServicePackages
                // .Include(p => p.ServicePackageItems) // No longer strictly needed for expense creation if we just use package total, but good to keep if we change logic later
                .FirstOrDefaultAsync(p => p.Id == packageId);

            if (package == null)
                return NotFound("Package not found");

            var weddingEvent = await _context.Events
                .Include(e => e.Budgets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (weddingEvent == null)
                return NotFound("Event not found");

            // 1. Ensure Event has a Budget Category for Packages
            // Rename category to "Combo Trọn Gói" per user request
            var categoryName = "Combo Trọn Gói";
            var packageBudget = weddingEvent.Budgets.FirstOrDefault(b => b.Category == categoryName);

            if (packageBudget == null)
            {
                packageBudget = new Budget
                {
                    EventId = eventId,
                    Category = categoryName,
                    BudgetedAmount = 0, 
                    ActualAmount = 0
                };
                _context.Budgets.Add(packageBudget);
                await _context.SaveChangesAsync();
            }
            
            // 2. Accumulate Budget
            // User wants the budget to increase as packages are added
            packageBudget.BudgetedAmount += package.Price;
            
            // 3. Create SINGLE Expense for the Package (instead of individual items)
            // This reduces clutter in the UI
            var expense = new Expense
            {
                BudgetId = packageBudget.Id,
                EventId = eventId,
                Description = package.Name, // e.g. "Gói VIP 1"
                Amount = package.Price, 
                ExpenseDate = DateTime.Now
            };
            
            _context.Expenses.Add(expense);
            
            // Update Actual Spend
            packageBudget.ActualAmount += package.Price;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã áp dụng gói {package.Name} thành công!" });
        }


        private bool ServicePackageExists(int id)
        {
            return _context.ServicePackages.Any(e => e.Id == id);
        }
    }
}
