using Tavenem.Wiki.MarkdownExtensions;

namespace Tavenem.Wiki;

/// <summary>
/// The delegate signature used by <see cref="IWikiOptions.OnCreated"/>.
/// </summary>
/// <param name="article">The new article.</param>
/// <param name="editor">The ID of the editor who created the article.</param>
public delegate ValueTask OnCreatedFunc(Article article, string editor);

/// <summary>
/// The delegate signature used by <see cref="IWikiOptions.OnDeleted"/>.
/// </summary>
/// <param name="article">The deleted article.</param>
/// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
/// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
public delegate ValueTask OnDeletedFunc(Article article, string? oldOwner, string? newOwner);

/// <summary>
/// The delegate signature used by <see cref="IWikiOptions.OnEdited"/>.
/// </summary>
/// <param name="article">The edited article.</param>
/// <param name="revision">The revision applied.</param>
/// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
/// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
public delegate ValueTask OnEditedFunc(Article article, Revision revision, string? oldOwner, string? newOwner);

/// <summary>
/// Various customization and configuration options for the wiki system.
/// </summary>
public interface IWikiOptions
{
    /// <summary>
    /// <para>
    /// The title of the main about page.
    /// </para>
    /// <para>
    /// Default is "About"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the about page.
    /// </para>
    /// </summary>
    string? AboutPageTitle { get; }

    /// <summary>
    /// <para>
    /// An optional collection of namespaces which may not be assigned to pages by non-admin
    /// users.
    /// </para>
    /// <para>
    /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
    /// </para>
    /// </summary>
    IEnumerable<string> AdminNamespaces { get; }

    /// <summary>
    /// <para>
    /// The name of the article on categories in the main wiki.
    /// </para>
    /// <para>
    /// If omitted "Categories" will be used.
    /// </para>
    /// </summary>
    string CategoriesTitle { get; }

    /// <summary>
    /// <para>
    /// The name of the categories namespace.
    /// </para>
    /// <para>
    /// If omitted "Category" will be used.
    /// </para>
    /// </summary>
    string CategoryNamespace { get; }

    /// <summary>
    /// <para>
    /// The title of the main contact page.
    /// </para>
    /// <para>
    /// Default is "Contact"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the contact page.
    /// </para>
    /// </summary>
    string? ContactPageTitle { get; }

    /// <summary>
    /// <para>
    /// The title of the main contents page.
    /// </para>
    /// <para>
    /// Default is "Contents"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the contents page.
    /// </para>
    /// </summary>
    string? ContentsPageTitle { get; }

    /// <summary>
    /// <para>
    /// The title of the main copyright page.
    /// </para>
    /// <para>
    /// Default is "Copyright"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the copyright page and the copyright notice on pages.
    /// </para>
    /// <para>
    /// Consider carefully before omitting this special page, unless you supply an alternate
    /// copyright notice on your wiki.
    /// </para>
    /// </summary>
    string? CopyrightPageTitle { get; }

    /// <summary>
    /// <para>
    /// The name of the default namespace.
    /// </para>
    /// <para>
    /// If omitted "Wiki" will be used.
    /// </para>
    /// </summary>
    string DefaultNamespace { get; }

    /// <summary>
    /// <para>
    /// The default number of levels of nesting shown in an article's table of contents.
    /// </para>
    /// <para>
    /// Can be overridden by specifying the level for a given article.
    /// </para>
    /// </summary>
    int DefaultTableOfContentsDepth { get; }

    /// <summary>
    /// <para>
    /// The default title of tables of contents.
    /// </para>
    /// <para>
    /// If omitted "Contents" will be used.
    /// </para>
    /// </summary>
    string DefaultTableOfContentsTitle { get; }

    /// <summary>
    /// <para>
    /// The name of the file namespace.
    /// </para>
    /// <para>
    /// If omitted "File" will be used.
    /// </para>
    /// </summary>
    string FileNamespace { get; }

    /// <summary>
    /// <para>
    /// The name of the user group namespace.
    /// </para>
    /// <para>
    /// If omitted "Group" is used.
    /// </para>
    /// </summary>
    string GroupNamespace { get; }

    /// <summary>
    /// <para>
    /// The title of the main help page.
    /// </para>
    /// <para>
    /// Default is "Help"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the help page.
    /// </para>
    /// </summary>
    string? HelpPageTitle { get; }

    /// <summary>
    /// <para>
    /// A string added to all wiki links, if non-empty.
    /// </para>
    /// <para>
    /// The string '{LINK}', if included, will be replaced by the full article title being
    /// linked.
    /// </para>
    /// </summary>
    string? LinkTemplate { get; }

    /// <summary>
    /// <para>
    /// The title of the main page (shown when no article title is given).
    /// </para>
    /// <para>
    /// If omitted "Main" will be used.
    /// </para>
    /// </summary>
    string MainPageTitle { get; }

    /// <summary>
    /// <para>
    /// The maximum size (in bytes) of uploaded files.
    /// </para>
    /// <para>
    /// Setting this to a value less than or equal to zero effectively prevents file uploads.
    /// </para>
    /// </summary>
    int MaxFileSize { get; }

    /// <summary>
    /// Gets a string representing the <see cref="MaxFileSize"/> in a reasonable unit (GB for
    /// large sizes, down to bytes for small ones).
    /// </summary>
    public string MaxFileSizeString => MaxFileSize switch
    {
        >= 1000000000 => $"{MaxFileSize / 1000000000.0:N3} GB",
        >= 1000000 => $"{MaxFileSize / 1000000.0:N3} MB",
        >= 1000 => $"{MaxFileSize / 1000.0:G} KB",
        _ => $"{MaxFileSize} bytes"
    };

    /// <summary>
    /// <para>
    /// The minimum number of headings required in an article to display a table of contents by
    /// default.
    /// </para>
    /// <para>
    /// Can be overridden by specifying the location of a table of contents explicitly for a given article.
    /// </para>
    /// </summary>
    int MinimumTableOfContentsHeadings { get; }

    /// <summary>
    /// <para>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is created.
    /// </para>
    /// <para>
    /// Receives the new <see cref="Article"/> as a parameter.
    /// </para>
    /// </summary>
    OnCreatedFunc? OnCreated { get; }

    /// <summary>
    /// <para>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is deleted.
    /// </para>
    /// <para>
    /// Receives the deleted <see cref="Article"/>, the original <see cref="Article.Owner"/>,
    /// and the new <see cref="Article.Owner"/> as parameters.
    /// </para>
    /// </summary>
    OnDeletedFunc? OnDeleted { get; }

    /// <summary>
    /// <para>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is edited (not including deletion if <see
    /// cref="OnDeleted"/> is provided).
    /// </para>
    /// <para>
    /// Receives the edited <see cref="Article"/>, the <see cref="Revision"/> which was applied,
    /// the original <see cref="Article.Owner"/>, and the new <see cref="Article.Owner"/> as
    /// parameters.
    /// </para>
    /// </summary>
    OnEditedFunc? OnEdited { get; }

    /// <summary>
    /// <para>
    /// The title of the main policy page.
    /// </para>
    /// <para>
    /// Default is "Policies"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the policy page.
    /// </para>
    /// </summary>
    string? PolicyPageTitle { get; }

    /// <summary>
    /// A collection of preprocessors which transform the HTML of an article
    /// <i>after</i> it is parsed from markdown but <i>before</i> it is sanitized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Processors are run in the order they are added to the collection.
    /// </para>
    /// <para>
    /// Note that no processors are run if the initial content is empty.
    /// </para>
    /// </remarks>
    IList<IArticleProcessor>? Postprocessors { get; }

    /// <summary>
    /// <para>
    /// An optional collection of namespaces which may not be assigned to pages by users.
    /// </para>
    /// <para>
    /// The namespaces assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/>
    /// are included automatically.
    /// </para>
    /// </summary>
    IEnumerable<string> ReservedNamespaces { get; }

    /// <summary>
    /// <para>
    /// The name of the script namespace.
    /// </para>
    /// <para>
    /// If omitted "Script" will be used.
    /// </para>
    /// </summary>
    string ScriptNamespace { get; }

    /// <summary>
    /// <para>
    /// The name of the wiki. Displayed as a subheading below each article title.
    /// </para>
    /// <para>
    /// If omitted "a NeverFoundry wiki" will be used.
    /// </para>
    /// </summary>
    string SiteName { get; }

    /// <summary>
    /// <para>
    /// The name of the system namespace.
    /// </para>
    /// <para>
    /// If omitted "System" is used.
    /// </para>
    /// </summary>
    string SystemNamespace { get; }

    /// <summary>
    /// <para>
    /// The name of the talk pseudo-namespace.
    /// </para>
    /// <para>
    /// If omitted "Talk" will be used.
    /// </para>
    /// </summary>
    string TalkNamespace { get; }

    /// <summary>
    /// <para>
    /// The name of the transclusion namespace.
    /// </para>
    /// <para>
    /// If omitted "Transclusion" will be used.
    /// </para>
    /// </summary>
    string TransclusionNamespace { get; }

    /// <summary>
    /// <para>
    /// The name of the user namespace.
    /// </para>
    /// <para>
    /// If omitted "User" is used.
    /// </para>
    /// </summary>
    string UserNamespace { get; }

    /// <summary>
    /// <para>
    /// The prefix added before wiki links (to distinguish them from other pages on the same
    /// server).
    /// </para>
    /// <para>
    /// If omitted "Wiki" will be used.
    /// </para>
    /// </summary>
    string WikiLinkPrefix { get; }
}