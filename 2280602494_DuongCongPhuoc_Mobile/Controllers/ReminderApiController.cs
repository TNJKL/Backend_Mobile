using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReminderApiController : ControllerBase
    {
        private readonly IReminderRepository _reminderRepository;

        public ReminderApiController(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetReminders()
        {
            try
            {
                var reminders = await _reminderRepository.GetRemindersAsync();
                return Ok(reminders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetRemindersByEventId(int eventId)
        {
            try
            {
                var reminders = await _reminderRepository.GetRemindersByEventIdAsync(eventId);
                return Ok(reminders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminderById(int id)
        {
            try
            {
                var reminder = await _reminderRepository.GetReminderByIdAsync(id);
                if (reminder == null)
                    return NotFound();
                return Ok(reminder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReminder([FromBody] Reminder reminder)
        {
            try
            {
                await _reminderRepository.AddReminderAsync(reminder);
                return CreatedAtAction(nameof(GetReminderById), new { id = reminder.Id }, reminder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, [FromBody] Reminder reminder)
        {
            try
            {
                if (id != reminder.Id)
                    return BadRequest();
                await _reminderRepository.UpdateReminderAsync(reminder);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            try
            {
                await _reminderRepository.DeleteReminderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingReminders()
        {
            try
            {
                var reminders = await _reminderRepository.GetPendingRemindersAsync(DateTime.Now);
                return Ok(reminders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

