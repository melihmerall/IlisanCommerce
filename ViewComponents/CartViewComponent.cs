using Microsoft.AspNetCore.Mvc;
using IlisanCommerce.Services;
using IlisanCommerce.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using IlisanCommerce.Models;

namespace IlisanCommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public CartViewComponent(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
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
            var cartTotal = cartItems.Sum(item => item.Quantity * item.Product.Price);

            var model = new CartMiniViewModel
            {
                ItemCount = cartCount,
                TotalAmount = cartTotal,
                Items = cartItems.Take(3).ToList() // Mini-sepet için sadece ilk 3 ürün
            };

            return View(model);
        }
    }

    public class CartMiniViewModel
    {
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }
}
