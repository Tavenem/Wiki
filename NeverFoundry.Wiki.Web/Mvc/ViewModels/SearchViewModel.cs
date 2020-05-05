using NeverFoundry.Wiki.Mvc.Services.Search;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
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
}
