using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            return await _context.Events
                .Include(e => e.EventCategory)
                .Include(e => e.User)
                .Where(e => !e.IsHidden)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByUserIdAsync(string userId)
        {
            return await _context.Events
                .Include(e => e.EventCategory)
                .Include(e => e.User)
                .Where(e => e.UserId == userId && !e.IsHidden)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.EventCategory)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsHidden);
        }

        public async Task AddEventAsync(Event eventItem)
        {
            eventItem.CreatedAt = DateTime.Now;
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            eventItem.UpdatedAt = DateTime.Now;
            _context.Entry(eventItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventAsync(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                // Xóa Expenses thủ công vì FK là NoAction (tránh multiple cascade paths)
                var expenses = await _context.Expenses
                    .Where(e => e.EventId == id)
                    .ToListAsync();
                if (expenses.Any())
                {
                    _context.Expenses.RemoveRange(expenses);
                }
                
                // Soft delete - ẩn sự kiện thay vì xóa
                eventItem.IsHidden = true;
                eventItem.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .Include(e => e.EventCategory)
                .Include(e => e.User)
                .Where(e => !e.IsHidden && 
                           e.StartTime <= endDate && 
                           e.EndTime >= startDate)
                .ToListAsync();
        }
    }
}

