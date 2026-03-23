namespace backend.Models
{
    public class Chat
    {
        public int Id { get; set; }
        
        public int User1Id { get; set; }
        public User User1 { get; set; } = null!;
        
        public int User2Id { get; set; }
        public User User2 { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<ChatMessage> Messages { get; set; } = [];
    }
}
