namespace CrypterLicense.Models
{
    public class CryptResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RemainingCrypts { get; set; } = -1;
    }
}