namespace StajYerim.Application.DTOs.Application
{
    public class StudentApplicationDto
    {
        public int ApplicationId { get; set; }
        public int PostingId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string PostingTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
    }
}
