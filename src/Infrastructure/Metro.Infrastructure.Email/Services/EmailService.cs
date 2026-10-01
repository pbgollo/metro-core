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
            var from = GetRequired("Email:Email");
            var host = GetRequired("Email:Host");
            var port = int.Parse(GetRequired("Email:Port"));
            var user = GetRequired("Email:User");
            var pass = GetRequired("Email:Pass");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = html
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
            await smtp.AuthenticateAsync(user, pass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string GetRequired(string key)
            => _configuration[key]
                ?? throw new InvalidOperationException($"{key} is not configured.");
    }
}
