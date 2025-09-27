using System;
using System.Threading.Tasks;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IlisanCommerce.Services
{
    public interface IApplicationLogService
    {
        Task LogInformationAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null);
        
        Task LogWarningAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null);
        
        Task LogErrorAsync(string message, Exception? exception = null, string? source = null, 
            string? action = null, string? userId = null, string? userEmail = null, 
            string? userRole = null, object? properties = null);
        
        Task LogDebugAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null);
    }

    public class ApplicationLogService : IApplicationLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApplicationLogService> _logger;

        public ApplicationLogService(ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ApplicationLogService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogInformationAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null)
        {
            await LogAsync(LogLevel.Information, message, null, source, action, userId, userEmail, userRole, properties);
        }

        public async Task LogWarningAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null)
        {
            await LogAsync(LogLevel.Warning, message, null, source, action, userId, userEmail, userRole, properties);
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, string? source = null, 
            string? action = null, string? userId = null, string? userEmail = null, 
            string? userRole = null, object? properties = null)
        {
            await LogAsync(LogLevel.Error, message, exception, source, action, userId, userEmail, userRole, properties);
        }

        public async Task LogDebugAsync(string message, string? source = null, string? action = null, 
            string? userId = null, string? userEmail = null, string? userRole = null, 
            object? properties = null)
        {
            await LogAsync(LogLevel.Debug, message, null, source, action, userId, userEmail, userRole, properties);
        }

        private async Task LogAsync(LogLevel level, string message, Exception? exception = null, 
            string? source = null, string? action = null, string? userId = null, 
            string? userEmail = null, string? userRole = null, object? properties = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                
                var log = new ApplicationLog
                {
                    Level = level.ToString(),
                    Message = message,
                    Exception = exception?.ToString(),
                    Source = source,
                    Action = action,
                    UserId = userId,
                    UserEmail = userEmail,
                    UserRole = userRole,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString(),
                    RequestPath = httpContext?.Request?.Path,
                    RequestMethod = httpContext?.Request?.Method,
                    Properties = properties != null ? JsonSerializer.Serialize(properties) : null,
                    SessionId = httpContext?.Session?.Id,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ApplicationLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Fallback to standard logger if database logging fails
                _logger.LogError(ex, "Failed to write log to database: {Message}", message);
            }
        }
    }
}
