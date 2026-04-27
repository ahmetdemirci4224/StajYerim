using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Common;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;
using System.Linq.Expressions;

namespace StajYerim.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly StajYerimDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(StajYerimDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
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

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            _dbSet.Update(entity); // Soft Delete uyguluyoruz
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                entity.IsDeleted = true;
                entity.IsActive = false;
                entity.UpdatedDate = DateTime.UtcNow;
            }
            _dbSet.UpdateRange(entities); // Soft Delete
        }
    }
}
