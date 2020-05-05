using NeverFoundry.DataStorage;

namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// The result of a search operation.
    /// </summary>
    public interface ISearchResult
    {
        /// <summary>
        /// Whether the results are in descending order.
        /// </summary>
        bool Descending { get; set; }

        /// <summary>
        /// The original search query.
        /// </summary>
        string? Query { get; set; }

        /// <summary>
        /// An <see cref="IPagedList{T}"/> of <see cref="ISearchHit"/> representing the results.
        /// </summary>
        IPagedList<ISearchHit> SearchHits { get; set; }

        /// <summary>
        /// The originally specified sort property.
        /// </summary>
        string? Sort { get; set; }
    }
}
