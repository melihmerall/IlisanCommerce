using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // Üye müşteriler için
        public int? UserId { get; set; }
        public User? User { get; set; }

        // Misafir müşteriler için (Session ID)
        [StringLength(100)]
        public string? SessionId { get; set; }

        // Ürün bilgileri
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Varyant bilgileri (renk, boyut vb.)
        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den az olamaz")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; } // Özel notlar

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Computed Properties
        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;

        [NotMapped]
        public bool IsGuestCart => !UserId.HasValue && !string.IsNullOrEmpty(SessionId);

        [NotMapped]
        public string DisplayName => ProductVariant?.VariantName ?? Product?.Name ?? "Ürün";
    }
}
