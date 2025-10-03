using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class ProductSpecification
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Özellik adı zorunludur")]
        [StringLength(100)]
        public string SpecificationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Özellik değeri zorunludur")]
        [StringLength(500)]
        public string SpecificationValue { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Unit { get; set; } // Birim (kg, cm, adet vb.)

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
