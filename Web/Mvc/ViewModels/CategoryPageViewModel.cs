namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The category page DTO.
    /// </summary>
    public class CategoryPageViewModel
    {
        /// <summary>
        /// The full title of the page (includes namespace if the namespace is not <see
        /// cref="WikiConfig.DefaultNamespace"/>.
        /// </summary>
        public string FullTitle => Article.GetFullTitle(Title, WikiNamespace);

        /// <summary>
        /// The title of the page.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The namespace of the page.
        /// </summary>
        public string WikiNamespace { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="CategoryPageViewModel"/>.
        /// </summary>
        /// <param name="title">The title of the page.</param>
        /// <param name="wikiNamespace">The namespace of the page.</param>
        public CategoryPageViewModel(string title, string wikiNamespace)
        {
            Title = title;
            WikiNamespace = wikiNamespace;
        }
    }
}
