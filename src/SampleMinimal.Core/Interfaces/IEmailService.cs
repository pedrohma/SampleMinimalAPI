namespace SampleMinimal.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendEmailAsync(string[] toEmail, string subject, string body);
    }
}
