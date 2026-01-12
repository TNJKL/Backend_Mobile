namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? GuestType { get; set; } // Family, Friend, Colleague, Other
        
        // RSVP
        public string RSVPStatus { get; set; } = "Pending"; // Pending, Confirmed, Declined
        public int PlusOneCount { get; set; } = 0;
        
        // Bàn tiệc
        public int? TableNumber { get; set; }
        
        // Yêu cầu đặc biệt
        public string? DietaryRequirements { get; set; }
        public string? Notes { get; set; }
        
        // Quà cưới
        public bool GiftReceived { get; set; } = false;
        public string? GiftDescription { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
    }
}







