﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using Tavenem.Wiki.MarkdownExtensions;

namespace Tavenem.Wiki;

/// <summary>
/// The delegate signature used by <see cref="WikiOptions.GetDomainPermission"/>.
/// </summary>
/// <param name="userId">The ID of a user.</param>
/// <param name="domain">The domain.</param>
public delegate ValueTask<WikiPermission> GetDomainPermissionFunc(string userId, string domain);

/// <summary>
/// The delegate signature used by <see cref="WikiOptions.OnCreated"/>.
/// </summary>
/// <param name="article">The new article.</param>
/// <param name="editor">The ID of the editor who created the article.</param>
public delegate ValueTask OnCreatedFunc(Article article, string editor);

/// <summary>
/// The delegate signature used by <see cref="WikiOptions.OnDeleted"/>.
/// </summary>
/// <param name="article">The deleted article.</param>
/// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
/// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
public delegate ValueTask OnDeletedFunc(Article article, string? oldOwner, string? newOwner);

/// <summary>
/// The delegate signature used by <see cref="WikiOptions.OnEdited"/>.
/// </summary>
/// <param name="article">The edited article.</param>
/// <param name="revision">The revision applied.</param>
/// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
/// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
public delegate ValueTask OnEditedFunc(Article article, Revision revision, string? oldOwner, string? newOwner);

/// <summary>
/// Various customization and configuration options for the wiki system.
/// </summary>
public class WikiOptions
{
    /// <summary>
    /// The title of the main about page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is "About"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the about page.
    /// </para>
    /// </remarks>
    public string? AboutPageTitle { get; set; } = "About";

    /// <summary>
    /// An optional collection of namespaces for which are reserved for admin users.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The namespace assigned to <see cref="SystemNamespace"/> is included automatically.
    /// </para>
    /// <para>
    /// Pages in these namespaces may have their <see cref="Article.AllowedEditors"/> set to include
    /// any users, but only admin users will be able to create new pages or delete existing pages,
    /// and no user may be assigned as the owner of pages in these namespaces; instead, all admin
    /// users have owenership priviledges.
    /// </para>
    /// </remarks>
    public IEnumerable<string> AdminNamespaces => (CustomAdminNamespaces ?? Enumerable.Empty<string>())
        .Concat(new[] { SystemNamespace });

    private const string CategoriesTitleDefault = "Categories";
    private string _categoriesTitle = CategoriesTitleDefault;
    /// <summary>
    /// The name of the article on categories in the main wiki.
    /// </summary>
    /// <remarks>
    /// If omitted "Categories" will be used.
    /// </remarks>
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
    /// The name of the categories namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Category" will be used.
    /// </remarks>
    [AllowNull]
    public string CategoryNamespace
    {
        get => _categoryNamespace;
        set => _categoryNamespace = string.IsNullOrWhiteSpace(value)
            ? CategoryNamespaceDefault
            : value;
    }

    /// <summary>
    /// The title of the main contact page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is "Contact"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the contact page.
    /// </para>
    /// </remarks>
    public string? ContactPageTitle { get; set; } = "Contact";

    /// <summary>
    /// The title of the main contents page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is "Contents"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the contents page.
    /// </para>
    /// </remarks>
    public string? ContentsPageTitle { get; set; } = "Contents";

    /// <summary>
    /// The title of the main copyright page.
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public string? CopyrightPageTitle { get; set; } = "Copyright";

    /// <summary>
    /// An optional collection of namespaces which may not be assigned to pages by non-admin users.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AdminNamespaces"/> to get the full list, which includes the namespace
    /// assigned to <see cref="SystemNamespace"/> automatically.
    /// </remarks>
    public IList<string>? CustomAdminNamespaces { get; set; }

    /// <summary>
    /// An optional collection of namespaces which may not be assigned to pages by users.
    /// </summary>
    /// <remarks>
    /// Use <see cref="ReservedNamespaces"/> to get the full list, which includes the namespaces
    /// assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/> automatically.
    /// </remarks>
    public IList<string>? CustomReservedNamespaces { get; set; }

    /// <summary>
    /// The default permission granted to an anonymous user for wiki content with no configured
    /// access control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This defaults to <see cref="WikiPermission.Read"/>, which allows anonymous users to view any
    /// content for which no specific access has been configured. It can be set to <see
    /// cref="WikiPermission.None"/> to disable anonymous browsing, and require all users to sign in
    /// prior to viewing any content.
    /// </para>
    /// <para>
    /// Note that anonymous users cannot make any changes regardless of this setting. A specific
    /// editor is required for all content creation and revision.
    /// </para>
    /// <para>
    /// Note also that content in a domain always uses a default permission of <see
    /// cref="WikiPermission.None"/>.
    /// </para>
    /// </remarks>
    public WikiPermission DefaultAnonymousPermission { get; set; } = WikiPermission.Read;

    private const string DefaultNamespaceDefault = "Wiki";
    private string _defaultNamespace = DefaultNamespaceDefault;
    /// <summary>
    /// The name of the default namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Wiki" will be used.
    /// </remarks>
    [AllowNull]
    public string DefaultNamespace
    {
        get => _defaultNamespace;
        set => _defaultNamespace = string.IsNullOrWhiteSpace(value)
            ? DefaultNamespaceDefault
            : value;
    }

    /// <summary>
    /// The default permission granted to a registered user for wiki content with no configured
    /// access control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This defaults to <see cref="WikiPermission.All"/>, which allows registered users full access
    /// when no specific access controls take precedence.
    /// </para>
    /// <para>
    /// Note that content in a domain always uses a default permission of <see
    /// cref="WikiPermission.None"/>.
    /// </para>
    /// </remarks>
    public WikiPermission DefaultRegisteredPermission { get; set; } = WikiPermission.All;

    /// <summary>
    /// The default number of levels of nesting shown in an article's table of contents.
    /// </summary>
    /// <remarks>
    /// Can be overridden by specifying the level for a given article.
    /// </remarks>
    public int DefaultTableOfContentsDepth { get; set; } = 3;

    private const string DefaultTableOfContentsTitleDefault = "Contents";
    private string _defaultTableOfContentsTitle = DefaultTableOfContentsTitleDefault;
    /// <summary>
    /// The default title of tables of contents.
    /// </summary>
    /// <remarks>
    /// If omitted "Contents" will be used.
    /// </remarks>
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
    /// The name of the file namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "File" will be used.
    /// </remarks>
    [AllowNull]
    public string FileNamespace
    {
        get => _fileNamespace;
        set => _fileNamespace = string.IsNullOrWhiteSpace(value)
            ? FileNamespaceDefault
            : value;
    }

    /// <summary>
    /// When a user attempts to interact with an article in a domain (including viewing, creating,
    /// editing, or deleting items), this function is invoked (if provided) to determine the
    /// permissions the user has for that domain.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="WikiUser.AllowedViewDomains"/> property for the given user will also be
    /// checked, and will provide <see cref="WikiPermission.Read"/> permission, if a matching domain
    /// name is found.
    /// </para>
    /// <para>
    /// The user's effective permission is determined by the combination of this function, <see
    /// cref="WikiUser.AllowedViewDomains"/>, and <see cref="WikiGroup.AllowedViewDomains"/>, as
    /// well as any access controls on the specific article, which override the general permissions
    /// for the domain, if present.
    /// </para>
    /// <para>
    /// Note that the default when no permission is specified is to be denied access (unlike the
    /// default for non-domain articles, which is to grant full access even to anonymous users).
    /// </para>
    /// Also see <seealso cref="UserDomains"/>.
    /// </remarks>
    public GetDomainPermissionFunc? GetDomainPermission { get; set; }

    private const string GroupNamespaceDefault = "Group";
    private string _groupNamespace = GroupNamespaceDefault;
    /// <summary>
    /// The name of the user group namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Group" is used.
    /// </remarks>
    [AllowNull]
    public string GroupNamespace
    {
        get => _groupNamespace;
        set => _groupNamespace = string.IsNullOrWhiteSpace(value)
            ? GroupNamespaceDefault
            : value;
    }

    /// <summary>
    /// The title of the main help page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is "Help"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the help page.
    /// </para>
    /// </remarks>
    public string? HelpPageTitle { get; set; } = "Help";

    /// <summary>
    /// A string added to all wiki links, if non-empty.
    /// </summary>
    /// <remarks>
    /// The string '{LINK}', if included, will be replaced by the full article title being
    /// linked.
    /// </remarks>
    public string? LinkTemplate { get; set; }

    private const string MainPageTitleDefault = "Main";
    private string _mainPageTitle = MainPageTitleDefault;
    /// <summary>
    /// The title of the main page (shown when no article title is given).
    /// </summary>
    /// <remarks>
    /// If omitted "Main" will be used.
    /// </remarks>
    [AllowNull]
    public string MainPageTitle
    {
        get => _mainPageTitle;
        set => _mainPageTitle = string.IsNullOrWhiteSpace(value)
            ? MainPageTitleDefault
            : value;
    }

    /// <summary>
    /// The maximum size (in bytes) of uploaded files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is 5 MB.
    /// </para>
    /// <para>
    /// Setting this to a value less than or equal to zero effectively prevents file uploads.
    /// </para>
    /// </remarks>
    public int MaxFileSize { get; set; } = 5000000; // 5 MB

    /// <summary>
    /// Gets a string representing the <see cref="MaxFileSize"/> in a reasonable unit (GiB for
    /// large sizes, down to bytes for small ones).
    /// </summary>
    public string MaxFileSizeString => MaxFileSize switch
    {
        >= 1024 * 1024 * 1024 => $"{MaxFileSize / (1024 * 1024 * 1024.0):N3} GiB",
        >= 1024 * 1024 => $"{MaxFileSize / 1024 * 1024.0:N3} MiB",
        >= 1024 => $"{MaxFileSize / 1024.0:G} KiB",
        _ => $"{MaxFileSize} bytes"
    };

    /// <summary>
    /// The minimum number of headings required in an article to display a table of contents by
    /// default.
    /// </summary>
    /// <remarks>
    /// Can be overridden by specifying the location of a table of contents explicitly for a given article.
    /// </remarks>
    public int MinimumTableOfContentsHeadings { get; set; } = 3;

    /// <summary>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is created.
    /// </summary>
    /// <remarks>
    /// Receives the new <see cref="Article"/> as a parameter.
    /// </remarks>
    public OnCreatedFunc? OnCreated { get; set; }

    /// <summary>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is deleted.
    /// </summary>
    /// <remarks>
    /// Receives the deleted <see cref="Article"/>, the original <see cref="Article.Owner"/>,
    /// and the new <see cref="Article.Owner"/> as parameters.
    /// </remarks>
    public OnDeletedFunc? OnDeleted { get; set; }

    /// <summary>
    /// An optional callback invoked when a new <see cref="Article"/> (including <see
    /// cref="Category"/> and <see cref="WikiFile"/>) is edited (not including deletion if <see
    /// cref="OnDeleted"/> is provided).
    /// </summary>
    /// <remarks>
    /// Receives the edited <see cref="Article"/>, the <see cref="Revision"/> which was applied,
    /// the original <see cref="Article.Owner"/>, and the new <see cref="Article.Owner"/> as
    /// parameters.
    /// </remarks>
    public OnEditedFunc? OnEdited { get; set; }

    /// <summary>
    /// The title of the main policy page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Default is "Policies"
    /// </para>
    /// <para>
    /// May be set to <see langword="null"/> or an empty <see cref="string"/>, which disables
    /// the policy page.
    /// </para>
    /// </remarks>
    public string? PolicyPageTitle { get; set; } = "Policies";

    /// <summary>
    /// A collection of preprocessors which transform the HTML of an article <em>after</em> it is
    /// parsed from markdown but <em>before</em> it is sanitized.
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

    /// <summary>
    /// An optional collection of namespaces which may not be assigned to pages by users.
    /// </summary>
    /// <remarks>
    /// The namespaces assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/> are
    /// included automatically.
    /// </remarks>
    public IEnumerable<string> ReservedNamespaces => (CustomReservedNamespaces ?? Enumerable.Empty<string>())
        .Concat(new[] { FileNamespace, TalkNamespace });

    private const string ScriptNamespaceDefault = "Script";
    private string _scriptNamespace = ScriptNamespaceDefault;
    /// <summary>
    /// The name of the script namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Script" will be used.
    /// </remarks>
    [AllowNull]
    public string ScriptNamespace
    {
        get => _scriptNamespace;
        set => _scriptNamespace = string.IsNullOrWhiteSpace(value)
            ? ScriptNamespaceDefault
            : value;
    }

    private const string SiteNameDefault = "a Tavenem wiki";
    private string _siteName = SiteNameDefault;
    /// <summary>
    /// The name of the wiki. Displayed as a subheading below each article title.
    /// </summary>
    /// <remarks>
    /// If omitted "a Tavenem wiki" will be used.
    /// </remarks>
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
    /// The name of the system namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "System" is used.
    /// </remarks>
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
    /// The name of the talk pseudo-namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Talk" will be used.
    /// </remarks>
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
    /// The name of the transclusion namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "Transclusion" will be used.
    /// </remarks>
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
    /// The name of the user namespace.
    /// </summary>
    /// <remarks>
    /// If omitted "User" is used.
    /// </remarks>
    [AllowNull]
    public string UserNamespace
    {
        get => _userNamespace;
        set => _userNamespace = string.IsNullOrWhiteSpace(value)
            ? UserNamespaceDefault
            : value;
    }

    /// <summary>
    /// If set to <see langword="true"/> each user (and only that user) is automatically granted
    /// full permission in a domain with the same name as their user ID.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="GetDomainPermission"/> function, the <see
    /// cref="WikiUser.AllowedViewDomains"/> property, and the <see
    /// cref="WikiGroup.AllowedViewDomains"/> property will still be checked for other users
    /// attempting to access content in such domains, but the user with the matching ID will always
    /// be granted all permissions automatically.
    /// </para>
    /// <para>
    /// A possible use for user domains is as a "scratch-pad" area where articles can be drafted and
    /// tested prior to publication.
    /// </para>
    /// </remarks>
    public bool UserDomains { get; set; }

    private const string WikiLinkPrefixDefault = "Wiki";
    private string _wikiLinkPrefix = WikiLinkPrefixDefault;
    /// <summary>
    /// The prefix added before wiki links (to distinguish them from other pages on the same
    /// server).
    /// </summary>
    /// <remarks>
    /// If omitted "Wiki" will be used.
    /// </remarks>
    [AllowNull]
    public string WikiLinkPrefix
    {
        get => _wikiLinkPrefix;
        set => _wikiLinkPrefix = string.IsNullOrWhiteSpace(value)
            ? WikiLinkPrefixDefault
            : value;
    }

    /// <summary>
    /// Builds a relative URL for a wiki page.
    /// </summary>
    /// <param name="title">
    /// <para>
    /// The title of the page.
    /// </para>
    /// <para>
    /// If omitted, and <paramref name="wikiNamespace"/> is either also omitted, or is the same as
    /// <see cref="DefaultNamespace"/>, the <see cref="MainPageTitle"/> will be used.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the page.
    /// </para>
    /// <para>
    /// If omitted, <see cref="DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="talk">
    /// Whether to generate a link for the talk page.
    /// </param>
    /// <param name="route">
    /// <para>
    /// Any additional route which should be appended to the URL.
    /// </para>
    /// <para>
    /// Should not contain a leading '/' character. One will automatically be appended if necessary.
    /// </para>
    /// <para>
    /// The value should already be URL encoded. No additional encoding will be performed, in order
    /// to avoid incorrectly encoding intentional URL control characters, such as path separators.
    /// </para>
    /// </param>
    /// <param name="query">
    /// <para>
    /// Any additional query string.
    /// </para>
    /// <para>
    /// Should not contain a leading '?' character. One will automatically be appended if necessary.
    /// The query <em>should</em> contain internal '&amp;' characters to separate values.
    /// </para>
    /// <para>
    /// The value should already be URL encoded. No additional encoding will be performed, in order
    /// to avoid incorrectly encoding intentional URL control characters, such as the '&amp;' and
    /// '=' characters.
    /// </para>
    /// </param>
    /// <returns>
    /// A relative URL for the wiki page with the given characteristics. The string will be URL
    /// encoded.
    /// </returns>
    public string GetWikiPageUrl(
        string? title = null,
        string? wikiNamespace = null,
        bool talk = false,
        string? route = null,
        string? query = null)
    {
        var writer = new StringWriter();
        writer.Write("./");
        UrlEncoder.Default.Encode(writer, WikiLinkPrefix);
        writer.Write('/');
        if (talk)
        {
            UrlEncoder.Default.Encode(writer, TalkNamespace);
            writer.Write(':');
        }
        if (talk
            || !string.Equals(wikiNamespace, DefaultNamespace, StringComparison.OrdinalIgnoreCase))
        {
            UrlEncoder.Default.Encode(writer, wikiNamespace ?? DefaultNamespace);
            writer.Write(':');
        }
        UrlEncoder.Default.Encode(writer, title ?? MainPageTitle);
        if (!string.IsNullOrEmpty(route))
        {
            writer.Write('/');
            writer.Write(route);
        }
        if (!string.IsNullOrEmpty(query))
        {
            writer.Write('?');
            writer.Write(query);
        }
        return writer.ToString();
    }

    /// <summary>
    /// Builds a relative URL for a wiki page.
    /// </summary>
    /// <param name="queryParameters">
    /// <para>
    /// Any additional query parameters, as a collection of keys and values.
    /// </para>
    /// <para>
    /// Each value will be converted to a string by having its <see cref="object.ToString"/> method
    /// invoked.
    /// </para>
    /// </param>
    /// <param name="title">
    /// <para>
    /// The title of the page.
    /// </para>
    /// <para>
    /// If omitted, and <paramref name="wikiNamespace"/> is either also omitted, or is the same as
    /// <see cref="DefaultNamespace"/>, the <see cref="MainPageTitle"/> will be used.
    /// </para>
    /// </param>
    /// <param name="wikiNamespace">
    /// <para>
    /// The namespace of the page.
    /// </para>
    /// <para>
    /// If omitted, <see cref="DefaultNamespace"/> will be used.
    /// </para>
    /// </param>
    /// <param name="talk">
    /// Whether to generate a link for the talk page.
    /// </param>
    /// <param name="route">
    /// <para>
    /// Any additional route which should be appended to the URL.
    /// </para>
    /// <para>
    /// Should not contain a leading '/' character. One will automatically be appended if necessary.
    /// </para>
    /// <para>
    /// The value should already be URL encoded. No additional encoding will be performed, in order
    /// to avoid incorrectly encoding intentional URL control characters, such as path separators.
    /// </para>
    /// </param>
    /// <returns>
    /// A relative URL for the wiki page with the given characteristics. The string will be URL
    /// encoded.
    /// </returns>
    public string GetWikiPageUrl(
        IReadOnlyDictionary<string, object?> queryParameters,
        string? title = null,
        string? wikiNamespace = null,
        bool talk = false,
        string? route = null)
    {
        var writer = new StringWriter();
        writer.Write("./");
        UrlEncoder.Default.Encode(writer, WikiLinkPrefix);
        writer.Write('/');
        if (talk)
        {
            UrlEncoder.Default.Encode(writer, TalkNamespace);
            writer.Write(':');
        }
        if (talk
            || !string.Equals(wikiNamespace, DefaultNamespace, StringComparison.OrdinalIgnoreCase))
        {
            UrlEncoder.Default.Encode(writer, wikiNamespace ?? DefaultNamespace);
            writer.Write(':');
        }
        UrlEncoder.Default.Encode(writer, title ?? MainPageTitle);
        if (!string.IsNullOrEmpty(route))
        {
            writer.Write('/');
            writer.Write(route);
        }
        if (queryParameters is not null)
        {
            var queries = new List<KeyValuePair<string, string>>();
            foreach (var key in queryParameters.Keys)
            {
                var value = queryParameters[key];
                if (value is null)
                {
                    continue;
                }
                var str = value.ToString();
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }
                queries.Add(new(key, str));
            }
            for (var i = 0; i < queries.Count; i++)
            {
                if (i == 0)
                {
                    writer.Write('?');
                }
                else
                {
                    writer.Write('&');
                }
                UrlEncoder.Default.Encode(writer, queries[i].Key);
                writer.Write('=');
                UrlEncoder.Default.Encode(writer, queries[i].Value);
            }
        }
        return writer.ToString();
    }
}
