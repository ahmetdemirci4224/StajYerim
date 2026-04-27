using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Invitation
{
    public class SendInvitationDto
    {
        [Required(ErrorMessage = "Öğrenci ID'si zorunludur.")]
        public int StudentId { get; set; }

        public int? PostingId { get; set; } // Hangi ilan için davet ediliyor (opsiyonel)

        [Required(ErrorMessage = "Davet mesajı zorunludur.")]
        public string Message { get; set; } = string.Empty;
    }
}
