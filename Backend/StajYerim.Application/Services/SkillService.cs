using StajYerim.Application.DTOs.Skill;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly IGenericRepository<Skill> _skillRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SkillService(IGenericRepository<Skill> skillRepository, IUnitOfWork unitOfWork)
        {
            _skillRepository = skillRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> CreateSkillAsync(CreateSkillDto dto, CancellationToken cancellationToken = default)
        {
            var existingSkills = await _skillRepository.FindAsync(s => s.Name.ToLower() == dto.Name.ToLower());
            if (existingSkills.Any())
                return Result<int>.Failure("Bu yetenek sistemde zaten mevcut.");

            var skill = new Skill
            {
                Name = dto.Name,
                Category = dto.Category
            };

            await _skillRepository.AddAsync(skill);
            await _unitOfWork.CompleteAsync();

            return Result<int>.Success(skill.Id, "Yetenek sisteme başarıyla eklendi.");
        }

        public async Task<Result<List<SkillDto>>> GetAllSkillsAsync(CancellationToken cancellationToken = default)
        {
            var skills = await _skillRepository.GetAllAsync();
            
            var dtoList = skills
                .OrderBy(s => s.Name)
                .Select(s => new SkillDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category
                }).ToList();

            return Result<List<SkillDto>>.Success(dtoList);
        }
    }
}
