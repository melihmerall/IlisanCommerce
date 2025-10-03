using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using IlisanCommerce.Models;
using Microsoft.Extensions.Configuration;
using IyzicoAddress = Iyzipay.Model.Address;

namespace IlisanCommerce.Services
{
    public interface IIyzicoPaymentService
    {
        Task<CheckoutFormInitialize> InitializeCheckoutFormAsync(CheckoutFormRequest request);
        Task<CheckoutForm> RetrieveCheckoutFormAsync(string token);
    }

    public class IyzicoPaymentService : IIyzicoPaymentService
    {
        private readonly Iyzipay.Options _options;
        private readonly ILogger<IyzicoPaymentService> _logger;

        public IyzicoPaymentService(IConfiguration configuration, ILogger<IyzicoPaymentService> logger)
        {
            _logger = logger;

            _options = new Iyzipay.Options
            {
                ApiKey = configuration["Iyzico:ApiKey"] ?? string.Empty,
                SecretKey = configuration["Iyzico:SecretKey"] ?? string.Empty,
                BaseUrl = configuration["Iyzico:BaseUrl"] ?? string.Empty
            };
        }

        public async Task<CheckoutFormInitialize> InitializeCheckoutFormAsync(CheckoutFormRequest request)
        {
            try
            {
                var checkoutFormRequest = new CreateCheckoutFormInitializeRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = request.ConversationId,
                    Price = request.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                    PaidPrice = request.PaidPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                    Currency = Currency.TRY.ToString(),
                    BasketId = request.BasketId,
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                    CallbackUrl = request.CallbackUrl,
                    EnabledInstallments = request.EnabledInstallments ?? new List<int> { 1, 2, 3, 6, 9, 12 },

                    Buyer = new Buyer
                    {
                        Id = request.BuyerId,
                        Name = request.BuyerName,
                        Surname = request.BuyerSurname,
                        GsmNumber = request.BuyerGsmNumber,
                        Email = request.BuyerEmail,
                        IdentityNumber = request.BuyerIdentityNumber,
                        LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        RegistrationAddress = request.BuyerAddress,
                        Ip = request.BuyerIp,
                        City = request.BuyerCity,
                        Country = "Turkey",
                        ZipCode = request.BuyerZipCode
                    },

                    ShippingAddress = new IyzicoAddress
                    {
                        ContactName = request.ShippingContactName,
                        City = request.ShippingCity,
                        Country = "Turkey",
                        Description = request.ShippingAddress,
                        ZipCode = request.ShippingZipCode
                    },

                    BillingAddress = new IyzicoAddress
                    {
                        ContactName = request.BillingContactName,
                        City = request.BillingCity,
                        Country = "Turkey",
                        Description = request.BillingAddress,
                        ZipCode = request.BillingZipCode
                    },

                    BasketItems = request.BasketItems?.Select(item => new BasketItem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Category1 = item.Category,
                        ItemType = BasketItemType.PHYSICAL.ToString(),
                        Price = item.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                    }).ToList() ?? new List<BasketItem>()
                };

                return await Task.Run(() => CheckoutFormInitialize.Create(checkoutFormRequest, _options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing checkout form for conversation ID: {ConversationId}", request.ConversationId);
                throw;
            }
        }

        public async Task<CheckoutForm> RetrieveCheckoutFormAsync(string token)
        {
            try
            {
                var retrieveRequest = new RetrieveCheckoutFormRequest
                {
                    Token = token
                };

                return await Task.Run(() => CheckoutForm.Retrieve(retrieveRequest, _options));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving checkout form for token: {Token}", token);
                throw;
            }
        }
    }

    // Checkout Form Request Model
    public class CheckoutFormRequest
    {
        public string ConversationId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public string BasketId { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public List<int>? EnabledInstallments { get; set; }

        // Alıcı bilgileri
        public string BuyerId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerSurname { get; set; } = string.Empty;
        public string BuyerGsmNumber { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerIdentityNumber { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;
        public string BuyerIp { get; set; } = string.Empty;
        public string BuyerCity { get; set; } = string.Empty;
        public string BuyerZipCode { get; set; } = string.Empty;

        // Teslimat adresi
        public string ShippingContactName { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingZipCode { get; set; } = string.Empty;

        // Fatura adresi
        public string BillingContactName { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingZipCode { get; set; } = string.Empty;

        // Sepet öğeleri
        public List<BasketItemModel>? BasketItems { get; set; }
    }

    // Legacy Payment Request Model (kept for backward compatibility)
    public class PaymentRequest
    {
        public string ConversationId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal PaidPrice { get; set; }
        public int Installment { get; set; } = 1;
        public string BasketId { get; set; } = string.Empty;

        // Kart bilgileri
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpireMonth { get; set; } = string.Empty;
        public string ExpireYear { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;

        // Alıcı bilgileri
        public string BuyerId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerSurname { get; set; } = string.Empty;
        public string BuyerGsmNumber { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string BuyerIdentityNumber { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;
        public string BuyerIp { get; set; } = string.Empty;
        public string BuyerCity { get; set; } = string.Empty;
        public string BuyerZipCode { get; set; } = string.Empty;

        // Teslimat adresi
        public string ShippingContactName { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingZipCode { get; set; } = string.Empty;

        // Fatura adresi
        public string BillingContactName { get; set; } = string.Empty;
        public string BillingCity { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string BillingZipCode { get; set; } = string.Empty;

        // Sepet öğeleri
        public List<BasketItemModel>? BasketItems { get; set; }
    }

    public class BasketItemModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}