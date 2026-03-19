using Application.Interfaces;
using Domain.Entities;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork; // Az adatbázis elérés
        private readonly ICurrentUserService _currentUserService;

        public ChatService( ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task SendMessageAsync(ChatMessageRequestDto dto)
        {
            var currentUserId = _currentUserService.UserId;

            // ELLENŐRZÉS: Létezik-e az igénylés és a user része-e?
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(dto.RequestId);
            if (request == null) throw new Exception("Nincs ilyen igénylés.");

            // termek keresés
            //var allProducts = await _unitOfWork.ProductRequests.GetByIdAsync(dto.RequestId);
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            // debug hibauzenet
            if (product == null)
            {
                throw new Exception($"DEBUG: Keresett termék ID: {request.ProductId}. Az EF összesen {request.ProductId} db terméket lát a Products táblában.");
            }

            // csak donor vagy raszoruló irhat
            if (product == null)
            {
                // itt jelenleg csak a donor ellenőrizhető.
                throw new UnauthorizedAccessException("Nincs jogosultságod ehhez a beszélgetéshez.");
            }

            var allChats = await _unitOfWork.Chats.GetAllAsync();
            var chat = allChats.FirstOrDefault(c => c.ProductRequestId == dto.RequestId);
            // chat kereses igénylés alapján
            if (chat == null)
            { 
                chat = new Chat
                {
                    ProductRequestId = dto.RequestId,
                    RequesterId = request.RequesterId,
                    DonorId = product.DonorId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
            }

            await _unitOfWork.Chats.AddAsync(chat);
            await _unitOfWork.CompleteAsync(); // Elmentjuk hogy az sql generaljon neki chat idt

            var message = new ChatMessage
            {
                ChatId = chat.ChatId, // Itt kapcsoljuk össze a chat-el!
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
            var request = await _unitOfWork.ProductRequests.GetByIdAsync(requestId);

            // meg kell keresni az igenyléshez tartozo chat et
            var allChats = await _unitOfWork.Chats.GetAllAsync();
            var chat = allChats.FirstOrDefault(c => c.ProductRequestId == requestId);

            if (chat == null) return new List<ChatMessageResponseDto>(); // nem jott még létre a beszelgetes azért üzenet nincs

            var allMessages = await _unitOfWork.ChatMessages.GetAllAsync();
            var allUseres = await _unitOfWork.Users.GetAllAsync();

            var chatHisrory = allMessages
                .Where(m => m.ChatId == chat.ChatId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageResponseDto
                {
                    Id = m.ChatMessageId,
                    RequestId = requestId,
                    SenderId = m.SenderId,
                    SenderName = allUseres.FirstOrDefault(u => u.UserId == m.SenderId )?.UserName?? "ismeretlen" ,
                    Content = m.Content,
                    SentAt = m.Timestamp,
                    IsRead = m.IsRead
                }).ToList();
            return chatHisrory;
        }

        public async Task MarkAsRead(int messageId)
        {
            var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow; // kirija mikor olvasta az üzenetet
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}