using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class SiteSetting
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ayar anahtarı zorunludur")]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ayar değeri zorunludur")]
        [StringLength(2000)]
        public string Value { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = "General";

        [StringLength(200)]
        public string? DisplayName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? DataType { get; set; } = "text"; // text, email, url, textarea, checkbox

        public bool IsRequired { get; set; } = false;

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}