namespace NeverFoundry.Wiki.Web
{
    /// <summary>
    /// Claims related to a web implementation of NeverFoundry.Wiki.
    /// </summary>
    public static class WikiClaims
    {
        /// <summary>
        /// A wiki administrator authorization claim.
        /// </summary>
        public const string Claim_WikiAdmin = "WikiAdmin";

        /// <summary>
        /// A wiki group membership claim.
        /// </summary>
        public const string Claim_WikiGroup = "WikiGroup";
    }
}
