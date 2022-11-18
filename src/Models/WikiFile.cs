using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A file tracked by the wiki system.
/// </summary>
/// <remarks>
/// <para>
/// Note that this does not represent an actual file, but only wiki information about a file. In
/// other words: storage and retrieval of the file itself is expected to be handled
/// independently. The file bytes are not recorded in the wiki system. The wiki instead records
/// information about the file such as its properties, its owner, permissions assigned to it,
/// wiki text describing it, revision history, and so forth.
/// </para>
/// <para>
/// This separation allows implementations of the wiki system to utilize any persistence
/// mechanism which suits their use case: local storage, database storage, cloud storage, CDN
/// delivery, etc.
/// </para>
/// </remarks>
public sealed class WikiFile : Page, IPage<WikiFile>
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string WikiFileIdItemTypeName = ":Page:WikiFile:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => WikiFileIdItemTypeName;

    /// <summary>
    /// The path to the physical file this entry represents.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// The size of the file (in bytes).
    /// </summary>
    public int FileSize { get; private set; }

    /// <summary>
    /// The MIME type of the file.
    /// </summary>
    public string FileType { get; private set; }

    /// <summary>
    /// The ID of the user who uploaded the file.
    /// </summary>
    public string Uploader { get; set; }

    /// <summary>
    /// Constructs a new instance of <see cref="WikiFile"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor should only be used by deserializers or for testing purposes.
    /// </para>
    /// <para>
    /// To create a new page as part of a normal user interaction, use the <see
    /// cref="WikiExtensions.AddOrReviseWikiPageAsync(IDataStore, WikiOptions, IWikiUserManager,
    /// IWikiGroupManager, IWikiUser, PageTitle, string?, string?, bool, string?,
    /// IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?,
    /// PageTitle?, PageTitle?)"/> method.
    /// </para>
    /// <para>
    /// To create a page programmatically, you can use a combination of <see
    /// cref="WikiExtensions.GetWikiPageAsync(IDataStore, WikiOptions, PageTitle, bool, bool)"/> (to
    /// get the current page, or a new page if one does not already exist), and <see
    /// cref="Page.UpdateAsync(WikiOptions, IDataStore, string, string?, string?, string?,
    /// IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?, IEnumerable{string}?,
    /// PageTitle?)"/> to update the result with the intended properties.
    /// </para>
    /// </remarks>
    [JsonConstructor]
    public WikiFile(
        PageTitle title,
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        string filePath,
        int fileSize,
        string fileType,
        string uploader,
        string? markdownContent = null,
        string? owner = null,
        Revision? revision = null,
        IReadOnlyCollection<string>? allowedEditors = null,
        IReadOnlyCollection<string>? allowedViewers = null,
        IReadOnlyCollection<string>? allowedEditorGroups = null,
        IReadOnlyCollection<string>? allowedViewerGroups = null,
        IReadOnlyCollection<PageTitle>? categories = null,
        IReadOnlyCollection<PageTitle>? redirectReferences = null,
        IReadOnlyCollection<PageTitle>? references = null,
        IReadOnlyCollection<PageTitle>? transclusionReferences = null,
        IReadOnlyCollection<PageTitle>? transclusions = null,
        bool isBrokenRedirect = false,
        bool isDoubleRedirect = false,
        PageTitle? redirectTitle = null) : base(
        title,
        html,
        preview,
        wikiLinks,
        markdownContent,
        owner,
        revision,
        allowedEditors,
        allowedViewers,
        allowedEditorGroups,
        allowedViewerGroups,
        categories,
        redirectReferences,
        references,
        transclusionReferences,
        transclusions,
        isBrokenRedirect,
        isDoubleRedirect,
        redirectTitle)
    {
        FileSize = fileSize;
        FilePath = filePath;
        FileType = fileType;
        Uploader = uploader;
    }

    private WikiFile(PageTitle title) : base(title)
    {
        FilePath = string.Empty;
        FileType = string.Empty;
        Uploader = string.Empty;
    }

    /// <summary>
    /// Gets an empty instance of <see cref="WikiFile"/>.
    /// </summary>
    /// <returns>An empty instance of <see cref="WikiFile"/>.</returns>
    public static new WikiFile Empty(PageTitle title) => new(title);

    /// <summary>
    /// Gets a copy of this instance.
    /// </summary>
    /// <param name="newNamespace">A new namespace to assign to the copied page.</param>
    /// <returns>
    /// A new instance of <see cref="WikiFile"/> with the same properties as this instance.
    /// </returns>
    public override WikiFile Copy(string? newNamespace = null)
    {
        var newPage = Copy<WikiFile>(newNamespace);
        newPage.FilePath = FilePath;
        newPage.FileSize = FileSize;
        newPage.FileType = FileType;
        newPage.Uploader = Uploader;
        return newPage;
    }

    /// <summary>
    /// Gets a string which shows the given file <paramref name="size"/> in bytes if &lt; 1KiB, in
    /// KiB if &lt; 1MiB, in MiB if &lt; 1GiB, or in GiB otherwise.
    /// </summary>
    /// <param name="size">A file size, in bytes.</param>
    /// <returns>
    /// A string which shows the given file <paramref name="size"/> in bytes if &lt; 1KiB, in KiB if
    /// &lt; 1MiB, in MiB if &lt; 1GiB, or in GiB otherwise.
    /// </returns>
    public static string GetFileSizeString(int size)
    {
        if (size < 1024)
        {
            return $"{size} B";
        }
        else if (size < 1024 * 1024)
        {
            return $"{size / 1024.0:N0} KiB";
        }
        else if (size < 1024 * 1024 * 1024)
        {
            return $"{size / (1024 * 1024.0):N0} MiB";
        }
        else
        {
            return $"{size / (1024 * 1024 * 1024.0):N0} GiB";
        }
    }

    /// <summary>
    /// Renames a <see cref="Page"/> instance, turns the previous title into a redirect to the new
    /// one, updates the wiki accordingly, and saves both pages to the data store.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The new title of the page.</param>
    /// <param name="editor">
    /// The ID of the user who made this edit.
    /// </param>
    /// <param name="markdown">The raw markdown content for the renamed page.</param>
    /// <param name="comment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The ID of the intended owner of both pages.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The users allowed to edit either page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The users allowed to view either page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit either page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view either page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="redirectTitle">
    /// If the new page will redirect to another, this indicates the title of the destination.
    /// </param>
    /// <remarks>
    /// Note: any redirects which point to this page will be updated to point to the new, renamed
    /// page instead.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// <para>
    /// The target namespace is reserved.
    /// </para>
    /// <para>
    /// Or, the namespace of either the original or renamed page is the category, file, group,
    /// script, or user namespace, and the namespaces of the original and renamed page do not match.
    /// Moving a page to or from those namespaces is not permitted.
    /// </para>
    /// </exception>
    public override Task RenameAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title,
        string editor,
        string? markdown = null,
        string? comment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null) => RenameAsync<WikiFile>(
            options,
            dataStore,
            title,
            editor,
            markdown,
            comment,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            redirectTitle);

    /// <summary>
    /// <para>
    /// Updates this page according to the given parameters.
    /// </para>
    /// <para>
    /// This can either be used to create a new page, or update an existing page.
    /// </para>
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="editor">
    /// The ID of the user who made this edit.
    /// </param>
    /// <param name="path">The relative path to the file.</param>
    /// <param name="fileSize">The size of the file, in bytes.</param>
    /// <param name="type">The MIME type of the file.</param>
    /// <param name="markdown">The raw markdown content.</param>
    /// <param name="comment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The ID of the intended owner of the page.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The users allowed to edit this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The users allowed to view this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be edited by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this page.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the page can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the page can only be viewed by those listed, plus its owner
    /// (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the page.
    /// </para>
    /// </param>
    /// <param name="redirectTitle">
    /// If this page will redirect to another, this indicates the title of the destination.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <para>
    /// A <paramref name="redirectTitle"/> was provided for a page in the category, group, or user
    /// namespaces. Redirects in those namespaces are not permitted.
    /// </para>
    /// <para>
    /// Or, a page in the group or user namespaces was given a domain. Pages in those namespaces
    /// cannot be given domains.
    /// </para>
    /// <para>
    /// Or, file size was less than or equal to zero.
    /// </para>
    /// <para>
    /// Or, <paramref name="path"/> was empty.
    /// </para>
    /// <para>
    /// Or, <paramref name="type"/> was empty.
    /// </para>
    /// </exception>
    public async Task UpdateAsync(
        WikiOptions options,
        IDataStore dataStore,
        string editor,
        string path,
        int fileSize,
        string type,
        string? markdown = null,
        string? comment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
        PageTitle? redirectTitle = null)
    {
        if (fileSize <= 0)
        {
            throw new ArgumentException("File size must be greater than zero", nameof(fileSize));
        }
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be empty", nameof(path));
        }
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type cannot be empty", nameof(type));
        }

        FilePath = path.Trim();
        FileSize = fileSize;
        FileType = type.Trim();

        await UpdateAsync(
            options,
            dataStore,
            editor,
            markdown,
            comment,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            redirectTitle)
            .ConfigureAwait(false);
    }
}
