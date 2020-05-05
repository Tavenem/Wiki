namespace NeverFoundry.Wiki.Mvc.ViewModels
{
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
}
