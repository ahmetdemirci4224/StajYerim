using StajYerim.Core.Common;

namespace StajYerim.Core.Entities
{
    public class Invitation : BaseEntity
    {
        public int CompanyId { get; set; }
        public CompanyProfile Company { get; set; } = null!;

        public int StudentId { get; set; }
        public StudentProfile Student { get; set; } = null!;

        public int? PostingId { get; set; }
        public InternshipPosting? Posting { get; set; }

        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
