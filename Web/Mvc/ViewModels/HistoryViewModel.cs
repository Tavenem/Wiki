using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class HistoryViewModel
    {
        public WikiRouteData Data { get; }

        public IPagedList<RevisionViewModel> Revisions { get; }

        public HistoryViewModel(WikiRouteData data, IPagedList<RevisionViewModel> revisions)
        {
            Data = data;
            Revisions = revisions;
        }

        public static async Task<HistoryViewModel> NewAsync(
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
                    pageNumber,
                    pageSize,
                    start,
                    end,
                    editor is null
                        ? (Expression<Func<WikiRevision, bool>>?)null
                        : x => x.Editor.Equals(editor, StringComparison.OrdinalIgnoreCase))
                .ConfigureAwait(false);
            var list = new List<RevisionViewModel>();
            foreach (var item in history)
            {
                list.Add(await RevisionViewModel.NewAsync(userManager, item).ConfigureAwait(false));
            }
            return new HistoryViewModel(
                data,
                new PagedList<RevisionViewModel>(list, history.PageNumber, history.PageSize, history.TotalCount));
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
