namespace NeverFoundry.Wiki.Mvc.ViewModels
{
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
}
