using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbcontext
    {
        DbSet<Chat> Chats { get; }
        
        
        // itt van az oszes tabléa amit a chat service hasznalni fog
        DbSet<ChatMessage> ChatMessages { get; set; }
        DbSet<ProductRequest> ProductRequests { get; set; }
        //metnés 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
