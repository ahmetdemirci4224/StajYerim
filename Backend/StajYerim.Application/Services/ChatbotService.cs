using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using StajYerim.Application.DTOs.Chatbot;
using StajYerim.Application.Interfaces;
using StajYerim.Core.Common;
using StajYerim.Core.Entities;
using StajYerim.Core.Interfaces;

namespace StajYerim.Application.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IJobPostingRepository _postingRepository;
        private readonly IStudentRepository _studentRepository;

        public ChatbotService(
            HttpClient httpClient, 
            IConfiguration configuration,
            IJobPostingRepository postingRepository,
            IStudentRepository studentRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _postingRepository = postingRepository;
            _studentRepository = studentRepository;
        }

        public async Task<Result<ChatbotFilterResultDto>> ExtractFiltersAsync(string userMessage, CancellationToken cancellationToken = default)
        {
            // ... (keep current implementation or update if needed, but the focus is on advice/smart matching now)
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return Result<ChatbotFilterResultDto>.Failure("Mesaj boş olamaz.");
            }

            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return FallbackRuleBasedParser(userMessage);
            }

            try
            {
                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var prompt = $@"
Aşağıdaki kullanıcı mesajını analiz et ve staj arama filtreleri çıkar. 
Sadece geçerli bir JSON nesnesi dön, başka hiçbir açıklama yapma. Markdown formatlaması (```json vs) kullanma.

Kullanıcı Mesajı: '{userMessage}'

Çıkarmak İstediğim JSON Formatı:
{{
  ""Location"": ""Şehir adı (Örn: Ankara) veya null"",
  ""Role"": ""İş rolü (Örn: Frontend, Backend, Mobil) veya null"",
  ""WorkModel"": ""Sadece şu 3 değerden biri olmalı: Remote, Hybrid, On-Site veya null"",
  ""Skills"": [""C#"", ""React"", ""Java"", ...] (bulamazsan boş dizi)
}}
";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    },
                    generationConfig = new { temperature = 0.1 }
                };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, requestBody, cancellationToken);
                if (!response.IsSuccessStatusCode) return FallbackRuleBasedParser(userMessage);

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                using var jsonDoc = JsonDocument.Parse(responseContent);
                var root = jsonDoc.RootElement;
                var candidates = root.GetProperty("candidates");
                if (candidates.GetArrayLength() > 0)
                {
                    var textResponse = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                    if (!string.IsNullOrEmpty(textResponse))
                    {
                        var cleanJson = textResponse.Trim().Trim('`').Replace("json\n", "");
                        var result = JsonSerializer.Deserialize<ChatbotFilterResultDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (result != null) return Result<ChatbotFilterResultDto>.Success(result, "Analiz edildi.");
                    }
                }
                return FallbackRuleBasedParser(userMessage);
            }
            catch { return FallbackRuleBasedParser(userMessage); }
        }

        private Result<ChatbotFilterResultDto> FallbackRuleBasedParser(string userMessage)
        {
            var result = new ChatbotFilterResultDto();
            string normalizedInput = userMessage.ToLowerInvariant();
            var cities = new[] { "ankara", "istanbul", "izmir", "konya", "antalya", "bursa" };
            var roles = new[] { "backend", "frontend", "fullstack", "mobil", "ios", "android", "veri" };
            var skills = new[] { "c#", ".net", "react", "java", "python", "javascript", "sql", "docker" };
            foreach (var city in cities) { if (normalizedInput.Contains(city)) { result.Location = char.ToUpper(city[0]) + city.Substring(1); break; } }
            foreach (var role in roles) { if (normalizedInput.Contains(role)) { result.Role = char.ToUpper(role[0]) + role.Substring(1); break; } }
            foreach (var skill in skills) { if (normalizedInput.Contains(skill)) { result.Skills.Add(skill); } }
            if (normalizedInput.Contains("uzaktan") || normalizedInput.Contains("remote")) result.WorkModel = "Remote";
            else if (normalizedInput.Contains("hibrit")) result.WorkModel = "Hybrid";
            else if (normalizedInput.Contains("ofisten")) result.WorkModel = "On-Site";
            return Result<ChatbotFilterResultDto>.Success(result, "Analiz tamamlandı.");
        }

        public async Task<Result<CareerAdviceResponseDto>> GetCareerAdviceAsync(int userId, string userRole, string userMessage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage)) return Result<CareerAdviceResponseDto>.Failure("Mesaj boş olamaz.");

            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
            {
                return Result<CareerAdviceResponseDto>.Success(new CareerAdviceResponseDto { AssistantReply = "AI Servisi şu an kapalı." });
            }

            try
            {
                // 1. Intent Analysis using Gemini
                var intentPrompt = $@"
Kullanıcı rolü: {userRole}
Kullanıcı Mesajı: '{userMessage}'

Görevin: Kullanıcının niyetini anla. 
Eğer kullanıcı bir arama yapıyorsa (staj arıyorum, aday arıyorum, backend ilanları var mı, frontend bilen öğrenci lazım vb.), ""SEARCH"" dön.
Eğer kullanıcı genel bir soru soruyorsa veya sohbet ediyorsa, ""ADVICE"" dön.
Sadece bu iki kelimeden birini dön.
";
                var intentResponse = await CallGeminiAsync(intentPrompt, 0.1, cancellationToken);
                bool isSearch = intentResponse.Contains("SEARCH", StringComparison.OrdinalIgnoreCase);

                if (isSearch)
                {
                    // 2. Extract Keywords for Search
                    var filterResult = await ExtractFiltersAsync(userMessage, cancellationToken);
                    var filters = filterResult.Data;

                    var responseDto = new CareerAdviceResponseDto();
                    responseDto.Matches = new List<ChatbotMatchItem>();

                    if (userRole == "Student")
                    {
                        var matches = await _postingRepository.SearchPostingsForChatbotAsync(filters?.Role, filters?.Location, cancellationToken);
                        
                        foreach (var m in matches)
                        {
                            responseDto.Matches.Add(new ChatbotMatchItem {
                                Id = m.Id.ToString(),
                                Title = m.Title,
                                Subtitle = m.Company?.User?.FullName ?? "Şirket Bilgisi Yok",
                                Type = "Posting"
                            });
                        }

                        responseDto.AssistantReply = matches.Any() 
                            ? $"Senin için kriterlerine en uygun {matches.Count} ilanı buldum. İncelemek için üzerine tıklayabilirsin!" 
                            : "Aradığın kriterlere uygun şu an aktif bir ilan bulamadım, ama aramaya devam edebilirsin.";
                    }
                    else if (userRole == "Company")
                    {
                        var matches = await _studentRepository.SearchStudentsForChatbotAsync(filters?.Role, filters?.Skills, cancellationToken);

                        foreach (var m in matches)
                        {
                            responseDto.Matches.Add(new ChatbotMatchItem {
                                Id = m.UserId.ToString(),
                                Title = m.User?.FullName ?? "İsimsiz Aday",
                                Subtitle = $"{m.University} - {m.Department}",
                                Type = "Student"
                            });
                        }

                        responseDto.AssistantReply = matches.Any() 
                            ? $"Havuzumuzda kriterlerine uyan {matches.Count} yetenekli aday buldum. Profillerini inceleyebilir ve davet gönderebilirsin." 
                            : "Şu an kriterlerine tam uyan bir aday bulamadım.";
                    }

                    return Result<CareerAdviceResponseDto>.Success(responseDto);
                }
                else
                {
                    // 3. Normal Advice Logic
                    var advicePrompt = $@"
Sen StajYerim platformunun Kariyer Asistanısın. 
Kullanıcı Rolü: {userRole}
Kullanıcı Mesajı: '{userMessage}'

Kısa, öz ve samimi bir cevap ver.
";
                    var reply = await CallGeminiAsync(advicePrompt, 0.7, cancellationToken);
                    return Result<CareerAdviceResponseDto>.Success(new CareerAdviceResponseDto { AssistantReply = reply });
                }
            }
            catch (Exception)
            {
                return Result<CareerAdviceResponseDto>.Failure("İşlem sırasında bir hata oluştu.");
            }
        }

        private async Task<string> CallGeminiAsync(string prompt, double temperature, CancellationToken cancellationToken)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var requestBody = new {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { temperature, maxOutputTokens = 500 }
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, requestBody, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Gemini Error: {errorContent}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return "Yapay zeka asistanı şu an aşırı yoğunluk nedeniyle geçici olarak hizmet veremiyor. Lütfen daha sonra tekrar dene.";
                }
                
                return $"Üzgünüm, şu an bağlantı kuramıyorum. Detay: {errorContent}";
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
        }
    }
}