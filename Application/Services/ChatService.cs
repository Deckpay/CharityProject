using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using Application.DTOs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatrepository;

        public ChatService(IChatRepository chatrepository) { 
            _chatrepository = chatrepository;
        }

        public async Task SendMessageAsync(ChatMessageRequestDto dto)
        {
            // 1. dtbol csinalunk entityt
            var message = new ChatMessage
            {
                Content = dto.Content,
                ReceiverId = dto.ReceiverId,
                Timestamp = DateTime.UtcNow,
                SenderId = 1,  // ez csak ideiglenes majd a bejeletkezett user idja kerül a helyére
                IsRead = true,
            };

            // átadjuk a repositorynak
            await _chatrepository.AddAsync(message);
            await _chatrepository.SaveChangesAsync();
        }

        public  Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int currentUserId, int otherUserId)
        {
            
            throw new NotImplementedException();
        }

        

        public Task MarkAsRead(int messageId)
        {
            throw new NotImplementedException();
        }
    }
}
