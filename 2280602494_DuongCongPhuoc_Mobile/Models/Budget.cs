namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Budget
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public string Category { get; set; } = string.Empty; // Venue, Catering, Decoration, Photography, etc.
        public decimal BudgetedAmount { get; set; }
        public decimal ActualAmount { get; set; } = 0;
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
        
        // Navigation property
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}







