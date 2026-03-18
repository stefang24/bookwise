namespace backend.Models
{
    public class ProviderService
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public User Provider { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal PriceEur { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
