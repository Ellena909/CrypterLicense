namespace CrypterLicense.Models
{
    public class LicenseValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? LicenseKey { get; set; }
        public string? PlanType { get; set; }
        public int MaxCrypts { get; set; }
        public int UsedCrypts { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}