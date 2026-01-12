namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; } // Appetizer, MainCourse, Dessert, Beverage
        public string? Description { get; set; }
        
        public int Quantity { get; set; } = 0;
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Notes { get; set; }
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsHidden { get; set; } = false;
    }
}







