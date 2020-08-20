﻿using NeverFoundry.DataStorage;

namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// The result of a search operation.
    /// </summary>
    public class SearchResult : ISearchResult
    {
        /// <summary>
        /// Whether the results are in descending order.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// The original search query.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// An <see cref="IPagedList{T}" /> of <see cref="ISearchHit" /> representing the results.
        /// </summary>
        public IPagedList<ISearchHit> SearchHits { get; set; } = new PagedList<ISearchHit>(null, 1, 50, 0);

        /// <summary>
        /// The originally specified sort property.
        /// </summary>
        public string? Sort { get; set; }
    }
}