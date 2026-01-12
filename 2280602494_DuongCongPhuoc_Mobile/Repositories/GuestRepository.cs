using Microsoft.EntityFrameworkCore;
using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ApplicationDbContext _context;

        public GuestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Guest>> GetGuestsByEventIdAsync(int eventId)
        {
            return await _context.Guests
                .Where(g => g.EventId == eventId && !g.IsHidden)
                .OrderBy(g => g.FullName)
                .ToListAsync();
        }

        public async Task<Guest> GetGuestByIdAsync(int id)
        {
            return await _context.Guests.FindAsync(id);
        }

        public async Task AddGuestAsync(Guest guest)
        {
            guest.CreatedAt = DateTime.Now;
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGuestAsync(Guest guest)
        {
            var existing = await _context.Guests.FindAsync(guest.Id);
            if (existing != null)
            {
                existing.FullName = guest.FullName;
                existing.Email = guest.Email;
                existing.Phone = guest.Phone;
                existing.GuestType = guest.GuestType;
                existing.RSVPStatus = guest.RSVPStatus;
                existing.PlusOneCount = guest.PlusOneCount;
                existing.TableNumber = guest.TableNumber;
                existing.DietaryRequirements = guest.DietaryRequirements;
                existing.Notes = guest.Notes;
                existing.GiftReceived = guest.GiftReceived;
                existing.GiftDescription = guest.GiftDescription;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGuestAsync(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                guest.IsHidden = true; // Soft delete
                await _context.SaveChangesAsync();
            }
        }

        public async Task BulkAddGuestsAsync(IEnumerable<Guest> guests)
        {
            foreach (var guest in guests)
            {
                guest.CreatedAt = DateTime.Now;
            }
            await _context.Guests.AddRangeAsync(guests);
            await _context.SaveChangesAsync();
        }
    }
}
