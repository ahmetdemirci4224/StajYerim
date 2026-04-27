using StajYerim.Application.DTOs.Application;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _applicationRepository;
        private readonly IGenericRepository<InternshipPosting> _postingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public JobApplicationService(
            IJobApplicationRepository applicationRepository, 
            IGenericRepository<InternshipPosting> postingRepository,
            IStudentRepository studentRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _applicationRepository = applicationRepository;
            _postingRepository = postingRepository;
            _studentRepository = studentRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> ApplyAsync(int studentUserId, ApplyToPostingDto dto, CancellationToken cancellationToken = default)
        {
            // 1. Profil var mı?
            var profile = await _studentRepository.GetProfileByUserIdAsync(studentUserId);
            if (profile == null) 
                return Result<int>.Failure("Başvuru yapabilmek için önce profilinizi tamamlamalısınız.");

            // 2. İlan geçerli mi?
            var posting = await _postingRepository.GetByIdAsync(dto.PostingId);
            if (posting == null || posting.IsDeleted || posting.Status != "Open")
                return Result<int>.Failure("İlan bulunamadı veya başvuruya kapanmış.");

            // 3. Mükerrer başvuru var mı?
            var existingApplication = await _applicationRepository.GetApplicationByStudentAndPostingAsync(studentUserId, dto.PostingId);
                
            if (existingApplication != null)
                return Result<int>.Failure("Bu ilana daha önce zaten başvurdunuz.");

            // 4. Başvuruyu kaydet
            var application = new JobApplication
            {
                StudentId = studentUserId,
                PostingId = dto.PostingId,
                CoverLetter = dto.CoverLetter,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            await _applicationRepository.AddApplicationAsync(application);
            await _unitOfWork.CompleteAsync();

            // Bildirim gönder (Şirkete)
            await _notificationService.CreateNotificationAsync(
                posting.CompanyId,
                "Yeni Bir Başvuru Aldınız!",
                $"Bir öğrenci '{posting.Title}' ilanınıza başvurdu. Lütfen aday listesini kontrol edin.",
                "Application",
                application.Id,
                cancellationToken
            );

            return Result<int>.Success(application.Id, "Başvurunuz şirkete başarıyla iletildi.");
        }

        public async Task<Result<bool>> UpdateStatusAsync(int companyUserId, int applicationId, UpdateApplicationStatusDto dto, CancellationToken cancellationToken = default)
        {
            // Başvuruyu çek
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
                
            if (application == null) 
                return Result<bool>.Failure("Başvuru bulunamadı.");

            // Güvenlik Duvarı: Sadece ilanın sahibi olan şirket, başvuru durumunu değiştirebilir
            if (application.Posting.CompanyId != companyUserId)
                return Result<bool>.Failure("Sadece ilanın sahibi şirket başvuru durumunu güncelleyebilir.");

            application.Status = dto.Status;
            application.UpdatedDate = DateTime.UtcNow;
            
            _applicationRepository.UpdateApplication(application);
            await _unitOfWork.CompleteAsync();

            // Bildirim gönder (Öğrenciye)
            var statusTr = dto.Status == "Accepted" ? "Kabul Edildi" : (dto.Status == "Declined" ? "Reddedildi" : dto.Status);
            await _notificationService.CreateNotificationAsync(
                application.StudentId,
                "Başvuru Durumunuz Güncellendi",
                $"'{application.Posting.Title}' ilanına yaptığınız başvurunun durumu '{statusTr}' olarak güncellendi.",
                "Application",
                application.Id,
                cancellationToken
            );

            return Result<bool>.Success(true, $"Başvuru durumu '{dto.Status}' olarak güncellendi.");
        }

        public async Task<Result<List<StudentApplicationDto>>> GetStudentApplicationsAsync(int studentUserId, CancellationToken cancellationToken = default)
        {
            // Öğrencinin geçmiş tüm başvurularını çekelim
            var applications = await _applicationRepository.GetApplicationsByStudentIdAsync(studentUserId);

            var dtoList = applications.Select(a => new StudentApplicationDto
            {
                ApplicationId = a.Id,
                PostingId = a.PostingId,
                CompanyName = a.Posting.Company.CompanyName,
                PostingTitle = a.Posting.Title,
                Status = a.Status,
                AppliedAt = a.AppliedAt
            }).ToList();

            return Result<List<StudentApplicationDto>>.Success(dtoList);
        }
    }
}
