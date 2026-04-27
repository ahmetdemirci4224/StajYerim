using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Student
{
    public class UpdateStudentProfileDto
    {
        [Required(ErrorMessage = "Üniversite adı zorunludur.")]
        public string University { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bölüm adı zorunludur.")]
        public string Department { get; set; } = string.Empty;
        
        public int? GraduationYear { get; set; }
        
        [Range(0.0, 4.0, ErrorMessage = "Not ortalaması 0.0 ile 4.0 arasında olmalıdır.")]
        public decimal? Gpa { get; set; }
        
        public string? Bio { get; set; }
        public string? GithubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        
        public string? PreferredLocation { get; set; }
        public string? PreferredWorkModel { get; set; }
        public string? InterestedRoles { get; set; }
    }
}
