using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Models.Constants;
using System.Text.Json;

namespace IlisanCommerce.Controllers.Admin
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ApplicationDbContext context, ILoggingService loggingService, ILogger<CategoriesController> logger)
        {
            _context = context;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                    .Include(c => c.Products)
                    .OrderBy(c => c.ParentCategoryId.HasValue ? c.ParentCategory!.Name : c.Name)
                    .ThenBy(c => c.Name)
                    .ToListAsync();

                await _loggingService.LogActivityAsync(
                    ActivityActions.View,
                    EntityTypes.Category,
                    null,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    null,
                    null,
                    null,
                    null,
                    "Categories list viewed"
                );

                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
                await _loggingService.LogErrorAsync(
                    "CategoriesController.Index",
                    "Kategori listesi yüklenirken hata oluştu",
                    ex,
                    new Dictionary<string, object>()
                );
                TempData["Error"] = "Kategoriler yüklenirken hata oluştu.";
                return View(new List<Category>());
            }
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await LoadParentCategoriesForViewBag();
                
                // Add stats for the view
                await LoadCategoryStatsForViewBag();
                
                return View("~/Views/Admin/Categories/Create.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category create page");
                TempData["Error"] = "Sayfa yüklenirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Generate slug
                    category.Slug = GenerateSlug(category.Name);
                    
                    // Check for duplicate slug
                    var existingSlug = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Slug == category.Slug);
                    if (existingSlug != null)
                    {
                        category.Slug += "-" + DateTime.Now.Ticks.ToString()[^6..];
                    }

                    category.CreatedDate = DateTime.Now;
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();

                    await _loggingService.LogActivityAsync(
                        ActivityActions.Create,
                        EntityTypes.Category,
                        category.Id,
                        User.Identity?.Name,
                        User.Identity?.Name,
                        null,
                        null,
                        null,
                        category,
                        $"Category created: {category.Name}"
                    );

                    TempData["Success"] = "Kategori başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                await LoadParentCategoriesForViewBag();
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                await _loggingService.LogErrorAsync(
                    "CategoriesController.Create",
                    "Kategori oluşturulurken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["categoryName"] = category.Name ?? "" }
                );
                TempData["Error"] = "Kategori oluşturulurken hata oluştu.";
                await LoadParentCategoriesForViewBag();
                return View("~/Views/Admin/Categories/Create.cshtml", category);
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.ParentCategory)
                    .Include(c => c.SubCategories)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    TempData["Error"] = "Kategori bulunamadı.";
                    return RedirectToAction("Index");
                }

                await LoadParentCategoriesForViewBag(category.Id);
                return View("~/Views/Admin/Categories/Edit.cshtml", category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category {CategoryId}", id);
                TempData["Error"] = "Kategori yüklenirken hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    // Check for circular reference
                    if (category.ParentCategoryId.HasValue && await HasCircularReference(category.Id, category.ParentCategoryId.Value))
                    {
                        ModelState.AddModelError("ParentCategoryId", "Döngüsel referans oluşturulamaz.");
                        await LoadParentCategoriesForViewBag(category.Id);
                        return View("~/Views/Admin/Categories/Edit.cshtml", category);
                    }

                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        TempData["Error"] = "Kategori bulunamadı.";
                        return RedirectToAction("Index");
                    }

                    // Update properties
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.ImageUrl = category.ImageUrl;
                    existingCategory.ParentCategoryId = category.ParentCategoryId;
                    existingCategory.IsActive = category.IsActive;
                    existingCategory.UpdatedDate = DateTime.Now;
                    
                    // Update slug if name changed
                    if (existingCategory.Name != category.Name)
                    {
                        existingCategory.Slug = GenerateSlug(category.Name);
                    }

                    await _context.SaveChangesAsync();

                    await _loggingService.LogActivityAsync(
                        ActivityActions.Update,
                        EntityTypes.Category,
                        category.Id,
                        User.Identity?.Name,
                        User.Identity?.Name,
                        null,
                        null,
                        existingCategory,
                        category,
                        $"Category updated: {category.Name}"
                    );

                    TempData["Success"] = "Kategori başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }

                await LoadParentCategoriesForViewBag(category.Id);
                return View("~/Views/Admin/Categories/Edit.cshtml", category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", id);
                await _loggingService.LogErrorAsync(
                    "CategoriesController.Edit",
                    "Kategori güncellenirken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["categoryId"] = id }
                );
                TempData["Error"] = "Kategori güncellenirken hata oluştu.";
                await LoadParentCategoriesForViewBag(category.Id);
                return View("~/Views/Admin/Categories/Edit.cshtml", category);
            }
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.SubCategories)
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return Json(new { success = false, message = "Kategori bulunamadı." });
                }

                // Check if category has subcategories
                if (category.SubCategories.Any())
                {
                    return Json(new { success = false, message = "Bu kategorinin alt kategorileri var. Önce alt kategorileri silin." });
                }

                // Check if category has products
                if (category.Products.Any())
                {
                    return Json(new { success = false, message = "Bu kategoride ürünler var. Önce ürünleri başka kategoriye taşıyın." });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                await _loggingService.LogActivityAsync(
                    ActivityActions.Delete,
                    EntityTypes.Category,
                    id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    null,
                    null,
                    category,
                    null,
                    $"Category deleted: {category.Name}"
                );

                return Json(new { success = true, message = "Kategori başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                await _loggingService.LogErrorAsync(
                    "CategoriesController.Delete",
                    "Kategori silinirken hata oluştu",
                    ex,
                    new Dictionary<string, object> { ["categoryId"] = id }
                );
                return Json(new { success = false, message = "Kategori silinirken hata oluştu." });
            }
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Kategori bulunamadı." });
                }

                category.IsActive = !category.IsActive;
                category.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();

                await _loggingService.LogActivityAsync(
                    ActivityActions.Update,
                    EntityTypes.Category,
                    id,
                    User.Identity?.Name,
                    User.Identity?.Name,
                    null,
                    null,
                    null,
                    category,
                    $"Category status toggled: {category.Name} - {(category.IsActive ? "Active" : "Inactive")}"
                );

                return Json(new { 
                    success = true, 
                    isActive = category.IsActive,
                    message = $"Kategori {(category.IsActive ? "aktif" : "pasif")} hale getirildi." 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status {CategoryId}", id);
                return Json(new { success = false, message = "Durum değiştirilirken hata oluştu." });
            }
        }

        private async Task LoadParentCategoriesForViewBag(int? excludeId = null)
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .Where(c => !excludeId.HasValue || c.Id != excludeId.Value)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            ViewBag.ParentCategories = categories;
        }

        private async Task<bool> HasCircularReference(int categoryId, int parentCategoryId)
        {
            var parent = await _context.Categories.FindAsync(parentCategoryId);
            while (parent != null)
            {
                if (parent.Id == categoryId)
                    return true;
                
                if (parent.ParentCategoryId.HasValue)
                    parent = await _context.Categories.FindAsync(parent.ParentCategoryId.Value);
                else
                    break;
            }
            return false;
        }

        private string GenerateSlug(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            return name.ToLowerInvariant()
                .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
                .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u")
                .Replace(" ", "-")
                .Replace(".", "").Replace(",", "").Replace("!", "")
                .Replace("?", "").Replace(";", "").Replace(":", "")
                .Replace("'", "").Replace("\"", "");
        }

        private async Task LoadCategoryStatsForViewBag()
        {
            try
            {
                var totalCategories = await _context.Categories.CountAsync();
                var activeCategories = await _context.Categories.CountAsync(c => c.IsActive);
                
                ViewBag.TotalCategories = totalCategories;
                ViewBag.ActiveCategories = activeCategories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category statistics");
                ViewBag.TotalCategories = 0;
                ViewBag.ActiveCategories = 0;
            }
        }
    }
}
