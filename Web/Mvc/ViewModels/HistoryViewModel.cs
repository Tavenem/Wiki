using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The history DTO.
    /// </summary>
    public record HistoryViewModel(WikiRouteData Data, IPagedList<RevisionViewModel> Revisions)
    {
        /// <summary>
        /// Get a new <see cref="HistoryViewModel"/>.
        /// </summary>
        public static async Task<HistoryViewModel> NewAsync(
            IWikiOptions wikiOptions,
            IWikiWebOptions wikiWebOptions,
            IDataStore dataStore,
            IWikiUserManager userManager,
            WikiRouteData data,
            int pageNumber = 1,
            int pageSize = 50,
            string? editor = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null)
        {
            if (data.WikiItem is null
                || start > end)
            {
                return new HistoryViewModel(data, new PagedList<RevisionViewModel>(null, 1, pageSize, 0));
            }

            var history = await data.WikiItem
                .GetHistoryAsync(
                    dataStore,
                    pageNumber,
                    pageSize,
                    start,
                    end,
                    editor is null
                        ? (Expression<Func<Revision, bool>>?)null
                        : x => x.Editor.Equals(editor, StringComparison.OrdinalIgnoreCase))
                .ConfigureAwait(false);
            var list = new List<RevisionViewModel>();
            foreach (var item in history)
            {
                list.Add(await RevisionViewModel
                    .NewAsync(wikiOptions, wikiWebOptions, dataStore, userManager, item)
                    .ConfigureAwait(false));
            }
            return new HistoryViewModel(
                data,
                new PagedList<RevisionViewModel>(list, history.PageNumber, history.PageSize, history.TotalCount));
        }
    }
}
