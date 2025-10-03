using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models.Logging
{
    /// <summary>
    /// Specialized error logging for detailed exception tracking
    /// Follows Single Responsibility Principle - only handles error data
    /// </summary>
    public class ErrorLog
    {
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string ErrorType { get; set; } = string.Empty; // Exception type name

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        public string? InnerException { get; set; }

        [Required]
        [StringLength(100)]
        public string Source { get; set; } = string.Empty; // Controller, Service, Method name

        [StringLength(200)]
        public string? RequestPath { get; set; }

        [StringLength(10)]
        public string? HttpMethod { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public string? RequestHeaders { get; set; } // JSON serialized headers

        public string? RequestBody { get; set; } // For POST/PUT requests (be careful with sensitive data)

        public string? QueryString { get; set; }

        [StringLength(100)]
        public string? SessionId { get; set; }

        // Error severity and categorization
        [Required]
        [StringLength(20)]
        public string Severity { get; set; } = ErrorSeverity.Medium.ToString();

        [StringLength(50)]
        public string? Category { get; set; } // Database, Network, Validation, Business, etc.

        // Resolution tracking
        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedDate { get; set; }

        [StringLength(100)]
        public string? ResolvedBy { get; set; }

        [StringLength(1000)]
        public string? ResolutionNotes { get; set; }

        // Related entity information
        public int? RelatedEntityId { get; set; }

        [StringLength(50)]
        public string? RelatedEntityType { get; set; }

        // Environment information
        [StringLength(50)]
        public string? Environment { get; set; } // Development, Staging, Production

        [StringLength(50)]
        public string? ServerName { get; set; }

        [StringLength(20)]
        public string? ApplicationVersion { get; set; }

        // Additional context
        public string? AdditionalData { get; set; } // JSON serialized additional context

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Error severity levels for prioritization
    /// </summary>
    public enum ErrorSeverity
    {
        Low = 1,        // Minor issues, doesn't affect functionality
        Medium = 2,     // Moderate issues, some functionality affected
        High = 3,       // Major issues, significant functionality affected
        Critical = 4    // System-breaking issues, immediate attention required
    }

    /// <summary>
    /// Common error categories for better organization
    /// </summary>
    public static class ErrorCategories
    {
        public const string Database = "Database";
        public const string Network = "Network";
        public const string Validation = "Validation";
        public const string Business = "Business";
        public const string Authentication = "Authentication";
        public const string Authorization = "Authorization";
        public const string FileSystem = "FileSystem";
        public const string ExternalApi = "ExternalApi";
        public const string Payment = "Payment";
        public const string Email = "Email";
        public const string Configuration = "Configuration";
        public const string Performance = "Performance";
    }

    /// <summary>
    /// DTO for error reporting and dashboard
    /// </summary>
    public class ErrorSummaryDto
    {
        public string ErrorType { get; set; } = string.Empty;
        public int Count { get; set; }
        public DateTime LastOccurrence { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int UnresolvedCount { get; set; }
    }
}
