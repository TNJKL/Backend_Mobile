using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class PaymentMilestone
    {
        [Key]
        public int Id { get; set; }

        public int EventVendorId { get; set; }
        [ForeignKey("EventVendorId")]
        public EventVendor? EventVendor { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } // e.g., "Đặt cọc lần 1", "Thanh toán đợt 2"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
