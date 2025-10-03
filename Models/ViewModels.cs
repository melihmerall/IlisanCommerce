using System.ComponentModel.DataAnnotations;
using IlisanCommerce.Services.Reports;

namespace IlisanCommerce.Models
{
    public class HomePageViewModel
    {
        public List<Slider> Sliders { get; set; } = new List<Slider>();
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Category> FeaturedCategories { get; set; } = new List<Category>();
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Konu zorunludur")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }

    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public Category? CurrentCategory { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCount => TotalProducts; // Alias for view compatibility
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling((double)TotalProducts / PageSize);
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? CategoryName => CurrentCategory?.Name;
        public string? CategorySlug { get; set; }
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
        public int SelectedVariantId { get; set; }
        public int Quantity { get; set; } = 1;
        
        // Seçilebilir özellikler için yeni alanlar
        public List<ProductOptionGroup> OptionGroups { get; set; } = new List<ProductOptionGroup>();
        public Dictionary<string, string> SelectedOptions { get; set; } = new Dictionary<string, string>();
        public ProductVariant? SelectedVariant { get; set; }
    }

    public class ProductOptionGroup
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<ProductOption> Options { get; set; } = new List<ProductOption>();
        public bool IsRequired { get; set; } = true;
        public string OptionType { get; set; } = "radio"; // radio, checkbox, select, color
    }

    public class ProductOption
    {
        public string Value { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PriceAdjustment { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
        public int StockQuantity { get; set; } = 0;
    }

    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<CartItem> Items => CartItems;
        public decimal SubTotal => CartItems.Sum(c => c.TotalPrice);
        public decimal ShippingCost { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal Total => SubTotal + ShippingCost + TaxAmount - DiscountAmount;
        public decimal TotalAmount => Total;
        public int TotalItems => CartItems.Sum(c => c.Quantity);
    }

    public class CheckoutViewModel
    {
        public CartViewModel Cart { get; set; } = new CartViewModel();
        public CheckoutFormModel Form { get; set; } = new CheckoutFormModel();
        public List<Address> UserAddresses { get; set; } = new List<Address>();
        public bool IsGuest { get; set; } = true;
        public string? SelectedPaymentType { get; set; }
        
        // Kargo bilgileri
        public decimal TotalDesi { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal CartTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class CheckoutFormModel
    {
        // Müşteri bilgileri (misafir için)
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        // Fatura adresi
        public int? BillingAddressId { get; set; }

        public AddressFormModel? BillingAddress { get; set; }

        // Teslimat adresi
        public int? ShippingAddressId { get; set; }

        public AddressFormModel? ShippingAddress { get; set; }

        public bool SameAsShipping { get; set; } = false;

        // Ödeme
        [Required(ErrorMessage = "Ödeme yöntemi seçiniz")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class AddressFormModel
    {
        [StringLength(50)]
        public string? AddressTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon zorunludur")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "İl zorunludur")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe zorunludur")]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Neighborhood { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(500)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AddressLine2 { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni hatırla")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi giriniz")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre onayı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanım şartlarını kabul etmelisiniz")]
        public bool AcceptTerms { get; set; }
        
        public bool AcceptMarketing { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-mail adresi giriniz")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Token zorunludur")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanım şartlarını kabul etmelisiniz")]
        public bool AcceptTerms { get; set; }
        
        public bool AcceptMarketing { get; set; }
    }

    public class SearchViewModel
    {
        public string? Query { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling((double)TotalResults / PageSize);
    }

    public class UserProfileViewModel
    {
        public User User { get; set; } = null!;
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Address> Addresses { get; set; } = new List<Address>();
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public int TotalOrders { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalOrders / PageSize);
        public OrderStatus? StatusFilter { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public Dictionary<string, int> OrdersByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> SalesByMonth { get; set; } = new Dictionary<string, decimal>();
    }

    public class IyzicoPaymentRequest
    {
        [Required(ErrorMessage = "Kart sahibi adı zorunludur")]
        [StringLength(50, ErrorMessage = "Kart sahibi adı en fazla 50 karakter olabilir")]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kart numarası zorunludur")]
        [StringLength(19, MinimumLength = 16, ErrorMessage = "Kart numarası 16-19 karakter arasında olmalıdır")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son kullanma ayı zorunludur")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Ay 2 haneli olmalıdır")]
        public string ExpireMonth { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son kullanma yılı zorunludur")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Yıl 4 haneli olmalıdır")]
        public string ExpireYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVC kodu zorunludur")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVC kodu 3-4 haneli olmalıdır")]
        public string Cvc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        public string BuyerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        public string BuyerSurname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir")]
        public string BuyerPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(100, ErrorMessage = "E-posta adresi en fazla 100 karakter olabilir")]
        public string BuyerEmail { get; set; } = string.Empty;

        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        public string? BuyerIdentityNumber { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        public string BuyerAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur")]
        [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir")]
        public string BuyerCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Posta kodu zorunludur")]
        [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir")]
        public string BuyerZipCode { get; set; } = string.Empty;

        [Range(1, 12, ErrorMessage = "Taksit sayısı 1-12 arasında olmalıdır")]
        public int Installment { get; set; } = 1;
    }

    public class ReportsViewModel
    {
        public OrderSummaryReport OrderSummary { get; set; } = new();
        public SalesReport SalesReport { get; set; } = new();
        public List<OrderStatusCount> StatusCounts { get; set; } = new();
        public List<DailySales> DailySales { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<MonthlyRevenue> MonthlyRevenue { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    // Customer Management ViewModels
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
        public bool IsActive { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class CustomersIndexViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class CustomerDetailViewModel
    {
        public User Customer { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
        public List<Address> Addresses { get; set; } = new();
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    public class TestEmailViewModel
    {
        [Required(ErrorMessage = "Alıcı email adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Alıcı Email")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Display(Name = "Alıcı Adı")]
        public string? RecipientName { get; set; }

        [Required(ErrorMessage = "Email tipi seçiniz")]
        [Display(Name = "Email Tipi")]
        public string EmailType { get; set; } = string.Empty;

        [Display(Name = "Konu")]
        public string? Subject { get; set; }

        [Display(Name = "Mesaj")]
        public string? Message { get; set; }
    }

}
