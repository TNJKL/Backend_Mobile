namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime ReminderTime { get; set; } // Thời gian nhắc nhở
        public string? Message { get; set; } // Nội dung nhắc nhở
        public bool IsNotified { get; set; } = false; // Đã gửi thông báo chưa
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

