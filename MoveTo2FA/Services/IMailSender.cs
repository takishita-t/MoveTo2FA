
namespace MoveTo2FA.Services
{
    public interface IMailSender
    {
        Task Execute(string subject, string message, string email);

        //Send Email
        Task SendEmailAsync(string email, string subject, string message);
    }
}