using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ErtekmentoDbContext _context; // Adatbázis
        private readonly DbSet<T> _dbSet; // Adatbázis tábla

        public GenericRepository(ErtekmentoDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            // csak jelöljük, hogy modosúlt
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            // csak jelöljök, hogy törlendő
            _dbSet.Remove(entity);
        }
    }
}
