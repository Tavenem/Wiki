namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class WikiListItemViewModel
    {
        public string FullTitle => Article.GetFullTitle(Title, WikiNamespace);

        public string Title { get; }

        public virtual string? WikiNamespace { get; }

        public WikiListItemViewModel(string title, string? wikiNamespace = null)
        {
            Title = title;
            WikiNamespace = wikiNamespace;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
