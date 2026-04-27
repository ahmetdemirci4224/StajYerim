namespace StajYerim.Core.Entities
{
    public class CompanyProfile
    {
        // Primary Key & Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string CompanyName { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public int? FoundedYear { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public ICollection<InternshipPosting> Postings { get; set; } = new List<InternshipPosting>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }
}
