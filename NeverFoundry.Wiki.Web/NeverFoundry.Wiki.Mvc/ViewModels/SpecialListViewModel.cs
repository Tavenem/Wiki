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

        public IPagedList<WikiListItemViewModel>? MissingItems { get; }

        public string? SecondaryDescription { get; }

        public string? Sort { get; }

        public SpecialListType Type { get; }

        public SpecialListViewModel(
            WikiRouteData data,
            SpecialListType type,
            bool descending,
            IPagedList<Article> items,
            IPagedList<WikiListItemViewModel>? missingItems = null,
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
                SpecialListType.All_Pages => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !(x is Category) && !(x is WikiFile)).ConfigureAwait(false),
                SpecialListType.All_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !string.IsNullOrEmpty(x.RedirectTitle)).ConfigureAwait(false),
                SpecialListType.Broken_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !string.IsNullOrEmpty(x.RedirectTitle)
                    && Article.GetArticle(x.RedirectTitle, x.RedirectNamespace) == null).ConfigureAwait(false),
                SpecialListType.Double_Redirects => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !string.IsNullOrEmpty(x.RedirectTitle)
                    && Article.GetArticle(x.RedirectTitle, x.RedirectNamespace) != null
                    && !string.IsNullOrEmpty(Article.GetArticle(x.RedirectTitle, x.RedirectNamespace)!.RedirectTitle)).ConfigureAwait(false),
                SpecialListType.Uncategorized_Articles => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !(x is Category) && !(x is WikiFile) && (x.Categories == null || x.Categories.Count == 0)).ConfigureAwait(false),
                SpecialListType.Uncategorized_Categories => await GetListAsync<Category>(pageNumber, pageSize, sort, descending, filter, x => x.Categories == null || x.Categories.Count == 0).ConfigureAwait(false),
                SpecialListType.Uncategorized_Files => await GetListAsync<WikiFile>(pageNumber, pageSize, sort, descending, filter, x => x.Categories == null || x.Categories.Count == 0).ConfigureAwait(false),
                SpecialListType.Unlinked_Files => await GetListAsync<WikiFile>(pageNumber, pageSize, sort, descending, filter, x => DataStore
                    .GetFirstItemWhere<Article>(y => y.WikiLinks.Any(z => z.IsLinkMatch(x))) == null).ConfigureAwait(false),
                SpecialListType.Unlinked_Pages => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => !(x is Category) && !(x is WikiFile) && DataStore
                    .GetFirstItemWhere<Article>(y => y.WikiLinks.Any(z => z.IsLinkMatch(x))) == null).ConfigureAwait(false),
                SpecialListType.Unused_Categories => await GetListAsync<Category>(pageNumber, pageSize, sort, descending, filter, x => x.ChildIds.Count == 0).ConfigureAwait(false),
                SpecialListType.What_Links_Here => await GetListAsync<Article>(pageNumber, pageSize, sort, descending, filter, x => ArticleLinksHere(x, data)).ConfigureAwait(false),
                _ => new PagedList<Article>(null, 1, pageSize, 0),
            };
            var missing = type == SpecialListType.Missing_Pages
                ? await GetMissingAsync(pageNumber, pageSize, descending, filter).ConfigureAwait(false)
                : null;

            return new SpecialListViewModel(data, type, descending, list, missing, sort, filter);
        }

        private static bool ArticleLinksHere(Article article, WikiRouteData data)
            => article.WikiLinks.Any(y => string.Equals(y.Title, data.Title, StringComparison.OrdinalIgnoreCase)
            && string.Equals(y.WikiNamespace, data.WikiNamespace, StringComparison.OrdinalIgnoreCase)
            && y.IsTalk == data.IsTalk)
            || (!data.IsTalk && article.Transclusions.Any(y =>
            {
                var (wikiNamespace, title, _, __) = Article.GetTitleParts(y);
                return string.Equals(title, data.Title, StringComparison.OrdinalIgnoreCase)
                && string.Equals(wikiNamespace, data.WikiNamespace, StringComparison.OrdinalIgnoreCase);
            }));

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
            SpecialListType.Unlinked_Files => "This page lists all files which are not linked from any articles, either alphabetically or by most recent update.",
            SpecialListType.Unlinked_Pages => "This page lists all articles which are not linked from any other articles, either alphabetically or by most recent update.",
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

            IPagedList<T> list;
            if (pageCondition is null)
            {
                if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    list = await DataStore
                        .GetPageOrderedByAsync<T, long>(x => x.TimestampTicks, pageNumber, pageSize, descending: descending)
                        .ConfigureAwait(false);
                }
                else
                {
                    list = await DataStore
                        .GetPageOrderedByAsync<T, string>(x => x.Title, pageNumber, pageSize, descending: descending)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                if (string.Equals(sort, "timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    list = await DataStore
                        .GetPageWhereOrderedByAsync(pageCondition, x => x.TimestampTicks, pageNumber, pageSize, descending: descending)
                        .ConfigureAwait(false);
                }
                else
                {
                    list = await DataStore
                        .GetPageWhereOrderedByAsync(pageCondition, x => x.Title, pageNumber, pageSize, descending: descending)
                        .ConfigureAwait(false);
                }
            }

            return list;
        }

        private static async Task<IPagedList<WikiListItemViewModel>> GetMissingAsync(
            int pageNumber = 1,
            int pageSize = 50,
            bool descending = false,
            string? filter = null)
        {
            Func<WikiLink, bool> condition;
            if (string.IsNullOrEmpty(filter))
            {
                condition = x => Article.GetArticle(x.Title, x.WikiNamespace) is null;
            }
            else
            {
                condition = x => x.FullTitle.Contains(filter)
                    && Article.GetArticle(x.Title, x.WikiNamespace) is null;
            }

            var result = DataStore
                .GetItemsAsync<Article>()
                .SelectMany(x => x.WikiLinks.ToAsyncEnumerable())
                .Where(condition)
                .Distinct()
                .Select(x => new WikiListItemViewModel(x.Title, x.WikiNamespace));
            if (descending)
            {
                result = result.OrderByDescending(x => x.Title);
            }
            else
            {
                result = result.OrderBy(x => x.Title);
            }
            var total = await result.LongCountAsync().ConfigureAwait(false);
            return new PagedList<WikiListItemViewModel>(
                await result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false),
                pageNumber,
                pageSize,
                total);
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
