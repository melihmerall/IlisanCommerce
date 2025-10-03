using IlisanCommerce.Data;
using IlisanCommerce.Models.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace IlisanCommerce.Services.Logging
{
    /// <summary>
    /// Enterprise-grade logging service implementation
    /// Follows SOLID principles - Single Responsibility, Dependency Inversion
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<LoggingService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        #region System Logging

        public async Task LogSystemEventAsync(string level, string source, string message, 
            string? userId = null, string? userName = null, 
            Dictionary<string, object>? properties = null, 
            int? relatedEntityId = null, string? relatedEntityType = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                
                var systemLog = new SystemLog
                {
                    Level = level,
                    Source = source,
                    Message = message,
                    UserId = userId,
                    UserName = userName,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request?.Headers["User-Agent"].FirstOrDefault(),
                    RequestPath = httpContext?.Request?.Path,
                    HttpMethod = httpContext?.Request?.Method,
                    Properties = properties != null ? JsonSerializer.Serialize(properties) : null,
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType,
                    Timestamp = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.SystemLogs.Add(systemLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Fallback to standard logging if database logging fails
                _logger.LogError(ex, "Failed to log system event to database: {Message}", message);
            }
        }

        public async Task LogDebugAsync(string source, string message, Dictionary<string, object>? properties = null)
        {
            await LogSystemEventAsync("Debug", source, message, properties: properties);
        }

        public async Task LogInformationAsync(string source, string message, Dictionary<string, object>? properties = null)
        {
            await LogSystemEventAsync("Information", source, message, properties: properties);
        }

        public async Task LogWarningAsync(string source, string message, Dictionary<string, object>? properties = null)
        {
            await LogSystemEventAsync("Warning", source, message, properties: properties);
        }

        public async Task LogErrorAsync(string source, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            var props = properties ?? new Dictionary<string, object>();
            if (exception != null)
            {
                props["Exception"] = exception.ToString();
            }
            await LogSystemEventAsync("Error", source, message, properties: props);
        }

        public async Task LogCriticalAsync(string source, string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        {
            var props = properties ?? new Dictionary<string, object>();
            if (exception != null)
            {
                props["Exception"] = exception.ToString();
            }
            await LogSystemEventAsync("Critical", source, message, properties: props);
        }

        #endregion

        #region Activity Logging

        public async Task LogActivityAsync(string action, string entityType, int? entityId = null,
            string? userId = null, string? userName = null, string? userEmail = null, string? userRole = null,
            object? oldValues = null, object? newValues = null, string? description = null,
            bool isSuccessful = true, string? errorMessage = null, long? executionTimeMs = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                
                var activityLog = new ActivityLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    UserId = userId,
                    UserName = userName,
                    UserEmail = userEmail,
                    UserRole = userRole,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request?.Headers["User-Agent"].FirstOrDefault(),
                    RequestPath = httpContext?.Request?.Path,
                    HttpMethod = httpContext?.Request?.Method,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    Description = description,
                    IsSuccessful = isSuccessful,
                    ErrorMessage = errorMessage,
                    ExecutionTimeMs = executionTimeMs,
                    SessionId = httpContext?.Session?.Id,
                    BusinessContext = GetBusinessContext(httpContext),
                    Timestamp = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity to database: {Action} on {EntityType}", action, entityType);
            }
        }

        public async Task LogUserLoginAsync(string userId, string userName, string userEmail, bool isSuccessful, string? errorMessage = null)
        {
            await LogActivityAsync(ActivityActions.Login, EntityTypes.User, 
                userId: userId, userName: userName, userEmail: userEmail,
                description: isSuccessful ? "User logged in successfully" : "User login failed",
                isSuccessful: isSuccessful, errorMessage: errorMessage);
        }

        public async Task LogUserLogoutAsync(string userId, string userName)
        {
            await LogActivityAsync(ActivityActions.Logout, EntityTypes.User,
                userId: userId, userName: userName,
                description: "User logged out");
        }

        public async Task LogCreateAsync(string entityType, int entityId, object newValues, string? userId = null, string? userName = null)
        {
            await LogActivityAsync(ActivityActions.Create, entityType, entityId,
                userId: userId, userName: userName,
                newValues: newValues,
                description: $"Created new {entityType}");
        }

        public async Task LogUpdateAsync(string entityType, int entityId, object oldValues, object newValues, string? userId = null, string? userName = null)
        {
            await LogActivityAsync(ActivityActions.Update, entityType, entityId,
                userId: userId, userName: userName,
                oldValues: oldValues, newValues: newValues,
                description: $"Updated {entityType}");
        }

        public async Task LogDeleteAsync(string entityType, int entityId, object oldValues, string? userId = null, string? userName = null)
        {
            await LogActivityAsync(ActivityActions.Delete, entityType, entityId,
                userId: userId, userName: userName,
                oldValues: oldValues,
                description: $"Deleted {entityType}");
        }

        public async Task LogBulkOperationAsync(string action, string entityType, int[] entityIds, string? userId = null, string? userName = null)
        {
            await LogActivityAsync(action, entityType,
                userId: userId, userName: userName,
                newValues: new { EntityIds = entityIds, Count = entityIds.Length },
                description: $"Bulk {action} on {entityIds.Length} {entityType} records");
        }

        #endregion

        #region Error Logging

        public async Task LogErrorAsync(Exception exception, string source, string? userId = null, string? userName = null,
            int? relatedEntityId = null, string? relatedEntityType = null,
            ErrorSeverity severity = ErrorSeverity.Medium, string? category = null,
            Dictionary<string, object>? additionalData = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                
                var errorLog = new ErrorLog
                {
                    ErrorType = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.ToString(),
                    Source = source,
                    RequestPath = httpContext?.Request?.Path,
                    HttpMethod = httpContext?.Request?.Method,
                    UserId = userId,
                    UserName = userName,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request?.Headers["User-Agent"].FirstOrDefault(),
                    RequestHeaders = httpContext != null ? JsonSerializer.Serialize(httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())) : null,
                    QueryString = httpContext?.Request?.QueryString.ToString(),
                    SessionId = httpContext?.Session?.Id,
                    Severity = severity.ToString(),
                    Category = category ?? GetErrorCategory(exception),
                    RelatedEntityId = relatedEntityId,
                    RelatedEntityType = relatedEntityType,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    ServerName = Environment.MachineName,
                    ApplicationVersion = GetApplicationVersion(),
                    AdditionalData = additionalData != null ? JsonSerializer.Serialize(additionalData) : null,
                    Timestamp = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ErrorLogs.Add(errorLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log error to database: {ErrorMessage}", exception.Message);
            }
        }

        public async Task LogValidationErrorAsync(string source, string message, Dictionary<string, string> validationErrors,
            string? userId = null, string? userName = null)
        {
            var additionalData = new Dictionary<string, object>
            {
                ["ValidationErrors"] = validationErrors
            };

            var validationException = new ValidationException($"Validation failed: {message}");
            await LogErrorAsync(validationException, source, userId, userName, 
                severity: ErrorSeverity.Low, category: ErrorCategories.Validation, 
                additionalData: additionalData);
        }

        public async Task LogBusinessErrorAsync(string source, string message, string businessContext,
            int? relatedEntityId = null, string? relatedEntityType = null,
            string? userId = null, string? userName = null)
        {
            var additionalData = new Dictionary<string, object>
            {
                ["BusinessContext"] = businessContext
            };

            var businessException = new BusinessException(message);
            await LogErrorAsync(businessException, source, userId, userName,
                relatedEntityId: relatedEntityId, relatedEntityType: relatedEntityType,
                severity: ErrorSeverity.Medium, category: ErrorCategories.Business,
                additionalData: additionalData);
        }

        public async Task MarkErrorAsResolvedAsync(int errorId, string resolvedBy, string? resolutionNotes = null)
        {
            try
            {
                var errorLog = await _context.ErrorLogs.FindAsync(errorId);
                if (errorLog != null)
                {
                    errorLog.IsResolved = true;
                    errorLog.ResolvedDate = DateTime.UtcNow;
                    errorLog.ResolvedBy = resolvedBy;
                    errorLog.ResolutionNotes = resolutionNotes;
                    
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark error as resolved: {ErrorId}", errorId);
            }
        }

        public async Task<List<ErrorSummaryDto>> GetErrorSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await _context.ErrorLogs
                    .Where(e => e.Timestamp >= fromDate && e.Timestamp <= toDate)
                    .GroupBy(e => new { e.ErrorType, e.Severity, e.Category })
                    .Select(g => new ErrorSummaryDto
                    {
                        ErrorType = g.Key.ErrorType,
                        Severity = g.Key.Severity,
                        Category = g.Key.Category ?? "Unknown",
                        Count = g.Count(),
                        LastOccurrence = g.Max(e => e.Timestamp),
                        UnresolvedCount = g.Count(e => !e.IsResolved)
                    })
                    .OrderByDescending(s => s.Count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get error summary");
                return new List<ErrorSummaryDto>();
            }
        }

        #endregion

        #region Helper Methods

        private string GetBusinessContext(HttpContext? httpContext)
        {
            if (httpContext == null) return "Unknown";
            
            var path = httpContext.Request.Path.Value?.ToLower() ?? "";
            
            if (path.StartsWith("/admin")) return "AdminPanel";
            if (path.StartsWith("/api")) return "API";
            if (path.StartsWith("/account")) return "Authentication";
            
            return "CustomerPortal";
        }

        private string GetErrorCategory(Exception exception)
        {
            return exception switch
            {
                DbUpdateException => ErrorCategories.Database,
                HttpRequestException => ErrorCategories.Network,
                UnauthorizedAccessException => ErrorCategories.Authorization,
                ArgumentException => ErrorCategories.Validation,
                FileNotFoundException => ErrorCategories.FileSystem,
                _ => "General"
            };
        }

        private string GetApplicationVersion()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        #endregion
    }

    // Custom exception classes for better categorization
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}
