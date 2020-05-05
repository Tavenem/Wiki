using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeverFoundry.Wiki.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages
{
    [Authorize(Policy = Constants.Claim_WikiAdmin)]
    public class AdminPortalModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;

        public int CurrentPage { get; set; }

        [TempData] public string? ErrorMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public bool IsSiteAdmin { get; set; }

        public int ItemsPerPage { get; set; }

        public long PageCount { get; set; }

        [TempData] public string? StatusMessage { get; set; }

        public long UserCountActive { get; set; }

        public long UserCountDeleted { get; set; }

        public long UserCountDisabled { get; set; }

        public long UserCountInactive { get; set; }

        public long UserCountTotal { get; set; }

        public List<UserWithClaims>? Users { get; set; }

        public class UserWithClaims
        {
            public IList<Claim> Claims { get; set; } = new List<Claim>();

            public WikiUser User { get; }

            public UserWithClaims(WikiUser user) => User = user;
        }

        public class InputModel
        {
            [Required] public string? Id { get; set; }
        }

        public AdminPortalModel(UserManager<WikiUser> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(int page = 1, int count = 25)
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            CurrentPage = page;
            ItemsPerPage = count;

            UserCountActive = _userManager.Users.LongCount(x => !x.IsDeleted && !x.IsDisabled && x.LastAccess.AddYears(1) >= DateTimeOffset.UtcNow);
            UserCountDeleted = _userManager.Users.LongCount(x => x.IsDeleted);
            UserCountDisabled = _userManager.Users.LongCount(x => x.IsDisabled);
            UserCountInactive = _userManager.Users.LongCount(x => !x.IsDeleted && !x.IsDisabled && x.LastAccess.AddYears(1) < DateTimeOffset.UtcNow);
            UserCountTotal = _userManager.Users.LongCount();

            PageCount = UserCountTotal / ItemsPerPage;
            if (UserCountTotal % ItemsPerPage != 0)
            {
                PageCount++;
            }

            Users = _userManager.Users
                .OrderBy(x => x.UserName)
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(x => new UserWithClaims(x))
                .ToList();
            foreach (var user in Users)
            {
                user.Claims = await _userManager.GetClaimsAsync(user.User).ConfigureAwait(false);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddWikiAdminAsync()
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(Input.Id).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }
            var identityResult = await _userManager.AddClaimAsync(user, new Claim(Constants.Claim_WikiAdmin, true.ToString(), ClaimValueTypes.Boolean)).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = "User has been made an admin";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDisableUserAsync()
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(Input.Id).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            user.IsDisabled = true;
            user.DisabledStart = DateTimeOffset.UtcNow;
            var identityResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = "User has been disabled";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEnableUserAsync()
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(Input.Id).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            user.IsDisabled = false;
            user.DisabledStart = null;
            var identityResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = "User has been enabled";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveWikiAdminAsync()
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            var user = await _userManager.FindByIdAsync(Input.Id).ConfigureAwait(false);
            if (user is null)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            claims = claims.Where(x => x.Type == Constants.Claim_WikiAdmin).ToList();
            var identityResult = await _userManager.RemoveClaimsAsync(user, claims).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = "User is no longer an admin";
            return RedirectToPage();
        }

        private async Task<IActionResult?> VerifyAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("/Account/Login");
            }

            var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            if (!claims.HasBoolClaim(Constants.Claim_WikiAdmin))
            {
                return Unauthorized();
            }

            return null;
        }
    }
}
