namespace backend.DTOs
{
    public class AppointmentResponse
    {
        public int Id { get; set; }
        public int ProviderServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public int ProviderId { get; set; }
        public string? ProviderImagePath { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string? ClientImagePath { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PriceEur { get; set; }
        public int DurationMinutes { get; set; }
    }
}
