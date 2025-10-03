using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200, ErrorMessage = "Ürün adı 200 karakterden uzun olamaz")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün kodu zorunludur")]
        [StringLength(50, ErrorMessage = "Ürün kodu 50 karakterden uzun olamaz")]
        public string ProductCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(1000, ErrorMessage = "Kısa açıklama 1000 karakterden uzun olamaz")]
        public string ShortDescription { get; set; } = string.Empty;

        public string? LongDescription { get; set; }
        
        public string? Description => LongDescription; // Alias for compatibility

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ComparePrice { get; set; } // Liste fiyatı
        
        public decimal? OriginalPrice => ComparePrice; // Alias for compatibility
        public bool IsOnSale => ComparePrice.HasValue && ComparePrice > Price; // Calculated property

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum stok miktarı negatif olamaz")]
        public int MinStockLevel { get; set; } = 10;

        [StringLength(100)]
        public string? Brand { get; set; } = "ILISAN";

        [StringLength(50)]
        public string? Weight { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Desi değeri 0'dan büyük olmalıdır")]
        public decimal Desi { get; set; } = 1.0m; // Kargo hesaplama için desi bilgisi

        [StringLength(100)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(200)]
        public string? Slug { get; set; }

        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public bool IsSpecialProduct { get; set; } = false; // Özel ürün (NATO standartları vb.)

        [StringLength(100)]
        public string? CertificateType { get; set; } // NATO, TSE vb. sertifika tipleri

        [Range(1, 10)]
        public int? ProtectionLevel { get; set; } // Koruma seviyesi (IIIA vb.)

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ProductImage? MainImage => ProductImages?.FirstOrDefault(i => i.IsMainImage); // Computed property
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
