using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NeverFoundry.Wiki.Samples.Simple.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? Code { get; set; }
    }
}
