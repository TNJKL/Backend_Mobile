using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<Vendor?> GetVendorByIdAsync(int id);
        Task<IEnumerable<Vendor>> GetVendorsByTypeAsync(string type);
        Task AddVendorAsync(Vendor vendor);
        Task UpdateVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(int id);
    }
}
