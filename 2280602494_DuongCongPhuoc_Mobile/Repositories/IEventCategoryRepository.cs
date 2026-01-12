using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IEventCategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetEventCategoriesAsync();
        Task<EventCategory?> GetEventCategoryByIdAsync(int id);
        Task AddEventCategoryAsync(EventCategory category);
        Task UpdateEventCategoryAsync(EventCategory category);
        Task DeleteEventCategoryAsync(int id); // Soft delete
    }
}

