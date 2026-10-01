using Metro.Domain.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Metro.Infrastructure.Email.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:Email"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = html
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["Email:Host"], int.Parse(_configuration["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
            await smtp.AuthenticateAsync(_configuration["Email:User"], _configuration["Email:Pass"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
