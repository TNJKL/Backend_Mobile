namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public string MealType { get; set; } = string.Empty; // Breakfast, Lunch, Dinner, Snack
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
        
        // Navigation property
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}







