namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Options used to configure the wiki system.
    /// </summary>
    public interface IWikiMvcOptions
    {
        /// <summary>
        /// <para>
        /// The path to the layout used when requesting a compact version of a wiki page. Wiki pages
        /// will be nested within this layout.
        /// </para>
        /// <para>
        /// If omitted, a default layout will be used.
        /// </para>
        /// </summary>
        string? CompactLayoutPath { get; set; }

        /// <summary>
        /// <para>
        /// The host part which will be recognized as indicating a request for the compact version
        /// of the wiki.
        /// </para>
        /// <para>
        /// If left empty the compact view cannot be reached at a particular host path.
        /// </para>
        /// </summary>
        string? CompactRouteHostPart { get; set; }

        /// <summary>
        /// <para>
        /// The position (zero-based) within the parts of the host string which will be examined to
        /// determine a request for the compact version of the wiki.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> position zero will be assumed.
        /// </para>
        /// <para>
        /// Only used when <see cref="CompactRouteHostPart"/> is non-empty.
        /// </para>
        /// </summary>
        int? CompactRouteHostPosition { get; set; }

        /// <summary>
        /// <para>
        /// The port which will be recognized as indicating a request for the compact version of the
        /// wiki.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the compact view cannot be reached at a particular port.
        /// </para>
        /// </summary>
        int? CompactRoutePort { get; set; }

        /// <summary>
        /// <para>
        /// The relative path to the site's login page.
        /// </para>
        /// <para>
        /// For security reasons, only a local path is permitted. If your authentication mechanisms
        /// are handled externally, this should point to a local page which redirects to that source
        /// (either automatically or via interaction).
        /// </para>
        /// <para>
        /// A query parameter with the name "returnUrl" whose value is set to the page which
        /// initiated the logic request will be appended to this URL (if provided). Your login page
        /// may ignore this parameter, but to improve user experience it should redirect the user
        /// back to this URL after performing a successful login. Be sure to validate that the value
        /// of the parameter is from a ligetimate source to avoid exploits.
        /// </para>
        /// <para>
        /// If this option is omitted, a generic "not signed in" message will be displayed whenever
        /// a user who is not logged in attempts any action which requires an account.
        /// </para>
        /// </summary>
        string? LoginPath { get; set; }

        /// <summary>
        /// <para>
        /// The path to the main layout for the application. Wiki pages will be nested within this
        /// layout.
        /// </para>
        /// <para>
        /// If omitted, a default layout will be used.
        /// </para>
        /// </summary>
        string? MainLayoutPath { get; set; }

        /// <summary>
        /// <para>
        /// The <see cref="ISearchClient"/> implementation used to search for wiki content.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="Services.Search.DefaultSearchClient"/> will be
        /// used. Note: the default is not recommended for production use.
        /// </para>
        /// </summary>
        ISearchClient? SearchClient { get; set; }

        /// <summary>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/>.
        /// </summary>
        string? TalkHubRoute { get; set; }

        /// <summary>
        /// <para>
        /// The API key to be used for Tenor GIF integration.
        /// </para>
        /// <para>
        /// Leave <see langword="null"/> (the default) to omit GIF functionality.
        /// </para>
        /// </summary>
        string? TenorAPIKey { get; set; }

        /// <summary>
        /// Gets the name or path of a partial view which should be displayed after the content of
        /// the given wiki article (before the category list).
        /// </summary>
        /// <param name="article">A wiki article.</param>
        /// <returns>
        /// The name or path of a partial view.
        /// </returns>
        string? GetArticleEndMatter(Article article);

        /// <summary>
        /// Gets the name or path of a partial view which should be displayed before the content of
        /// the given wiki article (after the subtitle).
        /// </summary>
        /// <param name="article">A wiki article.</param>
        /// <returns>
        /// The name or path of a partial view.
        /// </returns>
        string? GetArticleFrontMatter(Article article);
    }
}
