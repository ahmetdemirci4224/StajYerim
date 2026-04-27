using System.ComponentModel.DataAnnotations;

namespace StajYerim.Application.DTOs.Chatbot
{
    public class ChatbotRequestDto
    {
        [Required(ErrorMessage = "Lütfen ne aradığınızı doğal dille ifade edin.")]
        public string UserMessage { get; set; } = string.Empty;
    }
}
