using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CharityDbContext _context;

        // Itt tároljuk a konkrét repository példányokat
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<ProductCategory> Categories { get; private set; }
        public IGenericRepository<County> Counties { get; private set; }

        public UnitOfWork(CharityDbContext context)
        {
            _context = context;

            // Inicializáljuk a repository-kat, átadva nekik a közös context-et
            Products = new GenericRepository<Product>(_context);
            Users = new GenericRepository<User>(_context);
            Categories = new GenericRepository<ProductCategory>(_context);
            Counties = new GenericRepository<County>(_context);
        }

        // Ez a metódus az "indítógomb": egyszerre ment el mindent az adatbázisba
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Felszabadítjuk az erőforrásokat, ha végeztünk
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
