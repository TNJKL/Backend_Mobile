namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string VendorType { get; set; } = string.Empty; // Restaurant, Decoration, Photography, etc.
        
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        
        public decimal? Rating { get; set; } // 0.00 to 5.00
        public string? Notes { get; set; }
        public bool IsFavorite { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
        
        // Navigation properties
        public ICollection<EventVendor> EventVendors { get; set; } = new List<EventVendor>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}







