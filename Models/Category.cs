using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [StringLength(100, ErrorMessage = "Kategori adı 100 karakterden uzun olamaz")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama 500 karakterden uzun olamaz")]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? ImageUrl { get; set; }

        [StringLength(150)]
        public string? Slug { get; set; }

        public bool IsActive { get; set; } = true;

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
