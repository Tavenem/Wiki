using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace NeverFoundry.Wiki.MvcSample.Pages.Account
{
    public class RegisterFido2MfaModel : PageModel
    {
        [BindProperty] public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string? Email { get; set; }

            [Required, Display(Name = "User name")] public string? UserName { get; set; }
        }

        public void OnGet(string? returnUrl = null) => ReturnUrl = returnUrl;

        public void OnPost() { }
    }
}
