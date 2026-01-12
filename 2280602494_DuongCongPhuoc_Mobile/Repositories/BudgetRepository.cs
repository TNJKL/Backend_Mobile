using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Budget>> GetBudgetsByEventIdAsync(int eventId)
        {
            return await _context.Budgets
                .Include(b => b.Expenses.Where(e => !e.IsHidden))
                .Where(b => b.EventId == eventId && !b.IsHidden)
                .ToListAsync();
        }

        public async Task<Budget?> GetBudgetByIdAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Expenses.Where(e => !e.IsHidden))
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsHidden);
        }

        public async Task AddBudgetAsync(Budget budget)
        {
            budget.CreatedAt = DateTime.Now;
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            budget.UpdatedAt = DateTime.Now;
            _context.Entry(budget).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null)
            {
                budget.IsHidden = true;
                budget.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
