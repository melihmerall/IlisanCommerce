using Microsoft.AspNetCore.Mvc;
using IlisanCommerce.Services;
using IlisanCommerce.Data;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Models;
using System.Security.Claims;

namespace IlisanCommerce.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly SettingsService _settingsService;
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public HeaderViewComponent(SettingsService settingsService, ApplicationDbContext context, ICartService cartService)
        {
            _settingsService = settingsService;
            _context = context;
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Get cart count
            var userIdString = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }
            
            // Session'ı başlat
            if (!HttpContext.Session.Keys.Any())
            {
                HttpContext.Session.SetString("_SessionStart", DateTime.Now.ToString());
            }
            
            var sessionId = HttpContext.Session.Id;
            var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
            var cartCount = cartItems.Sum(item => item.Quantity);

            // Get user information
            var isAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
            var userName = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = HttpContext.User?.FindFirst(ClaimTypes.Email)?.Value;

            var model = new HeaderViewModel
            {
                SiteName = await _settingsService.GetSettingAsync("SiteName", "ILISAN"),
                SitePhone = await _settingsService.GetSettingAsync("Phone", "+90 (850) 532 5237"),
                SiteEmail = await _settingsService.GetSettingAsync("Email", "info@ilisan.com.tr"),
                SiteAddress = await _settingsService.GetSettingAsync("Address", "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş"),
                WorkingHours = await _settingsService.GetSettingAsync("WorkingHours", "24/7 Destek"),
                LogoUrl = await _settingsService.GetSettingAsync("Logo", "/logo.png"),
                FacebookUrl = await _settingsService.GetSettingAsync("FacebookUrl", "#"),
                InstagramUrl = await _settingsService.GetSettingAsync("InstagramUrl", "https://instagram.com/ilisansavunma"),
                LinkedInUrl = await _settingsService.GetSettingAsync("LinkedInUrl", "#"),
                Categories = await _context.Categories
                    .Where(c => c.IsActive && c.ParentCategoryId == null)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                CartCount = cartCount,
                
                // User Information
                IsAuthenticated = isAuthenticated,
                UserName = userName,
                UserEmail = userEmail,
                UserId = userId
            };

            return View(model);
        }
    }

    public class HeaderViewModel
    {
        public string SiteName { get; set; } = string.Empty;
        public string SitePhone { get; set; } = string.Empty;
        public string SiteEmail { get; set; } = string.Empty;
        public string SiteAddress { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string LinkedInUrl { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new();
        public int CartCount { get; set; }
        
        // User Information
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public int? UserId { get; set; }
    }
}
