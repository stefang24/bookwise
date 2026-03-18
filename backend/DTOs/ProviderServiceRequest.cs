namespace backend.DTOs
{
    public class ProviderServiceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PriceEur { get; set; }
        public int DurationMinutes { get; set; }
    }
}
