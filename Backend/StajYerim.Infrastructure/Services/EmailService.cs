using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using StajYerim.Application.Interfaces;

namespace StajYerim.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:FromEmail"] ?? "noreply@stajyerim.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                // DEMO AMAÇLI: Gönderilen mailleri konsola ve dosyaya yazıyoruz (Gerçek SMTP sunucusu olmadığı için)
                var demoEmailPath = Path.Combine(Directory.GetCurrentDirectory(), "sent_emails.log");
                var logMessage = $"[{DateTime.Now}] TO: {toEmail} | SUBJECT: {subject} | BODY: {body}\n";
                await File.AppendAllTextAsync(demoEmailPath, logMessage);
                Console.WriteLine(logMessage);

                // Fake SMTP host for demo/development if not configured
                var host = _configuration["EmailSettings:Host"] ?? "smtp.ethereal.email";
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var user = _configuration["EmailSettings:Username"] ?? "demo";
                var pass = _configuration["EmailSettings:Password"] ?? "demo";

                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                
                // Only authenticate if real credentials exist, for local testing we might skip or use fake.
                if (user != "demo")
                {
                    await smtp.AuthenticateAsync(user, pass);
                }
                
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E-posta gönderilirken hata oluştu: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
