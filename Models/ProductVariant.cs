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

        [Required(ErrorMessage = "Varyant adi zorunludur")]
        [StringLength(100)]
        public string VariantName { get; set; } = string.Empty; // Ornek: "Mavi - Large"

        [Required(ErrorMessage = "SKU zorunludur")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty; // Stok kodu

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceAdjustment { get; set; } = 0; // Ana urun fiyatina ekleme/cikarma

        // Computed property for view compatibility
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
        [Range(0.01, double.MaxValue, ErrorMessage = "Desi degeri 0'dan buyuk olmalidir")]
        public decimal? Desi { get; set; } // Varyant icin ozel desi bilgisi

        [StringLength(7)] // #FFFFFF formati icin
        public string? ColorCode { get; set; } // Hex renk kodu

        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<ProductVariantImage> VariantImages { get; set; } = new List<ProductVariantImage>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed properties
        [NotMapped]
        public decimal FinalPrice => Product?.Price + (PriceAdjustment ?? 0) ?? 0;

        [NotMapped]
        public bool IsInStock => StockQuantity > 0;

        [NotMapped]
        public bool IsLowStock => StockQuantity <= MinStockLevel && StockQuantity > 0;

        [NotMapped]
        public bool IsDeleted { get; set; } = false;
    }
}
