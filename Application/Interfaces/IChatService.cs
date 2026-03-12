using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IChatService
    {
        //üzenet küldés (ide a cimzett idje jön)
        Task SendMessageAsync(ChatMessageRequestDto dto);

        //beszekgetés lekérése a két fél között
        Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int currentUserId, int otherUserId);

        //olvasatlannak jelölés
        Task MarkAsRead(int messageId);
    }
}
