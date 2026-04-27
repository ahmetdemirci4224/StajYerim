using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Application
{
    public class ApplyToPostingDto
    {
        [Required(ErrorMessage = "İlan ID'si zorunludur.")]
        public int PostingId { get; set; }

        public string? CoverLetter { get; set; }
    }
}
