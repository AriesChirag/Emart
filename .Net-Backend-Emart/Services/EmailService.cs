using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Emart_DotNet.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPdfAsync(string toEmail, string subject, string body, byte[] pdfBytes, string fileName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _host = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
            _port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            _username = _configuration["EmailSettings:Username"];
            _password = _configuration["EmailSettings:Password"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_host, _port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_username, _password);
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendPdfAsync(string toEmail, string subject, string body, byte[] pdfBytes, string fileName)
        {
            using (var client = new SmtpClient(_host, _port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_username, _password);
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    mailMessage.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), fileName, "application/pdf"));
                }

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
