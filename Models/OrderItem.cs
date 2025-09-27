using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ProductCode { get; set; }

        [StringLength(200)]
        public string? VariantName { get; set; }

        [StringLength(50)]
        public string? VariantSKU { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den az olamaz")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(500)]
        public string? ProductImageUrl { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Computed Properties
        [NotMapped]
        public decimal TotalPrice 
        { 
            get => (UnitPrice * Quantity) - DiscountAmount;
            set { } // Setter for compatibility
        }

        [NotMapped]
        public string DisplayName => !string.IsNullOrEmpty(VariantName) ? $"{ProductName} - {VariantName}" : ProductName;
    }
}
