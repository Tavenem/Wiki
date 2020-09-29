using System;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Options used to configure the wiki system.
    /// </summary>
    public class WikiMvcOptions : IWikiMvcOptions
    {
        /// <summary>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/> used if <see
        /// cref="TalkHubRoute"/> is not provided.
        /// </summary>
        public const string DefaultLayoutPath = "/Views/Wiki/_DefaultWikiMainLayout.cshtml";

        /// <summary>
        /// The link template to be used for the MVC wiki system.
        /// </summary>
        public const string DefaultLinkTemplate = "onmousemove=\"wikimvc.showPreview(event, '{LINK}');\" onmouseleave=\"wikimvc.hidePreview();\"";

        /// <summary>
        /// The relative URL of the <see cref="Web.SignalR.IWikiTalkHub"/> used if <see
        /// cref="TalkHubRoute"/> is not provided.
        /// </summary>
        public const string DefaultTalkHubRoute = "/wikiTalkHub";

        /// <summary>
        /// A function which gets the name or path of a partial view which should be displayed after
        /// the content of the given wiki article (before the category list).
        /// </summary>
        public Func<Article, string?>? ArticleEndMatter { get; set; }

        /// <summary>
        /// A function which gets the name or path of a partial view which should be displayed
        /// before the content of the given wiki article (after the subtitle).
        /// </summary>
        public Func<Article, string?>? ArticleFrontMatter { get; set; }

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
        /// The host part which will be recognized as indicating a request for the compact version
        /// of the wiki.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the compact view can only be reached by using the query
        /// parameter "compact".
        /// </para>
        /// </summary>
        public string? CompactRouteHostPart { get; set; }

        /// <summary>
        /// <para>
        /// The position (zero-based) within the parts of the host string which will be examined to
        /// determine a request for the compact version of the wiki.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> position zero will be assumed.
        /// </para>
        /// </summary>
        public int? CompactRouteHostPosition { get; set; }

        /// <summary>
        /// <para>
        /// The port which will be recognized as indicating a request for the compact version of the
        /// wiki.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the compact view cannot be reached at a particular port.
        /// </para>
        /// </summary>
        public int? CompactRoutePort { get; set; }

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

        /// <summary>
        /// Gets the name or path of a partial view which should be displayed after the content of
        /// the given wiki article (before the category list).
        /// </summary>
        /// <param name="article">A wiki article.</param>
        /// <returns>
        /// The name or path of a partial view.
        /// </returns>
        public string? GetArticleEndMatter(Article article) => ArticleEndMatter?.Invoke(article);

        /// <summary>
        /// Gets the name or path of a partial view which should be displayed before the content of
        /// the given wiki article (after the subtitle).
        /// </summary>
        /// <param name="article">A wiki article.</param>
        /// <returns>
        /// The name or path of a partial view.
        /// </returns>
        public string? GetArticleFrontMatter(Article article) => ArticleFrontMatter?.Invoke(article);
    }
}
