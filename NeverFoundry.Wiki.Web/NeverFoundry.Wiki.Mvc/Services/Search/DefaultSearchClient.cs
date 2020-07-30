using Microsoft.Extensions.Logging;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// <para>
    /// The default search client performs a naive search of the <see cref="WikiConfig.DataStore"/>, looking
    /// for exact string matches of the query in the title and markdown of each item.
    /// </para>
    /// <para>
    /// Although this default is functional, it is neither powerful nor fast, nor does it rank
    /// results intelligently. A more robust search solution is recommended. The default is supplied
    /// only to ensure that search functions when no client is provided.
    /// </para>
    /// </summary>
    public class DefaultSearchClient : ISearchClient
    {
        private readonly ILogger<DefaultSearchClient> _logger;
        private readonly IWikiUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultSearchClient"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> instance.</param>
        /// <param name="userManager">An <see cref="IWikiUserManager"/>.</param>
        public DefaultSearchClient(
            ILogger<DefaultSearchClient> logger,
            IWikiUserManager userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Search for wiki content which matches the given search criteria.
        /// </summary>
        /// <param name="request">
        /// An <see cref="ISearchRequest" /> instance with search criteria.
        /// </param>
        /// <param name="principal">
        /// The <see cref="ClaimsPrincipal" /> making the request.
        /// </param>
        /// <returns>An <see cref="ISearchResult" /> instance with search results.</returns>
        public async Task<ISearchResult> SearchAsync(ISearchRequest request, ClaimsPrincipal principal)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return new SearchResult
                {
                    Descending = request.Descending,
                    Query = request.Query,
                    Sort = request.Sort,
                };
            }

            var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false);
            var groupIds = user?.Groups ?? new List<string>();

            IPagedList<Article> articles;
            System.Linq.Expressions.Expression<Func<Article, bool>> exp = x => x.Owner == null || x.AllowedViewers == null;
            if (user is not null)
            {
                exp = exp.OrElse(x => x.AllowedViewers!.Contains(user.Id)
                    || user.Id.Equals(x.Owner, StringComparison.OrdinalIgnoreCase)
                    || groupIds.Contains(x.Owner!)
                    || x.AllowedViewers!.Any(y => groupIds.Contains(y)));
            }
            exp = exp.AndAlso(x => x.Title.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                || x.MarkdownContent.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            try
            {
                if (string.Equals(request.Sort, "timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    articles = await WikiConfig.DataStore.Query<Article>()
                        .Where(exp)
                        .OrderBy(x => x.TimestampTicks, request.Descending)
                        .GetPageAsync(request.PageNumber, request.PageSize)
                        .ConfigureAwait(false);
                }
                else
                {
                    articles = await WikiConfig.DataStore.Query<Article>()
                        .Where(exp)
                        .OrderBy(x => x.Title, request.Descending)
                        .GetPageAsync(request.PageNumber, request.PageSize)
                        .ConfigureAwait(false);
                }
                var hits = new PagedList<SearchHit>(
                    articles.Select(x => new SearchHit(x.Title, x.WikiNamespace, x.GetPlainText())),
                    articles.PageNumber,
                    articles.PageSize,
                    articles.TotalCount);

                return new SearchResult
                {
                    Descending = request.Descending,
                    Query = request.Query,
                    SearchHits = hits,
                    Sort = request.Sort,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in search for query: {Query}", request.Query);
                return new SearchResult
                {
                    Descending = request.Descending,
                    Query = request.Query,
                    Sort = request.Sort,
                };
            }
        }
    }
}
