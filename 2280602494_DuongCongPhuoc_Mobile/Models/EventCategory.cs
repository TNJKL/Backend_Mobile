namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class EventCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; } // Màu sắc để hiển thị trên calendar
        public string? Description { get; set; }
        public bool IsHidden { get; set; } = false; // Soft delete - ẩn danh mục
    }
}

