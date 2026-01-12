using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IGuestRepository
    {
        Task<IEnumerable<Guest>> GetGuestsByEventIdAsync(int eventId);
        Task<Guest> GetGuestByIdAsync(int id);
        Task AddGuestAsync(Guest guest);
        Task UpdateGuestAsync(Guest guest);
        Task DeleteGuestAsync(int id);
        Task BulkAddGuestsAsync(IEnumerable<Guest> guests);
    }
}
