using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages.Account
{
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;

        public string? ConfirmationUrl { get; set; }

        public bool DisplayLink { get; set; }

        public ForgotPasswordConfirmation(UserManager<WikiUser> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string? email = null)
        {
            // TODO: replace with empty OnGet() when email service is configured
            if (email is null)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayLink = true;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            ConfirmationUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { code },
                protocol: Request.Scheme);

            return Page();
        }
    }
}
