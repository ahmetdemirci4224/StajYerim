namespace StajYerim.Application.DTOs.Invitation
{
    public class InvitationDto
    {
        public int InvitationId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string? PostingTitle { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
