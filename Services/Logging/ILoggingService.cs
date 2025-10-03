using IlisanCommerce.Models.Logging;

namespace IlisanCommerce.Services.Logging
{
    /// <summary>
    /// Main logging service interface - follows Interface Segregation Principle
    /// Separated into specific interfaces for different logging concerns
    /// </summary>
    public interface ILoggingService : ISystemLogger, IActivityLogger, IErrorLogger
    {
        // Combined interface for dependency injection convenience
    }

    /// <summary>
    /// System-level logging interface
    /// </summary>
    public interface ISystemLogger
    {
        Task LogSystemEventAsync(string level, string source, string message, 
            string? userId = null, string? userName = null, 
            Dictionary<string, object>? properties = null, 
            int? relatedEntityId = null, string? relatedEntityType = null);

        Task LogDebugAsync(string source, string message, Dictionary<string, object>? properties = null);
        Task LogInformationAsync(string source, string message, Dictionary<string, object>? properties = null);
        Task LogWarningAsync(string source, string message, Dictionary<string, object>? properties = null);
        Task LogErrorAsync(string source, string message, Exception? exception = null, Dictionary<string, object>? properties = null);
        Task LogCriticalAsync(string source, string message, Exception? exception = null, Dictionary<string, object>? properties = null);
    }

    /// <summary>
    /// User activity logging interface
    /// </summary>
    public interface IActivityLogger
    {
        Task LogActivityAsync(string action, string entityType, int? entityId = null,
            string? userId = null, string? userName = null, string? userEmail = null, string? userRole = null,
            object? oldValues = null, object? newValues = null, string? description = null,
            bool isSuccessful = true, string? errorMessage = null, long? executionTimeMs = null);

        Task LogUserLoginAsync(string userId, string userName, string userEmail, bool isSuccessful, string? errorMessage = null);
        Task LogUserLogoutAsync(string userId, string userName);
        Task LogCreateAsync(string entityType, int entityId, object newValues, string? userId = null, string? userName = null);
        Task LogUpdateAsync(string entityType, int entityId, object oldValues, object newValues, string? userId = null, string? userName = null);
        Task LogDeleteAsync(string entityType, int entityId, object oldValues, string? userId = null, string? userName = null);
        Task LogBulkOperationAsync(string action, string entityType, int[] entityIds, string? userId = null, string? userName = null);
    }

    /// <summary>
    /// Error logging interface
    /// </summary>
    public interface IErrorLogger
    {
        Task LogErrorAsync(Exception exception, string source, string? userId = null, string? userName = null,
            int? relatedEntityId = null, string? relatedEntityType = null,
            ErrorSeverity severity = ErrorSeverity.Medium, string? category = null,
            Dictionary<string, object>? additionalData = null);

        Task LogValidationErrorAsync(string source, string message, Dictionary<string, string> validationErrors,
            string? userId = null, string? userName = null);

        Task LogBusinessErrorAsync(string source, string message, string businessContext,
            int? relatedEntityId = null, string? relatedEntityType = null,
            string? userId = null, string? userName = null);

        Task MarkErrorAsResolvedAsync(int errorId, string resolvedBy, string? resolutionNotes = null);
        Task<List<ErrorSummaryDto>> GetErrorSummaryAsync(DateTime fromDate, DateTime toDate);
    }

    /// <summary>
    /// Log query interface for reading logs
    /// </summary>
    public interface ILogQueryService
    {
        Task<List<SystemLog>> GetSystemLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? level = null, string? source = null, string? userId = null,
            int page = 1, int pageSize = 50);

        Task<List<ActivityLog>> GetActivityLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? action = null, string? entityType = null, string? userId = null,
            int page = 1, int pageSize = 50);

        Task<List<ErrorLog>> GetErrorLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? errorType = null, string? severity = null, string? category = null,
            bool? isResolved = null, int page = 1, int pageSize = 50);

        Task<int> GetSystemLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? level = null, string? source = null, string? userId = null);

        Task<int> GetActivityLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? action = null, string? entityType = null, string? userId = null);

        Task<int> GetErrorLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? errorType = null, string? severity = null, string? category = null,
            bool? isResolved = null);

        Task<Dictionary<string, int>> GetLogStatisticsAsync(DateTime fromDate, DateTime toDate);
    }
}
