using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrypterLicense.Models
{
    [Table("CryptUsages")]
    public class CryptUsage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LicenseId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [StringLength(50)]
        public string EncryptionMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string HardwareId { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ClientVersion { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        public bool Success { get; set; } = true;

        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        // Navigation properties
        [ForeignKey("LicenseId")]
        public virtual License License { get; set; } = null!;
    }
}