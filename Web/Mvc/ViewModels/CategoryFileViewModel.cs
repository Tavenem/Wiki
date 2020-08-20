namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The category file DTO
    /// </summary>
    public class CategoryFileViewModel
    {
        /// <summary>
        /// The size of the file.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The title of the file.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="CategoryFileViewModel"/>.
        /// </summary>
        /// <param name="title">The title of the file.</param>
        /// <param name="size">The size of the file.</param>
        public CategoryFileViewModel(string title, int size)
        {
            Title = title;
            Size = size;
        }
    }
}
