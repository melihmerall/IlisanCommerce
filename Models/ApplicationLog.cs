using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IlisanCommerce.Models
{
    public class ApplicationLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } // Information, Warning, Error, Debug

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(1000)]
        public string? Exception { get; set; }

        [MaxLength(200)]
        public string? Source { get; set; } // Controller, Service, etc.

        [MaxLength(200)]
        public string? Action { get; set; } // Method name

        [MaxLength(200)]
        public string? UserId { get; set; } // Admin or regular user ID

        [MaxLength(200)]
        public string? UserEmail { get; set; }

        [MaxLength(200)]
        public string? UserRole { get; set; }

        [MaxLength(200)]
        public string? IpAddress { get; set; }

        [MaxLength(200)]
        public string? UserAgent { get; set; }

        [MaxLength(200)]
        public string? RequestPath { get; set; }

        [MaxLength(200)]
        public string? RequestMethod { get; set; }

        [MaxLength(2000)]
        public string? Properties { get; set; } // JSON string for additional properties

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? SessionId { get; set; }

        [MaxLength(200)]
        public string? CorrelationId { get; set; } // For tracking related logs
    }
}
