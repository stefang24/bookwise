namespace backend.DTOs
{
    public class AppNotificationResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int? RelatedUserId { get; set; }
        public string? RelatedUserName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
