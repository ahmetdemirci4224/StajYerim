using StajYerim.Application.DTOs.Student;

namespace StajYerim.Application.DTOs.Invitation
{
    public class StudentSearchResultDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal? Gpa { get; set; }
        public string? Bio { get; set; }
        public double MatchScore { get; set; } // Yapay zeka eşleşme skoru
        public List<StudentSkillDto> Skills { get; set; } = new List<StudentSkillDto>();
    }
}
