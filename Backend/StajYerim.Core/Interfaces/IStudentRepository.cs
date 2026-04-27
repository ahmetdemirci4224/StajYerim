using StajYerim.Core.Entities;

namespace StajYerim.Core.Interfaces
{
    public interface IStudentRepository
    {
        Task<StudentProfile?> GetProfileByUserIdAsync(int userId);
        Task AddProfileAsync(StudentProfile profile);
        void UpdateProfile(StudentProfile profile);
        Task<StudentSkill?> GetStudentSkillAsync(int studentId, int skillId);
        Task AddStudentSkillAsync(StudentSkill studentSkill);
        void RemoveStudentSkill(StudentSkill studentSkill);
        Task<List<StudentProfile>> SearchStudentsAsync(string? department, int? skillId, decimal? minGpa);
        Task<List<StudentProfile>> SearchStudentsForChatbotAsync(string? role, List<string>? skills, CancellationToken cancellationToken);
    }
}
