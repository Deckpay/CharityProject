using Application.Interfaces;
using Domain.Entities;
using Application.DTOs;
using Domain.Enums;

namespace Application.Services
{
    /// <summary>
    /// A chat funkciókért felelős szolgáltatás.
    /// Kezeli az üzenetküldést, az előzmények lekérdezését és az olvasatlan üzenetek állapotát.
    /// </summary>
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

            var request = await _unitOfWork.ProductRequests.GetByIdAsync(dto.RequestId);
            if (request == null)
                throw new Exception("Nincs ilyen igénylés.");

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new Exception($"A termék nem található (ID: {request.ProductId}).");

            var chat = (await _unitOfWork.Chats.FindAsync(c => c.ProductRequestId == dto.RequestId)).FirstOrDefault();

            if (chat == null)
            {
                chat = new Chat
                {
                    ProductRequestId = dto.RequestId,
                    RequesterId = request.RequesterId,
                    SenderId = product.SenderId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Chats.AddAsync(chat);
                await _unitOfWork.CompleteAsync();
            }

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
            var chat = (await _unitOfWork.Chats.FindAsync(c => c.ProductRequestId == requestId)).FirstOrDefault();

            if (chat == null)
                return new List<ChatMessageResponseDto>();

            var allMessages = await _unitOfWork.ChatMessages.GetAllAsync();
            var allUsers = await _unitOfWork.Users.GetAllAsync();

            var otherPartyId = (currentUserId == chat.SenderId) ? chat.RequesterId : chat.SenderId;
            var otherPartyName = allUsers.FirstOrDefault(u => u.UserId == otherPartyId)?.UserName ?? "Ismeretlen";

            return allMessages
                .Where(m => m.ChatId == chat.ChatId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageResponseDto
                {
                    Id = m.ChatMessageId,
                    RequestId = requestId,
                    SenderId = m.SenderId,
                    UserId = chat.SenderId,
                    SenderName = allUsers.FirstOrDefault(u => u.UserId == m.SenderId) is var user && user != null
                        ? $"{user.FirstName} {user.LastName}" : "Ismeretlen",
                    OtherPartyName = otherPartyName,
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
        public async Task<ChatInfoDto> GetChatInfoAsync(int requestId, int currentUserId)
        {
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);
            if (request == null)
                return new ChatInfoDto { OtherPartyName = "Ismeretlen" };

            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                return new ChatInfoDto { OtherPartyName = "Ismeretlen" };

            var allUsers = await _unitOfWork.Users.GetAllAsync();

            var otherPartyId = (currentUserId == product.SenderId)
                ? request.RequesterId
                : product.SenderId;

            var otherPartyName = allUsers.FirstOrDefault(u => u.UserId == otherPartyId) is var user && user != null
                        ? $"{user.FirstName} {user.LastName}" : "Ismeretlen";

            return new ChatInfoDto 
            { 
                OtherPartyName = otherPartyName,
                SenderId = product.SenderId,
                ProductName = product.ProductName
            };
        }

        public async Task<int> GetTotalUnreadCountAsync(int currentUserId)
        {
            var allMessages = await _unitOfWork.ChatMessages.GetAllAsync();
            var allChats = await _unitOfWork.Chats.GetAllAsync();
            var allRequests = await _unitOfWork.ProductRequests.GetAllAsync();

            var activeRequestsIds = allRequests
                .Where(r => r.RequestStatus == RequestStatus.Pending)
                .Select(r => r.ProductRequestId)
                .ToHashSet();

            var userChatIds = allChats
                .Where(c => (c.SenderId == currentUserId || c.RequesterId == currentUserId)
                    && activeRequestsIds.Contains(c.ProductRequestId))
                .Select(c => c.ChatId)
                .ToHashSet();

            return allMessages.Count(m =>
                userChatIds.Contains(m.ChatId) &&
                !m.IsRead &&
                m.SenderId != currentUserId

            );
        }

        public async Task MarkAsAllReadAsync(int requestId, int currentUserId)
        {
            var chat = (await _unitOfWork.Chats.FindAsync(c => c.ProductRequestId == requestId)).FirstOrDefault();
            if (chat == null)
            {
                return;
            } 

            var unread = (await _unitOfWork.ChatMessages.FindAsync(m => 
                m.ChatId == chat.ChatId && m.SenderId != currentUserId && !m.IsRead));            var unread = allMessages

            if (!unread.Any()) return;

            foreach (var msg in unread)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.UtcNow;
                _unitOfWork.ChatMessages.Update(msg);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
