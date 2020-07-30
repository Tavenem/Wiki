using NeverFoundry.DataStorage;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Options used to configure the wiki system.
    /// </summary>
    public interface IWikiOptions
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
        /// The <see cref="IDataStore"/> to be used by the wiki.
        /// </para>
        /// <para>
        /// If omitted, the static <see cref="DataStorage.DataStore.Instance"/> will be used.
        /// </para>
        /// </summary>
        IDataStore? DataStore { get; set; }

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
        /// <para>
        /// The API key to be used for Tenor GIF integration.
        /// </para>
        /// <para>
        /// Leave <see langword="null"/> (the default) to omit GIF functionality.
        /// </para>
        /// </summary>
        string? TenorAPIKey { get; set; }
    }
}
