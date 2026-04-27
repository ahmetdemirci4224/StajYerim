using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Skill;
using StajYerim.Application.Interfaces;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        // HERKES: Sistemdeki tüm yetenekleri listele
        [HttpGet]
        public async Task<IActionResult> GetAllSkills(CancellationToken cancellationToken)
        {
            var result = await _skillService.GetAllSkillsAsync(cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        // ŞİRKETLER ve ADMİNLER: Sisteme yeni bir yetenek ekle
        [HttpPost]
        [Authorize(Roles = "Admin,Company")] 
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _skillService.CreateSkillAsync(dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }
    }
}
