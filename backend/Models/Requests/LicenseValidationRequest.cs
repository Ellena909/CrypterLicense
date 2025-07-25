using System.ComponentModel.DataAnnotations;

namespace CrypterLicense.Models
{
    public class LicenseValidationRequest
    {
        [Required]
        public string LicenseKey { get; set; } = string.Empty;

        [Required]
        public string HardwareId { get; set; } = string.Empty;
    }
}