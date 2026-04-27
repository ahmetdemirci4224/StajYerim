using StajYerim.Core.Entities;

namespace StajYerim.Core.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetApplicationByIdAsync(int applicationId);
        Task<JobApplication?> GetApplicationByStudentAndPostingAsync(int studentId, int postingId);
        Task AddApplicationAsync(JobApplication application);
        void UpdateApplication(JobApplication application);
        Task<List<JobApplication>> GetApplicationsByStudentIdAsync(int studentId);
    }
}
