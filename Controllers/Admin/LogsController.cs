using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Models.Logging;

namespace IlisanCommerce.Controllers.Admin
{
    /// <summary>
    /// Professional logs management controller for admin panel
    /// Follows SOLID principles and provides comprehensive log viewing capabilities
    /// </summary>
    [Authorize]
    [Route("admin/logs")]
    public class LogsController : Controller
    {
        private readonly ILogQueryService _logQueryService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogQueryService logQueryService, ILoggingService loggingService, ILogger<LogsController> logger)
        {
            _logQueryService = logQueryService;
            _loggingService = loggingService;
            _logger = logger;
        }

        /// <summary>
        /// Dashboard with log statistics and overview
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var toDate = DateTime.UtcNow;
                var fromDate = toDate.AddDays(-7); // Last 7 days

                var stats = await _logQueryService.GetLogStatisticsAsync(fromDate, toDate);
                
                ViewBag.Stats = stats;
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                
                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read, 
                    "LogsDashboard", 
                    description: "Admin viewed logs dashboard"
                );

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading logs dashboard");
                await _loggingService.LogErrorAsync(ex, "LogsController.Index");
                TempData["Error"] = "Log dashboard yüklenirken hata oluştu.";
                return View();
            }
        }

        /// <summary>
        /// System logs viewer with advanced filtering
        /// </summary>
        [HttpGet("system")]
        public async Task<IActionResult> SystemLogs(
            DateTime? fromDate = null, 
            DateTime? toDate = null,
            string? level = null, 
            string? source = null, 
            string? userId = null,
            int page = 1, 
            int pageSize = 50)
        {
            try
            {
                // Default to last 24 hours if no dates provided
                fromDate ??= DateTime.UtcNow.AddDays(-1);
                toDate ??= DateTime.UtcNow;

                var logs = await _logQueryService.GetSystemLogsAsync(fromDate, toDate, level, source, userId, page, pageSize);
                var totalCount = await _logQueryService.GetSystemLogsCountAsync(fromDate, toDate, level, source, userId);

                var viewModel = new SystemLogsViewModel
                {
                    Logs = logs,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    FromDate = fromDate,
                    ToDate = toDate,
                    Level = level,
                    Source = source,
                    UserId = userId
                };

                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read, 
                    "SystemLogs", 
                    description: $"Admin viewed system logs page {page}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading system logs");
                await _loggingService.LogErrorAsync(ex, "LogsController.SystemLogs");
                TempData["Error"] = "Sistem logları yüklenirken hata oluştu.";
                return View(new SystemLogsViewModel());
            }
        }

        /// <summary>
        /// Activity logs viewer with advanced filtering
        /// </summary>
        [HttpGet("activity")]
        public async Task<IActionResult> ActivityLogs(
            DateTime? fromDate = null, 
            DateTime? toDate = null,
            string? action = null, 
            string? entityType = null, 
            string? userId = null,
            bool? isSuccessful = null,
            int page = 1, 
            int pageSize = 50)
        {
            try
            {
                // Default to last 24 hours if no dates provided
                fromDate ??= DateTime.UtcNow.AddDays(-1);
                toDate ??= DateTime.UtcNow;

                var logs = await _logQueryService.GetActivityLogsAsync(fromDate, toDate, action, entityType, userId, page, pageSize);
                var totalCount = await _logQueryService.GetActivityLogsCountAsync(fromDate, toDate, action, entityType, userId);

                var viewModel = new ActivityLogsViewModel
                {
                    Logs = logs,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    FromDate = fromDate,
                    ToDate = toDate,
                    Action = action,
                    EntityType = entityType,
                    UserId = userId,
                    IsSuccessful = isSuccessful
                };

                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read, 
                    "ActivityLogs", 
                    description: $"Admin viewed activity logs page {page}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading activity logs");
                await _loggingService.LogErrorAsync(ex, "LogsController.ActivityLogs");
                TempData["Error"] = "Aktivite logları yüklenirken hata oluştu.";
                return View(new ActivityLogsViewModel());
            }
        }

        /// <summary>
        /// Error logs viewer with advanced filtering and resolution tracking
        /// </summary>
        [HttpGet("errors")]
        public async Task<IActionResult> ErrorLogs(
            DateTime? fromDate = null, 
            DateTime? toDate = null,
            string? errorType = null, 
            string? severity = null, 
            string? category = null,
            bool? isResolved = null,
            int page = 1, 
            int pageSize = 50)
        {
            try
            {
                // Default to last 24 hours if no dates provided
                fromDate ??= DateTime.UtcNow.AddDays(-1);
                toDate ??= DateTime.UtcNow;

                var logs = await _logQueryService.GetErrorLogsAsync(fromDate, toDate, errorType, severity, category, isResolved, page, pageSize);
                var totalCount = await _logQueryService.GetErrorLogsCountAsync(fromDate, toDate, errorType, severity, category, isResolved);

                var viewModel = new ErrorLogsViewModel
                {
                    Logs = logs,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    FromDate = fromDate,
                    ToDate = toDate,
                    ErrorType = errorType,
                    Severity = severity,
                    Category = category,
                    IsResolved = isResolved
                };

                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Read, 
                    "ErrorLogs", 
                    description: $"Admin viewed error logs page {page}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading error logs");
                await _loggingService.LogErrorAsync(ex, "LogsController.ErrorLogs");
                TempData["Error"] = "Hata logları yüklenirken hata oluştu.";
                return View(new ErrorLogsViewModel());
            }
        }

        /// <summary>
        /// Mark error as resolved
        /// </summary>
        [HttpPost("errors/{id}/resolve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveError(int id, string? resolutionNotes = null)
        {
            try
            {
                var resolvedBy = User.Identity?.Name ?? "Unknown Admin";
                await _loggingService.MarkErrorAsResolvedAsync(id, resolvedBy, resolutionNotes);

                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    "ResolveError", 
                    "ErrorLog", 
                    id,
                    description: $"Admin resolved error log {id}",
                    newValues: new { ResolutionNotes = resolutionNotes }
                );

                TempData["Success"] = "Hata başarıyla çözümlendi olarak işaretlendi.";
                return Json(new { success = true, message = "Hata çözümlendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving error log {ErrorId}", id);
                await _loggingService.LogErrorAsync(ex, "LogsController.ResolveError");
                return Json(new { success = false, message = "Hata çözümlenirken bir sorun oluştu." });
            }
        }

        /// <summary>
        /// Get error summary for dashboard
        /// </summary>
        [HttpGet("api/error-summary")]
        public async Task<IActionResult> GetErrorSummary(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                fromDate ??= DateTime.UtcNow.AddDays(-7);
                toDate ??= DateTime.UtcNow;

                var summary = await _loggingService.GetErrorSummaryAsync(fromDate.Value, toDate.Value);
                return Json(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting error summary");
                await _loggingService.LogErrorAsync(ex, "LogsController.GetErrorSummary");
                return Json(new List<ErrorSummaryDto>());
            }
        }

        /// <summary>
        /// Export logs to CSV
        /// </summary>
        [HttpGet("export/{logType}")]
        public async Task<IActionResult> ExportLogs(
            string logType,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? format = "csv")
        {
            try
            {
                fromDate ??= DateTime.UtcNow.AddDays(-7);
                toDate ??= DateTime.UtcNow;

                // Log this admin activity
                await _loggingService.LogActivityAsync(
                    ActivityActions.Export, 
                    logType + "Logs", 
                    description: $"Admin exported {logType} logs from {fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}"
                );

                // This will be implemented in the next phase with PDF generation
                TempData["Info"] = "Export özelliği yakında eklenecek.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting logs");
                await _loggingService.LogErrorAsync(ex, "LogsController.ExportLogs");
                TempData["Error"] = "Log export sırasında hata oluştu.";
                return RedirectToAction("Index");
            }
        }
    }

    #region ViewModels

    public class SystemLogsViewModel
    {
        public List<SystemLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Level { get; set; }
        public string? Source { get; set; }
        public string? UserId { get; set; }
    }

    public class ActivityLogsViewModel
    {
        public List<ActivityLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? UserId { get; set; }
        public bool? IsSuccessful { get; set; }
    }

    public class ErrorLogsViewModel
    {
        public List<ErrorLog> Logs { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ErrorType { get; set; }
        public string? Severity { get; set; }
        public string? Category { get; set; }
        public bool? IsResolved { get; set; }
    }

    #endregion
}
