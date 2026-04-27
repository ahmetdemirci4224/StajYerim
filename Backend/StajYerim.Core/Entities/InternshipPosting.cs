using StajYerim.Core.Common;

namespace StajYerim.Core.Entities
{
    public class InternshipPosting : BaseEntity
    {
        public int CompanyId { get; set; }
        public CompanyProfile Company { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? WorkModel { get; set; } // Remote, Hybrid, On-Site
        public string Status { get; set; } = "Open"; // Open, Closed, Draft
        public DateTime? ExpiresAt { get; set; }

        // Navigation Properties
        public ICollection<PostingSkill> PostingSkills { get; set; } = new List<PostingSkill>();
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    }
}
