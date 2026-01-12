using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ApplicationDbContext _context;

        public ReminderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reminder>> GetRemindersAsync()
        {
            return await _context.Reminders
                .Include(r => r.Event)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersByEventIdAsync(int eventId)
        {
            return await _context.Reminders
                .Include(r => r.Event)
                .Where(r => r.EventId == eventId)
                .ToListAsync();
        }

        public async Task<Reminder?> GetReminderByIdAsync(int id)
        {
            return await _context.Reminders
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            reminder.CreatedAt = DateTime.Now;
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            _context.Entry(reminder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder != null)
            {
                _context.Reminders.Remove(reminder);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Reminder>> GetPendingRemindersAsync(DateTime currentTime)
        {
            return await _context.Reminders
                .Include(r => r.Event)
                .Where(r => !r.IsNotified && r.ReminderTime <= currentTime)
                .ToListAsync();
        }
    }
}

