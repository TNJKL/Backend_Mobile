using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuApiController : ControllerBase
    {
        private static readonly HashSet<string> AllowedMealTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Breakfast", "Lunch", "Dinner", "Snack"
        };
        
        private readonly IMenuRepository _menuRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ApplicationDbContext _context;

        public MenuApiController(IMenuRepository menuRepository, IEventRepository eventRepository, ApplicationDbContext context)
        {
            _menuRepository = menuRepository;
            _eventRepository = eventRepository;
            _context = context;
        }

        private bool IsAdminOrStaff()
        {
            var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            return userRoles.Contains("Admin") || userRoles.Contains("Staff");
        }

        private async Task<bool> CanAccessEventAsync(int eventId, string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            if (IsAdminOrStaff()) return true;
            var evt = await _eventRepository.GetEventByIdAsync(eventId);
            return evt != null && evt.UserId == userId;
        }

        [HttpGet("by-event/{eventId}")]
        public async Task<IActionResult> GetMenusByEvent(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(eventId, userId)) return Forbid();
            var menus = await _menuRepository.GetMenusByEventIdAsync(eventId);
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var menu = await _menuRepository.GetMenuByIdAsync(id);
            if (menu == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(menu.EventId, userId)) return Forbid();
            return Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] Menu menu)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(menu.EventId, userId)) return Forbid();
            if (string.IsNullOrWhiteSpace(menu.Name)) return BadRequest("Menu name is required");
            if (string.IsNullOrWhiteSpace(menu.MealType) || !AllowedMealTypes.Contains(menu.MealType))
                return BadRequest("MealType must be one of Breakfast, Lunch, Dinner, Snack");
            await _menuRepository.AddMenuAsync(menu);
            return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] Menu menu)
        {
            if (!IsAdminOrStaff()) return Forbid();
            if (id != menu.Id) return BadRequest();
            var existing = await _menuRepository.GetMenuByIdAsync(id);
            if (existing == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(existing.EventId, userId)) return Forbid();
            if (!string.IsNullOrWhiteSpace(menu.MealType) && !AllowedMealTypes.Contains(menu.MealType))
                return BadRequest("MealType must be one of Breakfast, Lunch, Dinner, Snack");
            existing.Name = menu.Name;
            existing.Description = menu.Description;
            existing.MealType = string.IsNullOrEmpty(menu.MealType) ? existing.MealType : menu.MealType;
            await _menuRepository.UpdateMenuAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var existing = await _menuRepository.GetMenuByIdAsync(id);
            if (existing == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(existing.EventId, userId)) return Forbid();
            await _menuRepository.DeleteMenuAsync(id);
            return NoContent();
        }

        [HttpPost("{menuId}/items")]
        public async Task<IActionResult> AddMenuItem(int menuId, [FromBody] MenuItem item)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var parent = await _menuRepository.GetMenuByIdAsync(menuId);
            if (parent == null) return NotFound("Menu not found");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(parent.EventId, userId)) return Forbid();
            item.MenuId = menuId;
            await _menuRepository.AddMenuItemAsync(item);
            return CreatedAtAction(nameof(GetMenuItem), new { id = item.Id }, item);
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            var item = await _menuRepository.GetMenuItemByIdAsync(id);
            if (item == null) return NotFound();
            var menu = await _menuRepository.GetMenuByIdAsync(item.MenuId);
            if (menu == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(menu.EventId, userId)) return Forbid();
            return Ok(item);
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItem item)
        {
            if (!IsAdminOrStaff()) return Forbid();
            if (id != item.Id) return BadRequest();
            var existing = await _menuRepository.GetMenuItemByIdAsync(id);
            if (existing == null) return NotFound();
            var menu = await _menuRepository.GetMenuByIdAsync(existing.MenuId);
            if (menu == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(menu.EventId, userId)) return Forbid();
            existing.Name = item.Name;
            existing.Category = item.Category;
            existing.Description = item.Description;
            existing.Quantity = item.Quantity;
            existing.UnitPrice = item.UnitPrice;
            existing.Notes = item.Notes;
            existing.DisplayOrder = item.DisplayOrder;
            await _menuRepository.UpdateMenuItemAsync(existing);
            return NoContent();
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            if (!IsAdminOrStaff()) return Forbid();
            var existing = await _menuRepository.GetMenuItemByIdAsync(id);
            if (existing == null) return NotFound();
            var menu = await _menuRepository.GetMenuByIdAsync(existing.MenuId);
            if (menu == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(menu.EventId, userId)) return Forbid();
            await _menuRepository.DeleteMenuItemAsync(id);
            return NoContent();
        }

        [HttpPost("{eventId}/place-order")]
        public async Task<IActionResult> PlaceOrder(int eventId, [FromBody] PlaceOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(eventId, userId)) return Forbid();
            if (request?.Items == null || request.Items.Count == 0) return BadRequest("No items to order");
            
            var now = DateTime.Now;
            foreach (var it in request.Items)
            {
                var menuItem = await _menuRepository.GetMenuItemByIdAsync(it.MenuItemId);
                decimal unit = 0;
                string itemName = "";
                if (menuItem != null)
                {
                    unit = menuItem.UnitPrice ?? 0;
                    itemName = menuItem.Name;
                }
                else
                {
                    var globalItem = await _context.GlobalMenuItems.FindAsync(it.MenuItemId);
                    if (globalItem == null) return NotFound($"Menu item {it.MenuItemId} not found");
                    unit = globalItem.UnitPrice ?? 0;
                    itemName = globalItem.Name;
                }
                var amount = unit * it.Quantity;
                var expense = new Expense
                {
                    EventId = eventId,
                    BudgetId = null,
                    VendorId = null,
                    Description = $"Order: {itemName}",
                    Amount = amount,
                    ExpenseDate = now,
                    PaymentMethod = "MenuOrder",
                    ReceiptUrl = null,
                    Notes = $"MenuItemId={it.MenuItemId}, Qty={it.Quantity}",
                    CreatedAt = now,
                    UpdatedAt = null,
                    IsHidden = false
                };
                _context.Expenses.Add(expense);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order placed", count = request.Items.Count });
        }
        public class PlaceOrderRequest
        {
            public List<OrderItem> Items { get; set; } = new();
        }

        public class OrderItem
        {
            public int MenuItemId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpGet("{eventId}/orders")]
        public async Task<IActionResult> GetOrders(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(eventId, userId)) return Forbid();
            var orders = _context.Expenses
                .Where(e => e.EventId == eventId && e.PaymentMethod == "MenuOrder" && !e.IsHidden)
                .OrderBy(e => e.ExpenseDate)
                .ThenBy(e => e.Description)
                .ToList();
            var items = new List<object>();
            decimal total = 0;
            foreach (var ex in orders)
            {
                int itemId = 0;
                int qty = 0;
                var notes = ex.Notes ?? "";
                foreach (var part in notes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (part.StartsWith("MenuItemId="))
                    {
                        int.TryParse(part.Substring("MenuItemId=".Length), out itemId);
                    }
                    else if (part.StartsWith("Qty="))
                    {
                        int.TryParse(part.Substring("Qty=".Length), out qty);
                    }
                }
                string name = ex.Description ?? "Order item";
                decimal unitPrice = 0;
                string? imageUrl = null;
                bool isGlobal = false;
                var menuItem = await _menuRepository.GetMenuItemByIdAsync(itemId);
                if (menuItem != null)
                {
                    name = menuItem.Name;
                    unitPrice = menuItem.UnitPrice ?? 0;
                }
                else
                {
                    var globalItem = await _context.GlobalMenuItems.FindAsync(itemId);
                    if (globalItem != null)
                    {
                        isGlobal = true;
                        name = globalItem.Name;
                        unitPrice = globalItem.UnitPrice ?? 0;
                        imageUrl = globalItem.ImageUrl;
                    }
                }
                var lineTotal = unitPrice * qty;
                total += lineTotal;
                items.Add(new
                {
                    expenseId = ex.Id,
                    menuItemId = itemId,
                    name,
                    quantity = qty,
                    unitPrice,
                    totalPrice = lineTotal,
                    imageUrl,
                    isGlobal
                });
            }
            return Ok(new { items, total });
        }

        public class UpdateOrderRequest
        {
            public int Quantity { get; set; }
        }

        [HttpPut("{eventId}/orders/{expenseId}")]
        public async Task<IActionResult> UpdateOrder(int eventId, int expenseId, [FromBody] UpdateOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(eventId, userId)) return Forbid();
            var ex = await _context.Expenses.FindAsync(expenseId);
            if (ex == null || ex.EventId != eventId) return NotFound();
            int itemId = 0;
            foreach (var part in (ex.Notes ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (part.StartsWith("MenuItemId="))
                {
                    int.TryParse(part.Substring("MenuItemId=".Length), out itemId);
                }
            }
            decimal unitPrice = 0;
            var menuItem = await _menuRepository.GetMenuItemByIdAsync(itemId);
            if (menuItem != null)
            {
                unitPrice = menuItem.UnitPrice ?? 0;
            }
            else
            {
                var globalItem = await _context.GlobalMenuItems.FindAsync(itemId);
                if (globalItem != null) unitPrice = globalItem.UnitPrice ?? 0;
            }
            var qty = Math.Max(0, request.Quantity);
            ex.Amount = unitPrice * qty;
            ex.Notes = $"MenuItemId={itemId}, Qty={qty}";
            ex.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { expenseId = ex.Id, quantity = qty, unitPrice, totalPrice = ex.Amount });
        }

        [HttpDelete("{eventId}/orders/{expenseId}")]
        public async Task<IActionResult> DeleteOrder(int eventId, int expenseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!await CanAccessEventAsync(eventId, userId)) return Forbid();
            var ex = await _context.Expenses.FindAsync(expenseId);
            if (ex == null || ex.EventId != eventId) return NotFound();
            ex.IsHidden = true;
            ex.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
