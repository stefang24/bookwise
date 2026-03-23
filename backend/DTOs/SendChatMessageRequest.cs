namespace backend.DTOs
{
    public class SendChatMessageRequest
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
