using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Application
{
    public class UpdateApplicationStatusDto
    {
        [Required(ErrorMessage = "Statü bilgisi zorunludur.")]
        [RegularExpression("Pending|InReview|Accepted|Rejected", ErrorMessage = "Statü 'Pending', 'InReview', 'Accepted' veya 'Rejected' olmalıdır.")]
        public string Status { get; set; } = string.Empty;
    }
}
