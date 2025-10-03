using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IlisanCommerce.Services;
using IlisanCommerce.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IlisanCommerce.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public CartApiController(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var sessionId = HttpContext.Session.Id;

                var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                var count = cartItems.Sum(item => item.Quantity);

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Sepet sayısı alınırken hata oluştu." });
            }
        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetCartPreview()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var sessionId = HttpContext.Session.Id;

                var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                var totalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price);

                var previewItems = cartItems.Take(3).Select(item => new
                {
                    id = item.Id,
                    productId = item.ProductId,
                    productName = item.Product.Name,
                    productSlug = item.Product.Slug,
                    quantity = item.Quantity,
                    price = item.Product.Price,
                    imageUrl = item.Product.ProductImages?.FirstOrDefault(pi => pi.IsMainImage)?.ImagePath ?? "/images/demo/demo_80x100.png"
                }).ToList();

                return Ok(new
                {
                    itemCount = cartItems.Sum(item => item.Quantity),
                    totalAmount = totalAmount,
                    items = previewItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Sepet önizlemesi alınırken hata oluştu." });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var sessionId = HttpContext.Session.Id;

                var result = await _cartService.AddToCartAsync(request.ProductId, request.ProductVariantId, request.Quantity, userId, sessionId);

                if (result != null)
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                    var count = cartItems.Sum(item => item.Quantity);

                    return Ok(new { success = true, message = "Ürün sepete eklendi.", count });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Ürün sepete eklenemedi." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ürün sepete eklenirken hata oluştu." });
            }
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var sessionId = HttpContext.Session.Id;

                var result = await _cartService.RemoveFromCartAsync(cartItemId, userId, sessionId);

                if (result)
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                    var count = cartItems.Sum(item => item.Quantity);

                    return Ok(new { success = true, message = "Ürün sepetten kaldırıldı.", count });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Ürün sepetten kaldırılamadı." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ürün sepetten kaldırılırken hata oluştu." });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? userId = null;
                if (int.TryParse(userIdString, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                
                var sessionId = HttpContext.Session.Id;

                var result = await _cartService.UpdateCartItemQuantityAsync(request.CartItemId, request.Quantity, userId, sessionId);

                if (result)
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId, sessionId);
                    var count = cartItems.Sum(item => item.Quantity);

                    return Ok(new { success = true, message = "Sepet güncellendi.", count });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Sepet güncellenemedi." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Sepet güncellenirken hata oluştu." });
            }
        }
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public int? ProductVariantId { get; set; }
    }

    public class UpdateCartItemRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
