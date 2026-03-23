namespace backend.DTOs
{
    public class SendChatMessageResponse
    {
        public int ChatId { get; set; }
        public bool IsNewChat { get; set; }
        public ChatMessageResponse Message { get; set; } = new();
        public AppNotificationResponse? Notification { get; set; }
    }
}
