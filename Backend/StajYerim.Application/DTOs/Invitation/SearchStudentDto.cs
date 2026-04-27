namespace StajYerim.Application.DTOs.Invitation
{
    public class SearchStudentDto
    {
        public string? Department { get; set; }
        public int? SkillId { get; set; }
        public decimal? MinGpa { get; set; }
        public int? PostingId { get; set; } // İlan bazlı akıllı eşleşme sıralaması için
    }
}
