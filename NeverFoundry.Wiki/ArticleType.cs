namespace NeverFoundry.Wiki
{
    /// <summary>
    /// The type of page represented by an <see cref="Article"/>.
    /// </summary>
    public enum ArticleType
    {
        /// <summary>
        /// A standard <see cref="Wiki.Article"/>.
        /// </summary>
        Article = 0,

        /// <summary>
        /// A <see cref="Wiki.Category"/>.
        /// </summary>
        Category = 1,

        /// <summary>
        /// A <see cref="WikiFile"/>.
        /// </summary>
        File = 2,
    }
}
