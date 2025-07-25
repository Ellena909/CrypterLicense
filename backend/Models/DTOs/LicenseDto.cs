namespace CrypterLicense.Models
{
    public class LicenseDto
    {
        public int Id { get; set; }
        public string LicenseKey { get; set; } = string.Empty;
        public string PlanType { get; set; } = string.Empty;
        public int MaxCrypts { get; set; }
        public int UsedCrypts { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }
}