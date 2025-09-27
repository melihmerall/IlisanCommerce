using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Subtitle { get; set; }

        [Required(ErrorMessage = "Resim URL'si zorunludur")]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ButtonText { get; set; }

        [StringLength(500)]
        public string? ButtonUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
    }
}
