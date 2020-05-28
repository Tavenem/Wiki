﻿namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Claims related to a web implementation of NeverFoundry.Wiki.
    /// </summary>
    public static class WikiClaims
    {
        /// <summary>
        /// A site administrator authorization claim.
        /// </summary>
        public const string Claim_SiteAdmin = "SiteAdmin";

        /// <summary>
        /// A wiki administrator authorization claim.
        /// </summary>
        public const string Claim_WikiAdmin = "WikiAdmin";
    }
}