namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// An object with search criteria.
    /// </summary>
    public class SearchRequest : ISearchRequest
    {
        /// <summary>
        /// Whether to sort in descending order (rather than ascending).
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// <para>
        /// The current page number in a list of results (1-based).
        /// </para>
        /// <para>
        /// Defaults to 1.
        /// </para>
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// <para>
        /// The number of results per page.
        /// </para>
        /// <para>
        /// Defaults to 50.
        /// </para>
        /// </summary>
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// The search query text.
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// <para>
        /// The field by which to sort results.
        /// </para>
        /// <para>
        /// Note: not all fields may be supported.
        /// </para>
        /// </summary>
        public string? Sort { get; set; }
    }
}
