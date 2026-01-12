using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IEventVendorRepository
    {
        Task<IEnumerable<EventVendor>> GetEventVendorsAsync(int eventId);
        Task<IEnumerable<EventVendor>> GetEventVendorsByVendorIdAsync(int vendorId); // New method
        Task<EventVendor?> GetEventVendorByIdAsync(int id);
        Task AddEventVendorAsync(EventVendor eventVendor);
        Task UpdateEventVendorAsync(EventVendor eventVendor);
        Task DeleteEventVendorAsync(int id);
    }
}
