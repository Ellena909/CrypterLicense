using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrypterLicense.Models
{
    [Table("StubVersions")]
    public class StubVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string DownloadUrl { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [StringLength(64)]
        public string? Checksum { get; set; }

        public bool IsActive { get; set; } = true;
    }
}