using StajYerim.Application.DTOs.Company;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Result<int>> CreatePostingAsync(int companyUserId, CreatePostingDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> AddSkillToPostingAsync(int companyUserId, int postingId, AddPostingSkillDto dto, CancellationToken cancellationToken = default);
        Task<Result<List<PostingApplicantDto>>> GetApplicantsAsync(int companyUserId, int postingId, CancellationToken cancellationToken = default);
    }
}
