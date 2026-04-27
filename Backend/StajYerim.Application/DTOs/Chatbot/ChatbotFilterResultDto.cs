namespace StajYerim.Application.DTOs.Chatbot
{
    public class ChatbotFilterResultDto
    {
        public string? Location { get; set; }
        public string? Role { get; set; }
        public string? WorkModel { get; set; } // Remote, Hybrid, On-Site
        public List<string> Skills { get; set; } = new List<string>();
        
        // Bu property, bulunan filtrelere göre kaç tane sonuç eşleştiğini tutmak için eklenebilir.
        // public int TotalMatchedPostings { get; set; } 
    }
}
