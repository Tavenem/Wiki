﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NeverFoundry.Wiki.Samples.Simple.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet() => Redirect("/wiki");
    }
}