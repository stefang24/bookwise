using backend.Data;
using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> HaveAppointmentConnectionAsync(int userId, int otherUserId)
        {
            return await _context.Appointments
                .AnyAsync(x =>
                    (x.ClientId == userId && x.ProviderService.ProviderId == otherUserId) ||
                    (x.ClientId == otherUserId && x.ProviderService.ProviderId == userId));
        }

        public async Task<Chat?> GetChatBetweenAsync(int user1Id, int user2Id)
        {
            return await _context.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => 
                    (c.User1Id == user1Id && c.User2Id == user2Id) || 
                    (c.User1Id == user2Id && c.User2Id == user1Id));
        }

        public async Task<Chat> CreateChatAsync(int user1Id, int user2Id)
        {
            int u1 = Math.Min(user1Id, user2Id);
            int u2 = Math.Max(user1Id, user2Id);
            
            var chat = new Chat {
                User1Id = u1,
                User2Id = u2,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<List<Chat>> GetChatsForUserAsync(int userId)
        {
            return await _context.Chats
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync();
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await Task.CompletedTask;
        }

        public async Task<List<ChatMessage>> GetMessagesByChatAsync(int chatId)
        {
            return await _context.ChatMessages
                .Include(x => x.Sender)
                .Include(x => x.Chat)
                .Where(x => x.ChatId == chatId)
                .OrderBy(x => x.SentAtUtc)
                .ToListAsync();
        }

        public async Task<List<User>> GetAppointmentContactsAsync(int userId, string role)
        {
            string normalizedRole = role.Trim();

            if (normalizedRole == UserRole.Provider.ToString())
            {
                List<int> ids = await _context.Appointments
                    .Where(x => x.ProviderService.ProviderId == userId)
                    .Select(x => x.ClientId)
                    .Distinct()
                    .ToListAsync();

                return await _context.Users
                    .Where(x => ids.Contains(x.Id))
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToListAsync();
            }

            List<int> providerIds = await _context.Appointments
                .Where(x => x.ClientId == userId)
                .Select(x => x.ProviderService.ProviderId)
                .Distinct()
                .ToListAsync();

            return await _context.Users
                .Where(x => providerIds.Contains(x.Id))
                .OrderBy(x => x.CompanyName ?? x.FirstName)
                .ToListAsync();
        }

        public async Task AddNotificationAsync(AppNotification notification)
        {
            _context.Notifications.Add(notification);
            await Task.CompletedTask;
        }

        public async Task<AppNotification> CreateAppointmentNotificationAsync(int providerId, int clientId, string clientName, Appointment appointment)
        {
            AppNotification notification = new()
            {
                UserId = providerId,
                Type = "appointment",
                Title = "New appointment booking",
                Message = $"{clientName} has booked an appointment",
                RelatedUserId = clientId,
                CreatedAtUtc = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            await Task.CompletedTask;
            return notification;
        }

        public async Task<List<AppNotification>> GetNotificationsByUserAsync(int userId)
        {
            return await _context.Notifications
                .Include(x => x.RelatedUser)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(100)
                .ToListAsync();
        }

        public async Task<AppNotification?> GetNotificationByIdAsync(int notificationId, int userId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
