using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Invitation;
using StajYerim.Application.Interfaces;
using System.Security.Claims;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
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

        // ŞİRKET: Öğrencileri filtreleyerek ara (Eşleşme algoritmasının temel adımı)
        [HttpGet("search-students")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> SearchStudents([FromQuery] SearchStudentDto dto, CancellationToken cancellationToken)
        {
            var companyId = GetUserId();
            var result = await _invitationService.SearchStudentsAsync(companyId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ŞİRKET: Seçtiği öğrenciye davet gönder
        [HttpPost("send")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> SendInvitation([FromBody] SendInvitationDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var companyId = GetUserId();
            var result = await _invitationService.SendInvitationAsync(companyId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ŞİRKET: Kendi gönderdiği davetleri listele
        [HttpGet("company-invitations")]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> GetCompanyInvitations(CancellationToken cancellationToken)
        {
            var companyId = GetUserId();
            var result = await _invitationService.GetCompanyInvitationsAsync(companyId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ÖĞRENCİ: Kendisine gelen davetleri listele
        [HttpGet("student-invitations")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentInvitations(CancellationToken cancellationToken)
        {
            var studentId = GetUserId();
            var result = await _invitationService.GetStudentInvitationsAsync(studentId, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ÖĞRENCİ: Daveti kabul veya red et
        [HttpPut("{invitationId}/respond")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RespondToInvitation(int invitationId, [FromBody] RespondInvitationDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var studentId = GetUserId();
            var result = await _invitationService.RespondToInvitationAsync(studentId, invitationId, dto, cancellationToken);
            
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }
    }
}
