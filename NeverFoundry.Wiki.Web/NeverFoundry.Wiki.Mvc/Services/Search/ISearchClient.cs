using NeverFoundry.Wiki.Mvc.Services.Search;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// A service which performs searches for wiki content.
    /// </summary>
    public interface ISearchClient
    {
        /// <summary>
        /// Search for wiki content which matches the given search criteria.
        /// </summary>
        /// <param name="request">
        /// An <see cref="ISearchRequest"/> instance with search criteria.
        /// </param>
        /// <param name="principal">
        /// The <see cref="ClaimsPrincipal"/> making the request.
        /// </param>
        /// <returns>An <see cref="ISearchResult"/> instance with search results.</returns>
        Task<ISearchResult> SearchAsync(ISearchRequest request, ClaimsPrincipal principal);
    }
}
