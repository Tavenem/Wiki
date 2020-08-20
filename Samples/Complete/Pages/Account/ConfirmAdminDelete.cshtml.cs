using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Pages.Account
{
    [Authorize(Policy = WikiClaims.Claim_WikiAdmin)]
    public class ConfirmAdminDeleteModel : PageModel
    {
        private readonly IDataStore _dataStore;
        private readonly UserManager<WikiUser> _userManager;

        [TempData] public string? ErrorMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required] public string? UserId { get; set; }
        }

        public ConfirmAdminDeleteModel(IDataStore dataStore, UserManager<WikiUser> userManager)
        {
            _dataStore = dataStore;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string? userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/AdminPortal");
            }
            Input.UserId = userId;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login");
            }

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
            {
                return Unauthorized();
            }

            user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "No such user. Please try again.";
                return RedirectToPage("/AdminPortal");
            }

            return Page();
        }

        public void OnPostCancel() => RedirectToPage("/AdminPortal");

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/AdminPortal");
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login");
            }
            var adminId = user.Id;

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
            {
                return Unauthorized();
            }

            user = await _userManager.FindByIdAsync(Input.UserId).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "No such user. Please try again.";
                return RedirectToPage("/AdminPortal");
            }

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ErrorMessage = "An error occurred. Please try again.";
            }

            claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
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

            var userPage = Article.GetArticle(user.Id, WikiWebConfig.UserNamespace);
            if (!(userPage is null))
            {
                await userPage.ReviseAsync(adminId, isDeleted: true).ConfigureAwait(false);
            }

            return RedirectToPage("/AdminPortal");
        }
    }
}
