using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetMenusByEventIdAsync(int eventId)
        {
            return await _context.Menus
                .Include(m => m.MenuItems)
                .Where(m => m.EventId == eventId && !m.IsHidden)
                .OrderBy(m => m.MealType)
                .ThenBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Menu?> GetMenuByIdAsync(int id)
        {
            return await _context.Menus
                .Include(m => m.MenuItems.OrderBy(i => i.DisplayOrder))
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsHidden);
        }

        public async Task AddMenuAsync(Menu menu)
        {
            menu.CreatedAt = DateTime.Now;
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMenuAsync(Menu menu)
        {
            menu.UpdatedAt = DateTime.Now;
            _context.Entry(menu).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMenuAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                menu.IsHidden = true;
                menu.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            return await _context.MenuItems.FirstOrDefaultAsync(mi => mi.Id == id && !mi.IsHidden);
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            if (item.UnitPrice.HasValue)
            {
                item.TotalPrice = item.UnitPrice.Value * item.Quantity;
            }
            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMenuItemAsync(MenuItem item)
        {
            if (item.UnitPrice.HasValue)
            {
                item.TotalPrice = item.UnitPrice.Value * item.Quantity;
            }
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMenuItemAsync(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item != null)
            {
                item.IsHidden = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

