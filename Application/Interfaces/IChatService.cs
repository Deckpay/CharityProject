using Application.DTOs;

namespace Application.Interfaces
{
    public interface IChatService
    {
        //üzenet küldés (ide a cimzett idje jön)
        Task SendMessageAsync(ChatMessageRequestDto dto);

        //beszekgetés lekérése a két fél között
        Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int requestId, int currentUserId);

        //olvasatlannak jelölés
        Task MarkAsRead(int messageId);
    }
}
