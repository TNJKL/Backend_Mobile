using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280602494_DuongCongPhuoc_Mobile.Models
{
    public class ServicePackageItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ServicePackageId { get; set; }

        [ForeignKey("ServicePackageId")]
        public ServicePackage? ServicePackage { get; set; }

        [Required]
        public string ItemType { get; set; } // 'Food' or 'Service'

        // If ItemType == 'Food', this links to a GlobalMenuItem
        public int? ReferenceId { get; set; }
        
        [ForeignKey("ReferenceId")]
        public GlobalMenuItem? GlobalMenuItem { get; set; }

        // If ItemType == 'Service', these fields describe the custom service 

        // If ItemType == 'Service', these fields describe the custom service
        public string CustomName { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CustomValue { get; set; }
    }
}
