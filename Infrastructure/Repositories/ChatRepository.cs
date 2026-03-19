using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly CharityDbContext _context;

        public ChatRepository(CharityDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatMessage message)
        {
            
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            
            await _context.SaveChangesAsync(); // dbcontext legeneralja az sqlt
        }

        public async Task<ChatMessage> GetByIdAsync(int id)
        {
            var message = await _context.ChatMessages.FindAsync(id);
            if (message == null) {
                throw new Exception("Az üzenet nem taalállható");
            }
            return message;

        }

        public async Task<List<ChatMessage>> GetAllMessagesAsync()
        {
            return await _context.ChatMessages.ToListAsync();
        }
    }
}
