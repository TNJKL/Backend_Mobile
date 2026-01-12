using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class EventVendorRepository : IEventVendorRepository
    {
        private readonly ApplicationDbContext _context;

        public EventVendorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventVendor>> GetEventVendorsAsync(int eventId)
        {
            return await _context.EventVendors
                .Include(ev => ev.Vendor)
                .Where(ev => ev.EventId == eventId)
                .OrderBy(ev => ev.Vendor.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventVendor>> GetEventVendorsByVendorIdAsync(int vendorId)
        {
            return await _context.EventVendors
                .Include(ev => ev.Vendor)
                 // Start of fix for including Event details
                .Include(ev => ev.Event)
                 // End of fix
                .Where(ev => ev.VendorId == vendorId)
                .OrderByDescending(ev => ev.CreatedAt)
                .ToListAsync();
        }

        public async Task<EventVendor?> GetEventVendorByIdAsync(int id)
        {
            return await _context.EventVendors
                .Include(ev => ev.Vendor)
                .FirstOrDefaultAsync(ev => ev.Id == id);
        }

        public async Task AddEventVendorAsync(EventVendor eventVendor)
        {
            eventVendor.CreatedAt = DateTime.Now;
            _context.EventVendors.Add(eventVendor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEventVendorAsync(EventVendor eventVendor)
        {
            eventVendor.UpdatedAt = DateTime.Now;
            _context.Entry(eventVendor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEventVendorAsync(int id)
        {
            var eventVendor = await _context.EventVendors.FindAsync(id);
            if (eventVendor != null)
            {
                _context.EventVendors.Remove(eventVendor); // Hard delete for link table is often okay, but we can do soft if needed. 
                // For now, hard delete to match requirements simpler.
                await _context.SaveChangesAsync();
            }
        }
    }
}
