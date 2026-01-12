using _2280602494_DuongCongPhuoc_Mobile.Models;

namespace _2280602494_DuongCongPhuoc_Mobile.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetMenusByEventIdAsync(int eventId);
        Task<Menu?> GetMenuByIdAsync(int id);
        Task AddMenuAsync(Menu menu);
        Task UpdateMenuAsync(Menu menu);
        Task DeleteMenuAsync(int id); // Soft delete
        
        Task<MenuItem?> GetMenuItemByIdAsync(int id);
        Task AddMenuItemAsync(MenuItem item);
        Task UpdateMenuItemAsync(MenuItem item);
        Task DeleteMenuItemAsync(int id);
    }
}

