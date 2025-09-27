using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required(ErrorMessage = "Varyant adı zorunludur")]
        [StringLength(100)]
        public string VariantName { get; set; } = string.Empty; // Örn: "Mavi - Large"

        [Required(ErrorMessage = "SKU zorunludur")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty; // Stok Kodu

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceAdjustment { get; set; } = 0; // Ana ürün fiyatına ekleme/çıkarma
        
        // Calculated property for view compatibility
        public decimal Price => Product?.Price + (PriceAdjustment ?? 0) ?? 0;

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int MinStockLevel { get; set; } = 5;

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(20)]
        public string? Size { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(50)]
        public string? Weight { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Desi değeri 0'dan büyük olmalıdır")]
        public decimal? Desi { get; set; } // Varyant için özel desi bilgisi (null ise ana ürünün desi bilgisini kullan)

        [StringLength(7)] // #FFFFFF formatı için
        public string? ColorCode { get; set; } // Hex color code

        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<ProductVariantImage> VariantImages { get; set; } = new List<ProductVariantImage>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed Properties
        [NotMapped]
        public decimal FinalPrice => Product?.Price + (PriceAdjustment ?? 0) ?? 0;

        [NotMapped]
        public bool IsInStock => StockQuantity > 0;

        [NotMapped]
        public bool IsLowStock => StockQuantity <= MinStockLevel && StockQuantity > 0;
    }
}
