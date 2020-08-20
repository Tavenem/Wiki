using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Pages.Account
{
    [AllowAnonymous]
    public class SignOutModel : PageModel
    {
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly ILogger<SignOutModel> _logger;

        public SignOutModel(SignInManager<WikiUser> signInManager, ILogger<SignOutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation("User logged out.");
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
