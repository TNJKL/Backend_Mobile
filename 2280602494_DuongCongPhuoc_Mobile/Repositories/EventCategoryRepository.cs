using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EventCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCategory>> GetEventCategoriesAsync()
        {
            return await _context.EventCategories
                .Where(c => !c.IsHidden)
                .ToListAsync();
        }

        public async Task<EventCategory?> GetEventCategoryByIdAsync(int id)
        {
            return await _context.EventCategories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsHidden);
        }

        public async Task AddEventCategoryAsync(EventCategory category)
        {
            _context.EventCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventCategoryAsync(EventCategory category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventCategoryAsync(int id)
        {
            var category = await _context.EventCategories.FindAsync(id);
            if (category != null)
            {
                // Soft delete - ẩn danh mục
                category.IsHidden = true;
                
                // Cập nhật tất cả events thuộc danh mục này thành null
                var events = await _context.Events
                    .Where(e => e.EventCategoryId == id)
                    .ToListAsync();
                
                foreach (var eventItem in events)
                {
                    eventItem.EventCategoryId = null;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}

