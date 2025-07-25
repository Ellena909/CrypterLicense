using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrypterLicense.Services;
using CrypterLicense.Models;
using System.Security.Claims;

namespace CrypterLicense.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all admin endpoints
    public class AdminController : ControllerBase
    {
        private readonly ILicenseService _licenseService;
        private readonly IAuthService _authService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILicenseService licenseService, IAuthService authService, ILogger<AdminController> logger)
        {
            _licenseService = licenseService;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("licenses")]
        public async Task<ActionResult<List<LicenseDto>>> GetAllLicenses()
        {
            try
            {
                // Check if user has admin privileges
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    return Forbid("Admin access required");
                }

                var licenses = await _licenseService.GetAllLicensesAsync();
                return Ok(licenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all licenses");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    return Forbid("Admin access required");
                }

                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("license/revoke/{licenseKey}")]
        public async Task<ActionResult> RevokeLicense(string licenseKey)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    return Forbid("Admin access required");
                }

                var result = await _licenseService.RevokeLicenseAsync(licenseKey);
                
                if (!result)
                {
                    return NotFound(new { message = "License not found" });
                }

                _logger.LogInformation("License revoked by admin: {LicenseKey}", licenseKey);
                return Ok(new { message = "License revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking license: {LicenseKey}", licenseKey);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminStatsDto>> GetAdminStats()
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole != "Admin")
                {
                    return Forbid("Admin access required");
                }

                var stats = await _licenseService.GetAdminStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin stats");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}