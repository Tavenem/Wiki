using System.Diagnostics.CodeAnalysis;
using Tavenem.Wiki.MarkdownExtensions;

namespace Tavenem.Wiki;

/// <summary>
/// Various customization and configuration options for the wiki system.
/// </summary>
public class WikiOptions : IWikiOptions
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
    public string? AboutPageTitle { get; set; } = "About";

    private List<string>? _adminNamespaces;
    /// <summary>
    /// <para>
    /// An optional collection of namespaces which may not be assigned to pages by non-admin
    /// users.
    /// </para>
    /// <para>
    /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
    /// </para>
    /// </summary>
    public IEnumerable<string> AdminNamespaces => (_adminNamespaces ?? Enumerable.Empty<string>())
        .Concat(new[] { SystemNamespace });

    private const string CategoriesTitleDefault = "Categories";
    private string _categoriesTitle = CategoriesTitleDefault;
    /// <summary>
    /// <para>
    /// The name of the article on categories in the main wiki.
    /// </para>
    /// <para>
    /// If omitted "Categories" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string CategoriesTitle
    {
        get => _categoriesTitle;
        set => _categoriesTitle = string.IsNullOrWhiteSpace(value)
            ? CategoriesTitleDefault
            : value;
    }

    private const string CategoryNamespaceDefault = "Category";
    private string _categoryNamespace = CategoryNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the categories namespace.
    /// </para>
    /// <para>
    /// If omitted "Category" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string CategoryNamespace
    {
        get => _categoryNamespace;
        set => _categoryNamespace = string.IsNullOrWhiteSpace(value)
            ? CategoryNamespaceDefault
            : value;
    }

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
    public string? ContactPageTitle { get; set; } = "Contact";

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
    public string? ContentsPageTitle { get; set; } = "Contents";

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
    public string? CopyrightPageTitle { get; set; } = "Copyright";

    private const string DefaultNamespaceDefault = "Wiki";
    private string _defaultNamespace = DefaultNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the default namespace.
    /// </para>
    /// <para>
    /// If omitted "Wiki" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string DefaultNamespace
    {
        get => _defaultNamespace;
        set => _defaultNamespace = string.IsNullOrWhiteSpace(value)
            ? DefaultNamespaceDefault
            : value;
    }

    /// <summary>
    /// <para>
    /// The default number of levels of nesting shown in an article's table of contents.
    /// </para>
    /// <para>
    /// Can be overridden by specifying the level for a given article.
    /// </para>
    /// </summary>
    public int DefaultTableOfContentsDepth { get; set; } = 3;

    private const string DefaultTableOfContentsTitleDefault = "Contents";
    private string _defaultTableOfContentsTitle = DefaultTableOfContentsTitleDefault;
    /// <summary>
    /// <para>
    /// The default title of tables of contents.
    /// </para>
    /// <para>
    /// If omitted "Contents" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string DefaultTableOfContentsTitle
    {
        get => _defaultTableOfContentsTitle;
        set => _defaultTableOfContentsTitle = string.IsNullOrWhiteSpace(value)
            ? DefaultTableOfContentsTitleDefault
            : value;
    }

    private const string FileNamespaceDefault = "File";
    private string _fileNamespace = FileNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the file namespace.
    /// </para>
    /// <para>
    /// If omitted "File" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string FileNamespace
    {
        get => _fileNamespace;
        set => _fileNamespace = string.IsNullOrWhiteSpace(value)
            ? FileNamespaceDefault
            : value;
    }

    private const string GroupNamespaceDefault = "Group";
    private string _groupNamespace = GroupNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the user group namespace.
    /// </para>
    /// <para>
    /// If omitted "Group" is used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string GroupNamespace
    {
        get => _groupNamespace;
        set => _groupNamespace = string.IsNullOrWhiteSpace(value)
            ? GroupNamespaceDefault
            : value;
    }

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
    public string? HelpPageTitle { get; set; } = "Help";

    /// <summary>
    /// <para>
    /// A string added to all wiki links, if non-empty.
    /// </para>
    /// <para>
    /// The string '{LINK}', if included, will be replaced by the full article title being
    /// linked.
    /// </para>
    /// </summary>
    public string? LinkTemplate { get; set; }

    private const string MainPageTitleDefault = "Main";
    private string _mainPageTitle = MainPageTitleDefault;
    /// <summary>
    /// <para>
    /// The title of the main page (shown when no article title is given).
    /// </para>
    /// <para>
    /// If omitted "Main" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string MainPageTitle
    {
        get => _mainPageTitle;
        set => _mainPageTitle = string.IsNullOrWhiteSpace(value)
            ? MainPageTitleDefault
            : value;
    }

    /// <summary>
    /// <para>
    /// The maximum size (in bytes) of uploaded files.
    /// </para>
    /// <para>
    /// Default is 5 MB.
    /// </para>
    /// <para>
    /// Setting this to a value less than or equal to zero effectively prevents file uploads.
    /// </para>
    /// </summary>
    public int MaxFileSize { get; set; } = 5000000; // 5 MB

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
    public int MinimumTableOfContentsHeadings { get; set; } = 3;

    /// <summary>
    /// <para>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is created.
    /// </para>
    /// <para>
    /// Receives the new <see cref="Article"/> as a parameter.
    /// </para>
    /// </summary>
    public OnCreatedFunc? OnCreated { get; set; }

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
    public OnDeletedFunc? OnDeleted { get; set; }

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
    public OnEditedFunc? OnEdited { get; set; }

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
    public string? PolicyPageTitle { get; set; } = "Policies";

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
    public IList<IArticleProcessor>? Postprocessors { get; set; } = new List<IArticleProcessor>();

    private List<string>? _reservedNamespaces;
    /// <summary>
    /// <para>
    /// An optional collection of namespaces which may not be assigned to pages by users.
    /// </para>
    /// <para>
    /// The namespaces assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/>
    /// are included automatically.
    /// </para>
    /// </summary>
    public IEnumerable<string> ReservedNamespaces => (_reservedNamespaces ?? Enumerable.Empty<string>())
        .Concat(new[] { FileNamespace, TalkNamespace });

    private const string ScriptNamespaceDefault = "Script";
    private string _scriptNamespace = ScriptNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the script namespace.
    /// </para>
    /// <para>
    /// If omitted "Script" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string ScriptNamespace
    {
        get => _scriptNamespace;
        set => _scriptNamespace = string.IsNullOrWhiteSpace(value)
            ? ScriptNamespaceDefault
            : value;
    }

    private const string SiteNameDefault = "a NeverFoundry wiki";
    private string _siteName = SiteNameDefault;
    /// <summary>
    /// <para>
    /// The name of the wiki. Displayed as a subheading below each article title.
    /// </para>
    /// <para>
    /// If omitted "a NeverFoundry wiki" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string SiteName
    {
        get => _siteName;
        set => _siteName = string.IsNullOrWhiteSpace(value)
            ? SiteNameDefault
            : value;
    }

    private const string SystemNamespaceDefault = "System";
    private string _systemNamespace = SystemNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the system namespace.
    /// </para>
    /// <para>
    /// If omitted "System" is used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string SystemNamespace
    {
        get => _systemNamespace;
        set => _systemNamespace = string.IsNullOrWhiteSpace(value)
            ? SystemNamespaceDefault
            : value;
    }

    private const string TalkNamespaceDefault = "Talk";
    private string _talkNamespace = TalkNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the talk pseudo-namespace.
    /// </para>
    /// <para>
    /// If omitted "Talk" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string TalkNamespace
    {
        get => _talkNamespace;
        set => _talkNamespace = string.IsNullOrWhiteSpace(value)
            ? TalkNamespaceDefault
            : value;
    }

    private const string TransclusionNamespaceDefault = "Transclusion";
    private string _transclusionNamespace = TransclusionNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the transclusion namespace.
    /// </para>
    /// <para>
    /// If omitted "Transclusion" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string TransclusionNamespace
    {
        get => _transclusionNamespace;
        set => _transclusionNamespace = string.IsNullOrWhiteSpace(value)
            ? TransclusionNamespaceDefault
            : value;
    }

    private const string UserNamespaceDefault = "User";
    private string _userNamespace = UserNamespaceDefault;
    /// <summary>
    /// <para>
    /// The name of the user namespace.
    /// </para>
    /// <para>
    /// If omitted "User" is used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string UserNamespace
    {
        get => _userNamespace;
        set => _userNamespace = string.IsNullOrWhiteSpace(value)
            ? UserNamespaceDefault
            : value;
    }

    private const string WikiLinkPrefixDefault = "Wiki";
    private string _wikiLinkPrefix = WikiLinkPrefixDefault;
    /// <summary>
    /// <para>
    /// The prefix added before wiki links (to distinguish them from other pages on the same
    /// server).
    /// </para>
    /// <para>
    /// If omitted "Wiki" will be used.
    /// </para>
    /// </summary>
    [AllowNull]
    public string WikiLinkPrefix
    {
        get => _wikiLinkPrefix;
        set => _wikiLinkPrefix = string.IsNullOrWhiteSpace(value)
            ? WikiLinkPrefixDefault
            : value;
    }

    /// <summary>
    /// Adds one or more namespaces to the list of reserved names which may not be assigned to
    /// pages by non-admin users.
    /// </summary>
    /// <param name="namespaces"></param>
    /// <returns>This instance.</returns>
    public WikiOptions AddAdminNamespace(params string[] namespaces)
    {
        for (var i = 0; i < namespaces.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(namespaces[i]))
            {
                (_adminNamespaces ??= new List<string>()).Add(namespaces[i]);
            }
        }
        return this;
    }

    /// <summary>
    /// <para>
    /// Adds one or more namespaces to the list of reserved names which may not be assigned to
    /// pages by users.
    /// </para>
    /// <para>
    /// The namespaces assigned to <see cref="CategoryNamespace"/>, <see cref="FileNamespace"/>,
    /// and <see cref="TalkNamespace"/> are included automatically.
    /// </para>
    /// </summary>
    /// <param name="namespaces">
    /// The namespace(s) to add.
    /// </param>
    /// <returns>This instance.</returns>
    public WikiOptions AddReservedNamespace(params string[] namespaces)
    {
        for (var i = 0; i < namespaces.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(namespaces[i]))
            {
                (_reservedNamespaces ??= new List<string>()).Add(namespaces[i]);
            }
        }
        return this;
    }
}
