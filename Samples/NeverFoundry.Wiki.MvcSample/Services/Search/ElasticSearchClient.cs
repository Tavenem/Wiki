using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nest;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Mvc.Services.Search;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Sample.Services
{
    /// <summary>
    /// An instance of <see cref="ISearchClient"/> which uses ElasticSearch.
    /// </summary>
    public class ElasticSearchClient : ISearchClient
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticSearchClient> _logger;
        private readonly UserManager<WikiUser> _userManager;

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticSearchClient"/>.
        /// </summary>
        public ElasticSearchClient(
            IElasticClient elasticClient,
            ILogger<ElasticSearchClient> logger,
            UserManager<WikiUser> userManager)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Search for wiki content which matches the given search criteria.
        /// </summary>
        /// <param name="request">
        /// An <see cref="Mvc.Services.Search.ISearchRequest" /> instance with search criteria.
        /// </param>
        /// <param name="principal">
        /// The <see cref="ClaimsPrincipal" /> making the request.
        /// </param>
        /// <returns>An <see cref="ISearchResult" /> instance with search results.</returns>
        public async Task<ISearchResult> SearchAsync(Mvc.Services.Search.ISearchRequest request, ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal).ConfigureAwait(false);
            if (user is null)
            {
                return new SearchResult();
            }

            var count = await _elasticClient.CountAsync<Article>(x =>
                x.Query(q =>
                    (q.Match(m => m.Field(f => f.AllowedViewers).Query(user.Id).Verbatim())
                    || q.Match(m => m.Field(f => f.Owner).Query(user.Id).Verbatim()))
                    && (q.Match(m => m.Field(f => f.Title).Query(request.Query))
                    || q.Match(m => m.Field(f => f.MarkdownContent).Query(request.Query)))))
                .ConfigureAwait(false);
            if (count?.IsValid != true)
            {
                if (count?.OriginalException != null)
                {
                    _logger.LogError(count.OriginalException, "Exception in Elasticsearch for query: {Query}", request.Query);
                }
                else if (!string.IsNullOrEmpty(count?.ServerError?.Error?.Reason))
                {
                    _logger.LogError("Server error in Elasticsearch for query: {Query}. Error: {Error}", request.Query, count.ServerError.Error.Reason);
                }
                return new SearchResult();
            }

            var sortDefault = string.CompareOrdinal(request.Sort, "timestamp") != 0;

            var response = await _elasticClient.SearchAsync<Article>(x =>
                x.Query(q =>
                    (q.Match(m => m.Field(f => f.AllowedViewers).Query(user.Id).Verbatim())
                    || q.Match(m => m.Field(f => f.Owner).Query(user.Id).Verbatim()))
                    && (q.Match(m => m.Field(f => f.Title).Query(request.Query).Boost(3))
                    || q.Match(m => m.Field(f => f.MarkdownContent).Query(request.Query))))
                .Highlight(h => h.Fields(f => f.Field(ff => ff.MarkdownContent)))
                .Sort(s =>
                {
                    if (sortDefault)
                    {
                        return s.Descending(SortSpecialField.Score);
                    }
                    else if (request.Descending)
                    {
                        return s.Descending(a => a.Timestamp);
                    }
                    else
                    {
                        return s.Ascending(a => a.Timestamp);
                    }
                })
                .From((request.PageNumber - 1) * request.PageSize)
                .Size(request.PageSize))
                .ConfigureAwait(false);
            if (response?.IsValid != true)
            {
                if (response?.OriginalException != null)
                {
                    _logger.LogError(response.OriginalException, "Exception in Elasticsearch for query: {Query}", request.Query);
                }
                else if (response != null && response.TryGetServerErrorReason(out var reason))
                {
                    _logger.LogError("Server error in Elasticsearch for query: {Query}. Error: {Error}", request.Query, reason);
                }
                return new SearchResult();
            }

            return new SearchResult
            {
                Descending = sortDefault || request.Descending,
                Query = request.Query,
                Sort = request.Sort,
                SearchHits = new PagedList<ISearchHit>(
                    response.Hits.Select(x => new SearchHit(x.Source.Title, x.Source.WikiNamespace, x.Highlight.FirstOrDefault().Value?.FirstOrDefault())),
                    request.PageNumber,
                    request.PageSize,
                    response.Total),
            };
        }
    }
}
