using StajYerim.Application.DTOs.Student;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Skill> _skillRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IStudentRepository studentRepository, 
                              IGenericRepository<User> userRepository, 
                              IGenericRepository<Skill> skillRepository, 
                              IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _skillRepository = skillRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudentProfileDto>> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result<StudentProfileDto>.Failure("Kullanıcı bulunamadı.");

            var profile = await _studentRepository.GetProfileByUserIdAsync(userId);
            if (profile == null) return Result<StudentProfileDto>.Failure("Öğrenci profilini henüz oluşturmamış. Lütfen önce profilinizi tamamlayın.");

            var dto = new StudentProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                University = profile.University,
                Department = profile.Department,
                GraduationYear = profile.GraduationYear,
                Gpa = profile.Gpa,
                Bio = profile.Bio,
                GithubUrl = profile.GithubUrl,
                LinkedinUrl = profile.LinkedinUrl,
                PreferredLocation = profile.PreferredLocation,
                PreferredWorkModel = profile.PreferredWorkModel,
                InterestedRoles = profile.InterestedRoles,
                Skills = profile.StudentSkills.Select(ss => new StudentSkillDto
                {
                    SkillId = ss.SkillId,
                    SkillName = ss.Skill.Name,
                    ProficiencyLevel = ss.ProficiencyLevel
                }).ToList()
            };

            return Result<StudentProfileDto>.Success(dto);
        }

        public async Task<Result<StudentProfileDto>> UpsertProfileAsync(int userId, UpdateStudentProfileDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return Result<StudentProfileDto>.Failure("Kullanıcı bulunamadı.");

            var profile = await _studentRepository.GetProfileByUserIdAsync(userId);
            
            // Eğer profil yoksa Insert (Ekle), varsa Update (Güncelle)
            if (profile == null)
            {
                profile = new StudentProfile
                {
                    UserId = userId,
                    University = dto.University,
                    Department = dto.Department,
                    GraduationYear = dto.GraduationYear,
                    Gpa = dto.Gpa,
                    Bio = dto.Bio,
                    GithubUrl = dto.GithubUrl,
                    LinkedinUrl = dto.LinkedinUrl,
                    PreferredLocation = dto.PreferredLocation,
                    PreferredWorkModel = dto.PreferredWorkModel,
                    InterestedRoles = dto.InterestedRoles,
                    UpdatedDate = DateTime.UtcNow
                };
                await _studentRepository.AddProfileAsync(profile);
            }
            else
            {
                profile.University = dto.University;
                profile.Department = dto.Department;
                profile.GraduationYear = dto.GraduationYear;
                profile.Gpa = dto.Gpa;
                profile.Bio = dto.Bio;
                profile.GithubUrl = dto.GithubUrl;
                profile.LinkedinUrl = dto.LinkedinUrl;
                profile.PreferredLocation = dto.PreferredLocation;
                profile.PreferredWorkModel = dto.PreferredWorkModel;
                profile.InterestedRoles = dto.InterestedRoles;
                profile.UpdatedDate = DateTime.UtcNow;
                
                _studentRepository.UpdateProfile(profile);
            }

            await _unitOfWork.CompleteAsync();

            return await GetProfileAsync(userId, cancellationToken);
        }

        public async Task<Result<bool>> AddSkillAsync(int userId, AddStudentSkillDto dto, CancellationToken cancellationToken = default)
        {
            var profile = await _studentRepository.GetProfileByUserIdAsync(userId);
            if (profile == null) return Result<bool>.Failure("Yetenek ekleyebilmek için önce profilinizi oluşturmalısınız.");

            var skill = await _skillRepository.GetByIdAsync(dto.SkillId);
            if (skill == null) return Result<bool>.Failure("Sistemde böyle bir yetenek bulunamadı.");

            var existingSkill = await _studentRepository.GetStudentSkillAsync(userId, dto.SkillId);
            
            if (existingSkill != null)
            {
                // Zaten bu yeteneğe sahipse, sadece seviyesini (ProficiencyLevel) güncelleyelim
                existingSkill.ProficiencyLevel = dto.ProficiencyLevel;
            }
            else
            {
                // Yoksa yeni bir ara tablo (Junction Table) kaydı atalım
                var studentSkill = new StudentSkill
                {
                    StudentId = userId,
                    SkillId = dto.SkillId,
                    ProficiencyLevel = dto.ProficiencyLevel
                };
                await _studentRepository.AddStudentSkillAsync(studentSkill);
            }

            await _unitOfWork.CompleteAsync();
            return Result<bool>.Success(true, "Yetenek profilinize başarıyla işlendi.");
        }

        public async Task<Result<bool>> RemoveSkillAsync(int userId, int skillId, CancellationToken cancellationToken = default)
        {
            var existingSkill = await _studentRepository.GetStudentSkillAsync(userId, skillId);
            if (existingSkill == null) return Result<bool>.Failure("Bu yetenek profilinizde zaten mevcut değil.");

            _studentRepository.RemoveStudentSkill(existingSkill);
            await _unitOfWork.CompleteAsync();

            return Result<bool>.Success(true, "Yetenek profilinizden silindi.");
        }
    }
}
