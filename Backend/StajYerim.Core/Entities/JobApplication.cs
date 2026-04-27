using StajYerim.Core.Common;

namespace StajYerim.Core.Entities
{
    public class JobApplication : BaseEntity
    {
        public int StudentId { get; set; }
        public StudentProfile Student { get; set; } = null!;

        public int PostingId { get; set; }
        public InternshipPosting Posting { get; set; } = null!;

        public string? CoverLetter { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, InReview, Accepted, Rejected
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
