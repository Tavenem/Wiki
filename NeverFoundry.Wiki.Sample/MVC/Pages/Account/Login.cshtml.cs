using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        [TempData] public string? ErrorMessage { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public LoginModel(SignInManager<WikiUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<WikiUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public class InputModel
        {
            [Required, EmailAddress] public string? Email { get; set; }

            [Required, DataType(DataType.Password)] public string? Password { get; set; }

            [Display(Name = "Stay signed in")] public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync().ConfigureAwait(false)).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email).ConfigureAwait(false);
            if (user is null)
            {
                return RedirectToPage("./Register", new { returnUrl });
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true).ConfigureAwait(false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
