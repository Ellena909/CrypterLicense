namespace CrypterLicense.Models
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
        public int TotalCrypts { get; set; }
        public int SuccessfulCrypts { get; set; }
    }
}