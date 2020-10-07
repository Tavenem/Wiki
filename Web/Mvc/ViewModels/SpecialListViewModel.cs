using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc.Models;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The wiki page list DTO.
    /// </summary>
    public record SpecialListViewModel
    (
        WikiRouteData Data,
        SpecialListType Type,
        bool Descending,
        IPagedList<Article> Items,
        string Description,
        string? SecondaryDescription = null,
        IPagedList<MissingPage>? MissingItems = null,
        string? Sort = null,
        string? Filter = null
    )
    {
        /// <summary>
        /// Initialize a new <see cref="SpecialListViewModel"/>.
        /// </summary>
        public SpecialListViewModel(
            IWikiOptions wikiOptions,
            IWikiWebOptions wikiWebOptions,
            WikiRouteData data,
            SpecialListType type,
            bool descending,
            IPagedList<Article> items,
            IPagedList<MissingPage>? missingItems = null,
            string? sort = null,
            string? filter = null) : this(
                data,
                type,
                descending,
                items,
                GetDescription(wikiOptions, type, data),
                GetSecondaryDescription(wikiOptions, wikiWebOptions, type),
                missingItems,
                sort,
                filter)
        { }

        /// <summary>
        /// Get a <see cref="SpecialListViewModel"/>.
        /// </summary>
        public static async Task<SpecialListViewModel> NewAsync(
            IWikiOptions wikiOptions,
            IWikiWebOptions wikiWebOptions,
            IDataStore dataStore,
            WikiRouteData data,
            SpecialListType type,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
        {
            var list = type switch
            {
                SpecialListType.All_Categories => await GetListAsync<Category>(dataStore, pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false),

                SpecialListType.All_Files => await GetListAsync<WikiFile>(dataStore, pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false),

                SpecialListType.All_Pages => await GetListAsync<Article>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.IdItemTypeName == Article.ArticleIdItemTypeName)
                .ConfigureAwait(false),

#pragma warning disable RCS1113 // Use 'string.IsNullOrEmpty' method: not necessarily supported by data provider
                SpecialListType.All_Redirects => await GetListAsync<Article>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.RedirectTitle != null && x.RedirectTitle != string.Empty)
                .ConfigureAwait(false),
#pragma warning restore RCS1113 // Use 'string.IsNullOrEmpty' method.

                SpecialListType.Broken_Redirects => await GetListAsync<Article>(dataStore, pageNumber, pageSize, sort, descending, filter, x => x.IsBrokenRedirect).ConfigureAwait(false),

                SpecialListType.Double_Redirects => await GetListAsync<Article>(dataStore, pageNumber, pageSize, sort, descending, filter, x => x.IsDoubleRedirect).ConfigureAwait(false),

#pragma warning disable CA1829 // Optimize LINQ method call: Count() is translated to SQL by various data providers (Relinq), while the Count property is not necessarily serialized/recognized
                SpecialListType.Uncategorized_Articles => await GetListAsync<Article>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.IdItemTypeName == Article.ArticleIdItemTypeName
                        && x.RedirectTitle == null
                        && x.WikiNamespace != wikiOptions.ScriptNamespace
                        && (x.Categories == null || x.Categories.Count() == 0))
                .ConfigureAwait(false),

                SpecialListType.Uncategorized_Categories => await GetListAsync<Category>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.Categories == null || x.Categories.Count() == 0)
                .ConfigureAwait(false),

                SpecialListType.Uncategorized_Files => await GetListAsync<WikiFile>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.Categories == null || x.Categories.Count() == 0)
                .ConfigureAwait(false),

                SpecialListType.Unused_Categories => await GetListAsync<Category>(
                    dataStore,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter,
                    x => x.ChildIds.Count() == 0)
                .ConfigureAwait(false),
#pragma warning restore CA1829 // Optimize LINQ method call.

                SpecialListType.What_Links_Here => await GetLinksHereAsync(
                    wikiOptions,
                    dataStore,
                    data.Title,
                    data.WikiNamespace,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter)
                .ConfigureAwait(false),

                _ => new PagedList<Article>(null, 1, pageSize, 0),
            };
            var missing = type == SpecialListType.Missing_Pages
                ? await GetMissingAsync(wikiWebOptions, dataStore, pageNumber, pageSize, descending, filter).ConfigureAwait(false)
                : null;

            return new SpecialListViewModel(wikiOptions, wikiWebOptions, data, type, descending, list, missing, sort, filter);
        }

        private static string GetDescription(IWikiOptions options, SpecialListType type, WikiRouteData data) => type switch
        {
            SpecialListType.All_Categories => "This page lists all categories, either alphabetically or by most recent update.",
            SpecialListType.All_Files => "This page lists all files, either alphabetically or by most recent update.",
            SpecialListType.All_Pages => "This page lists all articles, either alphabetically or by most recent update.",
            SpecialListType.All_Redirects => "This page lists all articles which redirect to another page, either alphabetically or by most recent update.",
            SpecialListType.Broken_Redirects => "This page lists all articles which redirect to an article that does not exist, either alphabetically or by most recent update.",
            SpecialListType.Double_Redirects => "This page lists all articles which redirect to a page that redirects someplace else, either alphabetically or by most recent update.",
            SpecialListType.Missing_Pages => "This page lists all pages which are linked but do not exist.",
            SpecialListType.Uncategorized_Articles => "This page lists all articles which are not categorized, either alphabetically or by most recent update.",
            SpecialListType.Uncategorized_Categories => "This page lists all categories which are not categorized, either alphabetically or by most recent update.",
            SpecialListType.Uncategorized_Files => "This page lists all files which are not categorized, either alphabetically or by most recent update.",
            SpecialListType.Unused_Categories => "This page lists all categories which have no articles or subcategories, either alphabetically or by most recent update.",
            SpecialListType.What_Links_Here => $"The following pages link to {Article.GetFullTitle(options, data.Title, data.WikiNamespace, data.IsTalk)}.",
            _ => string.Empty,
        };

        private static async Task<IPagedList<T>> GetListAsync<T>(
            IDataStore dataStore,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null,
            Expression<Func<T, bool>>? condition = null) where T : Article
        {
            var pageCondition = condition is null
                ? (T x) => !x.IsDeleted
                : condition.AndAlso(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(filter))
            {
                pageCondition = pageCondition.AndAlso(x => x.Title.Contains(filter));
            }

            var query = dataStore.Query<T>();
            if (pageCondition is not null)
            {
                query = query.Where(pageCondition);
            }
            if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(x => x.TimestampTicks, descending: descending);
            }
            else
            {
                query = query.OrderBy(x => x.Title, descending: descending);
            }

            return await query
                .GetPageAsync(pageNumber, pageSize)
                .ConfigureAwait(false);
        }

        private static async Task<IPagedList<Article>> GetLinksHereAsync(
            IWikiOptions options,
            IDataStore dataStore,
            string title,
            string wikiNamespace,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
        {
            var allReferences = new HashSet<string>();
            var references = await PageLinks.GetPageLinksAsync(dataStore, title, wikiNamespace).ConfigureAwait(false);
            if (references is not null)
            {
                foreach (var reference in references.References)
                {
                    allReferences.Add(reference);
                }
            }
            var transclusions = await PageTransclusions.GetPageTransclusionsAsync(dataStore, title, wikiNamespace).ConfigureAwait(false);
            if (transclusions is not null)
            {
                foreach (var reference in transclusions.References)
                {
                    allReferences.Add(reference);
                }
            }
            if (!string.Equals(wikiNamespace, options.CategoryNamespace, StringComparison.Ordinal)
                && !string.Equals(wikiNamespace, options.FileNamespace, StringComparison.Ordinal))
            {
                var redirects = await PageRedirects.GetPageRedirectsAsync(dataStore, title, wikiNamespace).ConfigureAwait(false);
                if (redirects is not null)
                {
                    foreach (var reference in redirects.References)
                    {
                        allReferences.Add(reference);
                    }
                }
            }

            var articles = new List<Article>();
            var hasFilter = !string.IsNullOrWhiteSpace(filter);
            foreach (var reference in allReferences)
            {
                var article = await dataStore.GetItemAsync<Article>(reference).ConfigureAwait(false);
                if (article is not null
                    && (!hasFilter
                    || article.Title.Contains(filter!)))
                {
                    articles.Add(article);
                }
            }
            if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
            {
                articles.Sort((x, y) => descending
                    ? -x.TimestampTicks.CompareTo(y.TimestampTicks)
                    : x.TimestampTicks.CompareTo(y.TimestampTicks));
            }
            else
            {
                articles.Sort((x, y) => descending
                    ? -x.Title.CompareTo(y.Title)
                    : x.Title.CompareTo(y.Title));
            }

            return new PagedList<Article>(
                articles.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                pageNumber,
                pageSize,
                articles.Count);
        }

        private static async Task<IPagedList<MissingPage>> GetMissingAsync(
            IWikiWebOptions options,
            IDataStore dataStore,
            int pageNumber = 1,
            int pageSize = 50,
            bool descending = false,
            string? filter = null)
        {
            var query = dataStore.Query<MissingPage>()
                .Where(x => x.WikiNamespace != options.UserNamespace && x.WikiNamespace != options.GroupNamespace);
            if (!string.IsNullOrEmpty(filter))
            {
#pragma warning disable IDE0057 // Use range operator; Not necessarily implemented by data providers.
                query = query.Where(x => x.Id.Substring(0, x.Id.Length - 8).Contains(filter));
#pragma warning restore IDE0057 // Use range operator
            }

            return await query
                .OrderBy(x => x.Title, descending: descending)
                .GetPageAsync(pageNumber, pageSize)
                .ConfigureAwait(false);
        }

        private static string? GetSecondaryDescription(IWikiOptions wikiOptions, IWikiWebOptions wikiWebOptions, SpecialListType type)
        {
            if (type == SpecialListType.All_Categories
                || type == SpecialListType.All_Files
                || type == SpecialListType.All_Pages)
            {
                if (!string.IsNullOrEmpty(wikiWebOptions.ContentsPageTitle))
                {
                    return $"For a more organized overview you may wish to check the <a href=\"/{wikiOptions.WikiLinkPrefix}/{wikiWebOptions.SystemNamespace}:{wikiWebOptions.ContentsPageTitle}\" class=\"wiki-link wiki-link-exists\">{wikiWebOptions.ContentsPageTitle}</a> page.";
                }
            }
            else if (type == SpecialListType.Uncategorized_Categories)
            {
                var sb = new StringBuilder("Note that top-level categories might show up in this list deliberately, and may not require categorization.");
                if (!string.IsNullOrEmpty(wikiWebOptions.ContentsPageTitle))
                {
                    sb.Append("Top-level categories are typically linked on the <a href=\"/")
                        .Append(wikiOptions.WikiLinkPrefix)
                        .Append('/')
                        .Append(wikiWebOptions.SystemNamespace)
                        .Append(':')
                        .Append(wikiWebOptions.ContentsPageTitle)
                        .Append("\" class=\"wiki-link wiki-link-exists\">")
                        .Append(wikiWebOptions.ContentsPageTitle)
                        .Append("</a>, or in some other prominent place (such as the <a href=\"/")
                        .Append(wikiOptions.WikiLinkPrefix)
                        .Append('/')
                        .Append(wikiOptions.MainPageTitle)
                        .Append("\">")
                        .Append(wikiOptions.MainPageTitle)
                        .Append("</a>).");
                }
                return sb.ToString();
            }
            else if (type == SpecialListType.Unused_Categories)
            {
                return "Note that some categories may be intended to classify articles with problems, and might show up in this list deliberately when no such issues currently exist. These types of categories should not be removed even when empty.";
            }
            return null;
        }
    }
}
