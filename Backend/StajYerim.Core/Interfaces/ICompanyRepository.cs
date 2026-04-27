using StajYerim.Core.Entities;

namespace StajYerim.Core.Interfaces
{
    public interface ICompanyRepository
    {
        Task<CompanyProfile?> GetProfileByUserIdAsync(int userId);
        Task<InternshipPosting?> GetPostingByIdAsync(int postingId, int companyId);
        Task AddPostingAsync(InternshipPosting posting);
        Task<PostingSkill?> GetPostingSkillAsync(int postingId, int skillId);
        Task AddPostingSkillAsync(PostingSkill postingSkill);
        void RemovePostingSkill(PostingSkill postingSkill);
        Task<List<JobApplication>> GetApplicantsForPostingAsync(int postingId, int companyId);
    }
}
