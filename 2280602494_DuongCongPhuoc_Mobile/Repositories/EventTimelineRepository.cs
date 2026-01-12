using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class EventTimelineRepository : IEventTimelineRepository
    {
        private readonly ApplicationDbContext _context;

        public EventTimelineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventTimeline>> GetTimelinesByEventIdAsync(int eventId)
        {
            return await _context.EventTimelines
                .Where(t => t.EventId == eventId && !t.IsHidden)
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<EventTimeline> GetTimelineByIdAsync(int id)
        {
            return await _context.EventTimelines.FindAsync(id);
        }

        public async Task AddTimelineAsync(EventTimeline timeline)
        {
            timeline.CreatedAt = DateTime.Now;
            _context.EventTimelines.Add(timeline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTimelineAsync(EventTimeline timeline)
        {
            var existing = await _context.EventTimelines.FindAsync(timeline.Id);
            if (existing != null)
            {
                existing.Title = timeline.Title;
                existing.StartTime = timeline.StartTime;
                existing.EndTime = timeline.EndTime;
                existing.Location = timeline.Location;
                existing.Description = timeline.Description;
                existing.VendorId = timeline.VendorId;
                existing.PersonInCharge = timeline.PersonInCharge;
                existing.DisplayOrder = timeline.DisplayOrder;
                existing.Status = timeline.Status;
                existing.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTimelineAsync(int id)
        {
            var timeline = await _context.EventTimelines.FindAsync(id);
            if (timeline != null)
            {
                timeline.IsHidden = true; // Soft delete
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateTimelineTemplateAsync(int eventId, string templateType)
        {
            var eventDate = await _context.Events.Where(e => e.Id == eventId).Select(e => e.StartTime.Date).FirstOrDefaultAsync();
            if (eventDate == default) eventDate = DateTime.Today;

            var templates = new List<EventTimeline>();

            if (templateType == "Traditional")
            {
                templates.AddRange(new[]
                {
                    new EventTimeline { EventId = eventId, Title = "Trang điểm cô dâu", StartTime = eventDate.AddHours(5), EndTime = eventDate.AddHours(7), Location = "Nhà gái", Description = "Trang điểm, làm tóc", DisplayOrder = 1 },
                    new EventTimeline { EventId = eventId, Title = "Đội bê tráp nhà trai có mặt", StartTime = eventDate.AddHours(8), EndTime = eventDate.AddHours(8).AddMinutes(30), Location = "Nhà trai", Description = "Chuẩn bị sính lễ", DisplayOrder = 2 },
                    new EventTimeline { EventId = eventId, Title = "Di chuyển sang nhà gái", StartTime = eventDate.AddHours(8).AddMinutes(30), EndTime = eventDate.AddHours(9), Location = "Trên đường", Description = "Xe hoa xuất phát", DisplayOrder = 3 },
                    new EventTimeline { EventId = eventId, Title = "Làm lễ rước dâu", StartTime = eventDate.AddHours(9), EndTime = eventDate.AddHours(10), Location = "Nhà gái", Description = "Trao quả, làm lễ gia tiên", DisplayOrder = 4 },
                    new EventTimeline { EventId = eventId, Title = "Đón dâu về nhà trai", StartTime = eventDate.AddHours(10), EndTime = eventDate.AddHours(10).AddMinutes(30), Location = "Nhà gái", Description = "Lên xe hoa", DisplayOrder = 5 },
                    new EventTimeline { EventId = eventId, Title = "Làm lễ tại nhà trai", StartTime = eventDate.AddHours(10).AddMinutes(30), EndTime = eventDate.AddHours(11).AddMinutes(30), Location = "Nhà trai", Description = "Lễ gia tiên nhà trai", DisplayOrder = 6 },
                    new EventTimeline { EventId = eventId, Title = "Tiệc rượu tại nhà hàng", StartTime = eventDate.AddHours(17), EndTime = eventDate.AddHours(21), Location = "Nhà hàng", Description = "Đón khách và đãi tiệc", DisplayOrder = 7 },
                });
            }
            else // Modern
            {
                templates.AddRange(new[]
                {
                    new EventTimeline { EventId = eventId, Title = "First Look (Chụp ảnh)", StartTime = eventDate.AddHours(14), EndTime = eventDate.AddHours(15), Location = "Sân vườn", Description = "Khoảnh khắc lần đầu nhìn thấy nhau", DisplayOrder = 1 },
                    new EventTimeline { EventId = eventId, Title = "Lễ thành hôn (Ceremony)", StartTime = eventDate.AddHours(16), EndTime = eventDate.AddHours(17), Location = "Sảnh lễ", Description = "Trao nhẫn, tuyên thệ", DisplayOrder = 2 },
                    new EventTimeline { EventId = eventId, Title = "Tiệc Cocktail (Pre-dinner)", StartTime = eventDate.AddHours(17), EndTime = eventDate.AddHours(18), Location = "Quầy Bar", Description = "Khách thưởng thức đồ uống nhẹ", DisplayOrder = 3 },
                    new EventTimeline { EventId = eventId, Title = "Nhập tiệc (Dinner)", StartTime = eventDate.AddHours(18), EndTime = eventDate.AddHours(20), Location = "Sảnh tiệc", Description = "Khai tiệc và ăn tối", DisplayOrder = 4 },
                    new EventTimeline { EventId = eventId, Title = "After Party", StartTime = eventDate.AddHours(20), EndTime = eventDate.AddHours(23), Location = "Sàn nhảy", Description = "Giao lưu, nhảy đầm", DisplayOrder = 5 },
                });
            }

            foreach (var t in templates)
            {
                t.CreatedAt = DateTime.Now;
                _context.EventTimelines.Add(t);
            }
            await _context.SaveChangesAsync();
        }
    }
}
