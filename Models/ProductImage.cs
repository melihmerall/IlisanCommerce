using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Resim yolu zorunludur")]
        [StringLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AltText { get; set; }

        public bool IsMainImage { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
