using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MoveTo2FA.Services
{
    public class SendMailParams
    {
        public string MailServer { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string SendAddress { get; set; }
    }

    public class MailSender : IMailSender
    {
        SendMailParams _sendMailParams;

        public MailSender(IOptions<SendMailParams> optionsAccessor)
        {
            _sendMailParams = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        public async Task Execute(string subject, string message, string email)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_sendMailParams.User, _sendMailParams.SendAddress));

            emailMessage.To.Add(new MailboxAddress(email, email));

            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_sendMailParams.MailServer, _sendMailParams.Port, SecureSocketOptions.Auto);
                //await client.AuthenticateAsync(_sendMailParams.User, _sendMailParams.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

    }
}
