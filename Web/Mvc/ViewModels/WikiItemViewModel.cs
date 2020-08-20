using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class WikiItemViewModel
    {
        public WikiRouteData Data { get; }

        public string Html { get; }

        public bool IsDiff { get; }

        public WikiItemViewModel(WikiRouteData data, string html, bool isDiff)
        {
            Data = data;
            Html = html;
            IsDiff = isDiff;
        }

        public static async Task<WikiItemViewModel> NewAsync(WikiRouteData data)
        {
            string html;
            var isDiff = false;
            if (data.WikiItem is null)
            {
                html = string.Empty;
            }
            else
            {
                if (data.RequestedDiffCurrent)
                {
                    isDiff = data.RequestedTimestamp.HasValue;
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem.GetDiffWithCurrentHtmlAsync(data.RequestedTimestamp.Value).ConfigureAwait(false)
                        : data.WikiItem.GetHtml();
                }
                else if (data.RequestedDiffPrevious)
                {
                    isDiff = true;
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem.GetDiffHtmlAsync(data.RequestedTimestamp.Value).ConfigureAwait(false)
                        : await data.WikiItem.GetDiffHtmlAsync(DateTimeOffset.UtcNow).ConfigureAwait(false);
                }
                else if (data.RequestedDiffTimestamp.HasValue)
                {
                    isDiff = true;
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem.GetDiffWithOtherAsync(data.RequestedDiffTimestamp.Value, data.RequestedTimestamp.Value).ConfigureAwait(false)
                        : await data.WikiItem.GetDiffWithCurrentHtmlAsync(data.RequestedDiffTimestamp.Value).ConfigureAwait(false);
                }
                else
                {
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem.GetHtmlAsync(data.RequestedTimestamp.Value).ConfigureAwait(false)
                        : data.WikiItem.GetHtml();
                }
                data.Categories = data.WikiItem.Categories;
            }

            return new WikiItemViewModel(data, html, isDiff);
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
