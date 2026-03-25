using backend.DTOs;

namespace backend.Services.Interfaces
{
    public interface IChatService
    {
        Task<ResultResponse<List<ChatContactResponse>>> GetContactsAsync(int userId, string role);
        Task<ResultResponse<List<ChatMessageResponse>>> GetMessagesAsync(int userId, int otherUserId);
        Task<ResultResponse<SendChatMessageResponse>> SendMessageAsync(int senderId, string senderRole, SendChatMessageRequest request);
        Task<ResultResponse<List<AppNotificationResponse>>> GetMyNotificationsAsync(int userId);
        Task<ResultResponse<bool>> MarkNotificationReadAsync(int userId, int notificationId);
    }
}
