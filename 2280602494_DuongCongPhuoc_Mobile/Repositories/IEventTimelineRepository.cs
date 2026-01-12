using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IEventTimelineRepository
    {
        Task<IEnumerable<EventTimeline>> GetTimelinesByEventIdAsync(int eventId);
        Task<EventTimeline> GetTimelineByIdAsync(int id);
        Task AddTimelineAsync(EventTimeline timeline);
        Task UpdateTimelineAsync(EventTimeline timeline);
        Task DeleteTimelineAsync(int id);
        Task CreateTimelineTemplateAsync(int eventId, string templateType);
    }
}
