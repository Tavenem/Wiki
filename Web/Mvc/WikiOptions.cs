using NeverFoundry.DataStorage;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Options used to configure the wiki system.
    /// </summary>
    public class WikiOptions : IWikiOptions
    {
        /// <summary>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/> used if <see
        /// cref="TalkHubRoute"/> is not provided.
        /// </summary>
        public const string DefaultLayoutPath = "/Views/Wiki/_DefaultWikiMainLayout.cshtml";

        /// <summary>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/> used if <see
        /// cref="TalkHubRoute"/> is not provided.
        /// </summary>
        public const string DefaultTalkHubRoute = "/wikiTalkHub";

        /// <summary>
        /// <para>
        /// The path to the layout used when requesting a compact version of a wiki page. Wiki pages
        /// will be nested within this layout.
        /// </para>
        /// <para>
        /// If omitted, the main layout will be used (as specified in <see cref="MainLayoutPath"/>).
        /// </para>
        /// </summary>
        public string? CompactLayoutPath { get; set; }

        /// <summary>
        /// <para>
        /// The <see cref="IDataStore"/> to be used by the wiki.
        /// </para>
        /// <para>
        /// If omitted, the value already set in <see cref="WikiConfig.DataStore"/> will not be changed.
        /// </para>
        /// </summary>
        public IDataStore? DataStore { get; set; }

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
        /// of the parameter is from a legitimate source to avoid exploits.
        /// </para>
        /// <para>
        /// If this option is omitted, an unauthorized page will be displayed whenever a user who is
        /// not logged in attempts any action which requires an account.
        /// </para>
        /// </summary>
        public string? LoginPath { get; set; }

        /// <summary>
        /// <para>
        /// The path to the main layout for the application. Wiki pages will be nested within this
        /// layout.
        /// </para>
        /// <para>
        /// If omitted, a default layout will be used (as specified in <see cref="DefaultLayoutPath"/>).
        /// </para>
        /// </summary>
        public string? MainLayoutPath { get; set; }

        /// <summary>
        /// <para>
        /// The <see cref="ISearchClient"/> implementation used to search for wiki content.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="Services.Search.DefaultSearchClient"/> will be
        /// used. Note: the default is not recommended for production use.
        /// </para>
        /// </summary>
        public ISearchClient? SearchClient { get; set; }

        /// <summary>
        /// <para>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/>.
        /// </para>
        /// <para>
        /// If omitted, <see cref="DefaultTalkHubRoute"/> is used.
        /// </para>
        /// </summary>
        public string? TalkHubRoute { get; set; }

        /// <summary>
        /// <para>
        /// The API key to be used for Tenor GIF integration.
        /// </para>
        /// <para>
        /// Leave <see langword="null"/> (the default) to omit GIF functionality.
        /// </para>
        /// </summary>
        public string? TenorAPIKey { get; set; }
    }
}
