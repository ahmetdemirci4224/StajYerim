namespace StajYerim.Application.DTOs.Chatbot
{
    public class CareerAdviceResponseDto
    {
        public string AssistantReply { get; set; } = string.Empty;
        public List<ChatbotMatchItem>? Matches { get; set; }
    }

    public class ChatbotMatchItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Posting" or "Student"
    }
}
