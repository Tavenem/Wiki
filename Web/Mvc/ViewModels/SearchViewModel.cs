using NeverFoundry.Wiki.Mvc.Services.Search;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The search DTO.
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>
        /// The page which exactly matches the search term, if any.
        /// </summary>
        public Article? ExactMatch { get; set; }

        /// <summary>
        /// The search result.
        /// </summary>
        public ISearchResult SearchResult { get; set; }

        /// <summary>
        /// Initialize a new <see cref="SearchViewModel"/>.
        /// </summary>
        public SearchViewModel(ISearchResult searchResult, Article? exactMatch = null)
        {
            SearchResult = searchResult;
            ExactMatch = exactMatch;
        }
    }
}
