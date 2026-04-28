using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajYerim.Application.DTOs.Student;
using StajYerim.Application.Interfaces;
using System.Security.Claims;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Yardımcı Metot: Token'ın içinden UserID (Sub) değerini okur
        private int GetUserId()
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimValue))
            {
                throw new UnauthorizedAccessException("Token geçersiz veya UserID (Sub) bulunamadı.");
            }
            return int.Parse(claimValue);
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var userId = GetUserId(); // Kendi User Id'sini buluyor (JWT üzerinden, güvenli!)
            var result = await _studentService.GetProfileAsync(userId, cancellationToken);
            
            if (!result.IsSuccess)
                return BadRequest(result); // Result modelimizin içeriğiyle dönecek

            return Ok(result);
        }

        [HttpGet("{id}/profile")]
        [Authorize(Roles = "Company,Student")]
        public async Task<IActionResult> GetStudentProfileById(int id, CancellationToken cancellationToken)
        {
            var result = await _studentService.GetProfileAsync(id, cancellationToken);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("profile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpsertProfile([FromBody] UpdateStudentProfileDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId(); // Başkasının profilini güncelleyemez, sadece kendi JWT'sinden çıkan id'yi kullanır
            var result = await _studentService.UpsertProfileAsync(userId, dto, cancellationToken);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("skills")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AddSkill([FromBody] AddStudentSkillDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var result = await _studentService.AddSkillAsync(userId, dto, cancellationToken);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("skills/{skillId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveSkill(int skillId, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _studentService.RemoveSkillAsync(userId, skillId, cancellationToken);
            
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
