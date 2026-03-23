using System.Security.Claims;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using backend.Hubs;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetContacts()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            ResultResponse<List<ChatContactResponse>> response = await _chatService.GetContactsAsync(userId, role);
            return Ok(response);
        }

        [HttpGet("messages/{otherUserId:int}")]
        public async Task<IActionResult> GetMessages(int otherUserId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<List<ChatMessageResponse>> response = await _chatService.GetMessagesAsync(userId, otherUserId);
            return Ok(response);
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest request)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            ResultResponse<SendChatMessageResponse> response = await _chatService.SendMessageAsync(userId, role, request);
            
            if (response.Success && response.Data != null)
            {
                ChatMessageResponse message = response.Data.Message;
                AppNotificationResponse? notification = response.Data.Notification;

                await _hubContext.Clients.Group($"user-{message.SenderId}").SendAsync("MessageReceived", message);
                await _hubContext.Clients.Group($"user-{message.ReceiverId}").SendAsync("MessageReceived", message);

                if (notification != null)
                {
                    await _hubContext.Clients.Group($"user-{message.ReceiverId}").SendAsync("NotificationReceived", notification);
                }
            }

            return Ok(response);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<List<AppNotificationResponse>> response = await _chatService.GetMyNotificationsAsync(userId);
            return Ok(response);
        }

        [HttpPost("notifications/{id:int}/read")]
        public async Task<IActionResult> MarkNotificationRead(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ResultResponse<bool> response = await _chatService.MarkNotificationReadAsync(userId, id);
            return Ok(response);
        }
    }
}
