using CrypterLicense.Models;

namespace CrypterLicense.Services
{
    public interface ILicenseService
    {
        Task<LicenseValidationResponse> ValidateLicenseAsync(string licenseKey, string hardwareId);
        Task<CryptResponse> ProcessCryptAsync(CryptRequest request);
        Task<LicenseValidationResponse> CreateLicenseAsync(int userId, CreateLicenseRequest request);
        Task<StubInfo?> GetLatestStubInfoAsync();
        Task<List<CryptUsageDto>> GetUsageHistoryAsync(string licenseKey);
        Task<List<LicenseDto>> GetAllLicensesAsync();
        Task<bool> RevokeLicenseAsync(string licenseKey);
        Task<AdminStatsDto> GetAdminStatsAsync();
        string GenerateLicenseKey();
    }
}