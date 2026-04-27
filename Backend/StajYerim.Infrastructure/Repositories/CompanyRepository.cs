using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly StajYerimDbContext _context;

        public CompanyRepository(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyProfile?> GetProfileByUserIdAsync(int userId)
        {
            return await _context.CompanyProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == userId);
        }

        public async Task<InternshipPosting?> GetPostingByIdAsync(int postingId, int companyId)
        {
            return await _context.InternshipPostings
                .Include(p => p.PostingSkills)
                .FirstOrDefaultAsync(p => p.Id == postingId && p.CompanyId == companyId && !p.IsDeleted);
        }

        public async Task AddPostingAsync(InternshipPosting posting)
        {
            await _context.InternshipPostings.AddAsync(posting);
        }

        public async Task<PostingSkill?> GetPostingSkillAsync(int postingId, int skillId)
        {
            return await _context.PostingSkills
                .FirstOrDefaultAsync(ps => ps.PostingId == postingId && ps.SkillId == skillId);
        }

        public async Task AddPostingSkillAsync(PostingSkill postingSkill)
        {
            await _context.PostingSkills.AddAsync(postingSkill);
        }

        public void RemovePostingSkill(PostingSkill postingSkill)
        {
            _context.PostingSkills.Remove(postingSkill);
        }

        public async Task<List<JobApplication>> GetApplicantsForPostingAsync(int postingId, int companyId)
        {
            // Şirketin ilanı olup olmadığını kontrol et (güvenlik)
            var postingExists = await _context.InternshipPostings
                .AnyAsync(p => p.Id == postingId && p.CompanyId == companyId && !p.IsDeleted);
                
            if (!postingExists) return new List<JobApplication>();

            // Başvuruları, öğrenci bilgileri ve yetenekleriyle beraber getir
            return await _context.Applications
                .Include(a => a.Student)
                    .ThenInclude(sp => sp.User)
                .Include(a => a.Student)
                    .ThenInclude(sp => sp.StudentSkills)
                        .ThenInclude(ss => ss.Skill)
                .Include(a => a.Posting) // Eşleşme skoru için ilanın yeteneklerini de çekiyoruz
                    .ThenInclude(p => p.PostingSkills)
                .Where(a => a.PostingId == postingId)
                .ToListAsync();
        }
    }
}
