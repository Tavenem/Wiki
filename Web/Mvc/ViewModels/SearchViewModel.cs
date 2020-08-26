using NeverFoundry.Wiki.Mvc.Services.Search;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The search DTO.
    /// </summary>
    public record SearchViewModel(ISearchResult SearchResult, Article? ExactMatch = null);
}
