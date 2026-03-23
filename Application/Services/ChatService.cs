using Application.Interfaces;
using Domain.Entities;
using Application.DTOs;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ChatService(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task SendMessageAsync(ChatMessageRequestDto dto)
        {
            var currentUserId = _currentUserService.UserId;

            // 1. Igénylés megkeresése
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(dto.RequestId);
            if (request == null)
                throw new Exception("Nincs ilyen igénylés.");

            // 2. Termék megkeresése
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new Exception($"A termék nem található (ID: {request.ProductId}).");

            // 3. Chat keresése – ha nincs, csak akkor hozzuk létre
            var allChats = await _unitOfWork.Chats.GetAllAsync();
            var chat = allChats.FirstOrDefault(c => c.ProductRequestId == dto.RequestId);

            if (chat == null)
            {
                chat = new Chat
                {
                    ProductRequestId = dto.RequestId,
                    RequesterId = request.RequesterId,
                    DonorId = product.DonorId,
                    CreatedAt = DateTime.UtcNow
                };

                // ✅ AddAsync csak akkor, ha VALÓBAN új chat
                await _unitOfWork.Chats.AddAsync(chat);
                await _unitOfWork.CompleteAsync(); // ID generáláshoz mentés
            }

            // 4. Üzenet hozzáadása
            var message = new ChatMessage
            {
                ChatId = chat.ChatId,
                SenderId = currentUserId,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.ChatMessages.AddAsync(message);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int requestId, int currentUserId)
        {
            var allChats = await _unitOfWork.Chats.GetAllAsync();
            var chat = allChats.FirstOrDefault(c => c.ProductRequestId == requestId);

            // Ha még nincs chat (nem küldtek üzenetet), üres listát adunk vissza
            if (chat == null)
                return new List<ChatMessageResponseDto>();

            var allMessages = await _unitOfWork.ChatMessages.GetAllAsync();
            var allUsers = await _unitOfWork.Users.GetAllAsync();

            return allMessages
                .Where(m => m.ChatId == chat.ChatId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageResponseDto
                {
                    Id = m.ChatMessageId,
                    RequestId = requestId,
                    SenderId = m.SenderId,
                    SenderName = allUsers.FirstOrDefault(u => u.UserId == m.SenderId)?.UserName ?? "Ismeretlen",
                    Content = m.Content,
                    SentAt = m.Timestamp,
                    IsRead = m.IsRead
                })
                .ToList();
        }

        public async Task MarkAsRead(int messageId)
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
