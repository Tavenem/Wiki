using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MVCSample.Pages.Account
{
    [Authorize]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<WikiUser> _userManager;
        private readonly SignInManager<WikiUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        [TempData] public string? ErrorMessage { get; set; }

        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public bool RequirePassword { get; set; }

        public class InputModel
        {
            [Required, DataType(DataType.Password)]
            public string? Password { get; set; }
        }

        public DeletePersonalDataModel(
            UserManager<WikiUser> userManager,
            SignInManager<WikiUser> signInManager,
            ILogger<DeletePersonalDataModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
            var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                _logger.LogError("Error occurred deleting user with ID '{UserId}'.", userId);
                ErrorMessage = "An error occurred. Please refresh the page and try again.";
                RedirectToPage();
            }

            await _signInManager.SignOutAsync().ConfigureAwait(false);

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
