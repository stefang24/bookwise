namespace backend.DTOs
{
    public class ProviderServiceResponse
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string? ProviderImagePath { get; set; }
        public string? ProviderCity { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PriceEur { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }
}
