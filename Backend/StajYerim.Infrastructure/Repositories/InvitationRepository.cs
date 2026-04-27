using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly StajYerimDbContext _context;

        public InvitationRepository(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task<Invitation?> GetInvitationByIdAsync(int invitationId)
        {
            return await _context.Invitations
                .Include(i => i.Company)
                .Include(i => i.Student)
                .Include(i => i.Posting)
                .FirstOrDefaultAsync(i => i.Id == invitationId);
        }

        public async Task AddInvitationAsync(Invitation invitation)
        {
            await _context.Invitations.AddAsync(invitation);
        }

        public void UpdateInvitation(Invitation invitation)
        {
            _context.Invitations.Update(invitation);
        }

        public async Task<List<Invitation>> GetInvitationsByStudentIdAsync(int studentId)
        {
            return await _context.Invitations
                .Include(i => i.Company)
                    .ThenInclude(c => c.User)
                .Include(i => i.Posting)
                .Where(i => i.StudentId == studentId)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
        }

        public async Task<List<Invitation>> GetInvitationsByCompanyIdAsync(int companyId)
        {
            return await _context.Invitations
                .Include(i => i.Student)
                    .ThenInclude(s => s.User)
                .Include(i => i.Posting)
                .Where(i => i.CompanyId == companyId)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
        }
    }
}
