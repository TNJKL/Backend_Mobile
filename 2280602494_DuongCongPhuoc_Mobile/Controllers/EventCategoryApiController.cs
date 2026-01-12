using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Cho phép user đã đăng nhập truy cập
    public class EventCategoryApiController : ControllerBase
    {
        private readonly IEventCategoryRepository _categoryRepository;

        public EventCategoryApiController(IEventCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetEventCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventCategoryById(int id)
        {
            try
            {
                var category = await _categoryRepository.GetEventCategoryByIdAsync(id);
                if (category == null)
                    return NotFound();
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")] // Admin và Staff được thêm danh mục
        public async Task<IActionResult> AddEventCategory([FromBody] EventCategory category)
        {
            try
            {
                await _categoryRepository.AddEventCategoryAsync(category);
                return CreatedAtAction(nameof(GetEventCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateEventCategory(int id, [FromBody] EventCategory category)
        {
            try
            {
                if (id != category.Id)
                    return BadRequest();
                await _categoryRepository.UpdateEventCategoryAsync(category);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteEventCategory(int id)
        {
            try
            {
                await _categoryRepository.DeleteEventCategoryAsync(id); // Soft delete
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/hide")]
        [Authorize(Roles = "Admin,Staff")] // Admin và Staff được ẩn danh mục
        public async Task<IActionResult> HideEventCategory(int id)
        {
            try
            {
                await _categoryRepository.DeleteEventCategoryAsync(id);
                return Ok(new { message = "Category hidden successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

