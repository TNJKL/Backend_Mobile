using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetApiController : ControllerBase
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetApiController(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        // GET: api/Budget/event/5
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudgetsByEventId(int eventId)
        {
            var budgets = await _budgetRepository.GetBudgetsByEventIdAsync(eventId);
            return Ok(budgets);
        }

        // GET: api/Budget/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(int id)
        {
            var budget = await _budgetRepository.GetBudgetByIdAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }

        // POST: api/Budget
        [HttpPost]
        public async Task<ActionResult<Budget>> PostBudget(Budget budget)
        {
            await _budgetRepository.AddBudgetAsync(budget);
            return CreatedAtAction("GetBudget", new { id = budget.Id }, budget);
        }

        // PUT: api/Budget/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBudget(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return BadRequest();
            }

            try
            {
                await _budgetRepository.UpdateBudgetAsync(budget);
            }
            catch (Exception ex)
            {
                if (await _budgetRepository.GetBudgetByIdAsync(id) == null)
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

        // DELETE: api/Budget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            await _budgetRepository.DeleteBudgetAsync(id);
            return NoContent();
        }
    }
}
