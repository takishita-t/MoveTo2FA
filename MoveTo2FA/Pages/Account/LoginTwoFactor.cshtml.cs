using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MoveTo2FA.Services;
using System.ComponentModel.DataAnnotations;

namespace MoveTo2FA.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IMailSender _mailSender;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IMailSender mailSender
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _mailSender = mailSender;
            this.EmailMFA = new EmailMFA();
        }

        public async Task OnGetAsync(string email, string rememberMe)
        {
            //Generate the code 
            var user = await userManager.FindByEmailAsync(email);
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

            //Send to the user
            //var emailMessage = new MimeMessage();

            //emailMessage.From.Add(new MailboxAddress("", "test@example.com"));

            //emailMessage.To.Add(new MailboxAddress("test@test.com", "test@test.com"));

            //emailMessage.Subject = "My Web App's OTP";

            //emailMessage.Body = new TextPart("plain") { Text = $"Please use this code as the OTP: {securityCode}" };

            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    await client.ConnectAsync("localhost", 1025, SecureSocketOptions.Auto);
            //    //await client.AuthenticateAsync(_sendMailParams.User, _sendMailParams.Password);
            //    await client.SendAsync(emailMessage);
            //    await client.DisconnectAsync(true);
            //}
            await _mailSender.SendEmailAsync(email, "Confirm your email",
                $"Please use this code as the OTP:{ securityCode}");


        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorSignInAsync("Email",
                this.EmailMFA.SecurityCode,
                this.EmailMFA.RememberMe,
                false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to login.");
                }

                return Page();
            }
        }
    }
    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }
}
