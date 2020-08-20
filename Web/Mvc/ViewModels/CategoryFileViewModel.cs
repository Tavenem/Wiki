namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class CategoryFileViewModel
    {
        public int Size { get; }

        public string Title { get; }

        public CategoryFileViewModel(string title, int size)
        {
            Title = title;
            Size = size;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
