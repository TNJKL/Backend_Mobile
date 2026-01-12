namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class WeddingTask
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        public string? AssignedToUserId { get; set; } // FK to AspNetUsers
        public User? AssignedToUser { get; set; }

        public string Category { get; set; } = "General"; // e.g., "Legal", "Venue", "Attire"
        
        public string Priority { get; set; } = "Normal"; // High, Normal, Low
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
        
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        
        // Sub-tasks support
        public int? ParentTaskId { get; set; }
        public WeddingTask? ParentTask { get; set; }
        public ICollection<WeddingTask> SubTasks { get; set; } = new List<WeddingTask>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; } = false;
    }
}

