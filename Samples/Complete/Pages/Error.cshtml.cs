using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace NeverFoundry.Wiki.MvcSample.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        [BindProperty(SupportsGet = true)]
        public int? Code { get; set; }

        [TempData] public string? ErrorMessage { get; set; }

        public ErrorModel(ILogger<ErrorModel> logger) => _logger = logger;

        public void OnGet()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var type = exceptionHandlerPathFeature?.Error?.GetType().Name ?? "unknown";
            var message = exceptionHandlerPathFeature?.Error?.Message ?? string.Empty;
            var path = exceptionHandlerPathFeature?.Path ?? "unknown";

            _logger.LogError("Error page shown for RequestId {RequestId} ({Code}): Exception type {ExceptionType} encountered at {Path}; {Message}", requestId, Code ?? -1, type, path, message);
        }
    }
}
