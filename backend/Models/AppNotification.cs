namespace backend.Models
{
    public class AppNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Type { get; set; } = "chat";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int? RelatedUserId { get; set; }
        public User? RelatedUser { get; set; }
        public int? ChatMessageId { get; set; }
        public ChatMessage? ChatMessage { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
