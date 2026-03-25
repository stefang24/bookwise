using System.Security.Claims;
using backend.DTOs;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            string? userIdRaw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userIdRaw))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userIdRaw));
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(SendChatMessageRequest request)
        {
            string? senderIdRaw = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            string senderRole = Context.User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(senderIdRaw) || !int.TryParse(senderIdRaw, out int senderId))
                throw new HubException("Unauthorized.");

            ResultResponse<SendChatMessageResponse> result = await _chatService.SendMessageAsync(senderId, senderRole, request);
            if (!result.Success || result.Data == null)
                throw new HubException(result.Message ?? "Message failed.");

            ChatMessageResponse message = result.Data.Message;
            AppNotificationResponse? notification = result.Data.Notification;
            bool isNewChat = result.Data.IsNewChat;
            int chatId = result.Data.ChatId;

            await Clients.Group(UserGroup(message.SenderId.ToString())).SendAsync("MessageReceived", message);
            await Clients.Group(UserGroup(message.ReceiverId.ToString())).SendAsync("MessageReceived", message);

            if (isNewChat)
            {
                await Clients.Group(UserGroup(message.ReceiverId.ToString())).SendAsync("ChatCreated", new { ChatId = chatId, SenderId = message.SenderId });
            }

            if (notification != null)
            {
                await Clients.Group(UserGroup(message.ReceiverId.ToString())).SendAsync("NotificationReceived", notification);
            }
        }

        private static string UserGroup(string userId)
        {
            return $"user-{userId}";
        }
    }
}
