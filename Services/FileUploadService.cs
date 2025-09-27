using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;

namespace IlisanCommerce.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, int? maxWidth = null, int? maxHeight = null);
        Task<bool> DeleteImageAsync(string imagePath);
        Task<string> ResizeImageAsync(string imagePath, int maxWidth, int maxHeight);
        bool IsValidImageFile(IFormFile file);
        string GetImageUrl(string imagePath);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB for professional images

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder, int? maxWidth = null, int? maxHeight = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("Dosya seçilmedi veya dosya boş.");
                }

                if (!IsValidImageFile(file))
                {
                    throw new ArgumentException($"Geçersiz dosya formatı. Sadece {string.Join(", ", _allowedExtensions)} formatları kabul edilir.");
                }

                if (file.Length > _maxFileSize)
                {
                    var maxSizeMB = _maxFileSize / (1024 * 1024);
                    throw new ArgumentException($"Dosya boyutu {maxSizeMB}MB'dan büyük olamaz. Seçilen dosya: {file.Length / (1024 * 1024)}MB");
                }

                // Ana uploads klasörünü oluştur
                var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }

                // Alt klasörü oluştur
                var uploadsFolder = Path.Combine(uploadsRoot, folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Yıl/ay klasörü oluştur (daha iyi organizasyon için)
                var dateFolder = DateTime.Now.ToString("yyyy/MM");
                var datePath = Path.Combine(uploadsFolder, dateFolder);
                if (!Directory.Exists(datePath))
                {
                    Directory.CreateDirectory(datePath);
                }

                // Güvenli dosya adı oluştur
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{SanitizeFileName(originalFileName)}_{timestamp}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
                var filePath = Path.Combine(datePath, fileName);

                // Dosya zaten varsa farklı bir ad oluştur
                int counter = 1;
                while (File.Exists(filePath))
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    fileName = $"{fileNameWithoutExt}_{counter}{extension}";
                    filePath = Path.Combine(datePath, fileName);
                    counter++;
                }

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Dosya kaydedildi: {FilePath}", filePath);

                // Resim boyutlandırma (eğer gerekiyorsa)
                if ((maxWidth.HasValue || maxHeight.HasValue) && IsImageFile(extension))
                {
                    try
                    {
                        var resizedPath = await ResizeImageAsync(filePath, maxWidth ?? 1920, maxHeight ?? 1080);
                        if (resizedPath != filePath && File.Exists(resizedPath))
                        {
                            // Orijinal dosyayı sil ve yeni dosyayı eski dosyanın yerine taşı
                            File.Delete(filePath);
                            File.Move(resizedPath, filePath);
                            _logger.LogInformation("Resim yeniden boyutlandırıldı: {FilePath}", filePath);
                        }
                    }
                    catch (Exception resizeEx)
                    {
                        _logger.LogWarning(resizeEx, "Resim boyutlandırma başarısız, orijinal dosya korundu: {FilePath}", filePath);
                    }
                }

                // Web URL'ini döndür
                var webPath = $"/uploads/{folder}/{dateFolder}/{fileName}".Replace("\\", "/");
                _logger.LogInformation("Dosya başarıyla yüklendi. Web URL: {WebPath}", webPath);
                
                return webPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yükleme hatası. FileName: {FileName}, FileSize: {FileSize}", 
                    file?.FileName ?? "N/A", file?.Length ?? 0);
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    return false;
                }

                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Dosya başarıyla silindi: {FilePath}", imagePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya silme hatası: {FilePath}", imagePath);
                return false;
            }
        }

        public async Task<string> ResizeImageAsync(string imagePath, int maxWidth, int maxHeight)
        {
            try
            {
                return await Task.Run(() =>
                {
                    using (var image = Image.FromFile(imagePath))
                {
                    // Orijinal boyutları al
                    var originalWidth = image.Width;
                    var originalHeight = image.Height;

                    // Yeni boyutları hesapla (orantılı)
                    var ratioX = (double)maxWidth / originalWidth;
                    var ratioY = (double)maxHeight / originalHeight;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(originalWidth * ratio);
                    var newHeight = (int)(originalHeight * ratio);

                    // Eğer resim zaten küçükse, boyutlandırma yapma
                    if (newWidth >= originalWidth && newHeight >= originalHeight)
                    {
                        return imagePath;
                    }

                    // Yeni resim oluştur
                    using (var resizedImage = new Bitmap(newWidth, newHeight))
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                        // Geçici dosya adı
                        var tempPath = imagePath.Replace(Path.GetExtension(imagePath), "_temp" + Path.GetExtension(imagePath));
                        
                        // Kaliteli kaydetme
                        var encoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L);

                        resizedImage.Save(tempPath, encoder, encoderParams);
                        return tempPath;
                    }
                }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resim boyutlandırma hatası: {FilePath}", imagePath);
                return imagePath; // Hata durumunda orijinal dosyayı döndür
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return _allowedExtensions.Contains(extension);
        }

        public string GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return "/images/placeholder.jpg"; // Varsayılan resim
            }

            return imagePath.StartsWith("http") ? imagePath : imagePath;
        }

        /// <summary>
        /// Dosya adını güvenli hale getirir
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "file";

            // Türkçe karakterleri değiştir
            var sanitized = fileName.ToLowerInvariant()
                .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
                .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u")
                .Replace("İ", "i").Replace("Ğ", "g").Replace("Ş", "s")
                .Replace("Ö", "o").Replace("Ü", "u").Replace("Ç", "c");

            // Geçersiz karakterleri temizle
            var invalidChars = Path.GetInvalidFileNameChars().Concat(new[] { ' ', '.', ',', ';', ':', '!', '?', '\'', '"', '&', '%', '+', '=', '@', '#' }).ToArray();
            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c, '_');
            }

            // Birden fazla alt çizgiyi tek alt çizgiye çevir
            while (sanitized.Contains("__"))
                sanitized = sanitized.Replace("__", "_");

            // Başında ve sonunda alt çizgi varsa temizle
            sanitized = sanitized.Trim('_');

            // Çok uzunsa kısalt
            if (sanitized.Length > 50)
                sanitized = sanitized.Substring(0, 50).Trim('_');

            return string.IsNullOrEmpty(sanitized) ? "file" : sanitized;
        }

        /// <summary>
        /// Dosyanın resim dosyası olup olmadığını kontrol eder
        /// </summary>
        private bool IsImageFile(string extension)
        {
            return _allowedExtensions.Contains(extension.ToLowerInvariant());
        }

        /// <summary>
        /// Güvenli dosya yolunu kontrol eder
        /// </summary>
        private bool IsValidFilePath(string filePath)
        {
            try
            {
                // Path traversal saldırılarını önle
                var fullPath = Path.GetFullPath(filePath);
                var rootPath = Path.GetFullPath(_environment.WebRootPath);
                return fullPath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Resim formatının boyutlandırma için uygun olup olmadığını kontrol eder
        /// </summary>
        private bool CanResize(string extension)
        {
            var resizableFormats = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            return resizableFormats.Contains(extension.ToLowerInvariant());
        }
    }
}
