using StajYerim.Application.DTOs.Invitation;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface IInvitationService
    {
        Task<Result<List<StudentSearchResultDto>>> SearchStudentsAsync(int companyUserId, SearchStudentDto dto, CancellationToken cancellationToken = default);
        Task<Result<int>> SendInvitationAsync(int companyUserId, SendInvitationDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> RespondToInvitationAsync(int studentUserId, int invitationId, RespondInvitationDto dto, CancellationToken cancellationToken = default);
        Task<Result<List<InvitationDto>>> GetStudentInvitationsAsync(int studentUserId, CancellationToken cancellationToken = default);
        Task<Result<List<InvitationDto>>> GetCompanyInvitationsAsync(int companyUserId, CancellationToken cancellationToken = default);
    }
}
