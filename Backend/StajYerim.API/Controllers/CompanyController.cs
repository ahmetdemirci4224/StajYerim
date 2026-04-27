using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Company;
using StajYerim.Application.Interfaces;
using System.Security.Claims;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Company")] // Sadece kimliği doğrulanmış "Şirket" rolündeki JWT'ler
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        private int GetUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimValue))
            {
                throw new UnauthorizedAccessException("Token geçersiz.");
            }
            return int.Parse(claimValue);
        }

        [HttpPost("postings")]
        public async Task<IActionResult> CreatePosting([FromBody] CreatePostingDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var companyId = GetUserId();
            var result = await _companyService.CreatePostingAsync(companyId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result); // İlanın ID'sini de Result.Data içinde döner
        }

        [HttpPost("postings/{postingId}/skills")]
        public async Task<IActionResult> AddPostingSkill(int postingId, [FromBody] AddPostingSkillDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var companyId = GetUserId();
            var result = await _companyService.AddSkillToPostingAsync(companyId, postingId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("postings/{postingId}/applicants")]
        public async Task<IActionResult> GetApplicants(int postingId, CancellationToken cancellationToken)
        {
            var companyId = GetUserId();
            var result = await _companyService.GetApplicantsAsync(companyId, postingId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }
    }
}
