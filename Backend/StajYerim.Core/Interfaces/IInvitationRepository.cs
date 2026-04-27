using StajYerim.Core.Entities;

namespace StajYerim.Core.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation?> GetInvitationByIdAsync(int invitationId);
        Task AddInvitationAsync(Invitation invitation);
        void UpdateInvitation(Invitation invitation);
        Task<List<Invitation>> GetInvitationsByStudentIdAsync(int studentId);
        Task<List<Invitation>> GetInvitationsByCompanyIdAsync(int companyId);
    }
}
