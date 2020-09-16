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
            var uploaderEmpty = string.IsNullOrWhiteSpace(request.Uploader);
            if (queryEmpty && namespaceEmpty && ownerEmpty && uploaderEmpty)
            {
                return new SearchResult
                {
                    Descending = request.Descending,
                    Query = request.Query,
                    Sort = request.Sort,
                };
            }

            var namespaces = namespaceEmpty
                ? Array.Empty<string>()
                : request.WikiNamespace!.Split(';');
            var excludedNamespaces = namespaces
                .Where(x => x[0] == '!')
                .Select(x => x[1..])
                .ToList();
            var anyExcludedNamespaces = excludedNamespaces.Count > 0;
            var excludedNamespaceString = anyExcludedNamespaces
                ? string.Empty
                : string.Join(';', excludedNamespaces);
            var includedNamespaces = namespaces
                .Where(x => x[0] != '!')
                .ToList();
            var anyIncludedNamespaces = includedNamespaces.Count > 0;
            var includedNamespaceString = anyIncludedNamespaces
                ? string.Empty
                : string.Join(';', includedNamespaces);

            var owners = ownerEmpty
                ? Array.Empty<string>()
                : request.Owner!.Split(';');
            var excludedOwners = owners
                .Where(x => x[0] == '!')
                .Select(x => x[1..])
                .ToList();
            var anyExcludedOwners = excludedOwners.Count > 0;
            var excludedOwnerString = anyExcludedOwners
                ? string.Empty
                : string.Join(';', excludedOwners);
            var includedOwners = owners
                .Where(x => x[0] != '!')
                .ToList();
            var anyIncludedOwners = includedOwners.Count > 0;
            var includedOwnerString = anyIncludedOwners
                ? string.Empty
                : string.Join(';', includedOwners);

            var uploaders = uploaderEmpty
                ? Array.Empty<string>()
                : request.Uploader!.Split(';');
            var excludedUploaders = uploaders
                .Where(x => x[0] == '!')
                .Select(x => x[1..])
                .ToList();
            var anyExcludedUploaders = excludedUploaders.Count > 0;
            var excludedUploaderString = anyExcludedUploaders
                ? string.Empty
                : string.Join(';', excludedUploaders);
            var includedUploaders = uploaders
                .Where(x => x[0] != '!')
                .ToList();
            var anyIncludedUploaders = includedUploaders.Count > 0;
            var includedUploaderString = anyIncludedUploaders
                ? string.Empty
                : string.Join(';', includedUploaders);

            QueryContainer query = new MatchAllQuery();

            if (anyIncludedNamespaces)
            {
                query = query && Query<Article>.Match(m => m.Field(f => f.WikiNamespace).Query(includedNamespaceString).Verbatim());
            }
            if (anyExcludedNamespaces)
            {
                query = query && !Query<Article>.Match(m => m.Field(f => f.WikiNamespace).Query(excludedNamespaceString).Verbatim());
            }

            if (anyIncludedOwners)
            {
                query = query && Query<Article>.Match(m => m.Field(f => f.Owner).Query(includedOwnerString).Verbatim());
            }
            if (anyExcludedOwners)
            {
                query = query && !Query<Article>.Match(m => m.Field(f => f.Owner).Query(excludedOwnerString).Verbatim());
            }

            if (anyIncludedUploaders)
            {
                query = query && Query<WikiFile>.Match(m => m.Field(f => f.Uploader).Query(includedUploaderString).Verbatim());
            }
            if (anyExcludedUploaders)
            {
                query = query && !Query<WikiFile>.Match(m => m.Field(f => f.Uploader).Query(excludedUploaderString).Verbatim());
            }

            if (user is null)
            {
                if (anyIncludedOwners)
                {
                    query = query && (+!Query<Article>.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedViewers)));
                }
                else
                {
                    query = query && (+!Query<Article>.Exists(m => m.Field(f => f.Owner))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedViewers)));
                }
            }
            else if (!user.IsWikiAdmin)
            {
                var groupIds = user.Groups ?? new List<string>();
                if (anyIncludedOwners)
                {
                    query = query && (+!Query<Article>.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedViewers))
                        || Query<Article>.MultiMatch(m => m
                            .Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers))
                            .Query(user.Id)
                            .Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()));
                }
                else
                {
                    query = query && (+!Query<Article>.Exists(m => m.Field(f => f.Owner))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedEditors))
                        || +!Query<Article>.Exists(m => m.Field(f => f.AllowedViewers))
                        || Query<Article>.MultiMatch(m => m
                            .Fields(f => f.Fields(p => p.Owner, p => p.AllowedEditors, p => p.AllowedViewers))
                            .Query(user.Id)
                            .Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.Owner).Terms(groupIds).Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.AllowedEditors).Terms(groupIds).Verbatim())
                        || Query<Article>.Terms(m => m.Field(f => f.AllowedViewers).Terms(groupIds).Verbatim()));
                }
            }

            var countQuery = query && Query<Article>.SimpleQueryString(m => m
                .Fields(f => f.Fields(p => p.Title, p => p.MarkdownContent))
                .Query(request.Query)
                .DefaultOperator(Operator.And));

            var count = await _elasticClient
                .CountAsync<Article>(x => x.Query(_ => countQuery))
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

            query = query && (Query<Article>.SimpleQueryString(m => m
                .Fields(f => f.Field(p => p.Title))
                .Query(request.Query)
                .DefaultOperator(Operator.And)
                .Boost(3))
                || Query<Article>.SimpleQueryString(m => m
                .Fields(f => f.Field(p => p.MarkdownContent))
                .Query(request.Query)
                .DefaultOperator(Operator.And)));

            var sortDefault = string.CompareOrdinal(request.Sort, "timestamp") != 0;

            var response = await _elasticClient.SearchAsync<Article>(x =>
                x.Query(_ => query)
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
