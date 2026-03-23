using backend.Models;

namespace backend.Repositories
{
    public interface IChatRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> HaveAppointmentConnectionAsync(int userId, int otherUserId);
        Task<Chat?> GetChatBetweenAsync(int user1Id, int user2Id);
        Task<Chat> CreateChatAsync(int user1Id, int user2Id);
        Task<List<Chat>> GetChatsForUserAsync(int userId);
        Task AddMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesByChatAsync(int chatId);
        Task<List<User>> GetAppointmentContactsAsync(int userId, string role);
        Task AddNotificationAsync(AppNotification notification);
        Task<AppNotification> CreateAppointmentNotificationAsync(int providerId, int clientId, string clientName, Appointment appointment);
        Task<List<AppNotification>> GetNotificationsByUserAsync(int userId);
        Task<AppNotification?> GetNotificationByIdAsync(int notificationId, int userId);
        Task SaveChangesAsync();
    }
}
