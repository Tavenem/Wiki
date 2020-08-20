using NeverFoundry.Wiki.Mvc.Services.Search;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class SearchViewModel
    {
        public Article? ExactMatch { get; set; }

        public ISearchResult SearchResult { get; set; }

        public SearchViewModel(ISearchResult searchResult, Article? exactMatch = null)
        {
            SearchResult = searchResult;
            ExactMatch = exactMatch;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
