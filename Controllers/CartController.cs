using Microsoft.AspNetCore.Mvc;
using IlisanCommerce.Services;
using IlisanCommerce.Models;
using System.Security.Claims;

namespace IlisanCommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var sessionId = GetCurrentSessionId();

            var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
            
            var model = new CartViewModel
            {
                CartItems = cartItems
            };

            ViewData["Title"] = "Sepetim";
            ViewData["Description"] = "Alışveriş sepeti";

            return View(model);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, int? variantId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var cartItem = await _cartService.AddToCartAsync(productId, variantId, quantity, userId, sessionId);
                
                if (cartItem != null)
                {
                    var cartCount = await _cartService.GetCartItemCountAsync(userId, sessionId);
                    
                    return Json(new { 
                        success = true, 
                        message = "Ürün sepete eklendi",
                        cartCount = cartCount
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = "Ürün sepete eklenirken bir hata oluştu" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to cart", productId);
                return Json(new { 
                    success = false, 
                    message = "Ürün sepete eklenirken bir hata oluştu" 
                });
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var success = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity, userId, sessionId);
                
                if (success)
                {
                    var cartTotal = await _cartService.GetCartTotalAsync(userId, sessionId);
                    var cartCount = await _cartService.GetCartItemCountAsync(userId, sessionId);
                    
                    return Json(new { 
                        success = true, 
                        message = "Sepet güncellendi",
                        cartTotal = cartTotal,
                        cartCount = cartCount
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = "Sepet güncellenirken bir hata oluştu" 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} quantity", cartItemId);
                return Json(new { 
                    success = false, 
                    message = "Sepet güncellenirken bir hata oluştu" 
                });
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var success = await _cartService.RemoveFromCartAsync(cartItemId, userId, sessionId);
        
                if (success)
                {
                    // Güncel sepet verilerini al
                    var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                    var cartTotal = cartItems.Sum(item => item.Quantity * item.Product.Price);
                    var cartCount = cartItems.Sum(item => item.Quantity);

                    var previewItems = cartItems.Take(3).Select(item => new
                    {
                        productId = item.ProductId,
                        productName = item.Product.Name,
                        productSlug = item.Product.Slug,
                        productPrice = item.Product.Price,
                        productImage = item.Product.ProductImages?.FirstOrDefault(pi => pi.IsMainImage)?.ImagePath ?? "/images/demo/demo_80x100.png",
                        quantity = item.Quantity
                    }).ToList();

                    return Json(new
                    {
                        success = true,
                        message = "Ürün sepetten kaldırıldı",
                        cartTotal = cartTotal,
                        cartCount = cartCount,
                        items = previewItems
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Ürün sepetten kaldırılırken bir hata oluştu"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {CartItemId}", cartItemId);
                return Json(new
                {
                    success = false,
                    message = "Ürün sepetten kaldırılırken bir hata oluştu"
                });
            }
        }

        // POST: Cart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var success = await _cartService.ClearCartAsync(userId, sessionId);
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = "Sepet temizlendi" 
                    });
                }
                else
                {
                    return Json(new { 
                        success = false, 
                        message = "Sepet temizlenirken bir hata oluştu" 
                    });
                }
            }
            catch (Exception ex)
            {
                var currentUserId = GetCurrentUserId();
                var currentSessionId = GetCurrentSessionId();
                _logger.LogError(ex, "Error clearing cart for user {UserId}, session {SessionId}", currentUserId, currentSessionId);
                return Json(new { 
                    success = false, 
                    message = "Sepet temizlenirken bir hata oluştu" 
                });
            }
        }

        // GET: Cart/GetCartCount
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var count = await _cartService.GetCartItemCountAsync(userId, sessionId);
                
                return Json(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(0);
            }
        }

        // GET: Cart/GetCartTotal
        [HttpGet]
        public async Task<IActionResult> GetCartTotal()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var total = await _cartService.GetCartTotalAsync(userId, sessionId);
                
                return Json(new { 
                    total = total,
                    formattedTotal = total.ToString("C", new System.Globalization.CultureInfo("tr-TR"))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart total");
                return Json(new { total = 0, formattedTotal = "0,00 ₺" });
            }
        }

        // GET: Cart/GetCartItems (for mini cart)
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                
                var result = cartItems.Select(item => new
                {
                    id = item.Id,
                    productId = item.ProductId,
                    productName = item.Product.Name,
                    productImage = item.Product.ProductImages.FirstOrDefault(pi => pi.IsMainImage)?.ImagePath ?? "/images/demo/demo_80x100.png",
                    productSlug = item.Product.Slug,
                    variantId = item.ProductVariantId,
                    variantName = item.ProductVariant?.VariantName,
                    quantity = item.Quantity,
                    unitPrice = item.UnitPrice,
                    totalPrice = item.TotalPrice,
                    formattedUnitPrice = item.UnitPrice.ToString("C", new System.Globalization.CultureInfo("tr-TR")),
                    formattedTotalPrice = item.TotalPrice.ToString("C", new System.Globalization.CultureInfo("tr-TR"))
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items");
                return Json(new List<object>());
            }
        }

        // POST: Cart/ApplyCoupon
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            // TODO: Implement coupon logic
            return Json(new { 
                success = false, 
                message = "Kupon sistemi henüz aktif değil" 
            });
        }

        // Helper methods
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null; // Guest user
        }

        private string GetCurrentSessionId()
        {
            // Session'ı başlat
            if (!HttpContext.Session.Keys.Any())
            {
                HttpContext.Session.SetString("_SessionStart", DateTime.Now.ToString());
            }
            
            var sessionId = HttpContext.Session.Id;
            _logger.LogInformation("Current Session ID: {SessionId}", sessionId);
            return sessionId;
        }

        // GET: Cart/GetCartPreview
        [HttpGet]
        public async Task<IActionResult> GetCartPreview()
        {
            try
            {
                var userId = GetCurrentUserId();
                var sessionId = GetCurrentSessionId();

                var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);

                var cartCount = cartItems.Sum(item => item.Quantity);
                var cartTotal = cartItems.Sum(item => item.Quantity * item.UnitPrice);

                var previewData = new
                {
                    itemCount = cartCount,
                    totalAmount = cartTotal,
                    formattedTotalAmount = cartTotal.ToString("C", new System.Globalization.CultureInfo("tr-TR")),
                    items = cartItems.Take(3).Select(item => new
                    {
                        cartItemId = item.Id,
                        productId = item.ProductId,
                        productName = item.Product.Name,
                        productSlug = item.Product.Slug,
                        productPrice = item.UnitPrice,
                        formattedProductPrice = item.UnitPrice.ToString("C", new System.Globalization.CultureInfo("tr-TR")),
                        productImage = item.Product.ProductImages?.FirstOrDefault(pi => pi.IsMainImage)?.ImagePath ?? "/images/demo/demo_80x100.png",
                        quantity = item.Quantity
                    }).ToList()
                };

                return Json(previewData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart preview");
                return Json(new { itemCount = 0, totalAmount = 0, formattedTotalAmount = "0,00 ₺", items = new List<object>() });
            }
        }
    }
}
