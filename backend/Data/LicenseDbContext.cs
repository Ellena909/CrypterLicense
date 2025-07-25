using Microsoft.EntityFrameworkCore;
using CrypterLicense.Models;

namespace CrypterLicense.Data
{
    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<CryptUsage> CryptUsages { get; set; }
        public DbSet<StubVersion> StubVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.HardwareId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastLoginAt);
                entity.Property(e => e.IsActive).IsRequired();
            });

            // License configuration
            modelBuilder.Entity<License>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.LicenseKey).IsUnique();
                entity.Property(e => e.LicenseKey).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PlanType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MaxCrypts).IsRequired();
                entity.Property(e => e.UsedCrypts).IsRequired();
                entity.Property(e => e.ExpiresAt);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.PaymentId).HasMaxLength(255);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                // Foreign key relationship
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Licenses)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CryptUsage configuration
            modelBuilder.Entity<CryptUsage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FileSize).IsRequired();
                entity.Property(e => e.EncryptionMethod).IsRequired().HasMaxLength(100);
                entity.Property(e => e.HardwareId).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ClientVersion).HasMaxLength(50);
                entity.Property(e => e.ProcessedAt).IsRequired();
                entity.Property(e => e.Success).IsRequired();
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);

                // Foreign key relationship
                entity.HasOne(e => e.License)
                      .WithMany(l => l.CryptUsages)
                      .HasForeignKey(e => e.LicenseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // StubVersion configuration
            modelBuilder.Entity<StubVersion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Version).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ReleaseDate).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.DownloadUrl).HasMaxLength(500);
                entity.Property(e => e.FileSize);
                entity.Property(e => e.Checksum).HasMaxLength(255);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@crypter.local",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Default admin password
                    HardwareId = "ADMIN-HARDWARE-ID",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed initial stub version
            modelBuilder.Entity<StubVersion>().HasData(
                new StubVersion
                {
                    Id = 1,
                    Version = "1.0.0",
                    ReleaseDate = DateTime.UtcNow,
                    Description = "Initial stub version with basic encryption support",
                    DownloadUrl = "/api/stub/download/1.0.0",
                    FileSize = 1024000, // 1MB placeholder
                    Checksum = "placeholder-checksum",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}