using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Models;
using IlisanCommerce.Services;
using IlisanCommerce.Data;

namespace IlisanCommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly SettingsService _settingsService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, SettingsService settingsService)
    {
        _logger = logger;
        _context = context;
        _settingsService = settingsService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomePageViewModel
        {
            Sliders = await _context.Sliders
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ToListAsync(),
            
            FeaturedProducts = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsMainImage))
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.IsFeatured)
                .Take(8)
                .ToListAsync(),
            
            Categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .ToListAsync()
        };

        ViewData["Title"] = "Ana Sayfa";
        ViewData["Description"] = await _settingsService.GetSettingAsync("SiteDescription", "");

        return View(model);
    }

    public async Task<IActionResult> About()
    {
        ViewData["Title"] = "Hakkımızda";
        ViewData["Description"] = "ILISAN hakkında bilgi";
        
        var aboutUsTitle = await _settingsService.GetSettingAsync("AboutUsTitle", "Hakkımızda");
        var aboutUsText = await _settingsService.GetSettingAsync("AboutUsText", "");
        
        ViewBag.AboutUsTitle = aboutUsTitle;
        ViewBag.AboutUsText = aboutUsText;
        
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = "İletişim";
        ViewData["Description"] = "İletişim bilgileri";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Contact(ContactFormModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Contact form işleme burada yapılacak
                // Email gönderimi vs.
                
                // Log the contact form submission
                _logger.LogInformation("Contact form submitted: {Name} - {Email} - {Subject}", 
                    model.FullName, model.Email, model.Subject);
                
                TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form");
                TempData["Error"] = "Mesaj gönderilirken bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("Contact");
            }
        }

        ViewData["Title"] = "İletişim";
        return View(model);
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Gizlilik Politikası";
        return View();
    }

    public IActionResult ShippingReturns()
    {
        ViewData["Title"] = "Teslimat ve İade Şartları";
        ViewData["Description"] = "ILISAN teslimat, kargo, iade ve değişim koşulları";
        return View();
    }

    public IActionResult CookiePolicy()
    {
        ViewData["Title"] = "Çerez Politikası";
        ViewData["Description"] = "ILISAN çerez kullanım politikası";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
