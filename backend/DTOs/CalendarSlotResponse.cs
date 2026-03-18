namespace backend.DTOs
{
    public class CalendarSlotResponse
    {
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsAvailable { get; set; }
    }
}
