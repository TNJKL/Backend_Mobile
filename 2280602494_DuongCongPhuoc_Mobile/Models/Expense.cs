namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public int? BudgetId { get; set; } // Optional - có thể không thuộc hạng mục ngân sách cụ thể
        public Budget? Budget { get; set; }
        
        public int? VendorId { get; set; } // Optional - có thể không có nhà cung cấp
        public Vendor? Vendor { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? PaymentMethod { get; set; } // Cash, CreditCard, BankTransfer, etc.
        public string? ReceiptUrl { get; set; }
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
    }
}







