using NeverFoundry.DataStorage;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The wiki item DTO.
    /// </summary>
    public record WikiItemViewModel(WikiRouteData Data, string Html, bool IsDiff)
    {
        /// <summary>
        /// Get a new instance of <see cref="WikiItemViewModel"/>.
        /// </summary>
        public static async Task<WikiItemViewModel> NewAsync(IWikiOptions options, IDataStore dataStore, WikiRouteData data)
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
                        ? await data.WikiItem
                            .GetDiffWithCurrentHtmlAsync(options, dataStore, data.RequestedTimestamp.Value)
                            .ConfigureAwait(false)
                        : data.WikiItem.Html;
                }
                else if (data.RequestedDiffPrevious)
                {
                    isDiff = true;
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem
                            .GetDiffHtmlAsync(options, dataStore, data.RequestedTimestamp.Value)
                            .ConfigureAwait(false)
                        : await data.WikiItem
                            .GetDiffHtmlAsync(options, dataStore, DateTimeOffset.UtcNow)
                            .ConfigureAwait(false);
                }
                else if (data.RequestedDiffTimestamp.HasValue)
                {
                    isDiff = true;
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem
                            .GetDiffWithOtherAsync(dataStore, data.RequestedDiffTimestamp.Value, data.RequestedTimestamp.Value)
                            .ConfigureAwait(false)
                        : await data.WikiItem
                            .GetDiffWithCurrentHtmlAsync(options, dataStore, data.RequestedDiffTimestamp.Value)
                            .ConfigureAwait(false);
                }
                else
                {
                    html = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem
                            .GetHtmlAsync(options, dataStore, data.RequestedTimestamp.Value)
                            .ConfigureAwait(false)
                        : data.WikiItem.Html;
                }
                data.Categories = data.WikiItem.Categories;
            }

            return new WikiItemViewModel(data, html, isDiff);
        }
    }
}
