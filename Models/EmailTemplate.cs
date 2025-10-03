using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class EmailTemplate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Template adı zorunludur")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email konusu zorunludur")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email içeriği zorunludur")]
        public string Body { get; set; } = string.Empty;

        public EmailTemplateType TemplateType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
    }

    public enum EmailTemplateType
    {
        OrderConfirmation = 1,      // Sipariş onayı
        OrderStatusUpdate = 2,      // Sipariş durumu güncelleme
        ShippingNotification = 3,   // Kargo bildirimi
        DeliveryConfirmation = 4,   // Teslimat onayı
        OrderCancellation = 5,      // Sipariş iptali
        NewUserWelcome = 6,         // Yeni üye hoş geldiniz
        PasswordReset = 7,          // Şifre sıfırlama
        ContactForm = 8,            // İletişim formu
        AdminOrderNotification = 9  // Admin sipariş bildirimi
    }
}
