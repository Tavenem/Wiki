using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nest;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Mvc.Services.Search;
using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
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
            var claims = user is null
                ? new List<Claim>()
                : await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var groupIds = claims
                .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                .Select(x => x.Value)
                .ToList();

            CountResponse? count;
            if (user is null)
            {
                count = await _elasticClient.CountAsync<Article>(x =>
                    x.Query(q =>
                        +!q.Exists(m => m.Field(f => f.Owner))
                        && +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And))))
                    .ConfigureAwait(false);
            }
            else
            {
                count = await _elasticClient.CountAsync<Article>(x =>
                    x.Query(q =>
                        ((+!q.Exists(m => m.Field(f => f.Owner))
                        && +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.AllowedViewers, p => p.Owner)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And))))
                    .ConfigureAwait(false);
            }

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

            ISearchResponse<Article>? response;
            if (user is null)
            {
                response = await _elasticClient.SearchAsync<Article>(x =>
                    x.Query(q =>
                        +!q.Exists(m => m.Field(f => f.Owner))
                        && +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And))))
                    .Highlight(h =>
                        h.Fields(f =>
                            f.Field(ff => ff.MarkdownContent)
                            .FragmentSize(200)
                            .NoMatchSize(200))
                        .PreTags("#H#H#")
                        .PostTags("%H%H%"))
                    .Sort(s =>
                    {
                        if (sortDefault)
                        {
                            return s.Descending(SortSpecialField.Score);
                        }
                        else if (request.Descending)
                        {
                            return s.Descending(a => a.TimestampTicks);
                        }
                        else
                        {
                            return s.Ascending(a => a.TimestampTicks);
                        }
                    })
                    .From((request.PageNumber - 1) * request.PageSize)
                    .Size(request.PageSize))
                    .ConfigureAwait(false);
            }
            else
            {
                response = await _elasticClient.SearchAsync<Article>(x =>
                    x.Query(q =>
                        ((+!q.Exists(m => m.Field(f => f.Owner))
                        && +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.AllowedViewers, p => p.Owner)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And))))
                    .Highlight(h =>
                        h.Fields(f =>
                            f.Field(ff => ff.MarkdownContent)
                            .FragmentSize(200)
                            .NoMatchSize(200))
                        .PreTags("#H#H#")
                        .PostTags("%H%H%"))
                    .Sort(s =>
                    {
                        if (sortDefault)
                        {
                            return s.Descending(SortSpecialField.Score);
                        }
                        else if (request.Descending)
                        {
                            return s.Descending(a => a.TimestampTicks);
                        }
                        else
                        {
                            return s.Ascending(a => a.TimestampTicks);
                        }
                    })
                    .From((request.PageNumber - 1) * request.PageSize)
                    .Size(request.PageSize))
                    .ConfigureAwait(false);
            }
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
                    response.Hits.Select(x => new SearchHit(
                        x.Source.Title,
                        x.Source.WikiNamespace,
                        x.Source.GetPlainText(x.Highlight.FirstOrDefault().Value?.FirstOrDefault())
                        .Replace("#H#H#", "<strong class=\"wiki-search-hit\">")
                        .Replace("%H%H%", "</strong>"))),
                    request.PageNumber,
                    request.PageSize,
                    response.Total),
            };
        }
    }
}
