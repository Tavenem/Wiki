using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly SignInManager<WikiUser> _signInManager;

        public ConfirmEmailChangeModel(UserManager<WikiUser> userManager, SignInManager<WikiUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData] public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId is null || email is null || code is null)
            {
                return Redirect("~/");
            }

            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ErrorMessage = "Error changing email.";
                return RedirectToPage("/Error");
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            return Redirect("~/");
        }
    }
}
