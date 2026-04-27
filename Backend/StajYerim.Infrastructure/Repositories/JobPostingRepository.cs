using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class JobPostingRepository : GenericRepository<InternshipPosting>, IJobPostingRepository
    {
        private new readonly StajYerimDbContext _context;

        public JobPostingRepository(StajYerimDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<InternshipPosting>> SearchPostingsAsync(string? role, string? location)
        {
            var query = _context.InternshipPostings.AsQueryable();
            // ... implementation not needed for chatbot directly, but required by interface
            return await query.ToListAsync();
        }

        public async Task<List<InternshipPosting>> SearchPostingsForChatbotAsync(string? role, string? location, CancellationToken cancellationToken)
        {
            var query = _context.InternshipPostings
                .Include(p => p.Company).ThenInclude(c => c!.User)
                .Where(p => p.Status == "Open" && !p.IsDeleted);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(p => p.Title.Contains(role) || (p.Description != null && p.Description.Contains(role)));
            
            if (!string.IsNullOrEmpty(location))
                query = query.Where(p => p.Location != null && p.Location.Contains(location));

            return await query.Take(5).ToListAsync(cancellationToken);
        }
    }
}
