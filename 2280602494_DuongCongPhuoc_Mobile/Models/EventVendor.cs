namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class EventVendor
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public int VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        
        public string? ServiceDescription { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        
        public DateTime? ContractDate { get; set; }
        public DateTime? ServiceDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled
        
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}







