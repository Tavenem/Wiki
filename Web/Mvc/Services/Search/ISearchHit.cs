namespace NeverFoundry.Wiki.Mvc.Services.Search
{
    /// <summary>
    /// A search hit.
    /// </summary>
    public interface ISearchHit
    {
        /// <summary>
        /// An excerpt from the matching article (optional).
        /// </summary>
        string? Excerpt { get; set; }

        /// <summary>
        /// Gets the full title of this item (including namespace if the namespace is not
        /// <see cref="IWikiOptions.DefaultNamespace"/>).
        /// </summary>
        public string FullTitle { get; set; }

        /// <summary>
        /// The title of the matching wiki item.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The namespace of the matching wiki item.
        /// </summary>
        string WikiNamespace { get; set; }
    }
}