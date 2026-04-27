using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Chatbot;
using StajYerim.Application.Interfaces;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Her iki rol de kullanabilir
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        private (int UserId, string Role) GetUserInfo()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(idClaim) || string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Geçersiz kullanıcı bilgisi.");

            return (int.Parse(idClaim), roleClaim);
        }

        // Doğal dil ile ilan arama asistanı (Sadece öğrenci)
        [HttpPost("parse")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ParseNaturalLanguage([FromBody] ChatbotRequestDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _chatbotService.ExtractFiltersAsync(dto.UserMessage, cancellationToken);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // Kariyer Danışmanlığı ve Akıllı Arama Chatbot'u (Öğrenci ve Şirket)
        [HttpPost("advice")]
        public async Task<IActionResult> GetCareerAdvice([FromBody] CareerAdviceRequestDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var (userId, role) = GetUserInfo();
                var result = await _chatbotService.GetCareerAdviceAsync(userId, role, dto.UserMessage, cancellationToken);

                if (!result.IsSuccess) return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }}
