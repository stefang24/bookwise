using backend.DTOs;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<ResultResponse<List<ChatContactResponse>>> GetContactsAsync(int userId, string role)
        {
            List<Chat> userChats = await _chatRepository.GetChatsForUserAsync(userId);

            List<ChatContactResponse> result = userChats.Select(chat =>
            {
                User otherUser = chat.User1Id == userId ? chat.User2 : chat.User1;
                ChatMessage? lastMessage = chat.Messages.OrderByDescending(m => m.SentAtUtc).FirstOrDefault();
                
                return new ChatContactResponse
                {
                    UserId = otherUser.Id,
                    Name = otherUser.CompanyName ?? (otherUser.FirstName + " " + otherUser.LastName).Trim(),
                    Role = otherUser.Role.ToString(),
                    ProfileImagePath = otherUser.ProfileImagePath,
                    LastMessage = lastMessage?.Content,
                    LastMessageAtUtc = lastMessage?.SentAtUtc ?? chat.UpdatedAtUtc
                };
            })
            .Where(x => true)
            .OrderByDescending(x => x.LastMessageAtUtc)
            .ThenBy(x => x.Name)
            .ToList();

            return ResultResponse<List<ChatContactResponse>>.Ok(result);
        }

        public async Task<ResultResponse<List<ChatMessageResponse>>> GetMessagesAsync(int userId, int otherUserId)
        {
            User? otherUser = await _chatRepository.GetUserByIdAsync(otherUserId);
            if (otherUser == null)
                return ResultResponse<List<ChatMessageResponse>>.Fail("User not found.");

            Chat? chat = await _chatRepository.GetChatBetweenAsync(userId, otherUserId);
            if (chat == null) 
                return ResultResponse<List<ChatMessageResponse>>.Ok(new List<ChatMessageResponse>());

            List<ChatMessage> messages = await _chatRepository.GetMessagesByChatAsync(chat.Id);

            List<ChatMessageResponse> result = messages.Select(x => MapMessage(x, otherUser, chat)).ToList();
            return ResultResponse<List<ChatMessageResponse>>.Ok(result);
        }

        public async Task<ResultResponse<SendChatMessageResponse>> SendMessageAsync(int senderId, string senderRole, SendChatMessageRequest request)
        {
            string content = request.Content.Trim();
            if (string.IsNullOrWhiteSpace(content))
                return ResultResponse<SendChatMessageResponse>.Fail("Message content is required.");

            if (content.Length > 1500)
                return ResultResponse<SendChatMessageResponse>.Fail("Message is too long.");

            User? sender = await _chatRepository.GetUserByIdAsync(senderId);
            if (sender == null || !sender.IsActive)
                return ResultResponse<SendChatMessageResponse>.Fail("Sender not found.");

            User? receiver = await _chatRepository.GetUserByIdAsync(request.ReceiverId);
            if (receiver == null || !receiver.IsActive)
                return ResultResponse<SendChatMessageResponse>.Fail("Receiver not found.");

            bool isUserProviderPair =
                (sender.Role == UserRole.User && receiver.Role == UserRole.Provider) ||
                (sender.Role == UserRole.Provider && receiver.Role == UserRole.User);

            if (!isUserProviderPair)
                return ResultResponse<SendChatMessageResponse>.Fail("Chat is available only between users and providers.");

            Chat? chat = await _chatRepository.GetChatBetweenAsync(senderId, request.ReceiverId);
            bool isNewChat = chat == null;
            if (chat == null)
            {
                chat = await _chatRepository.CreateChatAsync(senderId, request.ReceiverId);
            }

            ChatMessage message = new()
            {
                ChatId = chat.Id,
                SenderId = senderId,
                Content = content,
                Sender = sender,
                Chat = chat
            };
            
            chat.UpdatedAtUtc = DateTime.UtcNow;

            await _chatRepository.AddMessageAsync(message);

            string senderName = sender.CompanyName ?? (sender.FirstName + " " + sender.LastName).Trim();
            AppNotification notification = new()
            {
                UserId = request.ReceiverId,
                Type = "chat",
                Title = "New message",
                Message = $"{senderName}: {content}",
                RelatedUserId = senderId,
                RelatedUser = sender,
                ChatMessage = message
            };

            await _chatRepository.AddNotificationAsync(notification);
            await _chatRepository.SaveChangesAsync();

            SendChatMessageResponse response = new()
            {
                ChatId = chat.Id,
                IsNewChat = isNewChat,
                Message = MapMessage(message, receiver, chat),
                Notification = MapNotification(notification)
            };

            return ResultResponse<SendChatMessageResponse>.Ok(response);
        }

        public async Task<ResultResponse<List<AppNotificationResponse>>> GetMyNotificationsAsync(int userId)
        {
            List<AppNotification> notifications = await _chatRepository.GetNotificationsByUserAsync(userId);
            List<AppNotificationResponse> result = notifications.Select(MapNotification).ToList();
            return ResultResponse<List<AppNotificationResponse>>.Ok(result);
        }

        public async Task<ResultResponse<bool>> MarkNotificationReadAsync(int userId, int notificationId)
        {
            AppNotification? notification = await _chatRepository.GetNotificationByIdAsync(notificationId, userId);
            if (notification == null)
                return ResultResponse<bool>.Fail("Notification not found.");

            notification.IsRead = true;
            await _chatRepository.SaveChangesAsync();
            return ResultResponse<bool>.Ok(true);
        }

        private static ChatMessageResponse MapMessage(ChatMessage message, User otherUser, Chat chat)
        {
            string senderName = message.Sender.CompanyName ?? (message.Sender.FirstName + " " + message.Sender.LastName).Trim();
            int receiverId = chat.User1Id == message.SenderId ? chat.User2Id : chat.User1Id;
            string receiverName = otherUser.CompanyName ?? (otherUser.FirstName + " " + otherUser.LastName).Trim();

            return new ChatMessageResponse
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = senderName,
                ReceiverId = receiverId,
                ReceiverName = receiverName,
                Content = message.Content,
                SentAtUtc = message.SentAtUtc
            };
        }

        private static AppNotificationResponse MapNotification(AppNotification notification)
        {
            return new AppNotificationResponse
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                RelatedUserId = notification.RelatedUserId,
                RelatedUserName = notification.RelatedUser == null
                    ? null
                    : (notification.RelatedUser.CompanyName ?? (notification.RelatedUser.FirstName + " " + notification.RelatedUser.LastName).Trim()),
                CreatedAtUtc = notification.CreatedAtUtc
            };
        }
    }
}