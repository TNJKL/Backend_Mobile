using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GlobalMenuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GlobalMenuApiController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _context.GlobalMenuItems.Where(i => !i.IsHidden).OrderBy(i => i.DisplayOrder).ThenBy(i => i.Name).ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.GlobalMenuItems.FirstOrDefault(i => i.Id == id && !i.IsHidden);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] GlobalMenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name)) return BadRequest("Name is required");
            _context.GlobalMenuItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] GlobalMenuItem item)
        {
            if (id != item.Id) return BadRequest();
            var existing = await _context.GlobalMenuItems.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = item.Name;
            existing.Category = item.Category;
            existing.Description = item.Description;
            existing.UnitPrice = item.UnitPrice;
            existing.Notes = item.Notes;
            existing.ImageUrl = item.ImageUrl;
            existing.DisplayOrder = item.DisplayOrder;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.GlobalMenuItems.FindAsync(id);
            if (existing == null) return NotFound();
            existing.IsHidden = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
