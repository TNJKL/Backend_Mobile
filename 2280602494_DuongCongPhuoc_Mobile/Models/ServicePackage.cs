using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class ServicePackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for items included in this package
        public ICollection<ServicePackageItem> ServicePackageItems { get; set; }
    }
}
