namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    public class SubcategoryViewModel
    {
        public long Count { get; }

        public string Title { get; }

        public SubcategoryViewModel(string title, long count)
        {
            Title = title;
            Count = count;
        }
    }
}
