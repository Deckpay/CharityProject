using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbcontext
    {
        DbSet<Chat> Chats { get; }
        
        
        // itt van az oszes tabléa amit a chat service hasznalni fog
        DbSet<ChatMessage> ChatMessages { get; set; }
        DbSet<Product> Requests { get; set; }
        //metnés 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
