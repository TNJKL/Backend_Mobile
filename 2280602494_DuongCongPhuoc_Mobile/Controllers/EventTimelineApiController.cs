using Microsoft.AspNetCore.Mvc;
using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;

namespace _2280602494_DuongCongPhuoc_Mobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTimelineApiController : ControllerBase
    {
        private readonly IEventTimelineRepository _repository;

        public EventTimelineApiController(IEventTimelineRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<EventTimeline>>> GetTimelines(int eventId)
        {
            var timelines = await _repository.GetTimelinesByEventIdAsync(eventId);
            return Ok(timelines);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventTimeline>> GetTimeline(int id)
        {
            var timeline = await _repository.GetTimelineByIdAsync(id);
            if (timeline == null) return NotFound();
            return Ok(timeline);
        }

        [HttpPost]
        public async Task<ActionResult<EventTimeline>> CreateTimeline(EventTimeline timeline)
        {
            await _repository.AddTimelineAsync(timeline);
            return CreatedAtAction(nameof(GetTimeline), new { id = timeline.Id }, timeline);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeline(int id, EventTimeline timeline)
        {
            if (id != timeline.Id) return BadRequest();
            await _repository.UpdateTimelineAsync(timeline);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeline(int id)
        {
            await _repository.DeleteTimelineAsync(id);
            return NoContent();
        }

        [HttpPost("template/{eventId}")]
        public async Task<IActionResult> CreateTemplate(int eventId, [FromQuery] string type = "Traditional")
        {
            await _repository.CreateTimelineTemplateAsync(eventId, type);
            return Ok(new { message = "Template created successfully" });
        }
    }
}
