using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using IlisanCommerce.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;

namespace IlisanCommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;
        private readonly SettingsService _settingsService;
        private readonly IApplicationLogService _applicationLogService;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, 
            IEmailService emailService, ICartService cartService, SettingsService settingsService,
            IApplicationLogService applicationLogService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _cartService = cartService;
            _settingsService = settingsService;
            _applicationLogService = applicationLogService;
        }

        // GET: Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Title"] = "Giriş Yap";
            ViewData["Description"] = "ILISAN hesabınıza giriş yapın";
            
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Önce normal kullanıcıları kontrol et
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

                    _logger.LogInformation("Login attempt for email: {Email}, User found: {UserFound}", 
                        model.Email, user != null);

                    // Eğer normal kullanıcı bulunamazsa, admin kullanıcılarını kontrol et
                    if (user == null)
                    {
                        var adminUser = await _context.AdminUsers
                            .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);
                        
                        _logger.LogInformation("Admin user search for email: {Email}, AdminUser found: {AdminUserFound}", 
                            model.Email, adminUser != null);

                        // Database logging
                        await _applicationLogService.LogInformationAsync(
                            $"Admin user search for email: {model.Email}, AdminUser found: {adminUser != null}",
                            source: "AccountController",
                            action: "Login",
                            userEmail: model.Email,
                            userRole: "Admin",
                            properties: new { Email = model.Email, AdminUserFound = adminUser != null }
                        );

                        if (adminUser != null)
                        {
                            _logger.LogInformation("AdminUser PasswordHash: {Hash}", adminUser.PasswordHash);
                            
                            // Database logging for password verification
                            await _applicationLogService.LogInformationAsync(
                                $"AdminUser PasswordHash verification started for user: {adminUser.Email}",
                                source: "AccountController",
                                action: "PasswordVerification",
                                userId: adminUser.Id.ToString(),
                                userEmail: adminUser.Email,
                                userRole: "Admin",
                                properties: new { AdminUserId = adminUser.Id, Email = adminUser.Email }
                            );
                            
                            bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, adminUser.PasswordHash);
                            _logger.LogInformation("Password verification result: {Result}", passwordValid);
                            
                            // Database logging for password verification result
                            await _applicationLogService.LogInformationAsync(
                                $"Password verification result: {passwordValid} for user: {adminUser.Email}",
                                source: "AccountController",
                                action: "PasswordVerification",
                                userId: adminUser.Id.ToString(),
                                userEmail: adminUser.Email,
                                userRole: "Admin",
                                properties: new { AdminUserId = adminUser.Id, Email = adminUser.Email, PasswordValid = passwordValid }
                            );
                            
                            if (passwordValid)
                            {
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
                                    new Claim(ClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
                                    new Claim(ClaimTypes.Email, adminUser.Email),
                                    new Claim(ClaimTypes.Role, adminUser.Role.ToString())
                                };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var authProperties = new AuthenticationProperties
                                {
                                    IsPersistent = model.RememberMe,
                                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                    new ClaimsPrincipal(claimsIdentity), authProperties);

                                // Update last login
                                adminUser.LastLoginDate = DateTime.Now;
                                await _context.SaveChangesAsync();

                                _logger.LogInformation("Admin User {Email} logged in successfully at {Time}", adminUser.Email, DateTime.Now);

                                TempData["SuccessMessage"] = "Panele başarıyla giriş yapıldı!";
                                
                                // Admin kullanıcısı için admin paneline yönlendir
                                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }

                                return RedirectToAction("Index", "Admin", new { area = "" });
                            }
                            else
                            {
                                _logger.LogWarning("Admin user {Email} password verification failed", model.Email);
                                TempData["ErrorMessage"] = "Şifre yanlış!";
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No user found with email: {Email}", model.Email);
                            TempData["ErrorMessage"] = "Bu email adresi ile kayıtlı kullanıcı bulunamadı!";
                        }
                    }
                    else
                    {
                        _logger.LogInformation("User PasswordHash: {Hash}", user.PasswordHash);
                        
                        bool passwordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                        _logger.LogInformation("Password verification result: {Result}", passwordValid);
                        
                        if (passwordValid)
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Role, user.Role.ToString())
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = model.RememberMe,
                                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
                            };

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                new ClaimsPrincipal(claimsIdentity), authProperties);

                            // LastLoginDate property removed - login tracking simplified
                            // await _context.SaveChangesAsync(); // Not needed for login tracking

                            // Merge guest cart to user cart
                            var sessionId = HttpContext.Session.Id;
                            if (!string.IsNullOrEmpty(sessionId))
                            {
                                await _cartService.MergeGuestCartToUserAsync(sessionId, user.Id);
                            }

                            _logger.LogInformation("User {Email} logged in successfully at {Time}", user.Email, DateTime.Now);
                            TempData["SuccessMessage"] = "Başarıyla giriş yapıldı!";

                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            _logger.LogWarning("User {Email} password verification failed", model.Email);
                            TempData["ErrorMessage"] = "Şifre yanlış!";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during login for email: {Email}", model.Email);
                    TempData["ErrorMessage"] = "Giriş sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                }
            }

            return View(model);
        }

        // GET: Account/Register
        public async Task<IActionResult> Register()
        {
            // Kullanıcı kaydına izin verilip verilmediğini kontrol et
            var enableRegistration = await _settingsService.GetEnableUserRegistrationAsync();
            if (!enableRegistration)
            {
                TempData["Error"] = "Kullanıcı kaydı şu anda devre dışı.";
                return RedirectToAction("Login");
            }

            ViewData["Title"] = "Kayıt Ol";
            ViewData["Description"] = "ILISAN'a üye olun";
            
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Kullanıcı kaydına izin verilip verilmediğini kontrol et
            var enableRegistration = await _settingsService.GetEnableUserRegistrationAsync();
            if (!enableRegistration)
            {
                TempData["Error"] = "Kullanıcı kaydı şu anda devre dışı.";
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users
                    .AnyAsync(u => u.Email == model.Email);

                if (existingUser)
                {
                    ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor.");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailConfirmed = false,
                    EmailConfirmationToken = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Merge guest cart to user cart
                var sessionId = HttpContext.Session.Id;
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await _cartService.MergeGuestCartToUserAsync(sessionId, user.Id);
                }

                // Send professional welcome email
                try
                {
                    await SendWelcomeEmailAsync(user);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                }

                TempData["SuccessMessage"] = $"🎉 Hoş geldin {user.FirstName}! Hesabın başarıyla oluşturuldu. Artık giriş yapabilirsin.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "Şifremi Unuttum";
            ViewData["Description"] = "Şifre sıfırlama";
            
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Önce admin kullanıcılarını kontrol et
                    var adminUser = await _context.AdminUsers
                        .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

                    if (adminUser != null)
                    {
                        // Admin kullanıcısı için token oluştur
                        var resetToken = Guid.NewGuid().ToString();
                        adminUser.PasswordResetToken = resetToken;
                        adminUser.PasswordResetTokenExpiry = DateTime.Now.AddHours(1); // 1 saat geçerli
                        
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Password reset token generated for admin: {Email}", model.Email);

                        // Professional email gönder
                        var emailSent = await _emailService.SendPasswordResetEmailAsync(
                            adminUser.Email, 
                            resetToken, 
                            $"{adminUser.FirstName} {adminUser.LastName}"
                        );

                        if (emailSent)
                        {
                            TempData["SuccessMessage"] = "Şifre sıfırlama linki email adresinize gönderildi! Email kutunuzu kontrol edin.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Email gönderilirken bir hata oluştu. Lütfen tekrar deneyin.";
                        }
                    }
                    else
                    {
                        // Normal kullanıcıları da kontrol et
                        var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);

                        if (user != null)
                        {
                            // Normal kullanıcı için token oluştur
                            var resetToken = Guid.NewGuid().ToString();
                            user.PasswordResetToken = resetToken;
                            user.PasswordResetTokenExpiry = DateTime.Now.AddHours(1);
                            
                            await _context.SaveChangesAsync();

                            _logger.LogInformation("Password reset token generated for user: {Email}", model.Email);

                            // Professional email gönder
                            var emailSent = await _emailService.SendPasswordResetEmailAsync(
                                user.Email, 
                                resetToken, 
                                $"{user.FirstName} {user.LastName}"
                            );

                            if (emailSent)
                            {
                                TempData["SuccessMessage"] = "Şifre sıfırlama linki email adresinize gönderildi! Email kutunuzu kontrol edin.";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Email gönderilirken bir hata oluştu. Lütfen tekrar deneyin.";
                            }
                        }
                        else
                        {
                            // Güvenlik için her zaman başarılı mesaj göster
                            TempData["SuccessMessage"] = "Eğer bu email adresi ile kayıtlı bir hesap varsa, şifre sıfırlama linki gönderildi.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during forgot password for email: {Email}", model.Email);
                    TempData["ErrorMessage"] = "Şifre sıfırlama sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                }

                return RedirectToAction("ForgotPassword");
            }

            return View(model);
        }

        // GET: Account/ResetPassword
        public async Task<IActionResult> ResetPassword(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Geçersiz şifre sıfırlama linki!";
                return RedirectToAction("ForgotPassword");
            }

            // Admin kullanıcılarını kontrol et
            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && 
                                         u.PasswordResetTokenExpiry > DateTime.Now &&
                                         u.IsActive);

            if (adminUser != null)
            {
                var model = new ResetPasswordViewModel { Token = token };
                ViewData["Title"] = "Şifre Sıfırla";
                ViewData["Description"] = "Yeni şifrenizi belirleyin";
                return View(model);
            }

            // Normal kullanıcıları kontrol et
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && 
                                         u.PasswordResetTokenExpiry > DateTime.Now &&
                                         u.IsActive);

            if (user != null)
            {
                var model = new ResetPasswordViewModel { Token = token };
                ViewData["Title"] = "Şifre Sıfırla";
                ViewData["Description"] = "Yeni şifrenizi belirleyin";
                return View(model);
            }

            TempData["ErrorMessage"] = "Şifre sıfırlama linki geçersiz veya süresi dolmuş!";
            return RedirectToAction("ForgotPassword");
        }

        // POST: Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Admin kullanıcılarını kontrol et
                    var adminUser = await _context.AdminUsers
                        .FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && 
                                                 u.PasswordResetTokenExpiry > DateTime.Now &&
                                                 u.IsActive);

                    if (adminUser != null)
                    {
                        // Admin şifresi sıfırla
                        adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        adminUser.PasswordResetToken = null;
                        adminUser.PasswordResetTokenExpiry = null;
                        
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Password reset successfully for admin: {Email}", adminUser.Email);
                        
                        TempData["SuccessMessage"] = "Şifreniz başarıyla sıfırlandı! Artık yeni şifrenizle giriş yapabilirsiniz.";
                        return RedirectToAction("Login");
                    }

                    // Normal kullanıcıları kontrol et
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.PasswordResetToken == model.Token && 
                                                 u.PasswordResetTokenExpiry > DateTime.Now &&
                                                 u.IsActive);

                    if (user != null)
                    {
                        // Normal kullanıcı şifresi sıfırla
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                        user.PasswordResetToken = null;
                        user.PasswordResetTokenExpiry = null;
                        
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Password reset successfully for user: {Email}", user.Email);
                        
                        TempData["SuccessMessage"] = "Şifreniz başarıyla sıfırlandı! Artık yeni şifrenizle giriş yapabilirsiniz.";
                        return RedirectToAction("Login");
                    }

                    TempData["ErrorMessage"] = "Geçersiz veya süresi dolmuş şifre sıfırlama linki!";
                    return RedirectToAction("ForgotPassword");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during password reset with token: {Token}", model.Token);
                    TempData["ErrorMessage"] = "Şifre sıfırlama sırasında bir hata oluştu. Lütfen tekrar deneyin.";
                }
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out at {Time}", DateTime.Now);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserProfileViewModel
            {
                User = user,
                RecentOrders = user.Orders.OrderByDescending(o => o.CreatedDate).Take(5).ToList(),
                Addresses = user.Addresses.Where(a => a.IsActive).ToList(),
                TotalOrders = user.Orders.Count,
                TotalSpent = user.Orders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount)
            };

            ViewData["Title"] = "Hesabım";
            ViewData["Description"] = "Kullanıcı profili";

            return View(model);
        }

        // Helper method
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        // Professional welcome email
        private async Task SendWelcomeEmailAsync(User user)
        {
            try
            {
                // Template'den hoş geldin email'i gönder
                await _emailService.SendNewUserWelcomeAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email template to {Email}", user.Email);
                
                // Fallback: Basit email gönder
                var subject = "ILISAN'a Hoş Geldiniz! 🎉";
                var body = $"Merhaba {user.FirstName},\n\nILISAN ailesine hoş geldiniz! Hesabınız başarıyla oluşturuldu.\n\nGiriş yapmak için: https://ilisan.com.tr/giris\n\nTeşekkürler,\nILISAN Savunma Sanayi";
                
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
        }
    }
}
