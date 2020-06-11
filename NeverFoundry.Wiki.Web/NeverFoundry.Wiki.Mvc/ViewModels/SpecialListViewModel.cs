using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc.Models;
using NeverFoundry.Wiki.Web;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class SpecialListViewModel
    {
        public WikiRouteData Data { get; }

        public bool Descending { get; }

        public string Description { get; }

        public string? Filter { get; }

        public IPagedList<Article> Items { get; }

        public IPagedList<MissingPage>? MissingItems { get; }

        public string? SecondaryDescription { get; }

        public string? Sort { get; }

        public SpecialListType Type { get; }

        public SpecialListViewModel(
            WikiRouteData data,
            SpecialListType type,
            bool descending,
            IPagedList<Article> items,
            IPagedList<MissingPage>? missingItems = null,
            string? sort = null,
            string? filter = null)
        {
            Data = data;
            Descending = descending;
            Description = GetDescription(type, data);
            Filter = filter;
            Items = items;
            MissingItems = missingItems;
            SecondaryDescription = GetSecondaryDescription(type);
            Sort = sort;
            Type = type;
        }

        public static async Task<SpecialListViewModel> NewAsync(
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
                SpecialListType.All_Categories => await GetListAsync<Category>(pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false),
                SpecialListType.All_Files => await GetListAsync<WikiFile>(pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false),
                SpecialListType.All_Pages => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.ArticleType == ArticleType.Article).ConfigureAwait(false),
#pragma warning disable RCS1113 // Use 'string.IsNullOrEmpty' method: not necessarily supported by data provider
                SpecialListType.All_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.RedirectTitle != null && x.RedirectTitle != "").ConfigureAwait(false),
#pragma warning restore RCS1113 // Use 'string.IsNullOrEmpty' method.
                SpecialListType.Broken_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.IsBrokenRedirect).ConfigureAwait(false),
                SpecialListType.Double_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.IsDoubleRedirect).ConfigureAwait(false),
#pragma warning disable RCS1077 // Optimize LINQ method call: Count() is translated to SQL by various data providers (Relinq), while the Count property is not necessarily serialized/recognized
                SpecialListType.Uncategorized_Articles => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.ArticleType == ArticleType.Article && (x.Categories == null || x.Categories.Count() == 0)).ConfigureAwait(false),
                SpecialListType.Uncategorized_Categories => await GetListAsync<Category>(pageNumber, pageSize, sort, descending, filter, x => x.Categories == null || x.Categories.Count() == 0).ConfigureAwait(false),
                SpecialListType.Uncategorized_Files => await GetListAsync<WikiFile>(pageNumber, pageSize, sort, descending, filter, x => x.Categories == null || x.Categories.Count() == 0).ConfigureAwait(false),
                SpecialListType.Unused_Categories => await GetListAsync<Category>(pageNumber, pageSize, sort, descending, filter, x => x.ChildIds.Count() == 0).ConfigureAwait(false),
#pragma warning restore RCS1077 // Optimize LINQ method call.
                SpecialListType.What_Links_Here => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => x.WikiLinks.Any(y => y.Title == data.Title
                    && y.WikiNamespace == data.WikiNamespace
                    && y.IsTalk == data.IsTalk)
                    || (!data.IsTalk && x.Transclusions != null
                    && x.Transclusions.Any(y => y.Title == data.Title && y.WikiNamespace == data.WikiNamespace)))
                    .ConfigureAwait(false),
                _ => new PagedList<Article>(null, 1, pageSize, 0),
            };
            var missing = type == SpecialListType.Missing_Pages
                ? await GetMissingAsync(pageNumber, pageSize, descending, filter).ConfigureAwait(false)
                : null;

            return new SpecialListViewModel(data, type, descending, list, missing, sort, filter);
        }

        private static string GetDescription(SpecialListType type, WikiRouteData data) => type switch
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
            SpecialListType.What_Links_Here => $"The following pages link to {Article.GetFullTitle(data.Title, data.WikiNamespace, data.IsTalk)}.",
            _ => string.Empty,
        };

        private static async Task<IPagedList<T>> GetListAsync<T>(
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null,
            Expression<Func<T, bool>>? condition = null) where T : Article
        {
            var pageCondition = condition;
            if (!string.IsNullOrEmpty(filter))
            {
                if (condition is null)
                {
                    pageCondition = (T x) => x.FullTitle.Contains(filter);
                }
                else
                {
                    Expression<Func<T, bool>> baseExp = x => x.FullTitle.Contains(filter);
                    pageCondition = condition is null ? baseExp : baseExp.AndAlso(condition);
                }
            }

            var query = WikiConfig.DataStore.Query<T>();
            if (!(pageCondition is null))
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

        private static async Task<IPagedList<MissingPage>> GetMissingAsync(
            int pageNumber = 1,
            int pageSize = 50,
            bool descending = false,
            string? filter = null)
        {
            var query = WikiConfig.DataStore.Query<MissingPage>()
                .Where(x => x.WikiNamespace != WikiWebConfig.UserNamespace && x.WikiNamespace != WikiWebConfig.GroupNamespace);
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Title.Contains(filter) || x.WikiNamespace.Contains(filter));
            }

            return await query
                .OrderBy(x => x.Title, descending: descending)
                .GetPageAsync(pageNumber, pageSize)
                .ConfigureAwait(false);
        }

        private static string? GetSecondaryDescription(SpecialListType type)
        {
            if (type == SpecialListType.All_Categories
                || type == SpecialListType.All_Files
                || type == SpecialListType.All_Pages)
            {
                if (!string.IsNullOrEmpty(WikiWebConfig.ContentsPageTitle))
                {
                    return $"For a more organized overview you may wish to check the <a href=\"/Wiki/{WikiWebConfig.SystemNamespace}:{WikiWebConfig.ContentsPageTitle}\" class=\"wiki-link wiki-link-exists\">{WikiWebConfig.ContentsPageTitle}</a> page.";
                }
            }
            else if (type == SpecialListType.Uncategorized_Categories)
            {
                var sb = new StringBuilder("Note that top-level categories might show up in this list deliberately, and may not require categorization.");
                if (!string.IsNullOrEmpty(WikiWebConfig.ContentsPageTitle))
                {
                    sb.Append("Top-level categories are typically linked on the <a href=\"/Wiki/")
                        .Append(WikiWebConfig.SystemNamespace)
                        .Append(":")
                        .Append(WikiWebConfig.ContentsPageTitle)
                        .Append("\" class=\"wiki-link wiki-link-exists\">")
                        .Append(WikiWebConfig.ContentsPageTitle)
                        .Append("</a>, or in some other prominent place (such as the <a href=\"/Wiki/")
                        .Append(WikiConfig.MainPageTitle)
                        .Append("\">")
                        .Append(WikiConfig.MainPageTitle)
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
#pragma warning restore CS1591 // No documentation for "internal" code
}
