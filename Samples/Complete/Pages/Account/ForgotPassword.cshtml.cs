using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using NeverFoundry.Wiki.Samples.Complete.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly IEmailService _emailService;

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public ForgotPasswordModel(UserManager<WikiUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public class InputModel
        {
            [Required, EmailAddress] public string? Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email).ConfigureAwait(false);
            if (user is null || !await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { code },
                protocol: Request.Scheme);

            var email = EmailTemplates.BuildForgotPasswordEmail(callbackUrl);
            email.ToAddresses.Add(new EmailAddress(Input.Email!));
            await _emailService.SendEmailAsync(email).ConfigureAwait(false);

            // TODO: remove email parameter when email service is configured
            return RedirectToPage("./ForgotPasswordConfirmation", new { email = Input.Email });
        }
    }
}
