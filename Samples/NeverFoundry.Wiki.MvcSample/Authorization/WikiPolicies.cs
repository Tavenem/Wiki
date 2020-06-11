using Microsoft.AspNetCore.Authorization;

namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Authorization policies for a web implementation of NeverFoundry.Wiki.
    /// </summary>
    public static class WikiPolicies
    {
        /// <summary>
        /// Requires that the user be authenticated, and have the wiki admin claim.
        /// </summary>
        public static AuthorizationPolicy IsWikiAdminPolicy()
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(WikiClaims.Claim_WikiAdmin, "true")
            .Build();

        /// <summary>
        /// Requires that the user be authenticated, and have the site admin claim.
        /// </summary>
        public static AuthorizationPolicy IsSiteAdminPolicy()
            => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(WikiMvcClaims.Claim_SiteAdmin, "true")
            .Build();
    }
}
