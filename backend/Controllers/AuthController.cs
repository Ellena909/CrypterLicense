using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CrypterLicense.Services;
using CrypterLicense.Models;
using System.Security.Claims;

namespace CrypterLicense.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _authService.RegisterAsync(request.Email, request.Password, request.HardwareId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                _logger.LogInformation("User registered successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Internal server error during registration"
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid request data"
                    });
                }

                var result = await _authService.LoginAsync(request.Email, request.Password, request.HardwareId);
                
                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                _logger.LogInformation("User logged in successfully: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Internal server error during login"
                });
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> RefreshToken()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    });
                }

                var result = await _authService.RefreshTokenAsync(int.Parse(userId));
                
                if (!result.Success)
                {
                    return Unauthorized(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new AuthResponse
                {
                    Success = false,
                    Message = "Internal server error during token refresh"
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await _authService.LogoutAsync(int.Parse(userId));
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "Internal server error during logout" });
            }
        }
    }
}