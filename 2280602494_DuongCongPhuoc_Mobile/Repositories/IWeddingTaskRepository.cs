using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IWeddingTaskRepository
    {
        Task<IEnumerable<WeddingTask>> GetTasksByEventIdAsync(int eventId);
        Task<WeddingTask?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(WeddingTask task);
        Task UpdateTaskAsync(WeddingTask task);
        Task DeleteTaskAsync(int id);
        Task<(int total, int completed)> GetTaskStatsAsync(int eventId);
    }
}
