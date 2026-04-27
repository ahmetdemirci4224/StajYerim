using StajYerim.Application.DTOs.Chatbot;
using StajYerim.Core.Common;

namespace StajYerim.Application.Interfaces
{
    public interface IChatbotService
    {
        Task<Result<ChatbotFilterResultDto>> ExtractFiltersAsync(string userMessage, CancellationToken cancellationToken = default);
        Task<Result<CareerAdviceResponseDto>> GetCareerAdviceAsync(int userId, string userRole, string userMessage, CancellationToken cancellationToken = default);
    }
}
