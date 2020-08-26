namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The category page DTO.
    /// </summary>
    public record CategoryPageViewModel(string Title, string WikiNamespace)
    {
        /// <summary>
        /// The full title of the page (includes namespace if the namespace is not <see
        /// cref="WikiConfig.DefaultNamespace"/>.
        /// </summary>
        public string FullTitle => Article.GetFullTitle(Title, WikiNamespace);
    }
}
