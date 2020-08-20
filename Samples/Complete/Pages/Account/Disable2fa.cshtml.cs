using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Pages.Account
{
    [Authorize]
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        [TempData] public string? ErrorMessage { get; set; }

        [TempData] public string? StatusMessage { get; set; }

        public Disable2faModel(
            UserManager<WikiUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { ReturnUrl = Url.Page("./Disable2fa") });
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false))
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { ReturnUrl = Url.Page("./Disable2fa") });
            }

            var userId = _userManager.GetUserId(User);
            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
            _ = await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            if (!disable2faResult.Succeeded)
            {
                _logger.LogError("Error occurred disabling 2FA for user with ID {UserId}.", userId);
                ErrorMessage = "An error occurred. Please refresh the page then try again.";
                return RedirectToPage();
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userId);
            StatusMessage = "Two-factor authentication has been disabled.";
            return RedirectToPage("./Index");
        }
    }
}