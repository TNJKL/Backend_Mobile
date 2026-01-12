using Microsoft.AspNetCore.Mvc;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventVendorApiController : ControllerBase
    {
        private readonly IEventVendorRepository _repository;

        public EventVendorApiController(IEventVendorRepository repository)
        {
            _repository = repository;
        }

        // GET: api/EventVendorApi/event/5 (Get all vendors for an event)
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<EventVendor>>> GetEventVendors(int eventId)
        {
            return Ok(await _repository.GetEventVendorsAsync(eventId));
        }

        // GET: api/EventVendorApi/vendor/5 (Get all contracts for a vendor)
        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<EventVendor>>> GetVendorContracts(int vendorId)
        {
            return Ok(await _repository.GetEventVendorsByVendorIdAsync(vendorId));
        }

        // POST: api/EventVendorApi
        [HttpPost]
        public async Task<ActionResult<EventVendor>> CreateEventVendor(EventVendor eventVendor)
        {
            await _repository.AddEventVendorAsync(eventVendor);
            return Ok(eventVendor);
        }

        // PUT: api/EventVendorApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventVendor(int id, EventVendor eventVendor)
        {
            if (id != eventVendor.Id) return BadRequest();
            await _repository.UpdateEventVendorAsync(eventVendor);
            return NoContent();
        }

        // DELETE: api/EventVendorApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventVendor(int id)
        {
            await _repository.DeleteEventVendorAsync(id);
            return NoContent();
        }
    }
}
