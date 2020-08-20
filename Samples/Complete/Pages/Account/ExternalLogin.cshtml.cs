using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.Samples.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly UserManager<WikiUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<WikiUser> signInManager,
            UserManager<WikiUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
        }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public string? LoginProvider { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData] public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string? Email { get; set; }

            [Required] public string? UserName { get; set; }
        }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
            if (info is null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity?.Name ?? "unknown", info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have a local account, attempt to auto-create one.
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var emailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var user = new WikiUser
                    {
                        Email = emailAddress,
                        UserName = info.Principal.Identity?.Name ?? emailAddress,
                    };
                    var createResult = await _userManager.CreateAsync(user).ConfigureAwait(false);
                    if (createResult.Succeeded)
                    {
                        createResult = await _userManager.AddLoginAsync(user, info).ConfigureAwait(false);
                        if (createResult.Succeeded)
                        {
                            _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                            // No verification required for emails obtained from an external
                            // provider; the provider's own authentication process is accepted as a
                            // sufficient verification step in order to improve user experience.
                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                                createResult = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
                                if (result.Succeeded)
                                {
                                    return LocalRedirect(returnUrl);
                                }

                                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                                var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                var callbackUrl = Url.Page(
                                    "/Account/ConfirmEmail",
                                    pageHandler: null,
                                    values: new { userId, code },
                                    protocol: Request.Scheme);

                                var email = EmailTemplates.BuildRegistrationConfirmationEmail(callbackUrl);
                                email.ToAddresses.Add(new EmailAddress(Input.Email!));
                                await _emailService.SendEmailAsync(email).ConfigureAwait(false);

                                return RedirectToPage("./RegisterConfirmation");
                            }

                            return LocalRedirect(returnUrl);
                        }

                        // If adding the external login failed, send the user to their new account page
                        // to manage the external provider manually.
                        RedirectToPage("./Manage/ExternalLogins");
                    }
                }

                // If a local account could not be created automatically, ask the user to create one.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync().ConfigureAwait(false);
            if (info is null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new WikiUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        // No verification required for emails obtained from an external
                        // provider; the provider's own authentication process is accepted as a
                        // sufficient verification step in order to improve user experience.
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                            result = await _userManager.ConfirmEmailAsync(user, code).ConfigureAwait(false);
                            if (result.Succeeded)
                            {
                                return LocalRedirect(returnUrl);
                            }

                            await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                            var userId = await _userManager.GetUserIdAsync(user).ConfigureAwait(false);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                            var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { userId, code },
                                protocol: Request.Scheme);

                            var email = EmailTemplates.BuildRegistrationConfirmationEmail(callbackUrl);
                            email.ToAddresses.Add(new EmailAddress(Input.Email!));
                            await _emailService.SendEmailAsync(email).ConfigureAwait(false);

                            return RedirectToPage("./RegisterConfirmation");
                        }

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
