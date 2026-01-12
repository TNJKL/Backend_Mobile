namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Location { get; set; }
        public int? EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }
        public string? UserId { get; set; } // Người tạo sự kiện
        public User? User { get; set; }
        
        // Thông tin cặp đôi
        public string? BrideName { get; set; }
        public string? GroomName { get; set; }
        
        // Thông tin sự kiện
        public string Status { get; set; } = "Planning"; // Planning, InProgress, Completed, Cancelled
        public int GuestCount { get; set; } = 0;
        public decimal? Budget { get; set; }
        public string? ImageUrl { get; set; }
        
        public bool IsHidden { get; set; } = false; // Soft delete - ẩn sự kiện
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<WeddingTask> Tasks { get; set; } = new List<WeddingTask>();
        public ICollection<EventVendor> EventVendors { get; set; } = new List<EventVendor>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? CreatorName { get; set; }
        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? CreatorRole { get; set; }
    }
}

