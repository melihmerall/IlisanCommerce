using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class Address
    {
        public int Id { get; set; }

        public int? UserId { get; set; } // Nullable for guest orders
        public User? User { get; set; }

        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        [StringLength(50)]
        public string AddressTitle { get; set; } = string.Empty; // Ev, İş vb.

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "İl zorunludur")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe zorunludur")]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Neighborhood { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(500)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AddressLine2 { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        public bool IsDefault { get; set; } = false;

        public AddressType AddressType { get; set; } = AddressType.Both;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Order> BillingOrders { get; set; } = new List<Order>();
        public virtual ICollection<Order> ShippingOrders { get; set; } = new List<Order>();
    }

    public enum AddressType
    {
        Shipping = 1,
        Billing = 2,
        Both = 3
    }
}
