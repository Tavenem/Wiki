using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;

        [TempData] public string? ErrorMessage { get; set; }

        public ConfirmEmailModel(UserManager<WikiUser> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId is null || code is null)
            {
                return Redirect("~/");
            }

            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Redirect("~/");
            }

            ErrorMessage = "Error confirming your email.";
            return RedirectToPage("/Error");
        }
    }
}
