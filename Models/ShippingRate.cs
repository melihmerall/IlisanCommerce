using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class ShippingRate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kargo adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kargo adı 100 karakterden uzun olamaz")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Minimum desi değeri zorunludur")]
        [Column(TypeName = "decimal(8,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum desi değeri 0'dan küçük olamaz")]
        public decimal MinDesi { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Maksimum desi değeri 0'dan küçük olamaz")]
        public decimal? MaxDesi { get; set; } // null ise sınırsız

        [Required(ErrorMessage = "Kargo ücreti zorunludur")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Kargo ücreti 0'dan büyük olmalıdır")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Ücretsiz kargo limiti 0'dan küçük olamaz")]
        public decimal? FreeShippingThreshold { get; set; } // Bu tutar üzerinde ücretsiz kargo

        [StringLength(50)]
        public string? EstimatedDeliveryDays { get; set; } // "1-2 gün", "3-5 gün" vb.

        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false; // Varsayılan kargo seçeneği

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Computed Properties
        [NotMapped]
        public string DesiRange => MaxDesi.HasValue 
            ? $"{MinDesi} - {MaxDesi} desi" 
            : $"{MinDesi}+ desi";

        [NotMapped]
        public string DisplayName => $"{Name} ({ShippingCost:C})";
    }
}
