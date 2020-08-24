using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages.Account
{
    [Authorize]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly IDataStore _dataStore;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly UserManager<WikiUser> _userManager;

        [TempData] public string? ErrorMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public bool RequirePassword { get; set; }

        public class InputModel
        {
            [Required, DataType(DataType.Password)]
            public string? Password { get; set; }
        }

        public DeletePersonalDataModel(
            IDataStore dataStore,
            ILogger<DeletePersonalDataModel> logger,
            SignInManager<WikiUser> signInManager,
            UserManager<WikiUser> userManager)
        {
            _dataStore = dataStore;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user).ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user).ConfigureAwait(false);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password).ConfigureAwait(false))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                _logger.LogError("Error occurred deleting user with ID '{UserId}'.", userId);
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                RedirectToPage();
            }

            await _signInManager.SignOutAsync().ConfigureAwait(false);

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            var groups = claims
                .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                .ToList();
            foreach (var groupClaim in groups)
            {
                var members = await _userManager.GetUsersForClaimAsync(groupClaim).ConfigureAwait(false);
                if (!members.Except(new[] { user }).Any())
                {
                    var group = await _dataStore.GetItemAsync<WikiGroup>(groupClaim.Value).ConfigureAwait(false);
                    if (!(group is null))
                    {
                        await _dataStore.RemoveItemAsync(group).ConfigureAwait(false);
                    }
                }
            }

            var userPage = Article.GetArticle(userId, WikiWebConfig.UserNamespace);
            if (!(userPage is null))
            {
                await userPage.ReviseAsync(userId, isDeleted: true).ConfigureAwait(false);
            }

            return Redirect("~/");
        }
    }
}
