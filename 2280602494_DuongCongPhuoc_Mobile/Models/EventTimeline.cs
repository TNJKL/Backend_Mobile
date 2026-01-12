using System.ComponentModel.DataAnnotations;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class EventTimeline
    {
        [Key]
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public int? VendorId { get; set; }

        public string? PersonInCharge { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public string Status { get; set; } = "Pending"; // Pending, Completed

        public bool IsHidden { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
