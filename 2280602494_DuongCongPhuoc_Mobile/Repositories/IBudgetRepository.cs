using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetBudgetsByEventIdAsync(int eventId);
        Task<Budget?> GetBudgetByIdAsync(int id);
        Task AddBudgetAsync(Budget budget);
        Task UpdateBudgetAsync(Budget budget);
        Task DeleteBudgetAsync(int id);
    }
}
