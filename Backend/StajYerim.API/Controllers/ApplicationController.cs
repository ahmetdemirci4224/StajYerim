using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Application;
using StajYerim.Application.Interfaces;
using System.Security.Claims;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _applicationService;

        public ApplicationController(IJobApplicationService applicationService)
        {
            _applicationService = applicationService;
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

        // ÖĞRENCİ: Bir ilana başvur
        [HttpPost("apply")]
        [Authorize(Roles = "Student")] // Sadece öğrenciler başvurabilir
        public async Task<IActionResult> Apply([FromBody] ApplyToPostingDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var studentId = GetUserId();
            var result = await _applicationService.ApplyAsync(studentId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        // ÖĞRENCİ: Kendi başvurularımı listele
        [HttpGet("my-applications")]
        [Authorize(Roles = "Student")] // Sadece öğrenciler geçmiş başvurularını görebilir
        public async Task<IActionResult> GetMyApplications(CancellationToken cancellationToken)
        {
            var studentId = GetUserId();
            var result = await _applicationService.GetStudentApplicationsAsync(studentId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }

        // ŞİRKET: Bir başvurunun durumunu güncelle (Kabul Et, Reddet vb.)
        [HttpPut("{applicationId}/status")]
        [Authorize(Roles = "Company")] // Sadece şirketler başvuru onay/ret yapabilir
        public async Task<IActionResult> UpdateStatus(int applicationId, [FromBody] UpdateApplicationStatusDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var companyId = GetUserId();
            var result = await _applicationService.UpdateStatusAsync(companyId, applicationId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);

            return Ok(result);
        }
    }
}
