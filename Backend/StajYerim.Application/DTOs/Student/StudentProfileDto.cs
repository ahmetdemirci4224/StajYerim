namespace StajYerim.Application.DTOs.Student
{
    public class StudentProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int? GraduationYear { get; set; }
        public decimal? Gpa { get; set; }
        public string? Bio { get; set; }
        public string? GithubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        
        public string? PreferredLocation { get; set; }
        public string? PreferredWorkModel { get; set; }
        public string? InterestedRoles { get; set; }
        
        // Eşleşme sistemi için Öğrencinin Yetenekleri listelenir
        public List<StudentSkillDto> Skills { get; set; } = new List<StudentSkillDto>();
    }

    public class StudentSkillDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; }
    }
}
