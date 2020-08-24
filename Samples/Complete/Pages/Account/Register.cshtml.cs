using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.Samples.Complete.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly UserManager<WikiUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailService;

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string? Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }

            [Required, Display(Name = "User name")] public string? UserName { get; set; }
        }

        public RegisterModel(
            UserManager<WikiUser> userManager,
            SignInManager<WikiUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return Page();
            }

            var user = new WikiUser { UserName = Input.UserName, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                // If there is an existing user for which the password is valid and the email is not
                // yet confirmed, re-send the confirmation email. Otherwise report the error.
                var existingUser = await _userManager.FindByEmailAsync(Input.Email).ConfigureAwait(false);
                if (existingUser is null
                    || !await _userManager.CheckPasswordAsync(existingUser, Input.Password).ConfigureAwait(false)
                    || existingUser.EmailConfirmed)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }

            _logger.LogInformation("User created a new account with password.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code },
                protocol: Request.Scheme);

            var email = EmailTemplates.BuildRegistrationConfirmationEmail(callbackUrl);
            email.ToAddresses.Add(new EmailAddress(Input.Email!));
            await _emailService.SendEmailAsync(email).ConfigureAwait(false);

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false).ConfigureAwait(false);
                return LocalRedirect(returnUrl);
            }
        }
    }
}
