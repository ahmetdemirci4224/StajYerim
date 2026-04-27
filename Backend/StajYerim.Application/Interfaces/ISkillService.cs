using StajYerim.Application.DTOs.Skill;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface ISkillService
    {
        Task<Result<int>> CreateSkillAsync(CreateSkillDto dto, CancellationToken cancellationToken = default);
        Task<Result<List<SkillDto>>> GetAllSkillsAsync(CancellationToken cancellationToken = default);
    }
}
