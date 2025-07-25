using System.ComponentModel.DataAnnotations;

namespace CrypterLicense.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string HardwareId { get; set; } = string.Empty;
    }
}