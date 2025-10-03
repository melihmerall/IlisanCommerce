using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsEmailConfirmed { get; set; } = false;

        [StringLength(100)]
        public string? CompanyName { get; set; } // Kurumsal müşteriler için

        [StringLength(50)]
        public string? TaxNumber { get; set; } // Vergi numarası

        public UserRole Role { get; set; } = UserRole.Customer;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Email confirmation
        [StringLength(100)]
        public string? EmailConfirmationToken { get; set; }
        
        // Password reset
        [StringLength(100)]
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation Properties
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Computed Property
        public string FullName => $"{FirstName} {LastName}";
    }

    public enum UserRole
    {
        Customer = 1,
        Admin = 2,
        SuperAdmin = 3
    }
}
