using Microsoft.AspNetCore.Authorization;

namespace NeverFoundry.Wiki.Mvc.Authorization
{
    /// <summary>
    /// Authorization policies for the MVC implementation of NeverFoundry.Wiki.
    /// </summary>
    public static class Policies
    {
        /// <summary>
        /// Requires that the user be authenticated, and have the site admin claim.
        /// </summary>
        public static AuthorizationPolicy IsSiteAdminPolicy()
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(Constants.Claim_SiteAdmin, "true")
            .Build();

        /// <summary>
        /// Requires that the user be authenticated, and have the wiki admin claim.
        /// </summary>
        public static AuthorizationPolicy IsWikiAdminPolicy()
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(Constants.Claim_WikiAdmin, "true")
            .Build();
    }
}
