using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The wiki item DTO.
    /// </summary>
    public class WikiItemViewModel
    {
        /// <summary>
        /// The associated <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData Data { get; }

        /// <summary>
        /// The rendered HTML.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Whether this is a diff.
        /// </summary>
        public bool IsDiff { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="WikiItemViewModel"/>.
        /// </summary>
        public WikiItemViewModel(WikiRouteData data, string html, bool isDiff)
        {
            Data = data;
            Html = html;
            IsDiff = isDiff;
        }

        /// <summary>
        /// Get a new instance of <see cref="WikiItemViewModel"/>.
        /// </summary>
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
                        : data.WikiItem.Html;
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
                        : data.WikiItem.Html;
                }
                data.Categories = data.WikiItem.Categories;
            }

            return new WikiItemViewModel(data, html, isDiff);
        }
    }
}
