using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sipariş numarası zorunludur")]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        // Müşteri bilgileri
        public int? UserId { get; set; } // Üye müşteriler için
        public User? User { get; set; }

        // Misafir müşteri bilgileri
        [StringLength(100)]
        public string? GuestEmail { get; set; }

        [StringLength(100)]
        public string? GuestFirstName { get; set; }

        [StringLength(100)]
        public string? GuestLastName { get; set; }

        [StringLength(20)]
        public string? GuestPhone { get; set; }

        // String adres alanları (view uyumluluğu için)
        [StringLength(100)]
        public string? ShippingFirstName { get; set; }

        [StringLength(100)]
        public string? ShippingLastName { get; set; }

        [StringLength(500)]
        public string? ShippingAddressText { get; set; }

        [StringLength(100)]
        public string? ShippingCity { get; set; }

        [StringLength(100)]
        public string? ShippingDistrict { get; set; }

        [StringLength(20)]
        public string? ShippingPostalCode { get; set; }

        [StringLength(20)]
        public string? ShippingPhone { get; set; }

        [StringLength(100)]
        public string? BillingFirstName { get; set; }

        [StringLength(100)]
        public string? BillingLastName { get; set; }

        [StringLength(500)]
        public string? BillingAddressText { get; set; }

        [StringLength(100)]
        public string? BillingCity { get; set; }

        [StringLength(100)]
        public string? BillingDistrict { get; set; }

        [StringLength(20)]
        public string? BillingPostalCode { get; set; }

        [StringLength(20)]
        public string? BillingPhone { get; set; }

        // Fiyat bilgileri
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Sipariş durumu
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Pending;

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BankTransfer;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(1000)]
        public string? AdminNotes { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        // Ödeme bilgileri
        [StringLength(255)]
        public string? PaymentToken { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        [StringLength(100)]
        public string? PaymentTransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }

        // Tarih bilgileri
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        // Navigation Properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed Properties
        [NotMapped]
        public string CustomerName
        {
            get
            {
                if (User != null)
                    return User.FullName;
                return $"{GuestFirstName} {GuestLastName}".Trim();
            }
        }

        [NotMapped]
        public string CustomerEmail => User?.Email ?? GuestEmail ?? "";

        [NotMapped]
        public bool IsGuestOrder => !UserId.HasValue;

        [NotMapped]
        public int TotalItems => OrderItems?.Sum(x => x.Quantity) ?? 0;

        // Helper method'lar - Türkçe gösterim için
        public static string GetOrderStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Beklemede",
                OrderStatus.Confirmed => "Onaylandı",
                OrderStatus.Processing => "Hazırlanıyor",
                OrderStatus.Shipped => "Kargoya Verildi",
                OrderStatus.Delivered => "Teslim Edildi",
                OrderStatus.Completed => "Tamamlandı",
                OrderStatus.Cancelled => "İptal Edildi",
                OrderStatus.Returned => "İade Edildi",
                _ => status.ToString()
            };
        }

        public static string GetPaymentStatusDisplayName(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "Beklemede",
                PaymentStatus.Paid => "Ödendi",
                PaymentStatus.Processing => "İşleniyor",
                PaymentStatus.Failed => "Başarısız",
                PaymentStatus.Completed => "Tamamlandı",
                PaymentStatus.Refunded => "İade Edildi",
                _ => status.ToString()
            };
        }

        public static string GetShippingStatusDisplayName(ShippingStatus status)
        {
            return status switch
            {
                ShippingStatus.Pending => "Beklemede",
                ShippingStatus.Prepared => "Hazırlandı",
                ShippingStatus.Shipped => "Kargoya Verildi",
                ShippingStatus.Delivered => "Teslim Edildi",
                ShippingStatus.Returned => "İade Edildi",
                _ => status.ToString()
            };
        }
    }

    public enum OrderStatus
    {
        Pending = 0,        // Beklemede
        Confirmed = 1,      // Onaylandı
        Processing = 2,     // Hazırlanıyor
        Shipped = 3,        // Kargoya verildi
        Delivered = 4,      // Teslim edildi
        Completed = 5,      // Tamamlandı
        Cancelled = 6,      // İptal edildi
        Returned = 7        // İade edildi
    }

    public enum PaymentStatus
    {
        Pending = 0,        // Beklemede
        Paid = 1,           // Ödendi
        Processing = 2,     // İşleniyor
        Failed = 3,         // Başarısız
        Completed = 4,      // Tamamlandı
        Refunded = 5        // İade edildi
    }

    public enum PaymentMethod
    {
        BankTransfer = 1,   // Havale/EFT
        CreditCard = 2,     // Kredi Kartı
        OnDelivery = 3      // Kapıda Ödeme
    }

    public enum ShippingStatus
    {
        Pending = 0,        // Beklemede
        Prepared = 1,       // Hazırlandı
        Shipped = 2,        // Kargoya verildi
        Delivered = 3,      // Teslim edildi
        Returned = 4        // İade edildi
    }
}
