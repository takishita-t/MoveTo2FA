using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using MoveTo2FA.Services;

namespace MoveTo2FA.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMailSender _mailSender;

        public RegisterModel(UserManager<IdentityUser> userManager, IMailSender mailSender)
        {
            this.userManager = userManager;
            _mailSender = mailSender;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            //validateing Email address

            //Create the userÅ@ìoò^éûÇÃê›íË
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                TwoFactorEnabled = true
            };

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken });
                //send mail
                //var emailMessage = new MimeMessage();

                //emailMessage.From.Add(new MailboxAddress("", "test@example.com"));

                //emailMessage.To.Add(new MailboxAddress("test@test.com", "test@test.com"));

                //emailMessage.Subject = "Confirm your email";

                //emailMessage.Body = new TextPart("plain") { Text = $"Confirm your email{confirmationLink}"};

                //using (var client = new MailKit.Net.Smtp.SmtpClient())
                //{
                //    await client.ConnectAsync("localhost", 1025, SecureSocketOptions.Auto);
                //    //await client.AuthenticateAsync(_sendMailParams.User, _sendMailParams.Password);
                //    await client.SendAsync(emailMessage);
                //    await client.DisconnectAsync(true);
                //}
                await _mailSender.SendEmailAsync(RegisterViewModel.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{confirmationLink}");


                return RedirectToPage("/Account/Login");
                //return Page();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invaild email address.")]
        public string Email { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }
    }
}
