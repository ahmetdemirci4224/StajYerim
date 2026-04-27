using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajYerim.Infrastructure.Data;
using StajYerim.Core.Entities;
using StajYerim.Application.DTOs.Company;
using StajYerim.Application.Services;

namespace StajYerim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly StajYerimDbContext _context;

        public JobPostingController(StajYerimDbContext context)
        {
            _context = context;
        }

        // Herkes aktif ilanları görebilir
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPostings()
        {
            var postings = await _context.InternshipPostings
                .Where(p => p.Status == "Open")
                .Include(p => p.Company)
                    .ThenInclude(c => c.User)
                .Include(p => p.PostingSkills)
                    .ThenInclude(ps => ps.Skill)
                .Select(p => new
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    WorkModel = p.WorkModel,
                    Status = p.Status,
                    Company = p.Company.User.FullName,
                    Skills = p.PostingSkills.Select(ps => ps.Skill.Name).ToList()
                })
                .ToListAsync();

            return Ok(new { isSuccess = true, data = postings });
        }

        [Authorize(Roles = "Student")]
        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedPostings()
        {
            var studentUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(studentUserIdString, out var studentUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var studentProfile = await _context.StudentProfiles
                .Include(sp => sp.StudentSkills)
                    .ThenInclude(ss => ss.Skill)
                .FirstOrDefaultAsync(sp => sp.UserId == studentUserId);

            if (studentProfile == null)
            {
                return BadRequest(new { message = "Öğrenci profili bulunamadı. Lütfen önce profilinizi tamamlayın." });
            }

            var postings = await _context.InternshipPostings
                .Where(p => p.Status == "Open")
                .Include(p => p.Company)
                    .ThenInclude(c => c.User)
                .Include(p => p.PostingSkills)
                    .ThenInclude(ps => ps.Skill)
                .ToListAsync();

            var recommendedList = postings.Select(p => new
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                WorkModel = p.WorkModel,
                Status = p.Status,
                Company = p.Company?.User?.FullName ?? "Bilinmeyen Şirket",
                Skills = p.PostingSkills.Select(ps => ps.Skill.Name).ToList(),
                MatchScore = MatchCalculator.CalculateMatchScore(p, studentProfile)
            })
            .OrderByDescending(p => p.MatchScore)
            .ToList();

            return Ok(new { isSuccess = true, data = recommendedList });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostingById(int id)
        {
            var p = await _context.InternshipPostings
                .Include(x => x.Company)
                    .ThenInclude(c => c.User)
                .Include(x => x.PostingSkills)
                    .ThenInclude(ps => ps.Skill)
                .Include(x => x.Applications)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null)
                return NotFound(new { message = "İlan bulunamadı." });

            bool hasApplied = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdString, out var userId))
                {
                    hasApplied = p.Applications.Any(a => a.StudentId == userId);
                }
            }

            var result = new
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                WorkModel = p.WorkModel,
                Status = p.Status,
                Company = p.Company?.User?.FullName ?? "Bilinmeyen Şirket",
                Skills = p.PostingSkills.Select(ps => ps.Skill.Name).ToList(),
                ExpiresAt = p.ExpiresAt,
                HasApplied = hasApplied
            };

            return Ok(new { isSuccess = true, data = result });
        }

        // Sadece 'Company' rolüne sahip, token'ı doğrulanmış kullanıcılar erişebilir
        [Authorize(Roles = "Company")]
        [HttpGet("my-postings")]
        public async Task<IActionResult> GetMyPostings()
        {
            var companyUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(companyUserIdString, out var companyUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var postings = await _context.InternshipPostings
                .Include(p => p.Applications)
                    .ThenInclude(a => a.Student)
                        .ThenInclude(s => s.User)
                .Where(p => p.CompanyId == companyUserId)
                .Select(p => new
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    WorkModel = p.WorkModel,
                    Status = p.Status,
                    CreatedAt = p.CreatedDate,
                    ApplicantCount = p.Applications.Count,
                    Applicants = p.Applications.Select(a => new {
                        ApplicationId = a.Id,
                        StudentName = a.Student != null && a.Student.User != null ? a.Student.User.FullName : "Bilinmeyen Aday",
                        Status = a.Status,
                        AppliedAt = a.AppliedAt
                    }).ToList()
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(new { isSuccess = true, data = postings });
        }

        [Authorize(Roles = "Company")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateJobPosting([FromBody] CreatePostingDto dto)
        {
            var companyUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(companyUserIdString, out var companyUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var companyProfile = await _context.CompanyProfiles.FirstOrDefaultAsync(c => c.UserId == companyUserId);
            if (companyProfile == null)
            {
                var user = await _context.Users.FindAsync(companyUserId);
                companyProfile = new CompanyProfile
                {
                    UserId = companyUserId,
                    CompanyName = user?.FullName ?? "Bilinmeyen Şirket"
                };
                _context.CompanyProfiles.Add(companyProfile);
                await _context.SaveChangesAsync();
            }

            var posting = new InternshipPosting
            {
                CompanyId = companyProfile.UserId,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                WorkModel = dto.WorkModel,
                Status = "Open",
                ExpiresAt = dto.ExpiresAt ?? DateTime.UtcNow.AddMonths(1)
            };

            _context.InternshipPostings.Add(posting);
            await _context.SaveChangesAsync();

            return Ok(new { 
                isSuccess = true,
                message = "Yeni staj ilanı başarıyla oluşturuldu.", 
                data = new { postingId = posting.Id }
            });
        }
        
        [Authorize(Roles = "Company")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateJobPosting(int id, [FromBody] CreatePostingDto dto)
        {
            var companyUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(companyUserIdString, out var companyUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var posting = await _context.InternshipPostings.FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyUserId);
            if (posting == null)
                return NotFound(new { message = "İlan bulunamadı veya yetkiniz yok." });

            posting.Title = dto.Title;
            posting.Description = dto.Description;
            posting.Location = dto.Location;
            posting.WorkModel = dto.WorkModel;
            if (dto.ExpiresAt.HasValue) posting.ExpiresAt = dto.ExpiresAt.Value;

            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "İlan başarıyla güncellendi." });
        }

        [Authorize(Roles = "Company")]
        [HttpPut("close/{id}")]
        public async Task<IActionResult> CloseJobPosting(int id)
        {
            var companyUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(companyUserIdString, out var companyUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var posting = await _context.InternshipPostings.FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyUserId);
            if (posting == null)
                return NotFound(new { message = "İlan bulunamadı veya yetkiniz yok." });

            posting.Status = "Closed";
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "İlan başarıyla yayından kaldırıldı." });
        }
        
        // Sadece 'Student' rolüne sahip, token'ı doğrulanmış kullanıcılar erişebilir
        [Authorize(Roles = "Student")]
        [HttpPost("apply/{id}")]
        public async Task<IActionResult> ApplyToJobPosting(int id)
        {
            var studentUserIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(studentUserIdString, out var studentUserId))
                return Unauthorized(new { message = "Geçersiz token." });

            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == studentUserId);
            if (studentProfile == null)
                return BadRequest(new { message = "Profiliniz eksik. İlanlara başvurabilmek için önce profilinizi tamamlayın." });

            var posting = await _context.InternshipPostings.FindAsync(id);
            if (posting == null || posting.Status != "Open")
                return BadRequest(new { message = "İlan bulunamadı veya başvuruya kapalı." });

            var existingApplication = await _context.Applications.FirstOrDefaultAsync(a => a.PostingId == id && a.StudentId == studentProfile.UserId);
            if (existingApplication != null)
                return BadRequest(new { message = "Bu ilana zaten başvurdunuz." });

            var application = new JobApplication
            {
                PostingId = id,
                StudentId = studentProfile.UserId,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            
            return Ok(new {
                isSuccess = true,
                message = "İlana başarıyla başvuruldu."
            });
        }
    }
}
