using StajYerim.Application.DTOs.Student;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface IStudentService
    {
        Task<Result<StudentProfileDto>> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<StudentProfileDto>> UpsertProfileAsync(int userId, UpdateStudentProfileDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> AddSkillAsync(int userId, AddStudentSkillDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> RemoveSkillAsync(int userId, int skillId, CancellationToken cancellationToken = default);
    }
}
