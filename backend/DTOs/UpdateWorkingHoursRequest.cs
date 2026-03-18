namespace backend.DTOs
{
    public class UpdateWorkingHoursRequest
    {
        public List<ProviderWorkingHourDto> Days { get; set; } = [];
    }
}
