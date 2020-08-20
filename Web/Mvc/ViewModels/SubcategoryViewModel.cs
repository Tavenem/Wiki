namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The subcategory DTO.
    /// </summary>
    public class SubcategoryViewModel
    {
        /// <summary>
        /// The count of included items.
        /// </summary>
        public long Count { get; }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="SubcategoryViewModel"/>.
        /// </summary>
        public SubcategoryViewModel(string title, long count)
        {
            Title = title;
            Count = count;
        }
    }
}
