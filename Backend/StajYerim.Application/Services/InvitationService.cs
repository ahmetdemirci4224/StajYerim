using StajYerim.Application.DTOs.Invitation;
using StajYerim.Application.DTOs.Student;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public InvitationService(
            IInvitationRepository invitationRepository,
            IStudentRepository studentRepository,
            ICompanyRepository companyRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _invitationRepository = invitationRepository;
            _studentRepository = studentRepository;
            _companyRepository = companyRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<StudentSearchResultDto>>> SearchStudentsAsync(int companyUserId, SearchStudentDto dto, CancellationToken cancellationToken = default)
        {
            var profiles = await _studentRepository.SearchStudentsAsync(dto.Department, dto.SkillId, dto.MinGpa);

            ICollection<PostingSkill>? postingSkills = null;
            if (dto.PostingId.HasValue)
            {
                var posting = await _companyRepository.GetPostingByIdAsync(dto.PostingId.Value, companyUserId);
                if (posting != null)
                {
                    postingSkills = posting.PostingSkills;
                }
            }

            var resultList = profiles.Select(p => new StudentSearchResultDto
            {
                StudentId = p.UserId,
                FullName = p.User.FullName,
                University = p.University,
                Department = p.Department,
                Gpa = p.Gpa,
                Bio = p.Bio,
                MatchScore = postingSkills != null ? MatchCalculator.CalculateMatchScore(postingSkills, p.StudentSkills, p.Gpa) : 0,
                Skills = p.StudentSkills.Select(ss => new StudentSkillDto
                {
                    SkillId = ss.SkillId,
                    SkillName = ss.Skill.Name,
                    ProficiencyLevel = ss.ProficiencyLevel
                }).ToList()
            });

            if (postingSkills != null)
            {
                resultList = resultList.OrderByDescending(r => r.MatchScore);
            }

            return Result<List<StudentSearchResultDto>>.Success(resultList.ToList());
        }

        public async Task<Result<int>> SendInvitationAsync(int companyUserId, SendInvitationDto dto, CancellationToken cancellationToken = default)
        {
            var companyProfile = await _companyRepository.GetProfileByUserIdAsync(companyUserId);
            if (companyProfile == null)
                return Result<int>.Failure("Şirket profili bulunamadı.");

            var studentProfile = await _studentRepository.GetProfileByUserIdAsync(dto.StudentId);
            if (studentProfile == null)
                return Result<int>.Failure("Öğrenci profili bulunamadı.");

            InternshipPosting? posting = null;
            if (dto.PostingId.HasValue)
            {
                posting = await _companyRepository.GetPostingByIdAsync(dto.PostingId.Value, companyUserId);
                if (posting == null)
                    return Result<int>.Failure("Geçersiz ilan ID'si veya ilan size ait değil.");
            }

            var invitation = new Invitation
            {
                CompanyId = companyUserId,
                StudentId = dto.StudentId,
                PostingId = dto.PostingId,
                Message = dto.Message,
                Status = "Pending",
                SentAt = DateTime.UtcNow
            };

            await _invitationRepository.AddInvitationAsync(invitation);
            await _unitOfWork.CompleteAsync();

            // Bildirim Gönder (Öğrenciye)
            var postingTitleStr = posting != null ? $"'{posting.Title}' pozisyonundaki ilan" : "bir pozisyon";
            await _notificationService.CreateNotificationAsync(
                dto.StudentId,
                "Yeni Davet Aldınız!",
                $"{companyProfile.CompanyName} şirketi {postingTitleStr} için size davet gönderdi.",
                "Invitation",
                invitation.Id,
                cancellationToken
            );

            return Result<int>.Success(invitation.Id, "Davet öğrenciye başarıyla gönderildi.");
        }

        public async Task<Result<bool>> RespondToInvitationAsync(int studentUserId, int invitationId, RespondInvitationDto dto, CancellationToken cancellationToken = default)
        {
            var invitation = await _invitationRepository.GetInvitationByIdAsync(invitationId);
            if (invitation == null)
                return Result<bool>.Failure("Davet bulunamadı.");

            if (invitation.StudentId != studentUserId)
                return Result<bool>.Failure("Sadece kendi davetlerinize yanıt verebilirsiniz.");

            if (invitation.Status != "Pending")
                return Result<bool>.Failure("Bu davet zaten yanıtlanmış.");

            invitation.Status = dto.Status; // Accepted veya Declined
            invitation.UpdatedDate = DateTime.UtcNow;

            _invitationRepository.UpdateInvitation(invitation);
            await _unitOfWork.CompleteAsync();

            // Bildirim Gönder (Şirkete)
            var studentProfile = await _studentRepository.GetProfileByUserIdAsync(studentUserId);
            var studentName = studentProfile?.User?.FullName ?? "Bir öğrenci";
            var statusTr = dto.Status == "Accepted" ? "Kabul Etti" : "Reddetti";
            
            await _notificationService.CreateNotificationAsync(
                invitation.CompanyId,
                "Davet Yanıtı",
                $"{studentName}, gönderdiğiniz daveti {statusTr}.",
                "Invitation",
                invitation.Id,
                cancellationToken
            );

            return Result<bool>.Success(true, $"Davet durumu '{dto.Status}' olarak güncellendi.");
        }

        public async Task<Result<List<InvitationDto>>> GetStudentInvitationsAsync(int studentUserId, CancellationToken cancellationToken = default)
        {
            var invitations = await _invitationRepository.GetInvitationsByStudentIdAsync(studentUserId);

            var dtoList = invitations.Select(i => new InvitationDto
            {
                InvitationId = i.Id,
                CompanyName = i.Company.User.FullName, // User tablosundan
                StudentName = i.Student.User.FullName,
                PostingTitle = i.Posting?.Title,
                Message = i.Message,
                Status = i.Status,
                SentAt = i.SentAt
            }).ToList();

            return Result<List<InvitationDto>>.Success(dtoList);
        }

        public async Task<Result<List<InvitationDto>>> GetCompanyInvitationsAsync(int companyUserId, CancellationToken cancellationToken = default)
        {
            var invitations = await _invitationRepository.GetInvitationsByCompanyIdAsync(companyUserId);

            var dtoList = invitations.Select(i => new InvitationDto
            {
                InvitationId = i.Id,
                CompanyName = i.Company.User.FullName,
                StudentName = i.Student.User.FullName,
                PostingTitle = i.Posting?.Title,
                Message = i.Message,
                Status = i.Status,
                SentAt = i.SentAt
            }).ToList();

            return Result<List<InvitationDto>>.Success(dtoList);
        }
    }
}
