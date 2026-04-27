using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StajYerimDbContext _context;

        public UnitOfWork(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
