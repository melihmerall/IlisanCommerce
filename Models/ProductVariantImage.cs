using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class ProductVariantImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;

        [Required(ErrorMessage = "Resim yolu zorunludur")]
        [StringLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AltText { get; set; }

        [StringLength(500)]
        public string? Caption { get; set; }

        public bool IsMainImage { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public ImageType ImageType { get; set; } = ImageType.Product;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum ImageType
    {
        Product = 1,        // Ürün resmi
        Detail = 2,         // Detay resmi
        Lifestyle = 3,      // Kullanım resmi
        Certificate = 4,    // Sertifika resmi
        Technical = 5       // Teknik çizim
    }
}
