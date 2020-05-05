namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class CategoryPageViewModel
    {
        public string FullTitle => Article.GetFullTitle(Title, WikiNamespace);

        public string Title { get; }

        public string WikiNamespace { get; }

        public CategoryPageViewModel(string title, string wikiNamespace)
        {
            Title = title;
            WikiNamespace = wikiNamespace;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
