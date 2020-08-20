using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages
{
    [Authorize(Policy = WikiClaims.Claim_WikiAdmin)]
    public class AdminPortalModel : PageModel
    {
        private readonly IDataStore _dataStore;
        private readonly UserManager<WikiUser> _userManager;

        public int CurrentPage { get; set; }

        [TempData] public string? ErrorMessage { get; set; }

        public long? GroupCount { get; set; }

        public List<WikiGroup>? Groups { get; set; }

        public bool HasNextPage { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public bool IsSiteAdmin { get; set; }

        public int ItemsPerPage { get; set; }

        public long? PageCount { get; set; }

        public bool ShowGroups { get; set; }

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

        public AdminPortalModel(IDataStore dataStore, UserManager<WikiUser> userManager)
        {
            _dataStore = dataStore;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int page = 1, int count = 25, bool groups = false)
        {
            var result = await VerifyAdminAsync().ConfigureAwait(false);
            if (result != null)
            {
                return result;
            }

            ShowGroups = groups;

            CurrentPage = page;
            ItemsPerPage = count;

            UserCountActive = _userManager.Users.LongCount(x => !x.IsDeleted && !x.IsDisabled && x.LastAccess.AddYears(1) >= DateTimeOffset.UtcNow);
            UserCountDeleted = _userManager.Users.LongCount(x => x.IsDeleted);
            UserCountDisabled = _userManager.Users.LongCount(x => x.IsDisabled);
            UserCountInactive = _userManager.Users.LongCount(x => !x.IsDeleted && !x.IsDisabled && x.LastAccess.AddYears(1) < DateTimeOffset.UtcNow);
            UserCountTotal = _userManager.Users.LongCount();

            if (groups)
            {
                var groupPage = await _dataStore.Query<WikiGroup>()
                    .OrderBy(x => x.GroupName)
                    .GetPageAsync(CurrentPage, ItemsPerPage)
                    .ConfigureAwait(false);
                Groups = groupPage.ToList();
                HasNextPage = groupPage.HasNextPage;
                GroupCount = groupPage.TotalCount;
                PageCount = groupPage.TotalPages;
            }
            else
            {
                PageCount = UserCountTotal / ItemsPerPage;
                if (UserCountTotal % ItemsPerPage != 0)
                {
                    PageCount++;
                }
                HasNextPage = PageCount > CurrentPage;

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
            var identityResult = await _userManager.AddClaimAsync(user, new Claim(WikiClaims.Claim_WikiAdmin, true.ToString(), ClaimValueTypes.Boolean)).ConfigureAwait(false);
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
            claims = claims.Where(x => x.Type == WikiClaims.Claim_WikiAdmin).ToList();
            var identityResult = await _userManager.RemoveClaimsAsync(user, claims).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = "User is no longer an admin";
            return RedirectToPage();
        }

        public async Task<IActionResult> ToggleGroupUploadPermission()
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

            var group = await _dataStore.GetItemAsync<WikiGroup>(Input.Id).ConfigureAwait(false);
            if (group is null)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            group.HasUploadPermission = !group.HasUploadPermission;
            var dataResult = await _dataStore.StoreItemAsync(group).ConfigureAwait(false);
            if (!dataResult)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = $"Group {(group.HasUploadPermission ? "now has" : "no longer has ")} permission to upload files";
            return RedirectToPage();
        }

        public async Task<IActionResult> ToggleUploadPermission()
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

            user.HasUploadPermission = !user.HasUploadPermission;
            var identityResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                return RedirectToPage();
            }

            StatusMessage = $"User {(user.HasUploadPermission ? "now has" : "no longer has ")} permission to upload files";
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
            if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
            {
                return Unauthorized();
            }

            return null;
        }
    }
}
