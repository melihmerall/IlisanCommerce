using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data;
using IlisanCommerce.Models;
using System.Security.Claims;

namespace IlisanCommerce.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AddressApiController> _logger;

        public AddressApiController(ApplicationDbContext context, ILogger<AddressApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var address = await _context.Addresses
                    .Where(a => a.Id == id && a.UserId == userId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (address == null)
                {
                    return NotFound();
                }

                var result = new
                {
                    id = address.Id,
                    fullName = address.FullName,
                    phone = address.Phone,
                    city = address.City,
                    district = address.District,
                    neighborhood = address.Neighborhood,
                    addressLine1 = address.AddressLine1,
                    addressLine2 = address.AddressLine2,
                    postalCode = address.PostalCode,
                    addressTitle = address.AddressTitle,
                    addressType = address.AddressType.ToString(),
                    isDefault = address.IsDefault
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address {AddressId}", id);
                return StatusCode(500, "Adres bilgileri alınırken hata oluştu.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressFormModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var address = new Address
                {
                    UserId = userId.Value,
                    FullName = model.FullName,
                    Phone = model.Phone,
                    City = model.City,
                    District = model.District,
                    Neighborhood = model.Neighborhood,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    PostalCode = model.PostalCode,
                    AddressTitle = model.AddressTitle ?? "Yeni Adres",
                    AddressType = AddressType.Both,
                    IsDefault = false,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                var result = new
                {
                    id = address.Id,
                    fullName = address.FullName,
                    phone = address.Phone,
                    city = address.City,
                    district = address.District,
                    neighborhood = address.Neighborhood,
                    addressLine1 = address.AddressLine1,
                    addressLine2 = address.AddressLine2,
                    postalCode = address.PostalCode,
                    addressTitle = address.AddressTitle,
                    addressType = address.AddressType.ToString(),
                    isDefault = address.IsDefault
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return StatusCode(500, "Adres oluşturulurken hata oluştu.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressFormModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var address = await _context.Addresses
                    .Where(a => a.Id == id && a.UserId == userId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (address == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                address.FullName = model.FullName;
                address.Phone = model.Phone;
                address.City = model.City;
                address.District = model.District;
                address.Neighborhood = model.Neighborhood;
                address.AddressLine1 = model.AddressLine1;
                address.AddressLine2 = model.AddressLine2;
                address.PostalCode = model.PostalCode;
                address.AddressTitle = model.AddressTitle ?? address.AddressTitle;
                address.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                var result = new
                {
                    id = address.Id,
                    fullName = address.FullName,
                    phone = address.Phone,
                    city = address.City,
                    district = address.District,
                    neighborhood = address.Neighborhood,
                    addressLine1 = address.AddressLine1,
                    addressLine2 = address.AddressLine2,
                    postalCode = address.PostalCode,
                    addressTitle = address.AddressTitle,
                    addressType = address.AddressType.ToString(),
                    isDefault = address.IsDefault
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address {AddressId}", id);
                return StatusCode(500, "Adres güncellenirken hata oluştu.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var address = await _context.Addresses
                    .Where(a => a.Id == id && a.UserId == userId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (address == null)
                {
                    return NotFound();
                }

                address.IsActive = false;
                address.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Adres başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {AddressId}", id);
                return StatusCode(500, "Adres silinirken hata oluştu.");
            }
        }

        [HttpPost("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var address = await _context.Addresses
                    .Where(a => a.Id == id && a.UserId == userId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (address == null)
                {
                    return NotFound();
                }

                // Diğer adreslerin varsayılan durumunu kaldır
                var otherAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsActive && a.Id != id)
                    .ToListAsync();

                foreach (var otherAddress in otherAddresses)
                {
                    otherAddress.IsDefault = false;
                    otherAddress.UpdatedDate = DateTime.Now;
                }

                // Bu adresi varsayılan yap
                address.IsDefault = true;
                address.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Varsayılan adres başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default address {AddressId}", id);
                return StatusCode(500, "Varsayılan adres güncellenirken hata oluştu.");
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
    }
}
