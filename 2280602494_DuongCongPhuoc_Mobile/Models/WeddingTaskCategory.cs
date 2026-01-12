using System.ComponentModel.DataAnnotations;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class WeddingTaskCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
