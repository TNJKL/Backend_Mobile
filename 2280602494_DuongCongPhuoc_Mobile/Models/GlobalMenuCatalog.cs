using System.ComponentModel.DataAnnotations;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class GlobalMenuCatalog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
