using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrypterLicense.Services;
using CrypterLicense.Models;
using System.Security.Claims;

namespace CrypterLicense.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(ILicenseService licenseService, ILogger<LicenseController> logger)
        {
            _licenseService = licenseService;
            _logger = logger;
        }

        [HttpPost("validate")]
        public async Task<ActionResult<LicenseValidationResponse>> ValidateLicense([FromBody] LicenseValidationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _licenseService.ValidateLicenseAsync(request.LicenseKey, request.HardwareId);
                
                _logger.LogInformation("License validation completed for key: {LicenseKey}", 
                    request.LicenseKey.Substring(0, Math.Min(8, request.LicenseKey.Length)) + "...");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating license: {LicenseKey}", request.LicenseKey);
                return StatusCode(500, new LicenseValidationResponse
                {
                    IsValid = false,
                    Message = "Internal server error during license validation"
                });
            }
        }

        [HttpPost("crypt")]
        public async Task<ActionResult<CryptResponse>> ProcessCrypt([FromBody] CryptRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CryptResponse
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _licenseService.ProcessCryptAsync(request);
                
                _logger.LogInformation("Crypt request processed for file: {FileName}", request.FileName);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing crypt request for file: {FileName}", request.FileName);
                return StatusCode(500, new CryptResponse
                {
                    Success = false,
                    Message = "Internal server error during crypt processing"
                });
            }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<LicenseValidationResponse>> CreateLicense([FromBody] CreateLicenseRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "Invalid request data"
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "Invalid authentication token"
                    });
                }

                var result = await _licenseService.CreateLicenseAsync(int.Parse(userId), request);
                
                _logger.LogInformation("License created for user: {UserId}, plan: {PlanType}", userId, request.PlanType);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating license for plan: {PlanType}", request.PlanType);
                return StatusCode(500, new LicenseValidationResponse
                {
                    IsValid = false,
                    Message = "Internal server error during license creation"
                });
            }
        }

        [HttpGet("stub/info")]
        public async Task<ActionResult<StubInfo>> GetLatestStub()
        {
            try
            {
                var result = await _licenseService.GetLatestStubInfoAsync();
                
                if (result == null)
                {
                    return NotFound(new { message = "No stub information available" });
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest stub information");
                return StatusCode(500, new { message = "Internal server error getting stub information" });
            }
        }

        [HttpGet("usage/{licenseKey}")]
        public async Task<ActionResult<List<CryptUsageDto>>> GetUsageHistory(string licenseKey)
        {
            try
            {
                var result = await _licenseService.GetUsageHistoryAsync(licenseKey);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage history for license: {LicenseKey}", licenseKey);
                return StatusCode(500, new { message = "Internal server error getting usage history" });
            }
        }
    }
}