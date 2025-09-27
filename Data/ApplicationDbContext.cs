using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Models;
using IlisanCommerce.Models.Logging;

namespace IlisanCommerce.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductVariantImage> ProductVariantImages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<ShippingRate> ShippingRates { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AdminActivityLog> AdminActivityLogs { get; set; }
        
        // Logging System DbSets
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category Self-Reference
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductImage - Product
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductSpecification - Product
            modelBuilder.Entity<ProductSpecification>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSpecifications)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductVariant - Product
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductVariantImage - ProductVariant
            modelBuilder.Entity<ProductVariantImage>()
                .HasOne(pvi => pvi.ProductVariant)
                .WithMany(pv => pv.VariantImages)
                .HasForeignKey(pvi => pvi.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Address - User (Optional for guest orders)
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Allow null UserId for guest orders

            // CartItem relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductVariant)
                .WithMany(pv => pv.CartItems)
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.BillingAddress)
                .WithMany(a => a.BillingOrders)
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(a => a.ShippingOrders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany(pv => pv.OrderItems)
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Note: OrderStatusHistory removed - not needed for current implementation

            // EmailLog relationships
            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.EmailTemplate)
                .WithMany(et => et.EmailLogs)
                .HasForeignKey(el => el.EmailTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.Order)
                .WithMany()
                .HasForeignKey(el => el.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.User)
                .WithMany()
                .HasForeignKey(el => el.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for performance
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(pv => pv.SKU)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.UserId, ci.ProductId, ci.ProductVariantId });

            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => ci.SessionId);

            // SiteSetting configuration
            modelBuilder.Entity<SiteSetting>()
                .HasIndex(s => s.Key)
                .IsUnique();

            modelBuilder.Entity<SiteSetting>()
                .Property(s => s.Key)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<SiteSetting>()
                .Property(s => s.Value)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<SiteSetting>()
                .Property(s => s.Category)
                .HasMaxLength(50)
                .HasDefaultValue("General");

            // AdminUser relationships
            modelBuilder.Entity<AdminUser>()
                .HasIndex(au => au.Email)
                .IsUnique();

            // AdminActivityLog relationships
            modelBuilder.Entity<AdminActivityLog>()
                .HasOne(aal => aal.AdminUser)
                .WithMany(au => au.ActivityLogs)
                .HasForeignKey(aal => aal.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShippingRate indexes
            modelBuilder.Entity<ShippingRate>()
                .HasIndex(sr => new { sr.MinDesi, sr.MaxDesi });

            modelBuilder.Entity<ShippingRate>()
                .HasIndex(sr => sr.IsDefault);

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Ana kategoriler
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Balistik Yelekler",
                    Description = "NATO standartlarında kurşun geçirmez balistik yelekler",
                    Slug = "balistik-yelekler",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Category
                {
                    Id = 2,
                    Name = "Anti-Rad Ürünleri",
                    Description = "Radyasyon önleyici ürünler",
                    Slug = "anti-rad-urunleri",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Category
                {
                    Id = 3,
                    Name = "Askeri Tekstil",
                    Description = "Askeri kamuflaj ve tekstil ürünleri",
                    Slug = "askeri-tekstil",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Admin kullanıcı
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@ilisan.com.tr",
                    PasswordHash = "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", // Admin123!
                    Role = UserRole.SuperAdmin,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Admin Panel Kullanıcısı
            modelBuilder.Entity<AdminUser>().HasData(
                new AdminUser
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@ilisan.com.tr",
                    PasswordHash = "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", // Admin123!
                    Role = AdminRole.SuperAdmin,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Kargo Fiyatlandırması
            modelBuilder.Entity<ShippingRate>().HasData(
                new ShippingRate
                {
                    Id = 1,
                    Name = "Standart Kargo",
                    Description = "1-5 desi arası ürünler için standart kargo",
                    MinDesi = 0,
                    MaxDesi = 5,
                    ShippingCost = 20.00m,
                    FreeShippingThreshold = 500.00m,
                    EstimatedDeliveryDays = "1-2 gün",
                    IsActive = true,
                    IsDefault = true,
                    SortOrder = 1,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new ShippingRate
                {
                    Id = 2,
                    Name = "Ağır Kargo",
                    Description = "5-10 desi arası ürünler için ağır kargo",
                    MinDesi = 5,
                    MaxDesi = 10,
                    ShippingCost = 40.00m,
                    FreeShippingThreshold = 1000.00m,
                    EstimatedDeliveryDays = "2-3 gün",
                    IsActive = true,
                    IsDefault = false,
                    SortOrder = 2,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new ShippingRate
                {
                    Id = 3,
                    Name = "Özel Kargo",
                    Description = "10 desi üzeri ürünler için özel kargo",
                    MinDesi = 10,
                    MaxDesi = null,
                    ShippingCost = 80.00m,
                    FreeShippingThreshold = 2000.00m,
                    EstimatedDeliveryDays = "3-5 gün",
                    IsActive = true,
                    IsDefault = false,
                    SortOrder = 3,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Email şablonları
            modelBuilder.Entity<EmailTemplate>().HasData(
                new EmailTemplate
                {
                    Id = 1,
                    Name = "Sipariş Onayı - Müşteri",
                    Subject = "ILISAN - Siparişiniz Alındı #{OrderNumber}",
                    Body = @"
                        <h2>Sayın {CustomerName},</h2>
                        <p>Siparişiniz başarıyla alınmıştır.</p>
                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>
                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>
                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>
                        <p>Siparişiniz en kısa sürede hazırlanacaktır.</p>
                        <p>Teşekkür ederiz.</p>
                        <p><strong>ILISAN Savunma Sanayi</strong></p>
                    ",
                    TemplateType = EmailTemplateType.OrderConfirmation,
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new EmailTemplate
                {
                    Id = 2,
                    Name = "Yeni Sipariş - Admin",
                    Subject = "ILISAN - Yeni Sipariş Alındı #{OrderNumber}",
                    Body = @"
                        <h2>Yeni Sipariş Alındı</h2>
                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>
                        <p><strong>Müşteri:</strong> {CustomerName}</p>
                        <p><strong>Email:</strong> {CustomerEmail}</p>
                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>
                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>
                        <p>Lütfen siparişi kontrol edip onaylayınız.</p>
                    ",
                    TemplateType = EmailTemplateType.AdminOrderNotification,
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Site Ayarları (Yeni SiteSetting Modeli) - Temporarily disabled


            // Slider
            modelBuilder.Entity<Slider>().HasData(
                new Slider
                {
                    Id = 1,
                    Title = "Savunma Sanayi",
                    Subtitle = "Tam bağımsız Türkiye için hiç durmadan çalışmaya devam edeceğiz.",
                    ImageUrl = "/images/slideshow/slide_1920x1080.png",
                    ButtonText = "Ürünlerimizi İnceleyin",
                    ButtonUrl = "/urunler",
                    IsActive = true,
                    SortOrder = 1,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Örnek Ürünler ve Diğer Data'lar
            SeedProducts(modelBuilder);
            SeedAdditionalData(modelBuilder);
        }

        private void SeedProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                // Balistik Yelekler
                new Product
                {
                    Id = 1,
                    Name = "ILISAN BV-01 Balistik Yelek",
                    ProductCode = "ILISAN-BV-01",
                    Slug = "ilisan-bv-01-balistik-yelek",
                    ShortDescription = "NATO standartlarında Level IIIA koruma sağlayan hafif balistik yelek",
                    LongDescription = "ILISAN BV-01 Balistik Yelek, NATO standartlarında Level IIIA koruma sağlayan son teknoloji balistik koruma sistemidir.",
                    Price = 2500.00m,
                    ComparePrice = 3000.00m,
                    CategoryId = 1,
                    Brand = "ILISAN",
                    Material = "Bor + Polietilen",
                    Weight = "1.2 kg",
                    Desi = 2.5m,
                    Color = "Siyah",
                    ProtectionLevel = 3,
                    CertificateType = "NATO STANAG 2920",
                    StockQuantity = 50,
                    MinStockLevel = 5,
                    IsActive = true,
                    IsFeatured = true,
                    IsSpecialProduct = true,
                    MetaTitle = "ILISAN BV-01 Balistik Yelek",
                    MetaDescription = "NATO STANAG 2920 Level IIIA sertifikalı balistik yelek",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 2,
                    Name = "ILISAN BV-02 Taktikal Yelek",
                    ProductCode = "ILISAN-BV-02",
                    Slug = "ilisan-bv-02-taktikal-yelek",
                    ShortDescription = "Özel kuvvetler için tasarlanmış hafif taktikal balistik yelek",
                    LongDescription = "ILISAN BV-02 Taktikal Yelek, özel kuvvetler için özel olarak tasarlanmış hafif balistik koruma sistemidir.",
                    Price = 1800.00m,
                    ComparePrice = 2200.00m,
                    CategoryId = 1,
                    Brand = "ILISAN",
                    Material = "Aramid + Seramik",
                    Weight = "0.9 kg",
                    Desi = 2.0m,
                    Color = "Kamuflaj",
                    ProtectionLevel = 2,
                    CertificateType = "NIJ Level II",
                    StockQuantity = 30,
                    MinStockLevel = 3,
                    IsActive = true,
                    IsFeatured = true,
                    IsSpecialProduct = false,
                    MetaTitle = "ILISAN BV-02 Taktikal Yelek",
                    MetaDescription = "Özel kuvvetler için taktikal balistik yelek",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 3,
                    Name = "ILISAN AR-01 Anti-Rad Önlük",
                    ProductCode = "ILISAN-AR-01",
                    Slug = "ilisan-ar-01-anti-rad-onluk",
                    ShortDescription = "Tıbbi personel için radyasyon koruyucu kurşun içermeyen önlük",
                    LongDescription = "ILISAN AR-01 Anti-Rad Önlük, tıbbi personel için özel olarak geliştirilmiş kurşun içermeyen radyasyon koruma sistemidir.",
                    Price = 850.00m,
                    CategoryId = 2,
                    Brand = "ILISAN",
                    Material = "Bor + Tekstil",
                    Weight = "0.6 kg",
                    Desi = 1.5m,
                    Color = "Mavi",
                    StockQuantity = 100,
                    MinStockLevel = 10,
                    IsActive = true,
                    IsFeatured = true,
                    IsSpecialProduct = false,
                    MetaTitle = "ILISAN AR-01 Anti-Rad Önlük",
                    MetaDescription = "Kurşunsuz radyasyon koruyucu önlük",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Product
                {
                    Id = 4,
                    Name = "ILISAN AT-01 Kamuflaj Üniforma",
                    ProductCode = "ILISAN-AT-01",
                    Slug = "ilisan-at-01-kamuflaj-uniforma",
                    ShortDescription = "Askeri standartlarda kamuflaj üniforma takımı",
                    LongDescription = "ILISAN AT-01 Kamuflaj Üniforma, TSK standartlarına uygun olarak üretilen yüksek kaliteli kamuflaj üniforma takımıdır.",
                    Price = 350.00m,
                    CategoryId = 3,
                    Brand = "ILISAN",
                    Material = "Polyester/Pamuk",
                    Size = "XS/S/M/L/XL/XXL",
                    Desi = 1.0m,
                    Color = "Kamuflaj",
                    StockQuantity = 150,
                    MinStockLevel = 15,
                    IsActive = true,
                    IsFeatured = true,
                    IsSpecialProduct = false,
                    MetaTitle = "ILISAN AT-01 Kamuflaj Üniforma",
                    MetaDescription = "TSK standartlarında kamuflaj üniforma",
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Ürün Resimleri
            modelBuilder.Entity<ProductImage>().HasData(
                new ProductImage { Id = 1, ProductId = 1, ImagePath = "/images/demo/demo_400x280.png", AltText = "ILISAN BV-01", IsMainImage = true, SortOrder = 1, CreatedDate = new DateTime(2024, 1, 1) },
                new ProductImage { Id = 2, ProductId = 2, ImagePath = "/images/demo/demo_400x280.png", AltText = "ILISAN BV-02", IsMainImage = true, SortOrder = 1, CreatedDate = new DateTime(2024, 1, 1) },
                new ProductImage { Id = 3, ProductId = 3, ImagePath = "/images/demo/demo_400x280.png", AltText = "ILISAN AR-01", IsMainImage = true, SortOrder = 1, CreatedDate = new DateTime(2024, 1, 1) },
                new ProductImage { Id = 4, ProductId = 4, ImagePath = "/images/demo/demo_400x280.png", AltText = "ILISAN AT-01", IsMainImage = true, SortOrder = 1, CreatedDate = new DateTime(2024, 1, 1) },
                new ProductImage { Id = 5, ProductId = 1, ImagePath = "/images/demo/demo_370x530.png", AltText = "ILISAN BV-01 Yan Görünüm", IsMainImage = false, SortOrder = 2, CreatedDate = new DateTime(2024, 1, 1) },
                new ProductImage { Id = 6, ProductId = 2, ImagePath = "/images/demo/demo_370x530.png", AltText = "ILISAN BV-02 Detay", IsMainImage = false, SortOrder = 2, CreatedDate = new DateTime(2024, 1, 1) }
            );
        }

        private void SeedAdditionalData(ModelBuilder modelBuilder)
        {
            // Ek Kullanıcılar
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    FirstName = "Ahmet",
                    LastName = "Yılmaz",
                    Email = "ahmet@test.com",
                    PasswordHash = "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", // Test123!
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedDate = new DateTime(2024, 1, 2)
                },
                new User
                {
                    Id = 3,
                    FirstName = "Ayşe",
                    LastName = "Demir",
                    Email = "ayse@test.com",
                    PasswordHash = "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", // Test123!
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedDate = new DateTime(2024, 1, 3)
                },
                new User
                {
                    Id = 4,
                    FirstName = "Mehmet",
                    LastName = "Kaya",
                    Email = "mehmet@test.com",
                    PasswordHash = "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", // Test123!
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    CreatedDate = new DateTime(2024, 1, 4)
                }
            );

            // Ürün Varyantları
            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant
                {
                    Id = 1,
                    ProductId = 1,
                    VariantName = "Small",
                    Size = "S",
                    Color = "Siyah",
                    Material = "Bor + Polietilen",
                    PriceAdjustment = 0.00m,
                    StockQuantity = 15,
                    MinStockLevel = 2,
                    SKU = "BV01-S-BLK",
                    IsActive = true,
                    IsDefault = true,
                    SortOrder = 1,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new ProductVariant
                {
                    Id = 2,
                    ProductId = 1,
                    VariantName = "Medium",
                    Size = "M",
                    Color = "Siyah",
                    Material = "Bor + Polietilen",
                    PriceAdjustment = 0.00m,
                    StockQuantity = 20,
                    MinStockLevel = 3,
                    SKU = "BV01-M-BLK",
                    IsActive = true,
                    IsDefault = false,
                    SortOrder = 2,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new ProductVariant
                {
                    Id = 3,
                    ProductId = 1,
                    VariantName = "Large",
                    Size = "L",
                    Color = "Siyah", 
                    Material = "Bor + Polietilen",
                    PriceAdjustment = 100.00m,
                    StockQuantity = 15,
                    MinStockLevel = 2,
                    SKU = "BV01-L-BLK",
                    IsActive = true,
                    IsDefault = false,
                    SortOrder = 3,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new ProductVariant
                {
                    Id = 4,
                    ProductId = 2,
                    VariantName = "Kamuflaj - Medium",
                    Size = "M",
                    Color = "Kamuflaj",
                    Material = "Aramid + Seramik",
                    PriceAdjustment = 0.00m,
                    StockQuantity = 12,
                    MinStockLevel = 2,
                    SKU = "BV02-M-CAM",
                    IsActive = true,
                    IsDefault = true,
                    SortOrder = 1,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // Logging System Configurations
            ConfigureLoggingEntities(modelBuilder);
            
        }

        /// <summary>
        /// Configure logging entities with proper indexing and constraints
        /// Follows Single Responsibility Principle - separate configuration method
        /// </summary>
        private void ConfigureLoggingEntities(ModelBuilder modelBuilder)
        {
            // SystemLog Configuration
            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // Indexes for performance
                entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_SystemLog_Timestamp");
                entity.HasIndex(e => e.Level).HasDatabaseName("IX_SystemLog_Level");
                entity.HasIndex(e => e.Source).HasDatabaseName("IX_SystemLog_Source");
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_SystemLog_UserId");
                entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId }).HasDatabaseName("IX_SystemLog_RelatedEntity");
                entity.HasIndex(e => e.CreatedDate).HasDatabaseName("IX_SystemLog_CreatedDate");
                
                // Constraints
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Exception).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Properties).HasColumnType("nvarchar(max)");
            });

            // ActivityLog Configuration
            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // Indexes for performance
                entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_ActivityLog_Timestamp");
                entity.HasIndex(e => e.Action).HasDatabaseName("IX_ActivityLog_Action");
                entity.HasIndex(e => e.EntityType).HasDatabaseName("IX_ActivityLog_EntityType");
                entity.HasIndex(e => e.UserId).HasDatabaseName("IX_ActivityLog_UserId");
                entity.HasIndex(e => new { e.EntityType, e.EntityId }).HasDatabaseName("IX_ActivityLog_Entity");
                entity.HasIndex(e => e.CreatedDate).HasDatabaseName("IX_ActivityLog_CreatedDate");
                entity.HasIndex(e => e.IsSuccessful).HasDatabaseName("IX_ActivityLog_IsSuccessful");
                
                // Constraints
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OldValues).HasColumnType("nvarchar(max)");
                entity.Property(e => e.NewValues).HasColumnType("nvarchar(max)");
            });

            // ErrorLog Configuration
            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                // Indexes for performance
                entity.HasIndex(e => e.Timestamp).HasDatabaseName("IX_ErrorLog_Timestamp");
                entity.HasIndex(e => e.ErrorType).HasDatabaseName("IX_ErrorLog_ErrorType");
                entity.HasIndex(e => e.Severity).HasDatabaseName("IX_ErrorLog_Severity");
                entity.HasIndex(e => e.Category).HasDatabaseName("IX_ErrorLog_Category");
                entity.HasIndex(e => e.IsResolved).HasDatabaseName("IX_ErrorLog_IsResolved");
                entity.HasIndex(e => e.Source).HasDatabaseName("IX_ErrorLog_Source");
                entity.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId }).HasDatabaseName("IX_ErrorLog_RelatedEntity");
                entity.HasIndex(e => e.CreatedDate).HasDatabaseName("IX_ErrorLog_CreatedDate");
                
                // Constraints
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.ErrorType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Severity).IsRequired().HasMaxLength(20);
                entity.Property(e => e.StackTrace).HasColumnType("nvarchar(max)");
                entity.Property(e => e.InnerException).HasColumnType("nvarchar(max)");
                entity.Property(e => e.RequestHeaders).HasColumnType("nvarchar(max)");
                entity.Property(e => e.RequestBody).HasColumnType("nvarchar(max)");
                entity.Property(e => e.AdditionalData).HasColumnType("nvarchar(max)");
            });
        }

    }
}
