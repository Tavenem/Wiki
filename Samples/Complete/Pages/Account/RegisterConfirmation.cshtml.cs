using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;

        public string? ConfirmationUrl { get; set; }

        public bool DisplayLink { get; set; }

        public RegisterConfirmationModel(UserManager<WikiUser> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string email)
        {
            // TODO: replace with empty OnGet() when email service is configured
            if (email is null)
            {
                return Redirect("~/");
            }

            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user is null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            DisplayLink = true;

            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            ConfirmationUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId, code },
                protocol: Request.Scheme);

            return Page();
        }
    }
}
