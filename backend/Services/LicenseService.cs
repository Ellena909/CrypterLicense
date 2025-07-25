using Microsoft.EntityFrameworkCore;
using CrypterLicense.Data;
using CrypterLicense.Models;
using System.Security.Cryptography;
using System.Text;

namespace CrypterLicense.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly LicenseDbContext _context;
        private readonly ILogger<LicenseService> _logger;

        public LicenseService(LicenseDbContext context, ILogger<LicenseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LicenseValidationResponse> ValidateLicenseAsync(string licenseKey, string hardwareId)
        {
            try
            {
                var license = await _context.Licenses
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LicenseKey == licenseKey && l.IsActive);

                if (license == null)
                {
                    return new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "Invalid license key"
                    };
                }

                // Check if license is expired
                if (license.ExpiresAt.HasValue && license.ExpiresAt.Value < DateTime.UtcNow)
                {
                    return new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "License has expired"
                    };
                }

                // Check if user is active
                if (!license.User.IsActive)
                {
                    return new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "User account is inactive"
                    };
                }

                // Check usage limits
                if (license.MaxCrypts > 0 && license.UsedCrypts >= license.MaxCrypts)
                {
                    return new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "License usage limit exceeded"
                    };
                }

                return new LicenseValidationResponse
                {
                    IsValid = true,
                    Message = "License is valid",
                    PlanType = license.PlanType,
                    MaxCrypts = license.MaxCrypts,
                    UsedCrypts = license.UsedCrypts,
                    ExpiresAt = license.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating license: {LicenseKey}", licenseKey);
                return new LicenseValidationResponse
                {
                    IsValid = false,
                    Message = "License validation failed"
                };
            }
        }

        public async Task<CryptResponse> ProcessCryptAsync(CryptRequest request)
        {
            try
            {
                // First validate the license
                var validationResult = await ValidateLicenseAsync(request.LicenseKey, request.HardwareId);
                if (!validationResult.IsValid)
                {
                    return new CryptResponse
                    {
                        Success = false,
                        Message = validationResult.Message
                    };
                }

                var license = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.LicenseKey == request.LicenseKey && l.IsActive);

                if (license == null)
                {
                    return new CryptResponse
                    {
                        Success = false,
                        Message = "License not found"
                    };
                }

                // Record the usage
                var usage = new CryptUsage
                {
                    LicenseId = license.Id,
                    FileName = request.FileName,
                    FileSize = request.FileSize,
                    EncryptionMethod = request.EncryptionMethod,
                    HardwareId = request.HardwareId,
                    ClientVersion = request.ClientVersion,
                    ProcessedAt = DateTime.UtcNow,
                    Success = true
                };

                _context.CryptUsages.Add(usage);

                // Increment usage count
                license.UsedCrypts++;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Crypt processed successfully for license: {LicenseKey}, file: {FileName}", 
                    request.LicenseKey, request.FileName);

                return new CryptResponse
                {
                    Success = true,
                    Message = "File processed successfully",
                    RemainingCrypts = license.MaxCrypts > 0 ? license.MaxCrypts - license.UsedCrypts : -1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing crypt request for license: {LicenseKey}", request.LicenseKey);
                
                // Record failed usage
                try
                {
                    var license = await _context.Licenses
                        .FirstOrDefaultAsync(l => l.LicenseKey == request.LicenseKey && l.IsActive);
                    
                    if (license != null)
                    {
                        var failedUsage = new CryptUsage
                        {
                            LicenseId = license.Id,
                            FileName = request.FileName,
                            FileSize = request.FileSize,
                            EncryptionMethod = request.EncryptionMethod,
                            HardwareId = request.HardwareId,
                            ClientVersion = request.ClientVersion,
                            ProcessedAt = DateTime.UtcNow,
                            Success = false,
                            ErrorMessage = ex.Message
                        };
                        
                        _context.CryptUsages.Add(failedUsage);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error recording failed usage");
                }

                return new CryptResponse
                {
                    Success = false,
                    Message = "Crypt processing failed"
                };
            }
        }

        public async Task<LicenseValidationResponse> CreateLicenseAsync(int userId, CreateLicenseRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
                if (user == null)
                {
                    return new LicenseValidationResponse
                    {
                        IsValid = false,
                        Message = "User not found or inactive"
                    };
                }

                // Determine license parameters based on plan type
                var (maxCrypts, expiresAt) = GetPlanLimits(request.PlanType);

                var license = new License
                {
                    UserId = userId,
                    LicenseKey = GenerateLicenseKey(),
                    PlanType = request.PlanType,
                    MaxCrypts = maxCrypts,
                    UsedCrypts = 0,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    PaymentId = request.PaymentId,
                    Amount = request.Amount
                };

                _context.Licenses.Add(license);
                await _context.SaveChangesAsync();

                _logger.LogInformation("License created successfully for user: {UserId}, plan: {PlanType}", 
                    userId, request.PlanType);

                return new LicenseValidationResponse
                {
                    IsValid = true,
                    Message = "License created successfully",
                    LicenseKey = license.LicenseKey,
                    PlanType = license.PlanType,
                    MaxCrypts = license.MaxCrypts,
                    UsedCrypts = license.UsedCrypts,
                    ExpiresAt = license.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating license for user: {UserId}, plan: {PlanType}", 
                    userId, request.PlanType);
                return new LicenseValidationResponse
                {
                    IsValid = false,
                    Message = "License creation failed"
                };
            }
        }

        public async Task<StubInfo?> GetLatestStubInfoAsync()
        {
            try
            {
                var latestStub = await _context.StubVersions
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.ReleaseDate)
                    .FirstOrDefaultAsync();

                if (latestStub == null)
                {
                    return null;
                }

                return new StubInfo
                {
                    Version = latestStub.Version,
                    ReleaseDate = latestStub.ReleaseDate,
                    Description = latestStub.Description,
                    DownloadUrl = latestStub.DownloadUrl,
                    FileSize = latestStub.FileSize,
                    Checksum = latestStub.Checksum
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest stub information");
                return null;
            }
        }

        public async Task<List<CryptUsageDto>> GetUsageHistoryAsync(string licenseKey)
        {
            try
            {
                var license = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.LicenseKey == licenseKey);

                if (license == null)
                {
                    return new List<CryptUsageDto>();
                }

                var usages = await _context.CryptUsages
                    .Where(u => u.LicenseId == license.Id)
                    .OrderByDescending(u => u.ProcessedAt)
                    .Take(100) // Limit to last 100 usages
                    .Select(u => new CryptUsageDto
                    {
                        Id = u.Id,
                        FileName = u.FileName,
                        FileSize = u.FileSize,
                        EncryptionMethod = u.EncryptionMethod,
                        ProcessedAt = u.ProcessedAt,
                        Success = u.Success,
                        ErrorMessage = u.ErrorMessage
                    })
                    .ToListAsync();

                return usages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting usage history for license: {LicenseKey}", licenseKey);
                return new List<CryptUsageDto>();
            }
        }

        public async Task<List<LicenseDto>> GetAllLicensesAsync()
        {
            try
            {
                var licenses = await _context.Licenses
                    .Include(l => l.User)
                    .Select(l => new LicenseDto
                    {
                        Id = l.Id,
                        LicenseKey = l.LicenseKey,
                        PlanType = l.PlanType,
                        MaxCrypts = l.MaxCrypts,
                        UsedCrypts = l.UsedCrypts,
                        ExpiresAt = l.ExpiresAt,
                        CreatedAt = l.CreatedAt,
                        IsActive = l.IsActive,
                        UserEmail = l.User.Email,
                        Amount = l.Amount
                    })
                    .ToListAsync();

                return licenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all licenses");
                return new List<LicenseDto>();
            }
        }

        public async Task<bool> RevokeLicenseAsync(string licenseKey)
        {
            try
            {
                var license = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.LicenseKey == licenseKey);

                if (license == null)
                {
                    return false;
                }

                license.IsActive = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("License revoked: {LicenseKey}", licenseKey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking license: {LicenseKey}", licenseKey);
                return false;
            }
        }

        public async Task<AdminStatsDto> GetAdminStatsAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
                var totalLicenses = await _context.Licenses.CountAsync();
                var activeLicenses = await _context.Licenses.CountAsync(l => l.IsActive);
                var totalCrypts = await _context.CryptUsages.CountAsync();
                var successfulCrypts = await _context.CryptUsages.CountAsync(u => u.Success);

                return new AdminStatsDto
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    TotalLicenses = totalLicenses,
                    ActiveLicenses = activeLicenses,
                    TotalCrypts = totalCrypts,
                    SuccessfulCrypts = successfulCrypts
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin stats");
                return new AdminStatsDto();
            }
        }

        public string GenerateLicenseKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder();

            // Generate format: XXXX-XXXX-XXXX-XXXX
            for (int i = 0; i < 4; i++)
            {
                if (i > 0) result.Append('-');
                for (int j = 0; j < 4; j++)
                {
                    result.Append(chars[random.Next(chars.Length)]);
                }
            }

            return result.ToString();
        }

        private (int maxCrypts, DateTime? expiresAt) GetPlanLimits(string planType)
        {
            return planType.ToLower() switch
            {
                "basic" => (100, DateTime.UtcNow.AddDays(30)),
                "pro" => (1000, DateTime.UtcNow.AddDays(90)),
                "enterprise" => (-1, null), // Unlimited
                "trial" => (10, DateTime.UtcNow.AddDays(7)),
                _ => (50, DateTime.UtcNow.AddDays(30))
            };
        }
    }
}