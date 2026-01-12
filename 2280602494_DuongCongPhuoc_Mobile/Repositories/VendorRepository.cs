using _2280602494_DuongCongPhuoc_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _context;

        public VendorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            return await _context.Vendors
                .Where(v => !v.IsHidden)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsHidden);
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByTypeAsync(string type)
        {
            return await _context.Vendors
                .Where(v => v.VendorType == type && !v.IsHidden)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task AddVendorAsync(Vendor vendor)
        {
            vendor.CreatedAt = DateTime.Now;
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVendorAsync(Vendor vendor)
        {
            vendor.UpdatedAt = DateTime.Now;
            _context.Entry(vendor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor != null)
            {
                vendor.IsHidden = true;
                vendor.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
