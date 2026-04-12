using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ErtekmentoDbContext _context;
        

        // Itt tároljuk a konkrét repository példányokat
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<ProductCategory> Categories { get; private set; }
        public IGenericRepository<County> Counties { get; private set; }
        public IGenericRepository<Chat> Chats { get; private set; }
        public IGenericRepository<ChatMessage> ChatMessages { get; private set; }
        public IGenericRepository<ProductRequest> ProductRequests { get; private set; }
        public IGenericRepository<RequesterLimitRule> RequesterLimitRules { get; private set; }
        public IGenericRepository<RequesterLimitUsage> RequesterLimitUsages { get; private set; }

        public UnitOfWork(ErtekmentoDbContext context)
        {
            _context = context;

            // Inicializáljuk a repository-kat, átadva nekik a közös context-et
            Products = new GenericRepository<Product>(_context);
            Users = new GenericRepository<User>(_context);
            Categories = new GenericRepository<ProductCategory>(_context);
            Counties = new GenericRepository<County>(_context);
            Chats = new GenericRepository<Chat>(_context);
            ChatMessages = new GenericRepository<ChatMessage>(_context);
            ProductRequests = new GenericRepository<ProductRequest>(_context);
            RequesterLimitRules = new GenericRepository<RequesterLimitRule>(_context);
            RequesterLimitUsages = new GenericRepository<RequesterLimitUsage>(_context);
        }

        // Ez a metódus az "indítógomb": egyszerre ment el mindent az adatbázisba
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync() =>
            await _context.Database.BeginTransactionAsync();

        // Felszabadítjuk az erőforrásokat, ha végeztünk
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
