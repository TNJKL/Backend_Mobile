using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetExpensesByEventIdAsync(int eventId)
        {
            return await _context.Expenses
                .Include(e => e.Budget)
                .Where(e => e.EventId == eventId && !e.IsHidden)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id)
        {
            return await _context.Expenses
                .Include(e => e.Budget)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsHidden);
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            expense.CreatedAt = DateTime.Now;
            _context.Expenses.Add(expense);
            
            // Update actual amount in budget if linked
            if (expense.BudgetId.HasValue)
            {
                var budget = await _context.Budgets.FindAsync(expense.BudgetId.Value);
                if (budget != null)
                {
                    budget.ActualAmount += expense.Amount;
                    budget.UpdatedAt = DateTime.Now; // Update timestamp
                }
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            var oldExpense = await _context.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == expense.Id);
            
            expense.UpdatedAt = DateTime.Now;
            _context.Entry(expense).State = EntityState.Modified;
            
            // Recalculate budget actual amount if amount changed or budgetId changed
            if (oldExpense != null && (oldExpense.Amount != expense.Amount || oldExpense.BudgetId != expense.BudgetId))
            {
                // Revert old budget
                if (oldExpense.BudgetId.HasValue)
                {
                    var oldBudget = await _context.Budgets.FindAsync(oldExpense.BudgetId.Value);
                    if (oldBudget != null)
                    {
                        oldBudget.ActualAmount -= oldExpense.Amount;
                    }
                }
                
                // Update new budget
                if (expense.BudgetId.HasValue)
                {
                    var newBudget = await _context.Budgets.FindAsync(expense.BudgetId.Value);
                    if (newBudget != null)
                    {
                        newBudget.ActualAmount += expense.Amount;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                expense.IsHidden = true;
                expense.UpdatedAt = DateTime.Now;
                
                // Revert budget amount
                if (expense.BudgetId.HasValue)
                {
                    var budget = await _context.Budgets.FindAsync(expense.BudgetId.Value);
                    if (budget != null)
                    {
                        budget.ActualAmount -= expense.Amount;
                    }
                }
                
                await _context.SaveChangesAsync();
            }
        }
    }
}
