namespace StajYerim.Application.DTOs.Company
{
    public class PostingApplicantDto
    {
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal? Gpa { get; set; }
        public string? CoverLetter { get; set; }
        public string ApplicationStatus { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public double MatchScore { get; set; } // Yapay zeka eşleşme skoru
        
        // İsteğe bağlı: Başvuranın yeteneklerini (ve seviyelerini) de listelemek,
        // şirketin değerlendirmesinde büyük kolaylık sağlar.
        public List<ApplicantSkillDto> Skills { get; set; } = new List<ApplicantSkillDto>();
    }

    public class ApplicantSkillDto
    {
        public string SkillName { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; }
    }
}
