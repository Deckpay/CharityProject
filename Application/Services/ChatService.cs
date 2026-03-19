using Application.Interfaces;
using Domain.Entities;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IApplicationDbcontext _context; // Az adatbázis elérés
        private readonly ICurrentUserService _currentUserService;

        public ChatService(IApplicationDbcontext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task SendMessageAsync(ChatMessageRequestDto dto)
        {
            var currentUserId = _currentUserService.UserId;

            // ELLENŐRZÉS: Létezik-e az igénylés és a user része-e?
            var chat = await _context.Chats
                .FirstOrDefaultAsync(r => r.ChatId == dto.RequestId);

            if (chat == null) throw new Exception("Nincs ilyen igénylés.");

            // Csak a donor vagy a rászoruló írhat
            if (chat.DonorId != currentUserId)
            {
                // itt jelenleg csak a donor ellenőrizhető.
                throw new UnauthorizedAccessException("Nincs jogosultságod ehhez a beszélgetéshez.");
            }

            var message = new ChatMessage
            {
                RequestId = dto.RequestId,
                SenderId = currentUserId,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int requestId, int currentUserId)
        {
            // Csak akkor adjuk vissza, ha a user tagja a beszélgetésnek
            var isAuthorized = await _context.Chats
                .AnyAsync(r => r.ChatId == requestId && r.DonorId == currentUserId);

            if (!isAuthorized) return new List<ChatMessageResponseDto>();

            return await _context.ChatMessages
                .Where(m => m.RequestId == requestId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageResponseDto
                {
                    Id = m.Id,
                    RequestId = m.RequestId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    SentAt = m.Timestamp,
                    IsRead = m.IsRead
                }).ToListAsync();
        }

        public async Task MarkAsRead(int messageId)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}