namespace StajYerim.Core.Entities
{
    public class StudentProfile
    {
        // Primary Key & Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string University { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int? GraduationYear { get; set; }
        public decimal? Gpa { get; set; }
        public string? Bio { get; set; }
        public string? GithubUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        // Eklentiler: Çift Taraflı Eşleştirme (Two-Way Matching) İçin Beklentiler
        public string? PreferredLocation { get; set; }
        public string? PreferredWorkModel { get; set; } // "Remote", "Hybrid", "On-Site"
        public string? InterestedRoles { get; set; }
        
        // Navigation Properties
        public ICollection<StudentSkill> StudentSkills { get; set; } = new List<StudentSkill>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }
}
