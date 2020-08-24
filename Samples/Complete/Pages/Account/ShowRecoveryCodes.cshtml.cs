using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages.Account
{
    [Authorize]
    public class ShowRecoveryCodesModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly ILogger<ShowRecoveryCodesModel> _logger;

        [TempData] public string[]? RecoveryCodes { get; set; }

        [TempData] public string? StatusMessage { get; set; }

        public ShowRecoveryCodesModel(
            UserManager<WikiUser> userManager,
            ILogger<ShowRecoveryCodesModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { ReturnUrl = Url.Page("./GenerateRecoveryCodes") });
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false);
            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            if (!isTwoFactorEnabled)
            {
                return RedirectToPage("./EnableAuthenticator");
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10).ConfigureAwait(false);
            RecoveryCodes = recoveryCodes.ToArray();

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);

            return Page();
        }
    }
}
