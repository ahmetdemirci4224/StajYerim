using StajYerim.Core.Entities;

namespace StajYerim.Core.Interfaces
{
    public interface IJobPostingRepository : IGenericRepository<InternshipPosting>
    {
        Task<List<InternshipPosting>> SearchPostingsAsync(string? role, string? location);
        Task<List<InternshipPosting>> SearchPostingsForChatbotAsync(string? role, string? location, CancellationToken cancellationToken);
    }
}
