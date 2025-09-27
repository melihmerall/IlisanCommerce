using IlisanCommerce.Data;
using IlisanCommerce.Models.Logging;
using Microsoft.EntityFrameworkCore;

namespace IlisanCommerce.Services.Logging
{
    /// <summary>
    /// Specialized service for querying logs - follows Single Responsibility Principle
    /// Separated from LoggingService to avoid violating SRP
    /// </summary>
    public class LogQueryService : ILogQueryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogQueryService> _logger;

        public LogQueryService(ApplicationDbContext context, ILogger<LogQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<SystemLog>> GetSystemLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? level = null, string? source = null, string? userId = null,
            int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.SystemLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(level))
                    query = query.Where(l => l.Level == level);

                if (!string.IsNullOrEmpty(source))
                    query = query.Where(l => l.Source.Contains(source));

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(l => l.UserId == userId);

                return await query
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system logs");
                return new List<SystemLog>();
            }
        }

        public async Task<List<ActivityLog>> GetActivityLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? action = null, string? entityType = null, string? userId = null,
            int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.ActivityLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(l => l.Action == action);

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(l => l.EntityType == entityType);

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(l => l.UserId == userId);

                return await query
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity logs");
                return new List<ActivityLog>();
            }
        }

        public async Task<List<ErrorLog>> GetErrorLogsAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? errorType = null, string? severity = null, string? category = null,
            bool? isResolved = null, int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.ErrorLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(errorType))
                    query = query.Where(l => l.ErrorType.Contains(errorType));

                if (!string.IsNullOrEmpty(severity))
                    query = query.Where(l => l.Severity == severity);

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(l => l.Category == category);

                if (isResolved.HasValue)
                    query = query.Where(l => l.IsResolved == isResolved.Value);

                return await query
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving error logs");
                return new List<ErrorLog>();
            }
        }

        public async Task<int> GetSystemLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? level = null, string? source = null, string? userId = null)
        {
            try
            {
                var query = _context.SystemLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(level))
                    query = query.Where(l => l.Level == level);

                if (!string.IsNullOrEmpty(source))
                    query = query.Where(l => l.Source.Contains(source));

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(l => l.UserId == userId);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting system logs");
                return 0;
            }
        }

        public async Task<int> GetActivityLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? action = null, string? entityType = null, string? userId = null)
        {
            try
            {
                var query = _context.ActivityLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(l => l.Action == action);

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(l => l.EntityType == entityType);

                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(l => l.UserId == userId);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting activity logs");
                return 0;
            }
        }

        public async Task<int> GetErrorLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null,
            string? errorType = null, string? severity = null, string? category = null,
            bool? isResolved = null)
        {
            try
            {
                var query = _context.ErrorLogs.AsQueryable();

                if (fromDate.HasValue)
                    query = query.Where(l => l.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(l => l.Timestamp <= toDate.Value);

                if (!string.IsNullOrEmpty(errorType))
                    query = query.Where(l => l.ErrorType.Contains(errorType));

                if (!string.IsNullOrEmpty(severity))
                    query = query.Where(l => l.Severity == severity);

                if (!string.IsNullOrEmpty(category))
                    query = query.Where(l => l.Category == category);

                if (isResolved.HasValue)
                    query = query.Where(l => l.IsResolved == isResolved.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting error logs");
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetLogStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var stats = new Dictionary<string, int>();

                // System log statistics
                var systemLogCounts = await _context.SystemLogs
                    .Where(l => l.Timestamp >= fromDate && l.Timestamp <= toDate)
                    .GroupBy(l => l.Level)
                    .Select(g => new { Level = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var stat in systemLogCounts)
                {
                    stats[$"SystemLog_{stat.Level}"] = stat.Count;
                }

                // Activity log statistics
                var activityLogCounts = await _context.ActivityLogs
                    .Where(l => l.Timestamp >= fromDate && l.Timestamp <= toDate)
                    .GroupBy(l => l.Action)
                    .Select(g => new { Action = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var stat in activityLogCounts)
                {
                    stats[$"Activity_{stat.Action}"] = stat.Count;
                }

                // Error log statistics
                var errorLogCounts = await _context.ErrorLogs
                    .Where(l => l.Timestamp >= fromDate && l.Timestamp <= toDate)
                    .GroupBy(l => l.Severity)
                    .Select(g => new { Severity = g.Key, Count = g.Count() })
                    .ToListAsync();

                foreach (var stat in errorLogCounts)
                {
                    stats[$"Error_{stat.Severity}"] = stat.Count;
                }

                // Unresolved errors
                var unresolvedErrors = await _context.ErrorLogs
                    .Where(l => l.Timestamp >= fromDate && l.Timestamp <= toDate && !l.IsResolved)
                    .CountAsync();

                stats["UnresolvedErrors"] = unresolvedErrors;

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving log statistics");
                return new Dictionary<string, int>();
            }
        }
    }
}
