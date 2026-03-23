namespace backend.DTOs
{
    public class AdminAppointmentResponse
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public int ProviderId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PriceEur { get; set; }
    }
}
