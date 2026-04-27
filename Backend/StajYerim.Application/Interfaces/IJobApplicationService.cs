using StajYerim.Application.DTOs.Application;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface IJobApplicationService
    {
        Task<Result<int>> ApplyAsync(int studentUserId, ApplyToPostingDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> UpdateStatusAsync(int companyUserId, int applicationId, UpdateApplicationStatusDto dto, CancellationToken cancellationToken = default);
        Task<Result<List<StudentApplicationDto>>> GetStudentApplicationsAsync(int studentUserId, CancellationToken cancellationToken = default);
    }
}
