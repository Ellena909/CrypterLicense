using CrypterLicense.Models;

namespace CrypterLicense.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string password, string hardwareId);
        Task<AuthResponse> LoginAsync(string email, string password, string hardwareId);
        Task<AuthResponse> RefreshTokenAsync(int userId);
        Task LogoutAsync(int userId);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ValidateUserAsync(string email, string password);
        string GenerateJwtToken(User user);
    }
}