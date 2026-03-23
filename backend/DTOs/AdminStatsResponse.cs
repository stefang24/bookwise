namespace backend.DTOs
{
    public class AdminStatsResponse
    {
        public int TotalUsers { get; set; }
        public int TotalProviders { get; set; }
        public int TotalAppointments { get; set; }
        public int ScheduledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public List<StatPointResponse> BookingsByDate { get; set; } = [];
        public List<StatPointResponse> BookingsByCategory { get; set; } = [];
    }

    public class StatPointResponse
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
