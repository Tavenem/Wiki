namespace NeverFoundry.Wiki.Mvc.ViewModels
{
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
}
