﻿using Microsoft.Extensions.Logging;
using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultSearchClient"/>.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> instance.</param>
        public DefaultSearchClient(ILogger<DefaultSearchClient> logger) => _logger = logger;

        /// <summary>
        /// Search for wiki content which matches the given search criteria.
        /// </summary>
        /// <param name="request">
        /// An <see cref="ISearchRequest" /> instance with search criteria.
        /// </param>
        /// <param name="user">
        /// The <see cref="IWikiUser" /> making the request.
        /// </param>
        /// <returns>An <see cref="ISearchResult" /> instance with search results.</returns>
        public async Task<ISearchResult> SearchAsync(ISearchRequest request, IWikiUser? user)
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

            var namespaces = namespaceEmpty
                ? Array.Empty<string>()
                : request.WikiNamespace!.Split(';');
            var excludedNamespaces = namespaces
                .Where(x => x[0] == '!')
                .Select(x => x[1..])
                .ToList();
            var anyExcludedNamespaces = excludedNamespaces.Count > 0;
            var includedNamespaces = namespaces
                .Where(x => x[0] != '!')
                .ToList();
            var anyIncludedNamespaces = includedNamespaces.Count > 0;

            var owners = ownerEmpty
                ? Array.Empty<string>()
                : request.Owner!.Split(';');
            var excludedOwners = owners
                .Where(x => x[0] == '!')
                .Select(x => x[1..])
                .ToList();
            var anyExcludedOwners = excludedOwners.Count > 0;
            var includedOwners = owners
                .Where(x => x[0] != '!')
                .ToList();
            var anyIncludedOwners = includedOwners.Count > 0;

            System.Linq.Expressions.Expression<Func<Article, bool>> exp = queryEmpty
                ? x => true
                : x => x.Title.Contains(request.Query!, StringComparison.OrdinalIgnoreCase)
                || x.MarkdownContent.Contains(request.Query!, StringComparison.OrdinalIgnoreCase);

            if (anyIncludedNamespaces)
            {
                exp = exp.AndAlso(x => includedNamespaces.Contains(x.WikiNamespace));
            }
            if (anyExcludedNamespaces)
            {
                exp = exp.AndAlso(x => !excludedNamespaces.Contains(x.WikiNamespace));
            }

            if (anyIncludedOwners)
            {
                exp = exp.AndAlso(x => x.Owner != null && includedOwners.Contains(x.Owner));
            }
            if (anyExcludedOwners)
            {
                exp = exp.AndAlso(x => x.Owner == null || !excludedOwners.Contains(x.Owner));
            }

            if (user is null)
            {
                if (anyIncludedOwners)
                {
                    exp = exp.AndAlso(x => x.AllowedEditors == null || x.AllowedViewers == null);
                }
                else
                {
                    exp = exp.AndAlso(x => x.Owner == null || x.AllowedEditors == null || x.AllowedViewers == null);
                }
            }
            else if (!user.IsWikiAdmin)
            {
                var groupIds = user.Groups ?? new List<string>();
                if (anyIncludedOwners)
                {
                    exp = exp.AndAlso(x => x.AllowedEditors == null
                        || x.AllowedViewers == null
                        || user.Id == x.Owner
                        || x.AllowedEditors.Contains(user.Id)
                        || x.AllowedViewers.Contains(user.Id)
                        || groupIds.Contains(x.Owner!)
                        || x.AllowedEditors.Any(y => groupIds.Contains(y))
                        || x.AllowedViewers.Any(y => groupIds.Contains(y)));
                }
                else
                {
                    exp = exp.AndAlso(x => x.Owner == null
                        || x.AllowedEditors == null
                        || x.AllowedViewers == null
                        || user.Id == x.Owner
                        || x.AllowedEditors.Contains(user.Id)
                        || x.AllowedViewers.Contains(user.Id)
                        || groupIds.Contains(x.Owner)
                        || x.AllowedEditors.Any(y => groupIds.Contains(y))
                        || x.AllowedViewers.Any(y => groupIds.Contains(y)));
                }
            }

            try
            {
                var query = WikiConfig.DataStore.Query<Article>().Where(exp);
                if (string.Equals(request.Sort, "timestamp", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(x => x.TimestampTicks, request.Descending);
                }
                else
                {
                    query = query.OrderBy(x => x.Title, request.Descending);
                }
                var articles = await query.GetPageAsync(request.PageNumber, request.PageSize)
                    .ConfigureAwait(false);

                var hits = new PagedList<SearchHit>(
                    articles.Select(x => new SearchHit(
                        x.Title,
                        x.WikiNamespace,
                        queryEmpty
                            ? x.GetPlainText()
                            : Regex.Replace(
                                x.GetPlainText(x.MarkdownContent[
                                    Math.Max(0, x.MarkdownContent.LastIndexOf(
                                        ' ',
                                        Math.Max(0, x.MarkdownContent.LastIndexOf(
                                            ' ',
                                            Math.Max(0, x.MarkdownContent.IndexOf(
                                                request.Query!,
                                                StringComparison.OrdinalIgnoreCase))) - 1)))..]),
                                $"({Regex.Escape(request.Query!)})",
                                "<strong class=\"wiki-search-hit\">$1</strong>",
                                RegexOptions.IgnoreCase))),
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
