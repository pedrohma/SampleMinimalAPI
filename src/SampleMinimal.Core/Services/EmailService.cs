using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace SampleMinimal.Core.Services
{
    public class EmailService : IEmailService
    {
        private IConfiguration Configuration;
        private readonly ILogger _logger;
        const string FROMEMAIL = "noreply@portj2.com";
        public EmailService(IConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string emailBody)
        {
            string[] email = { toEmail };
            await SendEmailAsync(email, subject, emailBody);
        }

        public async Task SendEmailAsync(string[] toEmail, string subject, string emailBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(FROMEMAIL, FROMEMAIL));
                foreach (var email in toEmail)
                    message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = emailBody
                };
                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    await client.ConnectAsync(Configuration["SMTP:Host"], Int32.Parse(Configuration["SMTP:Port"]), SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(Configuration["SMTP:Username"], Configuration["SMTP:Password"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending email with following error: {ex}");
            }
        }
    }
}
