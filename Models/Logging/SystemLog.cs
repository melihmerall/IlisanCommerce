using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models.Logging
{
    /// <summary>
    /// System-level logging for application events
    /// Follows Single Responsibility Principle - only handles system log data
    /// </summary>
    public class SystemLog
    {
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Level { get; set; } = string.Empty; // Debug, Info, Warning, Error, Critical

        [Required]
        [StringLength(100)]
        public string Source { get; set; } = string.Empty; // Controller, Service, etc.

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public string? Exception { get; set; } // Full exception details

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(200)]
        public string? RequestPath { get; set; }

        [StringLength(10)]
        public string? HttpMethod { get; set; }

        public string? Properties { get; set; } // JSON serialized additional properties

        // Indexing for performance
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties for related entities
        public int? RelatedEntityId { get; set; }
        
        [StringLength(50)]
        public string? RelatedEntityType { get; set; } // Order, Product, User, etc.
    }

    /// <summary>
    /// Log levels enumeration for type safety
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Information = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }

    /// <summary>
    /// Log categories for better organization
    /// </summary>
    public enum LogCategory
    {
        System = 0,
        Security = 1,
        Business = 2,
        Performance = 3,
        Integration = 4,
        UserActivity = 5
    }
}
