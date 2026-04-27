using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Invitation
{
    public class RespondInvitationDto
    {
        [Required]
        [RegularExpression("Accepted|Declined", ErrorMessage = "Statü 'Accepted' veya 'Declined' olmalıdır.")]
        public string Status { get; set; } = string.Empty;
    }
}
