using System.Net;

using System.Net.Mail;

using Microsoft.Extensions.Options;

using IlisanCommerce.Models;

using IlisanCommerce.Data;

using Microsoft.EntityFrameworkCore;



namespace IlisanCommerce.Services

{

    public interface IEmailService

    {

        Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? fromEmail = null);

        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName);

        Task<bool> SendOrderConfirmationAsync(Order order);

        Task<bool> SendAdminOrderNotificationAsync(Order order);

        Task<bool> SendOrderStatusUpdateAsync(Order order, OrderStatus newStatus, string? comment = null);

        Task<bool> SendNewUserWelcomeAsync(User user);

        Task<bool> SendShippingNotificationAsync(Order order, string trackingNumber, string shippingCompany);

        Task<bool> SendDeliveryConfirmationAsync(Order order);

        Task<bool> SendOrderCancellationAsync(Order order, string reason);

        Task<bool> SendContactFormAsync(string senderName, string senderEmail, string senderPhone, string subject, string message);

        Task LogEmailAsync(int templateId, string toEmail, string subject, string body, EmailStatus status, string? errorMessage = null, int? orderId = null, int? userId = null);

    }



    public class EmailService : IEmailService

    {

        private readonly EmailSettings _emailSettings;

        private readonly ApplicationDbContext _context;

        private readonly ILogger<EmailService> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;



        public EmailService(IOptions<EmailSettings> emailSettings, ApplicationDbContext context, ILogger<EmailService> logger, IHttpContextAccessor httpContextAccessor)

        {

            _emailSettings = emailSettings.Value;

            _context = context;

            _logger = logger;

            _httpContextAccessor = httpContextAccessor;

        }



        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? fromEmail = null)

        {

            try

            {

                using var client = new SmtpClient();

                // SMTP configuration
                client.EnableSsl = _emailSettings.EnableSsl;
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = _emailSettings.TimeoutSeconds * 1000; // Convert to milliseconds
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                
                // Port 465 için özel ayarlar
                if (_emailSettings.SmtpPort == 465)
                {
                    client.EnableSsl = true;
                    client.Host = _emailSettings.SmtpServer;
                    client.Port = _emailSettings.SmtpPort;
                }

                

                _logger.LogDebug("Attempting to send email via {SmtpServer}:{SmtpPort} with SSL: {EnableSsl}", 

                    _emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl);



                var mailMessage = new MailMessage

                {

                    From = new MailAddress(fromEmail ?? _emailSettings.SenderEmail, _emailSettings.SenderName),

                    Subject = subject,

                    Body = body,

                    IsBodyHtml = true

                };



                mailMessage.To.Add(toEmail);



                // SMTP sunucusuna bağlan ve mail gönder
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);

                return true;

            }

            catch (SmtpException smtpEx)

            {

                _logger.LogError(smtpEx, "SMTP error sending email to {ToEmail}. StatusCode: {StatusCode}, Message: {Message}", 

                    toEmail, smtpEx.StatusCode, smtpEx.Message);

                return false;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);

                return false;

            }

        }



        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)

        {

            try

            {

                var resetUrl = BuildResetPasswordUrl(resetToken);

                

                var subject = "ILISAN - Şifre Sıfırlama Talebi";

                

                var body = $@"

                <!DOCTYPE html>

                <html>

                <head>

                    <meta charset='utf-8'>

                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>

                    <title>Şifre Sıfırlama</title>

                    <style>

                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}

                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}

                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}

                        .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }}

                        .button {{ display: inline-block; background: #007bff; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}

                        .button:hover {{ background: #0056b3; }}

                        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 14px; }}

                        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}

                    </style>

                </head>

                <body>

                    <div class='container'>

                        <div class='header'>

                            <h1>🔐 Şifre Sıfırlama</h1>

                            <h2>ILISAN Savunma Sanayi</h2>

                        </div>

                        <div class='content'>

                            <h3>Merhaba {userName},</h3>

                            <p>Şifre sıfırlama talebiniz alınmıştır. Aşağıdaki butona tıklayarak yeni şifrenizi belirleyebilirsiniz:</p>

                            

                            <div style='text-align: center;'>

                                <a href='{resetUrl}' class='button'>Şifremi Sıfırla</a>

                            </div>

                            

                            <div class='warning'>

                                <strong>⚠️ Önemli Güvenlik Bilgileri:</strong><br>

                                • Bu link sadece <strong>1 saat</strong> geçerlidir<br>

                                • Link sadece bir kez kullanılabilir<br>

                                • Bu talebi siz yapmadıysanız, bu emaili görmezden gelin<br>

                                • Şifrenizi kimseyle paylaşmayın

                            </div>

                            

                            <p>Eğer buton çalışmıyorsa, aşağıdaki linki kopyalayıp tarayıcınıza yapıştırın:</p>

                            <p style='word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 5px; font-family: monospace;'>

                                {resetUrl}

                            </p>

                        </div>

                        <div class='footer'>

                            <p><strong>ILISAN Savunma Sanayi</strong></p>

                            <p>Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş</p>

                            <p>📞 +90 (850) 532 5237 | 📧 info@ilisan.com.tr</p>

                            <p>🌐 <a href='https://ilisan.com.tr'>ilisan.com.tr</a></p>

                        </div>

                    </div>

                </body>

                </html>";



                var success = await SendEmailAsync(toEmail, subject, body);

                

                if (success)

                {

                    _logger.LogInformation("Password reset email sent to {Email}", toEmail);

                }

                else

                {

                    _logger.LogWarning("Failed to send password reset email to {Email}", toEmail);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);

                return false;

            }

        }



        public async Task<bool> SendOrderConfirmationAsync(Order order)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.OrderConfirmation && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("Order confirmation email template not found");

                    return false;

                }



                var customerEmail = order.CustomerEmail;

                if (string.IsNullOrEmpty(customerEmail))

                {

                    _logger.LogWarning("Customer email is empty for order {OrderId}", order.Id);

                    return false;

                }



                var subject = ReplacePlaceholders(template.Subject, order);

                var body = ReplacePlaceholders(template.Body, order);



                var success = await SendEmailAsync(customerEmail, subject, body);



                await LogEmailAsync(template.Id, customerEmail, subject, body, 

                    success ? EmailStatus.Sent : EmailStatus.Failed, 

                    success ? null : "SMTP send failed", order.Id, order.UserId);



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error sending order confirmation for order {OrderId}", order.Id);

                return false;

            }

        }



        public async Task<bool> SendAdminOrderNotificationAsync(Order order)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.AdminOrderNotification && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("Admin order notification email template not found");

                    return false;

                }



                var subject = ReplacePlaceholders(template.Subject, order);

                var body = ReplacePlaceholders(template.Body, order);



                var success = await SendEmailAsync(_emailSettings.SenderEmail, subject, body);



                await LogEmailAsync(template.Id, _emailSettings.SenderEmail, subject, body,

                    success ? EmailStatus.Sent : EmailStatus.Failed,

                    success ? null : "SMTP send failed", order.Id, order.UserId);



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error sending admin order notification for order {OrderId}", order.Id);

                return false;

            }

        }



        public async Task<bool> SendOrderStatusUpdateAsync(Order order, OrderStatus newStatus, string? comment = null)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.OrderStatusUpdate && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("Order status update email template not found");

                    return false;

                }



                var customerEmail = order.CustomerEmail;

                if (string.IsNullOrEmpty(customerEmail))

                {

                    _logger.LogWarning("Customer email is empty for order {OrderId}", order.Id);

                    return false;

                }



                var subject = ReplacePlaceholders(template.Subject, order, newStatus, comment);

                var body = ReplacePlaceholders(template.Body, order, newStatus, comment);



                var success = await SendEmailAsync(customerEmail, subject, body);



                await LogEmailAsync(template.Id, customerEmail, subject, body,

                    success ? EmailStatus.Sent : EmailStatus.Failed,

                    success ? null : "SMTP send failed", order.Id, order.UserId);



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error sending order status update for order {OrderId}", order.Id);

                return false;

            }

        }



        public async Task LogEmailAsync(int templateId, string toEmail, string subject, string body, EmailStatus status, string? errorMessage = null, int? orderId = null, int? userId = null)

        {

            try

            {

                var emailLog = new EmailLog

                {

                    EmailTemplateId = templateId,

                    ToEmail = toEmail,

                    FromEmail = _emailSettings.SenderEmail,

                    Subject = subject,

                    Body = body,

                    Status = status,

                    ErrorMessage = errorMessage,

                    OrderId = orderId,

                    UserId = userId,

                    SentDate = status == EmailStatus.Sent ? DateTime.Now : null

                };



                _context.EmailLogs.Add(emailLog);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error logging email to database");

            }

        }



        private string ReplacePlaceholders(string text, Order order, OrderStatus? newStatus = null, string? comment = null)

        {

            var result = text

                .Replace("{OrderNumber}", order.OrderNumber)

                .Replace("{CustomerName}", order.CustomerName)

                .Replace("{CustomerEmail}", order.CustomerEmail)

                .Replace("{OrderDate}", order.OrderDate.ToString("dd.MM.yyyy HH:mm"))

                .Replace("{TotalAmount}", order.TotalAmount.ToString("N2"))

                .Replace("{SubTotal}", order.SubTotal.ToString("N2"))

                .Replace("{ShippingCost}", order.ShippingCost.ToString("N2"))

                .Replace("{TaxAmount}", order.TaxAmount.ToString("N2"));



            if (newStatus.HasValue)

            {

                result = result.Replace("{NewStatus}", GetStatusDisplayName(newStatus.Value));

            }



            if (!string.IsNullOrEmpty(comment))

            {

                result = result.Replace("{Comment}", comment);

            }



            return result;

        }



        private string GetStatusDisplayName(OrderStatus status)

        {

            return status switch

            {

                OrderStatus.Pending => "Beklemede",

                OrderStatus.Confirmed => "Onaylandı",

                OrderStatus.Processing => "Hazırlanıyor",

                OrderStatus.Shipped => "Kargoya Verildi",

                OrderStatus.Delivered => "Teslim Edildi",

                OrderStatus.Cancelled => "İptal Edildi",

                OrderStatus.Returned => "İade Edildi",

                _ => "Bilinmeyen"

            };

        }



        private string GetBaseUrl()

        {

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)

            {

                // Fallback URL for background services or when HttpContext is not available

                _logger.LogDebug("HttpContext not available, using fallback URL: {BaseUrl}", _emailSettings.BaseUrl);

                return _emailSettings.BaseUrl;

            }



            var request = httpContext.Request;

            var scheme = request.Scheme; // http veya https

            var host = request.Host.Value; // localhost:7049, localhost:5119, yourdomain.com

            

            var baseUrl = $"{scheme}://{host}";

            

            _logger.LogDebug("Generated base URL: {BaseUrl} from scheme: {Scheme}, host: {Host}", 

                baseUrl, scheme, host);

            

            return baseUrl;

        }



        public string BuildResetPasswordUrl(string token)

        {

            return GetBaseUrl() + $"/Account/ResetPassword?token={token}";

        }



        public string BuildEmailConfirmationUrl(string token)

        {

            return GetBaseUrl() + $"/Account/ConfirmEmail?token={token}";

        }

        public async Task<bool> SendNewUserWelcomeAsync(User user)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.NewUserWelcome && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("NewUserWelcome email template not found");

                    return false;

                }



                var subject = template.Subject;

                var body = template.Body

                    .Replace("{FirstName}", user.FirstName)

                    .Replace("{Email}", user.Email);



                var success = await SendEmailAsync(user.Email, subject, body);

                

                if (success)

                {

                    await LogEmailAsync(template.Id, user.Email, subject, body, EmailStatus.Sent, null, null, user.Id);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send new user welcome email to {Email}", user.Email);

                return false;

            }

        }



        public async Task<bool> SendShippingNotificationAsync(Order order, string trackingNumber, string shippingCompany)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.ShippingNotification && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("ShippingNotification email template not found");

                    return false;

                }



                var customerEmail = order.CustomerEmail;

                if (string.IsNullOrWhiteSpace(customerEmail))

                {

                    _logger.LogWarning("Shipping notification email skipped because customer email is empty for order {OrderId}", order.Id);

                    return false;

                }



                var customerName = order.CustomerName;

                var shippingCity = order.ShippingCity ?? order.ShippingCity ?? string.Empty;

                var shippingAddress = order.ShippingAddressText ?? order.ShippingAddressText ?? string.Empty;



                var subject = template.Subject.Replace("{OrderNumber}", order.OrderNumber);

                var body = template.Body

                    .Replace("{CustomerName}", customerName)

                    .Replace("{OrderNumber}", order.OrderNumber)

                    .Replace("{ShippingCompany}", shippingCompany)

                    .Replace("{TrackingNumber}", trackingNumber)

                    .Replace("{ShippingDate}", DateTime.Now.ToString("dd.MM.yyyy"))

                    .Replace("{EstimatedDelivery}", DateTime.Now.AddDays(3).ToString("dd.MM.yyyy"))

                    .Replace("{TrackingLink}", $"https://kargotakip.com/takip/{trackingNumber}")

                    .Replace("{ShippingAddress}", shippingAddress)

                    .Replace("{ShippingCity}", shippingCity)

                    .Replace("{CustomerEmail}", customerEmail);



                var success = await SendEmailAsync(customerEmail, subject, body);

                

                if (success)

                {

                    await LogEmailAsync(template.Id, customerEmail, subject, body, EmailStatus.Sent, null, order.Id, order.UserId);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send shipping notification email for order {OrderNumber}", order.OrderNumber);

                return false;

            }

        }



        public async Task<bool> SendDeliveryConfirmationAsync(Order order)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.DeliveryConfirmation && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("DeliveryConfirmation email template not found");

                    return false;

                }



                var customerEmail = order.CustomerEmail;

                if (string.IsNullOrWhiteSpace(customerEmail))

                {

                    _logger.LogWarning("Delivery confirmation email skipped because customer email is empty for order {OrderId}", order.Id);

                    return false;

                }



                var customerName = order.CustomerName;

                var deliveryAddress = order.ShippingAddressText ?? order.ShippingAddressText ?? string.Empty;

                var deliveryCity = order.ShippingCity ?? order.ShippingCity ?? string.Empty;

                string deliveryAddressDisplay;



                if (!string.IsNullOrWhiteSpace(deliveryAddress) && !string.IsNullOrWhiteSpace(deliveryCity))

                {

                    deliveryAddressDisplay = $"{deliveryAddress}, {deliveryCity}";

                }

                else if (!string.IsNullOrWhiteSpace(deliveryAddress))

                {

                    deliveryAddressDisplay = deliveryAddress;

                }

                else

                {

                    deliveryAddressDisplay = deliveryCity;

                }



                var subject = template.Subject.Replace("{OrderNumber}", order.OrderNumber);

                var body = template.Body

                    .Replace("{CustomerName}", customerName)

                    .Replace("{OrderNumber}", order.OrderNumber)

                    .Replace("{DeliveryDate}", DateTime.Now.ToString("dd.MM.yyyy"))

                    .Replace("{DeliveryAddress}", deliveryAddressDisplay)

                    .Replace("{CustomerEmail}", customerEmail);



                var success = await SendEmailAsync(customerEmail, subject, body);

                

                if (success)

                {

                    await LogEmailAsync(template.Id, customerEmail, subject, body, EmailStatus.Sent, null, order.Id, order.UserId);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send delivery confirmation email for order {OrderNumber}", order.OrderNumber);

                return false;

            }

        }



        public async Task<bool> SendOrderCancellationAsync(Order order, string reason)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.OrderCancellation && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("OrderCancellation email template not found");

                    return false;

                }



                var customerEmail = order.CustomerEmail;

                if (string.IsNullOrWhiteSpace(customerEmail))

                {

                    _logger.LogWarning("Order cancellation email skipped because customer email is empty for order {OrderId}", order.Id);

                    return false;

                }



                var customerName = order.CustomerName;



                var subject = template.Subject.Replace("{OrderNumber}", order.OrderNumber);

                var body = template.Body

                    .Replace("{CustomerName}", customerName)

                    .Replace("{OrderNumber}", order.OrderNumber)

                    .Replace("{CancellationDate}", DateTime.Now.ToString("dd.MM.yyyy"))

                    .Replace("{CancellationReason}", reason)

                    .Replace("{TotalAmount}", order.TotalAmount.ToString("N2"))

                    .Replace("{CustomerEmail}", customerEmail);



                var success = await SendEmailAsync(customerEmail, subject, body);

                

                if (success)

                {

                    await LogEmailAsync(template.Id, customerEmail, subject, body, EmailStatus.Sent, null, order.Id, order.UserId);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send order cancellation email for order {OrderNumber}", order.OrderNumber);

                return false;

            }

        }



        public async Task<bool> SendContactFormAsync(string senderName, string senderEmail, string senderPhone, string subject, string message)

        {

            try

            {

                var template = await _context.EmailTemplates

                    .FirstOrDefaultAsync(t => t.TemplateType == EmailTemplateType.ContactForm && t.IsActive);



                if (template == null)

                {

                    _logger.LogWarning("ContactForm email template not found");

                    return false;

                }



                var emailSubject = template.Subject;

                var body = template.Body

                    .Replace("{SenderName}", senderName)

                    .Replace("{SenderEmail}", senderEmail)

                    .Replace("{SenderPhone}", senderPhone)

                    .Replace("{Subject}", subject)

                    .Replace("{Message}", message)

                    .Replace("{SendDate}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));



                var success = await SendEmailAsync(_emailSettings.SenderEmail, emailSubject, body);

                

                if (success)

                {

                    await LogEmailAsync(template.Id, _emailSettings.SenderEmail, emailSubject, body, EmailStatus.Sent);

                }



                return success;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Failed to send contact form email from {SenderEmail}", senderEmail);

                return false;

            }

        }

    }



    public class EmailSettings

    {

        public string SmtpServer { get; set; } = string.Empty;

        public int SmtpPort { get; set; }

        public string SenderEmail { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool EnableSsl { get; set; }

        public EmailProvider Provider { get; set; } = EmailProvider.Gmail;

        public int TimeoutSeconds { get; set; } = 30;

        public string BaseUrl { get; set; } = "https://localhost:7049";

    }



    public enum EmailProvider

    {

        Gmail = 1,

        Outlook = 2,

        Yahoo = 3,

        Custom = 4

    }



    public static class EmailProviderSettings

    {

        public static EmailSettings GetProviderSettings(EmailProvider provider)

        {

            return provider switch

            {

                EmailProvider.Gmail => new EmailSettings

                {

                    SmtpServer = "smtp.gmail.com",

                    SmtpPort = 587,

                    EnableSsl = true

                },

                EmailProvider.Outlook => new EmailSettings

                {

                    SmtpServer = "smtp-mail.outlook.com",

                    SmtpPort = 587,

                    EnableSsl = true

                },

                EmailProvider.Yahoo => new EmailSettings

                {

                    SmtpServer = "smtp.mail.yahoo.com",

                    SmtpPort = 587,

                    EnableSsl = true

                },

                _ => new EmailSettings()

            };

        }

    }

}

