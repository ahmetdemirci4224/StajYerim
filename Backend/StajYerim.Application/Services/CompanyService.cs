using StajYerim.Application.DTOs.Company;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IGenericRepository<Skill> _skillRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(ICompanyRepository companyRepository, IGenericRepository<Skill> skillRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _skillRepository = skillRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> CreatePostingAsync(int companyUserId, CreatePostingDto dto, CancellationToken cancellationToken = default)
        {
            // Eğer CompanyProfile yoksa (sadece User tablosunda kaydı varsa) oluşturuyoruz
            var profile = await _companyRepository.GetProfileByUserIdAsync(companyUserId);
            if (profile == null)
            {
                // Normalde ayrı bir profil oluşturma endpoint'i de olabilir, ancak basitlik için otomatik açıyoruz
                profile = new CompanyProfile { UserId = companyUserId, CompanyName = "Şirket" }; // Gerçek senaryoda User tablosundan CompanyName çekilebilir.
                // Burada detaylara girmeden var sayıyoruz.
            }

            var posting = new InternshipPosting
            {
                CompanyId = companyUserId,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                WorkModel = dto.WorkModel,
                ExpiresAt = dto.ExpiresAt,
                Status = "Open"
            };

            await _companyRepository.AddPostingAsync(posting);
            await _unitOfWork.CompleteAsync();

            return Result<int>.Success(posting.Id, "Staj ilanı başarıyla oluşturuldu.");
        }

        public async Task<Result<bool>> AddSkillToPostingAsync(int companyUserId, int postingId, AddPostingSkillDto dto, CancellationToken cancellationToken = default)
        {
            var posting = await _companyRepository.GetPostingByIdAsync(postingId, companyUserId);
            if (posting == null) 
                return Result<bool>.Failure("İlan bulunamadı veya bu ilan size ait değil.");

            var skill = await _skillRepository.GetByIdAsync(dto.SkillId);
            if (skill == null) 
                return Result<bool>.Failure("Sistemde böyle bir yetenek (Skill) bulunamadı.");

            var existingPostingSkill = await _companyRepository.GetPostingSkillAsync(postingId, dto.SkillId);
            if (existingPostingSkill != null)
            {
                existingPostingSkill.IsMandatory = dto.IsMandatory;
            }
            else
            {
                var postingSkill = new PostingSkill
                {
                    PostingId = postingId,
                    SkillId = dto.SkillId,
                    IsMandatory = dto.IsMandatory
                };
                await _companyRepository.AddPostingSkillAsync(postingSkill);
            }

            await _unitOfWork.CompleteAsync();
            return Result<bool>.Success(true, "Yetenek ilana başarıyla eklendi.");
        }

        public async Task<Result<List<PostingApplicantDto>>> GetApplicantsAsync(int companyUserId, int postingId, CancellationToken cancellationToken = default)
        {
            var applications = await _companyRepository.GetApplicantsForPostingAsync(postingId, companyUserId);

            var dtoList = applications.Select(a => new PostingApplicantDto
            {
                ApplicationId = a.Id,
                StudentId = a.StudentId,
                StudentFullName = a.Student.User.FullName,
                University = a.Student.University,
                Department = a.Student.Department,
                Gpa = a.Student.Gpa,
                CoverLetter = a.CoverLetter,
                ApplicationStatus = a.Status,
                AppliedAt = a.AppliedAt,
                MatchScore = MatchCalculator.CalculateMatchScore(a.Posting.PostingSkills, a.Student.StudentSkills, a.Student.Gpa),
                Skills = a.Student.StudentSkills.Select(ss => new ApplicantSkillDto
                {
                    SkillName = ss.Skill.Name,
                    ProficiencyLevel = ss.ProficiencyLevel
                }).ToList()
            })
            .OrderByDescending(a => a.MatchScore) // En iyi eşleşenleri en üste koy!
            .ToList();

            return Result<List<PostingApplicantDto>>.Success(dtoList);
        }
    }
}
