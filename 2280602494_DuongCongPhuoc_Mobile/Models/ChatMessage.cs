using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; } // Can be a UserID or 'Admin', 'Staff' group? For now, 1:1 user-to-user (staff is also a User)
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}
