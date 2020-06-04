using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Information about the current wiki page.
    /// </summary>
    public class WikiRouteData
    {
        internal const string RouteTitle = "title";

        private const string RouteIsCompact = "isCompact";
        private const string RouteWikiNamespace = "wikiNamespace";

        /// <summary>
        /// Whether the current user has edit permission for the current <see cref="WikiItem"/>.
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// The categories to which the requested page belongs (if any).
        /// </summary>
        public IReadOnlyCollection<string>? Categories { get; set; }

        private string? _displayTitle;
        /// <summary>
        /// The title to display.
        /// </summary>
        public string DisplayTitle
        {
            get => _displayTitle ?? Title;
            set => _displayTitle = value;
        }

        /// <summary>
        /// Whether the requested page is a category.
        /// </summary>
        public bool IsCategory { get; }

        /// <summary>
        /// Whether a compact view was requested.
        /// </summary>
        public bool IsCompact { get; set; }

        /// <summary>
        /// Whether this is an edit operation.
        /// </summary>
        public bool IsEdit { get; set; }

        /// <summary>
        /// Whether the requested page is a file.
        /// </summary>
        public bool IsFile { get; }

        /// <summary>
        /// Whether this is a request to view the history of an item.
        /// </summary>
        public bool IsHistory { get; set; }

        /// <summary>
        /// Whether the requested page is a discussion page.
        /// </summary>
        public bool IsTalk { get; }

        /// <summary>
        /// Whether the requested page is the search page.
        /// </summary>
        public bool IsSearch { get; set; }

        /// <summary>
        /// Whether the requested page is a system page.
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Whether the requested page is a user page.
        /// </summary>
        public bool IsUserPage { get; }

        /// <summary>
        /// Whether a diff with the current version was requested.
        /// </summary>
        public bool RequestedDiffCurrent { get; }

        /// <summary>
        /// Whether a diff with the previous revision was requested.
        /// </summary>
        public bool RequestedDiffPrevious { get; }

        /// <summary>
        /// The timestamp of the requested diff revision.
        /// </summary>
        public DateTimeOffset? RequestedDiffTimestamp { get; }

        /// <summary>
        /// The timestamp of the requested revision.
        /// </summary>
        public DateTimeOffset? RequestedTimestamp { get; }

        /// <summary>
        /// Whether the <see cref="WikiNamespace"/> should be displayed.
        /// </summary>
        public bool ShowNamespace { get; }

        /// <summary>
        /// The title of the requested page.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The wiki item currently displayed (may be <see langword="null"/>).
        /// </summary>
        public Article? WikiItem { get; set; }

        /// <summary>
        /// The namespace of the requested page.
        /// </summary>
        public string WikiNamespace { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData()
        {
            Title = string.Empty;
            WikiNamespace = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData(RouteData routeData, IQueryCollection query)
        {
            IsCompact = routeData.Values.TryGetValue(RouteIsCompact, out var c)
                && c is bool iC
                && iC;

            string? wN = null;
            if (routeData.Values.TryGetValue(RouteWikiNamespace, out var n)
                && n is string w)
            {
                wN = w;
            }

            var separatorIndex = wN?.IndexOf(':') ?? -1;
            if (separatorIndex != -1
                && wN!.Substring(0, separatorIndex).Equals(WikiConfig.TalkNamespace, StringComparison.OrdinalIgnoreCase))
            {
                wN = wN[(separatorIndex + 1)..];
                IsTalk = true;
            }

            var haveNamespace = !string.IsNullOrWhiteSpace(wN);
            var defaultNamespace = !haveNamespace || string.Equals(wN, WikiConfig.DefaultNamespace, StringComparison.OrdinalIgnoreCase);
            WikiNamespace = defaultNamespace ? WikiConfig.DefaultNamespace : wN!;
            ShowNamespace = !defaultNamespace;

            Title = routeData.Values.TryGetValue(RouteTitle, out var t)
                && t is string wT
                && !string.IsNullOrWhiteSpace(wT)
                ? wT
                : WikiConfig.MainPageTitle;

            IsCategory = !defaultNamespace && string.Equals(WikiNamespace, WikiConfig.CategoriesTitle, StringComparison.OrdinalIgnoreCase);
            IsSystem = !defaultNamespace && !IsCategory && string.Equals(WikiNamespace, WikiWebConfig.SystemNamespace, StringComparison.OrdinalIgnoreCase);
            IsFile = !defaultNamespace && !IsCategory && !IsSystem && string.Equals(WikiNamespace, WikiConfig.FileNamespace, StringComparison.OrdinalIgnoreCase);
            IsUserPage = !defaultNamespace && !IsCategory && !IsSystem && !IsFile && string.Equals(WikiNamespace, WikiWebConfig.UserNamespace, StringComparison.OrdinalIgnoreCase);

            if (query.TryGetValue("rev", out var rev)
                && rev.Count >= 1)
            {
                if (DateTimeOffset.TryParse(rev[0], out var timestamp))
                {
                    RequestedTimestamp = timestamp;
                }
                else if (long.TryParse(rev[0], out var ticks))
                {
                    RequestedTimestamp = new DateTimeOffset(ticks, TimeSpan.Zero);
                }
            }
            if (query.TryGetValue("diff", out var diff)
                && diff.Count >= 1)
            {
                if (string.Equals(diff[0], "prev", StringComparison.OrdinalIgnoreCase))
                {
                    RequestedDiffPrevious = true;
                }
                else if (string.Equals(diff[0], "cur", StringComparison.OrdinalIgnoreCase))
                {
                    RequestedDiffCurrent = true;
                }
                else if (DateTimeOffset.TryParse(diff[0], out var diffTimestamp))
                {
                    RequestedDiffTimestamp = diffTimestamp;
                }
            }
        }
    }
}
