using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly StajYerimDbContext _context;

        public JobApplicationRepository(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.Posting)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<JobApplication?> GetApplicationByStudentAndPostingAsync(int studentId, int postingId)
        {
            return await _context.Applications
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.PostingId == postingId);
        }

        public async Task AddApplicationAsync(JobApplication application)
        {
            await _context.Applications.AddAsync(application);
        }

        public void UpdateApplication(JobApplication application)
        {
            _context.Applications.Update(application);
        }

        public async Task<List<JobApplication>> GetApplicationsByStudentIdAsync(int studentId)
        {
            return await _context.Applications
                .Include(a => a.Posting)
                    .ThenInclude(p => p.Company)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }
    }
}
