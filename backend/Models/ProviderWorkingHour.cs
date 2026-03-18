namespace backend.Models
{
    public class ProviderWorkingHour
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public User Provider { get; set; } = null!;
        public int DayOfWeek { get; set; }
        public bool IsWorking { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
