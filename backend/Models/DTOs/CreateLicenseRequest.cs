using System.ComponentModel.DataAnnotations;

namespace CrypterLicense.Models
{
    public class CreateLicenseRequest
    {
        [Required]
        public string PlanType { get; set; } = string.Empty;

        public string? PaymentId { get; set; }

        public decimal? Amount { get; set; }
    }
}