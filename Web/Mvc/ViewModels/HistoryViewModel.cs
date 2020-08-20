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
    public class HistoryViewModel
    {
        /// <summary>
        /// The associated <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData Data { get; }

        /// <summary>
        /// The list of revisions.
        /// </summary>
        public IPagedList<RevisionViewModel> Revisions { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="HistoryViewModel"/>.
        /// </summary>
        public HistoryViewModel(WikiRouteData data, IPagedList<RevisionViewModel> revisions)
        {
            Data = data;
            Revisions = revisions;
        }

        /// <summary>
        /// Get a new <see cref="HistoryViewModel"/>.
        /// </summary>
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
}
