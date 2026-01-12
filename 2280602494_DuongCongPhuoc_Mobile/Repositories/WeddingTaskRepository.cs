using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class WeddingTaskRepository : IWeddingTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public WeddingTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WeddingTask>> GetTasksByEventIdAsync(int eventId)
        {
            return await _context.Tasks
                .Include(t => t.AssignedToUser)
                .Where(t => t.EventId == eventId && !t.IsHidden)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<WeddingTask?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsHidden);
        }

        public async Task AddTaskAsync(WeddingTask task)
        {
            task.CreatedAt = DateTime.Now;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(WeddingTask task)
        {
            task.UpdatedAt = DateTime.Now;
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                task.IsHidden = true;
                task.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(int total, int completed)> GetTaskStatsAsync(int eventId)
        {
            var total = await _context.Tasks
                .CountAsync(t => t.EventId == eventId && !t.IsHidden);
            
            var completed = await _context.Tasks
                .CountAsync(t => t.EventId == eventId && !t.IsHidden && t.Status == "Completed");

            return (total, completed);
        }
    }
}
