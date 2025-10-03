using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad 50 karakterden uzun olamaz")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad 50 karakterden uzun olamaz")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100, ErrorMessage = "Email 100 karakterden uzun olamaz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public AdminRole Role { get; set; } = AdminRole.Admin;

        public bool IsActive { get; set; } = true;
        public bool IsEmailConfirmed { get; set; } = false;

        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Password Reset Fields
        [StringLength(255)]
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Navigation Properties
        public virtual ICollection<AdminActivityLog> ActivityLogs { get; set; } = new List<AdminActivityLog>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public string RoleDisplayName => Role switch
        {
            AdminRole.SuperAdmin => "Süper Admin",
            AdminRole.Admin => "Admin",
            AdminRole.Editor => "Editör",
            AdminRole.Viewer => "Görüntüleyici",
            _ => "Bilinmeyen"
        };
    }

    public enum AdminRole
    {
        SuperAdmin = 1,
        Admin = 2,
        Editor = 3,
        Viewer = 4
    }
}
