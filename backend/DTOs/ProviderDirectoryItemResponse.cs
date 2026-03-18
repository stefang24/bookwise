namespace backend.DTOs
{
    public class ProviderDirectoryItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string? PrimaryCategory { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public int ServicesCount { get; set; }
    }
}
