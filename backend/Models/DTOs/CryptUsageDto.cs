namespace CrypterLicense.Models
{
    public class CryptUsageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string EncryptionMethod { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}