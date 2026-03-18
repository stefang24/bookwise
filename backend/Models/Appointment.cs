namespace backend.Models
{
    public enum AppointmentStatus
    {
        Scheduled,
        Cancelled,
        Completed
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int ProviderServiceId { get; set; }
        public ProviderService ProviderService { get; set; } = null!;
        public int ClientId { get; set; }
        public User Client { get; set; } = null!;
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
