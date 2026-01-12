using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml; // EPPlus
using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestApiController : ControllerBase
    {
        private readonly IGuestRepository _repository;

        public GuestApiController(IGuestRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests(int eventId)
        {
            var guests = await _repository.GetGuestsByEventIdAsync(eventId);
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            var guest = await _repository.GetGuestByIdAsync(id);
            if (guest == null) return NotFound();
            return Ok(guest);
        }

        [HttpPost]
        public async Task<ActionResult<Guest>> CreateGuest(Guest guest)
        {
            await _repository.AddGuestAsync(guest);
            return CreatedAtAction(nameof(GetGuest), new { id = guest.Id }, guest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(int id, Guest guest)
        {
            if (id != guest.Id) return BadRequest();
            await _repository.UpdateGuestAsync(guest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            await _repository.DeleteGuestAsync(id);
            return NoContent();
        }

        [HttpPost("import/{eventId}")]
        public async Task<IActionResult> ImportGuests(int eventId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                var guests = new List<Guest>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        // Assumes Row 1 is Header. Data starts from Row 2.
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var fullName = worksheet.Cells[row, 1].Value?.ToString();
                            if (string.IsNullOrWhiteSpace(fullName)) continue; // Skip empty rows

                            var email = worksheet.Cells[row, 2].Value?.ToString();
                            var phone = worksheet.Cells[row, 3].Value?.ToString();
                            var rawType = worksheet.Cells[row, 4].Value?.ToString()?.ToLower().Trim() ?? "";
                            string finalType = "Khác";

                            if (rawType.Contains("trai") || rawType.Contains("groom")) finalType = "Nhà Trai";
                            else if (rawType.Contains("gái") || rawType.Contains("bride")) finalType = "Nhà Gái";
                            else if (rawType.Contains("bạn") || rawType.Contains("friend")) finalType = "Bạn Bè";
                            else if (rawType.Contains("đồng nghiệp") || rawType.Contains("công ty")) finalType = "Đồng Nghiệp";

                            guests.Add(new Guest
                            {
                                EventId = eventId,
                                FullName = fullName,
                                Email = email,
                                Phone = phone,
                                GuestType = finalType,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }

                if (guests.Any())
                {
                    await _repository.BulkAddGuestsAsync(guests);
                }

                return Ok(new { message = $"Successfully imported {guests.Count} guests." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
