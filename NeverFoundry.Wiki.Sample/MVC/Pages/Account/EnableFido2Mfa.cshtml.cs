using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages.Account
{
    [Authorize]
    public class EnableFido2MfaModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;

        public string? Email { get; set; }

        public string? ReturnUrl { get; set; }

        public string? UserName { get; set; }

        public EnableFido2MfaModel(UserManager<WikiUser> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./EnableFido2Mfa") });
            }

            Email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            UserName = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);

            return Page();
        }

        public void OnPost() { }
    }
}
