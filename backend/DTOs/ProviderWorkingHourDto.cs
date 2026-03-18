namespace backend.DTOs
{
    public class ProviderWorkingHourDto
    {
        public int DayOfWeek { get; set; }
        public bool IsWorking { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
