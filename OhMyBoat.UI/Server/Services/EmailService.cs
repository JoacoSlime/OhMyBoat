using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using OhMyBoat.UI.Server.Helpers;

namespace OhMyBoat.UI.Server.Services
{
    public interface IEmailService
    {
        Task Send(string to, string subject, string html, string? from = null);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _EmailSettings;

        public EmailService(IOptions<EmailSettings> EmailSettings)
        {
            _EmailSettings = EmailSettings.Value;
        }

        public async Task Send(string to, string subject, string html, string? from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _EmailSettings.EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_EmailSettings.SmtpHost, _EmailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_EmailSettings.SmtpUser, _EmailSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
}
}