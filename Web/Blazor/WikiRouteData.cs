namespace NeverFoundry.Wiki.Blazor
{
    /// <summary>
    /// Describes information determined during routing that specifies the wiki page to be
    /// displayed, as well as query parameters and/or an anchor.
    /// </summary>
    public struct WikiRouteData
    {
        /// <summary>
        /// The anchor requested, if any (may be <see langword="null"/>).
        /// </summary>
        public string? Anchor { get; }

        /// <summary>
        /// The page requested (may be <see langword="null"/> when routing to the main page).
        /// </summary>
        public string? Page { get; }

        /// <summary>
        /// Any query parameters (may be <see langword="null"/>).
        /// </summary>
        public string? QueryParams { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiRouteData"/>.
        /// </summary>
        /// <param name="page">
        /// The page requested (may be <see langword="null"/> when routing to the main page).
        /// </param>
        /// <param name="anchor">
        /// The anchor requested, if any (may be <see langword="null"/>).
        /// </param>
        /// <param name="queryParams">
        /// Any query parameters (may be <see langword="null"/>).
        /// </param>
        public WikiRouteData(string? page, string? anchor, string? queryParams)
        {
            Page = page;
            Anchor = anchor;
            QueryParams = queryParams;
        }
    }
}
