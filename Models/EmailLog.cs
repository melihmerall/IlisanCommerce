using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        [Required]
        public int EmailTemplateId { get; set; }
        public EmailTemplate EmailTemplate { get; set; } = null!;

        [Required(ErrorMessage = "Alıcı email adresi zorunludur")]
        [EmailAddress]
        [StringLength(255)]
        public string ToEmail { get; set; } = string.Empty;

        [StringLength(255)]
        public string? FromEmail { get; set; }

        [Required(ErrorMessage = "Email konusu zorunludur")]
        [StringLength(500)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email içeriği zorunludur")]
        public string Body { get; set; } = string.Empty;

        public EmailStatus Status { get; set; } = EmailStatus.Pending;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? SentDate { get; set; }

        public int? OrderId { get; set; } // İlgili sipariş
        public int? UserId { get; set; }   // İlgili kullanıcı

        public int RetryCount { get; set; } = 0;
        public DateTime? NextRetryDate { get; set; }

        [StringLength(100)]
        public string? Priority { get; set; } = "Normal"; // High, Normal, Low

        // Navigation Properties
        public Order? Order { get; set; }
        public User? User { get; set; }
    }

    public enum EmailStatus
    {
        Pending = 1,    // Beklemede
        Sent = 2,       // Gönderildi
        Failed = 3,     // Başarısız
        Cancelled = 4   // İptal edildi
    }
}
