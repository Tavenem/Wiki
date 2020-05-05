namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
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
#pragma warning restore CS1591 // No documentation for "internal" code
}
