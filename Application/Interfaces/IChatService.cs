using Application.DTOs;

namespace Application.Interfaces
{
    public interface IChatService
    {
        // üzenet küldés (ide a cimzett id-je jön)
        Task SendMessageAsync(ChatMessageRequestDto dto);

        // beszélgetés lekérése a két fél között
        Task<List<ChatMessageResponseDto>> GetChatHistoryAsync(int requestId, int currentUserId);

        //olvasatlannak jelölés
        Task MarkAsRead(int messageId);

        // chat profilok helyes megjelenitése
        Task<ChatInfoDto> GetChatInfoAsync(int requestId, int currentUserId);

        // olvasatlan üzenetek számának megjelenítése
        Task<int> GetTotalUnreadCountAsync(int currentUserId);

        Task MarkAsAllReadAsync(int requestId, int currentUserId);
    }
}
