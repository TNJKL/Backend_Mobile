using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseApiController : ControllerBase
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseApiController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        // GET: api/Expense/event/5
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByEventId(int eventId)
        {
            var expenses = await _expenseRepository.GetExpensesByEventIdAsync(eventId);
            return Ok(expenses);
        }

        // GET: api/Expense/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _expenseRepository.GetExpenseByIdAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        // POST: api/Expense
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(Expense expense)
        {
            await _expenseRepository.AddExpenseAsync(expense);
            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        // PUT: api/Expense/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            try
            {
                await _expenseRepository.UpdateExpenseAsync(expense);
            }
            catch (Exception ex)
            {
                if (await _expenseRepository.GetExpenseByIdAsync(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Expense/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            await _expenseRepository.DeleteExpenseAsync(id);
            return NoContent();
        }
    }
}
