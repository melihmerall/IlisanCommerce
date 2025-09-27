using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class AdminActivityLog
    {
        public int Id { get; set; }

        [Required]
        public int AdminUserId { get; set; }
        public AdminUser AdminUser { get; set; } = null!;

        [Required(ErrorMessage = "Aktivite türü zorunludur")]
        [StringLength(50)]
        public string ActivityType { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout

        [Required(ErrorMessage = "Aktivite açıklaması zorunludur")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityType { get; set; } // Product, Order, User vb.

        public int? EntityId { get; set; } // İlgili entity'nin ID'si

        [StringLength(1000)]
        public string? OldValues { get; set; } // JSON formatında eski değerler

        [StringLength(1000)]
        public string? NewValues { get; set; } // JSON formatında yeni değerler

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Computed Properties
        [NotMapped]
        public string ActivityDisplayName => ActivityType switch
        {
            "Create" => "Oluşturma",
            "Update" => "Güncelleme",
            "Delete" => "Silme",
            "Login" => "Giriş",
            "Logout" => "Çıkış",
            "View" => "Görüntüleme",
            _ => ActivityType
        };
    }
}
