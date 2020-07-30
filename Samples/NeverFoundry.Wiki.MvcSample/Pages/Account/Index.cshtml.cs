using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.MvcSample.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MvcSample.Pages.Account
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly UserManager<WikiUser> _userManager;

        public IList<UserLoginInfo>? CurrentLogins { get; set; }

        [TempData] public string? ErrorMessage { get; set; }

        public bool HasAuthenticator { get; set; }

        [BindProperty] public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public IList<AuthenticationScheme>? OtherLogins { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool ShowRemoveButton { get; set; }

        [TempData] public string? StatusMessage { get; set; }

        public string? Username { get; set; }

        public class InputModel
        {
            [EmailAddress] public string? Email { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string? CurrentPassword { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [Display(Name = "User name")] public string? UserName { get; set; }
        }

        public IndexModel(
            IEmailService emailService,
            UserManager<WikiUser> userManager,
            SignInManager<WikiUser> signInManager,
            ILogger<IndexModel> logger)
        {
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        private async Task LoadAsync(WikiUser user)
        {
            Input.UserName = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);
            Input.Email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);

            HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false) != null;
            Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false);
            IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user).ConfigureAwait(false);
            RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user).ConfigureAwait(false);

            CurrentLogins = await _userManager.GetLoginsAsync(user).ConfigureAwait(false);
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false))
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./Index") });
            }

            await LoadAsync(user).ConfigureAwait(false);
            return Page();
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./Index") });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user).ConfigureAwait(false)).ConfigureAwait(false);
            if (info is null)
            {
                _logger.LogError("Error occurred loading external login info for user with ID '{UserId}'.", user.Id);
                ErrorMessage = "An error occurred. Please refresh the page then try again.";
                return RedirectToPage();
            }

            var result = await _userManager.AddLoginAsync(user, info).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ErrorMessage = "The external login was not added. External logins can only be associated with one account.";
                return RedirectToPage();
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);

            StatusMessage = "The external login was added.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            StatusMessage = null;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./Index") });
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user).ConfigureAwait(false);
                return Page();
            }

            var username = await _userManager.GetUserNameAsync(user).ConfigureAwait(false);
            if (Input.UserName != username)
            {
                var result = await _userManager.SetUserNameAsync(user, Input.UserName).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                    _logger.LogError("Error occurred setting user name for user with ID {UserId}.", userId);
                    ErrorMessage = "An error occurred. Please refresh the page and try again.";
                }
            }

            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            if (Input.Email != email)
            {
                var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.Email).ConfigureAwait(false);
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId, email = Input.Email, code },
                    protocol: Request.Scheme);
                var message = EmailTemplates.BuildChangeAddressEmail(callbackUrl);
                message.ToAddresses.Add(new EmailAddress(Input.Email!));
                await _emailService.SendEmailAsync(message).ConfigureAwait(false);
                StatusMessage = "Confirmation link to change email sent. Please check your email.";
            }

            if (!string.IsNullOrEmpty(Input.NewPassword))
            {
                var hasPassword = await _userManager.HasPasswordAsync(user).ConfigureAwait(false);
                if (!hasPassword)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword).ConfigureAwait(false);
                    if (!addPasswordResult.Succeeded)
                    {
                        foreach (var error in addPasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }
                }
                else
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword).ConfigureAwait(false);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();
                    }
                }

                _logger.LogInformation("User changed their password successfully.");
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            StatusMessage ??= "Your account has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Page("./Index", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnPostForgetBrowserAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./Index") });
            }

            await _signInManager.ForgetTwoFactorClientAsync().ConfigureAwait(false);
            StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your two-factor authentication code.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./Index") });
            }

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                ErrorMessage = "The external login was not removed.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user).ConfigureAwait(false);
            StatusMessage = "The external login was removed.";
            return RedirectToPage();
        }
    }
}
