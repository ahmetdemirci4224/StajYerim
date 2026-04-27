using Microsoft.EntityFrameworkCore;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;
using StajYerim.Infrastructure.Data;

namespace StajYerim.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly StajYerimDbContext _context;

        public StudentRepository(StajYerimDbContext context)
        {
            _context = context;
        }

        public async Task<StudentProfile?> GetProfileByUserIdAsync(int userId)
        {
            // Öğrencinin profilini, yeteneklerini ve o yeteneklerin detaylarını (.Include) birlikte getirir
            return await _context.StudentProfiles
                .Include(sp => sp.StudentSkills)
                    .ThenInclude(ss => ss.Skill)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }

        public async Task AddProfileAsync(StudentProfile profile)
        {
            await _context.StudentProfiles.AddAsync(profile);
        }

        public void UpdateProfile(StudentProfile profile)
        {
            _context.StudentProfiles.Update(profile);
        }

        public async Task<StudentSkill?> GetStudentSkillAsync(int studentId, int skillId)
        {
            return await _context.StudentSkills
                .FirstOrDefaultAsync(ss => ss.StudentId == studentId && ss.SkillId == skillId);
        }

        public async Task AddStudentSkillAsync(StudentSkill studentSkill)
        {
            await _context.StudentSkills.AddAsync(studentSkill);
        }

        public void RemoveStudentSkill(StudentSkill studentSkill)
        {
            _context.StudentSkills.Remove(studentSkill);
        }

        public async Task<List<StudentProfile>> SearchStudentsAsync(string? department, int? skillId, decimal? minGpa)
        {
            var query = _context.StudentProfiles
                .Include(sp => sp.User)
                .Include(sp => sp.StudentSkills)
                    .ThenInclude(ss => ss.Skill)
                .AsQueryable();

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(sp => sp.Department.ToLower().Contains(department.ToLower()));
            }

            if (minGpa.HasValue)
            {
                query = query.Where(sp => sp.Gpa >= minGpa.Value);
            }

            if (skillId.HasValue)
            {
                query = query.Where(sp => sp.StudentSkills.Any(ss => ss.SkillId == skillId.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<List<StudentProfile>> SearchStudentsForChatbotAsync(string? role, List<string>? skills, CancellationToken cancellationToken)
        {
            var query = _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.StudentSkills).ThenInclude(ss => ss.Skill)
                .Where(s => s.User != null && !s.User.IsDeleted);

            if (!string.IsNullOrEmpty(role))
                query = query.Where(s => s.Department.Contains(role) || s.Bio.Contains(role));

            if (skills != null && skills.Any())
            {
                foreach (var s in skills)
                {
                    query = query.Where(p => p.StudentSkills.Any(ss => ss.Skill.Name.ToLower().Contains(s.ToLower())));
                }
            }

            return await query.Take(5).ToListAsync(cancellationToken);
        }
    }
}
