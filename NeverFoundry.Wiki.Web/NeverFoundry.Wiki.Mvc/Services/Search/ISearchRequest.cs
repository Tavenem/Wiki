namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// An object with search criteria.
    /// </summary>
    public interface ISearchRequest
    {
        /// <summary>
        /// Whether to sort in descending order (rather than ascending).
        /// </summary>
        bool Descending { get; set; }

        /// <summary>
        /// The current page number in a list of results (1-based).
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// The number of results per page.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// The search query text.
        /// </summary>
        string? Query { get; set; }

        /// <summary>
        /// <para>
        /// The field by which to sort results.
        /// </para>
        /// <para>
        /// Note: not all fields may be supported.
        /// </para>
        /// </summary>
        string? Sort { get; set; }
    }
}
