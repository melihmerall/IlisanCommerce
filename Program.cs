using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using IlisanCommerce.Data;
using IlisanCommerce.Services;
using IlisanCommerce.Services.Logging;
using IlisanCommerce.Services.Pdf;
using IlisanCommerce.Services.Reports;
using IlisanCommerce.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Log tablosu ayarları
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn("UserName", SqlDbType.NVarChar, allowNull: true, dataLength: 50),
        new SqlColumn("MachineName", SqlDbType.NVarChar, allowNull: true, dataLength: 50),
    }
};

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // appsettings.json'dan oku
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "IlisanShop") // sabit uygulama adı ekle
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "ilisan.Logs",
            AutoCreateSqlTable = true // tablo yoksa otomatik oluştur
        },
        columnOptions: columnOptions
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddControllersWithViews();

// Memory cache for settings
builder.Services.AddMemoryCache();

// Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Session support for guest cart
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add HttpContext accessor
builder.Services.AddHttpContextAccessor();

// Register services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<IIyzicoPaymentService, IyzicoPaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IApplicationLogService, ApplicationLogService>();

// Logging services - Enterprise-grade logging system
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<ILogQueryService, LogQueryService>();

// PDF services - Professional document generation
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session before authorization
app.UseSession();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom routing for SEO-friendly URLs
// Ana Sayfa - SEO Friendly (root domain)
app.MapControllerRoute(
    name: "home",
    pattern: "",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "products",
    pattern: "urunler",
    defaults: new { controller = "Product", action = "Index" });

app.MapControllerRoute(
    name: "about", 
    pattern: "hakkimizda",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "contact",
    pattern: "iletisim",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "cart",
    pattern: "sepet",
    defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "login",
    pattern: "giris", 
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "register",
    pattern: "uye-ol",
    defaults: new { controller = "Account", action = "Register" });

// User Account Pages
app.MapControllerRoute(
    name: "profile",
    pattern: "hesabim",
    defaults: new { controller = "Account", action = "Profile" });

// Checkout & Orders
app.MapControllerRoute(
    name: "checkout",
    pattern: "odeme",
    defaults: new { controller = "Order", action = "Checkout" });

app.MapControllerRoute(
    name: "orders",
    pattern: "siparislerim",
    defaults: new { controller = "Order", action = "Index" });

app.MapControllerRoute(
    name: "product",
    pattern: "urun/{slug}",
    defaults: new { controller = "Product", action = "Details" });

app.MapControllerRoute(
    name: "category",
    pattern: "kategori/{slug}",
    defaults: new { controller = "Product", action = "Category" });

// Admin Area Routes - Fixed routing configuration
app.MapControllerRoute(
    name: "admin_categories",
    pattern: "admin/categories/{action=Index}/{id?}",
    defaults: new { controller = "Categories" });

app.MapControllerRoute(
    name: "admin_reports", 
    pattern: "admin/reports/{action=Index}/{id?}",
    defaults: new { controller = "Reports" });

app.MapControllerRoute(
    name: "admin_customers",
    pattern: "admin/customers/{action=Index}/{id?}",
    defaults: new { controller = "Customers" });

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });

// API Routes - More specific pattern
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=api}/{action=Index}/{id?}",
    constraints: new { controller = @"^[Aa]pi.*" });

// Default route - fallback for any unmatched URLs  
app.MapControllerRoute(
    name: "about",
    pattern: "hakkimizda",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "contact",
    pattern: "iletisim",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
        Log.Information("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while initializing the database");
    }
}

Log.Information("ILISAN Commerce application started");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
