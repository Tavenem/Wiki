using Microsoft.Extensions.Logging;
using Nest;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Mvc.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    /// <summary>
    /// An instance of <see cref="ISearchClient"/> which uses ElasticSearch.
    /// </summary>
    public class ElasticSearchClient : ISearchClient
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticSearchClient> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ElasticSearchClient"/>.
        /// </summary>
        public ElasticSearchClient(
            IElasticClient elasticClient,
            ILogger<ElasticSearchClient> logger)
        {
            _elasticClient = elasticClient;
            _logger = logger;
        }

        /// <summary>
        /// Search for wiki content which matches the given search criteria.
        /// </summary>
        /// <param name="request">
        /// An <see cref="Mvc.Services.Search.ISearchRequest" /> instance with search criteria.
        /// </param>
        /// <param name="user">
        /// The <see cref="IWikiUser" /> making the request.
        /// </param>
        /// <returns>An <see cref="ISearchResult" /> instance with search results.</returns>
        public async Task<ISearchResult> SearchAsync(Mvc.Services.Search.ISearchRequest request, IWikiUser? user)
        {
            var queryEmpty = string.IsNullOrWhiteSpace(request.Query);
            var namespaceEmpty = string.IsNullOrWhiteSpace(request.WikiNamespace);
            var ownerEmpty = string.IsNullOrWhiteSpace(request.Owner);
            if (queryEmpty && namespaceEmpty && ownerEmpty)
            {
                return new SearchResult
                {
                    Descending = request.Descending,
                    Query = request.Query,
                    Sort = request.Sort,
                };
            }

            var groupIds = user?.Groups ?? new List<string>();

            Func<QueryContainerDescriptor<Article>, QueryContainer> query;
            if (user is null)
            {
                if (namespaceEmpty)
                {
                    if (ownerEmpty)
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                            || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                    }
                    else
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                            && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                    }
                }
                else if (queryEmpty)
                {
                    if (ownerEmpty)
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                            || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim());
                    }
                    else
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                            && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim());
                    }
                }
                else if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
            }
            else if (namespaceEmpty)
            {
                if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
            }
            else if (queryEmpty)
            {
                if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
                }
            }
            else if (ownerEmpty)
            {
                query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                    || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                    || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                    || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                    || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                    && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                    && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
            }
            else
            {
                query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                    || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                    || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                    || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                    && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                    && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                    && q.SimpleQueryString(m => m.Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And));
            }

            var count = await _elasticClient
                .CountAsync<Article>(x => x.Query(query))
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

            if (user is null)
            {
                if (namespaceEmpty)
                {
                    if (ownerEmpty)
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                            || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                            || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                    }
                    else
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                            && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                            || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                    }
                }
                else if (queryEmpty)
                {
                    if (ownerEmpty)
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                            || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim());
                    }
                    else
                    {
                        query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                            || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                            && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                            && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim());
                    }
                }
                else if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers)))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
            }
            else if (namespaceEmpty)
            {
                if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
            }
            else if (queryEmpty)
            {
                if (ownerEmpty)
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                        || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
                else
                {
                    query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                        || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                        || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                        && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                        && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                        && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                        || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
                }
            }
            else if (ownerEmpty)
            {
                query = q => (+!q.Exists(m => m.Field(f => f.Owner))
                    || +!q.Exists(m => m.Field(f => f.AllowedEditors))
                    || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                    || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                    || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                    && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                    && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                    || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
            }
            else
            {
                query = q => (+!q.Exists(m => m.Field(f => f.AllowedEditors))
                    || +!q.Exists(m => m.Field(f => f.AllowedViewers))
                    || q.MultiMatch(m => m.Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers)).Query(user.Id).Verbatim())
                    || q.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                    || q.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()))
                    && q.Match(m => m.Field(f => f.WikiNamespace).Query(request.WikiNamespace).Verbatim())
                    && q.Match(m => m.Field(f => f.Owner).Query(request.Owner).Verbatim())
                    && (q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.Title)).Query(request.Query).DefaultOperator(Operator.And).Boost(3))
                    || q.SimpleQueryString(m => m.Fields(f => f.Field(p => p.MarkdownContent)).Query(request.Query).DefaultOperator(Operator.And)));
            }

            var sortDefault = string.CompareOrdinal(request.Sort, "timestamp") != 0;

            var response = await _elasticClient.SearchAsync<Article>(x =>
                x.Query(query)
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
