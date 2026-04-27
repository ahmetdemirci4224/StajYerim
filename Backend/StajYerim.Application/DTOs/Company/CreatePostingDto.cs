using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Company
{
    public class CreatePostingDto
    {
        [Required(ErrorMessage = "İlan başlığı zorunludur.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlan açıklaması zorunludur.")]
        public string Description { get; set; } = string.Empty;

        public string? Location { get; set; }
        
        [RegularExpression("Remote|Hybrid|On-Site", ErrorMessage = "Çalışma modeli 'Remote', 'Hybrid' veya 'On-Site' olmalıdır.")]
        public string? WorkModel { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
    }
}
