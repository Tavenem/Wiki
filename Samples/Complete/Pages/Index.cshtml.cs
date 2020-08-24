using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NeverFoundry.Wiki.Samples.Complete.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet() => Redirect("/wiki");
    }
}
