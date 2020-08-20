using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MvcSample.Pages.Account
{
    [Authorize]
    public class EnableAuthenticatorModel : PageModel
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly UserManager<WikiUser> _userManager;
        private readonly ILogger<EnableAuthenticatorModel> _logger;
        private readonly UrlEncoder _urlEncoder;

        public string? AuthenticatorUri { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        [TempData] public string[]? RecoveryCodes { get; set; }

        public string? SharedKey { get; set; }

        [TempData] public string? StatusMessage { get; set; }

        public EnableAuthenticatorModel(
            UserManager<WikiUser> userManager,
            ILogger<EnableAuthenticatorModel> logger,
            UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Verification Code")]
            public string? Code { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./EnableAuthenticator") });
            }

            await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAwait(false);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Login", new { returnUrl = Url.Page("./EnableAuthenticator") });
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAwait(false);
                return Page();
            }

            // Strip spaces and hypens
            var verificationCode = Input.Code?.Replace(" ", string.Empty).Replace("-", string.Empty) ?? string.Empty;

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode).ConfigureAwait(false);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user).ConfigureAwait(false);
                return Page();
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            return RedirectToPage("./ShowRecoveryCodes");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(WikiUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            }

            SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user).ConfigureAwait(false);
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey, currentPosition, 4).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey, currentPosition, unformattedKey.Length - currentPosition);
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("NeverFoundry.Wiki.MvcSample"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
