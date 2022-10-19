
namespace MoveTo2FA.Services
{
    public interface IMailSender
    {
        Task Execute(string subject, string message, string email);
        Task SendEmailAsync(string email, string subject, string message);
    }
}