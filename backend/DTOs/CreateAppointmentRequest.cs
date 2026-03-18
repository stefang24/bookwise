namespace backend.DTOs
{
    public class CreateAppointmentRequest
    {
        public int ProviderServiceId { get; set; }
        public DateTime StartUtc { get; set; }
    }
}
