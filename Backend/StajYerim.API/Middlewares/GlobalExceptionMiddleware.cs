using System.Net;
using System.Text.Json;

namespace StajYerim.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sunucu tarafında beklenmedik bir hata oluştu.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "İşleminiz gerçekleştirilirken bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                Detailed = exception.ToString()
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
