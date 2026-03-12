using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
