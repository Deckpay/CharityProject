using Domain.Entities;

namespace Application.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatMessage> GetByIdAsync(int id);
        //Task<ChatMessage?> GetByIdAsync(int id);
        Task<List<ChatMessage>> GetAllMessagesAsync();
        Task AddAsync(ChatMessage message); // ezt hivja meg a service
        Task SaveChangesAsync();
    }
}
