using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrypterLicense.Models
{
    [Table("Licenses")]
    public class License
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseKey { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PlanType { get; set; } = string.Empty;

        public int MaxCrypts { get; set; } = 0;
        
        public int UsedCrypts { get; set; } = 0;
        
        public DateTime? ExpiresAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(100)]
        public string? PaymentId { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<CryptUsage> CryptUsages { get; set; } = new List<CryptUsage>();
    }
}