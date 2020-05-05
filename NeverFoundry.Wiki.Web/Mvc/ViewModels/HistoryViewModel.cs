using NeverFoundry.DataStorage;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class HistoryViewModel
    {
        public WikiRouteData Data { get; }

        public IPagedList<WikiRevision> Revisions { get; }

        public HistoryViewModel(WikiRouteData data, IPagedList<WikiRevision> revisions)
        {
            Data = data;
            Revisions = revisions;
        }

        public static async Task<HistoryViewModel> NewAsync(
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
                return new HistoryViewModel(data, new PagedList<WikiRevision>(null, 1, pageSize, 0));
            }

            var history = await data.WikiItem
                .GetHistoryAsync(pageNumber, pageSize, start, end, x => x.Editor.Equals(editor, StringComparison.OrdinalIgnoreCase))
                .ConfigureAwait(false);
            return new HistoryViewModel(data, history);
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
