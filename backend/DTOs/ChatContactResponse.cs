namespace backend.DTOs
{
    public class ChatContactResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }
    }
}
