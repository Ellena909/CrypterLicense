using System.ComponentModel.DataAnnotations;

namespace CrypterLicense.Models
{
    public class CryptRequest
    {
        [Required]
        public string LicenseKey { get; set; } = string.Empty;

        [Required]
        public string HardwareId { get; set; } = string.Empty;

        [Required]
        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string EncryptionMethod { get; set; } = "AES256";

        public string? ClientVersion { get; set; }
    }
}