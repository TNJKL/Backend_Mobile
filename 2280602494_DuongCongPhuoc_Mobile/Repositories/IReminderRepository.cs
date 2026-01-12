using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IReminderRepository
    {
        Task<IEnumerable<Reminder>> GetRemindersAsync();
        Task<IEnumerable<Reminder>> GetRemindersByEventIdAsync(int eventId);
        Task<Reminder?> GetReminderByIdAsync(int id);
        Task AddReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(int id);
        Task<IEnumerable<Reminder>> GetPendingRemindersAsync(DateTime currentTime);
    }
}

