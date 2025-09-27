using System.ComponentModel.DataAnnotations;

namespace IlisanCommerce.Models.Logging
{
    /// <summary>
    /// User activity tracking for business intelligence and audit trails
    /// Follows Single Responsibility Principle - only handles user activity data
    /// </summary>
    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // Login, CreateOrder, UpdateProduct, etc.

        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // User, Order, Product, etc.

        public int? EntityId { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(100)]
        public string? UserEmail { get; set; }

        [StringLength(50)]
        public string? UserRole { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        [StringLength(200)]
        public string? RequestPath { get; set; }

        [StringLength(10)]
        public string? HttpMethod { get; set; }

        public string? OldValues { get; set; } // JSON serialized old values for updates

        public string? NewValues { get; set; } // JSON serialized new values for updates

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsSuccessful { get; set; } = true;

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        // Performance metrics
        public long? ExecutionTimeMs { get; set; }

        // Session tracking
        [StringLength(100)]
        public string? SessionId { get; set; }

        // Business context
        [StringLength(100)]
        public string? BusinessContext { get; set; } // AdminPanel, CustomerPortal, API, etc.

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Common user actions for consistency
    /// </summary>
    public static class ActivityActions
    {
        // Authentication
        public const string Login = "Login";
        public const string Logout = "Logout";
        public const string LoginFailed = "LoginFailed";
        public const string PasswordReset = "PasswordReset";
        public const string PasswordChanged = "PasswordChanged";

        // CRUD Operations
        public const string Create = "Create";
        public const string Read = "Read";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string BulkUpdate = "BulkUpdate";
        public const string BulkDelete = "BulkDelete";

        // Order Management
        public const string PlaceOrder = "PlaceOrder";
        public const string UpdateOrderStatus = "UpdateOrderStatus";
        public const string CancelOrder = "CancelOrder";
        public const string RefundOrder = "RefundOrder";
        public const string ShipOrder = "ShipOrder";

        // Product Management
        public const string CreateProduct = "CreateProduct";
        public const string UpdateProduct = "UpdateProduct";
        public const string DeleteProduct = "DeleteProduct";
        public const string UpdateStock = "UpdateStock";

        // System Operations
        public const string Export = "Export";
        public const string Import = "Import";
        public const string Backup = "Backup";
        public const string SystemMaintenance = "SystemMaintenance";
    }

    /// <summary>
    /// Entity types for consistent logging
    /// </summary>
    public static class EntityTypes
    {
        public const string User = "User";
        public const string AdminUser = "AdminUser";
        public const string Product = "Product";
        public const string Order = "Order";
        public const string Category = "Category";
        public const string Slider = "Slider";
        public const string ShippingRate = "ShippingRate";
        public const string EmailTemplate = "EmailTemplate";
        public const string SystemSetting = "SystemSetting";
    }
}
