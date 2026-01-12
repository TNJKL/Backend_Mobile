using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class EventApiController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<User> _userManager;

        public EventApiController(IEventRepository eventRepository, UserManager<User> userManager)
        {
            _eventRepository = eventRepository;
            _userManager = userManager;
        }

        private async Task PopulateCreatorInfo(IEnumerable<Event> events)
        {
            foreach (var evt in events)
            {
                if (evt.User != null)
                {
                    evt.CreatorName = evt.User.UserName;
                    var roles = await _userManager.GetRolesAsync(evt.User);
                    evt.CreatorRole = roles.FirstOrDefault() ?? "User";
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Kiểm tra quyền: Admin/Staff thấy tất cả, User chỉ thấy của mình
                var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                bool isAdminOrStaff = userRoles.Contains("Admin") || userRoles.Contains("Staff");

                IEnumerable<Event> events;

                if (isAdminOrStaff)
                {
                    // Admin/Staff: Xem tất cả sự kiện
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        events = await _eventRepository.GetEventsByDateRangeAsync(startDate.Value, endDate.Value);
                    }
                    else
                    {
                        events = await _eventRepository.GetEventsAsync();
                    }
                }
                else
                {
                    // User: Chỉ xem sự kiện của chính mình
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        // Lấy sự kiện của user trong khoảng thời gian
                        var allUserEvents = await _eventRepository.GetEventsByUserIdAsync(userId);
                        events = allUserEvents.Where(e => 
                            e.StartTime <= endDate.Value && 
                            (e.EndTime == null || e.EndTime >= startDate.Value));
                    }
                    else
                    {
                        events = await _eventRepository.GetEventsByUserIdAsync(userId);
                    }
                }

                await PopulateCreatorInfo(events);
                
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var events = await _eventRepository.GetEventsByUserIdAsync(userId);
                await PopulateCreatorInfo(events);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var eventItem = await _eventRepository.GetEventByIdAsync(id);
                if (eventItem == null)
                    return NotFound();
                
                await PopulateCreatorInfo(new[] { eventItem });
                
                return Ok(eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        // Cho phép tất cả User đã đăng nhập tạo event của chính họ
        public async Task<IActionResult> AddEvent([FromBody] Event eventItem)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                
                eventItem.UserId = userId;
                await _eventRepository.AddEventAsync(eventItem);
                return CreatedAtAction(nameof(GetEventById), new { id = eventItem.Id }, eventItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        // Cho phép User sửa event của chính họ, Admin/Staff sửa tất cả
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventItem)
        {
            try
            {
                if (id != eventItem.Id)
                    return BadRequest();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                
                // Kiểm tra quyền: Admin/Staff có thể sửa tất cả, User chỉ sửa event của mình
                var existingEvent = await _eventRepository.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound();

                var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                bool isAdminOrStaff = userRoles.Contains("Admin") || userRoles.Contains("Staff");
                
                if (!isAdminOrStaff && existingEvent.UserId != userId)
                    return Forbid();

                // Cập nhật các trường thông tin cơ bản
                existingEvent.Title = eventItem.Title;
                existingEvent.Description = eventItem.Description;
                existingEvent.StartTime = eventItem.StartTime;
                existingEvent.EndTime = eventItem.EndTime;
                existingEvent.Location = eventItem.Location;
                existingEvent.EventCategoryId = eventItem.EventCategoryId;
                
                // Cập nhật các trường mới cho sự kiện cưới
                existingEvent.BrideName = eventItem.BrideName;
                existingEvent.GroomName = eventItem.GroomName;
                existingEvent.Status = !string.IsNullOrEmpty(eventItem.Status) ? eventItem.Status : existingEvent.Status;
                existingEvent.Budget = eventItem.Budget;
                existingEvent.ImageUrl = eventItem.ImageUrl;
                existingEvent.GuestCount = eventItem.GuestCount;
                
                // Cập nhật UpdatedAt
                existingEvent.UpdatedAt = DateTime.Now;
                
                // Không cập nhật UserId và CreatedAt
                // existingEvent.UserId = ... (Giữ nguyên)
                
                await _eventRepository.UpdateEventAsync(existingEvent);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        // Cho phép User xóa event của chính họ, Admin/Staff xóa tất cả
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                
                // Kiểm tra quyền: Admin/Staff có thể xóa tất cả, User chỉ xóa event của mình
                var existingEvent = await _eventRepository.GetEventByIdAsync(id);
                if (existingEvent == null)
                    return NotFound();

                var userRoles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                bool isAdminOrStaff = userRoles.Contains("Admin") || userRoles.Contains("Staff");
                
                if (!isAdminOrStaff && existingEvent.UserId != userId)
                    return Forbid();

                await _eventRepository.DeleteEventAsync(id); // Soft delete
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideEvent(int id)
        {
            try
            {
                await _eventRepository.DeleteEventAsync(id);
                return Ok(new { message = "Event hidden successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

