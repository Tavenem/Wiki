using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;

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
public sealed class WikiFile : Article
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string WikiFileIdItemTypeName = ":Article:WikiFile:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonPropertyOrder(-1)]
    public override string IdItemTypeName => WikiFileIdItemTypeName;

    /// <summary>
    /// The size of the file (in bytes).
    /// </summary>
    public int FileSize { get; private set; }

    /// <summary>
    /// The path to the physical file this entry represents.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// The MIME type of the file.
    /// </summary>
    public string FileType { get; private set; }

    /// <summary>
    /// The ID of the user who uploaded the file.
    /// </summary>
    public string Uploader { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="Category"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="idItemTypeName">The type discriminator.</param>
    /// <param name="title">
    /// The title of this article. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="filePath">
    /// The path to the physical file this entry represents.
    /// </param>
    /// <param name="fileSize">
    /// The size of the file (in bytes).
    /// </param>
    /// <param name="fileType">
    /// The MIME type of the file.
    /// </param>
    /// <param name="uploader">
    /// The ID of the user who uploaded the file.
    /// </param>
    /// <param name="html">The rendered HTML content.</param>
    /// <param name="markdownContent">The raw markdown.</param>
    /// <param name="preview">A preview of this item's rendered HTML.</param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    /// <param name="timestampTicks">
    /// The timestamp when this message was sent, in UTC Ticks.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace to which this article belongs.
    /// </param>
    /// <param name="isDeleted">
    /// Indicates that this article has been marked as deleted.
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The owner of this article.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The user(s) and/or group(s) allowed to edit this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be edited by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// <para>
    /// Cannot be set if the <paramref name="owner"/> is <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The user(s) and/or group(s) allowed to view this article.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the article can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the article can only be viewed by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the article.
    /// </para>
    /// <para>
    /// Cannot be set if the <paramref name="owner"/> is <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="categories">
    /// The titles of the categories to which this article belongs.
    /// </param>
    /// <param name="transclusions">
    /// The transclusions within this article.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers.
    /// </remarks>
    [JsonConstructor]
    public WikiFile(
        string id,
        string idItemTypeName,
        string title,
        string filePath,
        int fileSize,
        string fileType,
        string uploader,
        string html,
        string markdownContent,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        long timestampTicks,
        string wikiNamespace,
        bool isDeleted,
        string? owner,
        IReadOnlyCollection<string>? allowedEditors,
        IReadOnlyCollection<string>? allowedViewers,
        IReadOnlyCollection<string> categories,
        IReadOnlyList<Transclusion>? transclusions) : base(
            id,
            idItemTypeName,
            title,
            html,
            markdownContent,
            preview,
            wikiLinks,
            timestampTicks,
            wikiNamespace,
            isDeleted,
            owner,
            allowedEditors,
            allowedViewers,
            null,
            null,
            false,
            false,
            categories,
            transclusions)
    {
        FilePath = filePath;
        FileSize = fileSize;
        FileType = fileType;
        Uploader = uploader;
    }

    private WikiFile(
        string id,
        string title,
        string filePath,
        int fileSize,
        string fileType,
        string uploader,
        string? markdown,
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        long timestampTicks,
        string wikiNamespace,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IList<string>? categories = null,
        IList<Transclusion>? transclusions = null) : base(
            id,
            title,
            markdown,
            html,
            preview,
            wikiLinks,
            timestampTicks,
            wikiNamespace,
            isDeleted,
            owner,
            allowedEditors,
            allowedViewers,
            categories,
            transclusions)
    {
        FilePath = filePath;
        FileSize = fileSize;
        FileType = fileType;
        Uploader = uploader;
    }

    /// <summary>
    /// Gets the latest revision for the file with the given title.
    /// </summary>
    /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the file to retrieve.</param>
    /// <returns>The latest revision for the file with the given title; or <see
    /// langword="null"/> if no such file exists.</returns>
    public static WikiFile? GetFile(
        IWikiOptions options,
        IDataStore dataStore,
        string title)
    {
        WikiFile? file = null;
        var reference = PageReference.GetPageReference(dataStore, title, options.FileNamespace);
        if (reference is not null)
        {
            file = dataStore.GetItem<WikiFile>(reference.Reference);
        }
        // If no exact match exists, ignore case if only one such match exists.
        if (file is null)
        {
            var normalizedReference = NormalizedPageReference.GetNormalizedPageReference(dataStore, title, options.FileNamespace);
            if (normalizedReference?.References.Count == 1)
            {
                file = dataStore.GetItem<WikiFile>(normalizedReference.References[0]);
            }
        }

        return file;
    }

    /// <summary>
    /// Gets a new <see cref="WikiFile"/> instance.
    /// </summary>
    /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the file. Must be unique and non-empty.</param>
    /// <param name="editor">
    /// The ID of the user who created this file.
    /// </param>
    /// <param name="filePath">The relative path to the file.</param>
    /// <param name="fileSize">The size of the file, in bytes.</param>
    /// <param name="type">
    /// The MIME type of the file.
    /// </param>
    /// <param name="markdown">The raw markdown content.</param>
    /// <param name="revisionComment">
    /// An optional comment supplied for this revision (e.g. to explain the upload).
    /// </param>
    /// <param name="owner">
    /// <para>
    /// The ID of the intended owner of the file.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The user(s) and/or group(s) allowed to edit this file.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the file can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the file can only be edited by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The user(s) and/or group(s) allowed to view this file.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the file can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the file can only be viewed by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the file.
    /// </para>
    /// </param>
    public static async Task<WikiFile> NewAsync(
        IWikiOptions options,
        IDataStore dataStore,
        string title,
        string editor,
        string filePath,
        int fileSize,
        string type,
        string? markdown = null,
        string? revisionComment = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
        }
        title = title.ToWikiTitleCase();

        var wikiId = dataStore.CreateNewIdFor<WikiFile>();

        await CreatePageReferenceAsync(dataStore, wikiId, title, options.FileNamespace)
            .ConfigureAwait(false);

        var revision = new Revision(
            wikiId,
            editor,
            title,
            options.FileNamespace,
            null,
            markdown,
            revisionComment);
        await dataStore.StoreItemAsync(revision).ConfigureAwait(false);

        var md = markdown;
        List<Transclusion> transclusions;
        if (string.IsNullOrEmpty(markdown))
        {
            transclusions = new List<Transclusion>();
        }
        else
        {
            md = TransclusionParser.Transclude(
                options,
                dataStore,
                title,
                $"{options.FileNamespace}:{title}",
                markdown,
                out transclusions);
        }

        var wikiLinks = GetWikiLinks(options, dataStore, md, title, options.FileNamespace);

        var categories = await UpdateCategoriesAsync(
            options,
            dataStore,
            wikiId,
            editor,
            owner,
            allowedEditors,
            allowedViewers,
            wikiLinks)
            .ConfigureAwait(false);

        var file = new WikiFile(
            wikiId,
            title,
            filePath,
            fileSize,
            editor,
            type,
            markdown,
            RenderHtml(options, dataStore, PostprocessArticleMarkdown(options, dataStore, title, options.FileNamespace, markdown)),
            RenderPreview(options, dataStore, PostprocessArticleMarkdown(options, dataStore, title, options.FileNamespace, markdown, true)),
            new ReadOnlyCollection<WikiLink>(wikiLinks),
            revision.TimestampTicks,
            options.FileNamespace,
            isDeleted: false,
            owner,
            allowedEditors,
            allowedViewers,
            categories,
            transclusions);
        await dataStore.StoreItemAsync(file).ConfigureAwait(false);

        await AddPageTransclusionsAsync(dataStore, wikiId, transclusions)
            .ConfigureAwait(false);

        await AddPageLinksAsync(dataStore, wikiId, wikiLinks)
            .ConfigureAwait(false);

        await UpdateReferencesAsync(
            options,
            dataStore,
            title,
            options.FileNamespace,
            false,
            true,
            null,
            null,
            false,
            false)
            .ConfigureAwait(false);

        if (options.OnCreated is not null)
        {
            await options.OnCreated.Invoke(file, editor).ConfigureAwait(false);
        }

        return file;
    }

    /// <summary>
    /// Revises this <see cref="WikiFile"/> instance.
    /// </summary>
    /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="editor">
    /// The ID of the user who made this revision.
    /// </param>
    /// <param name="title">
    /// <para>
    /// The optional new title of the file. Must be unique within its namespace, and non-empty.
    /// </para>
    /// <para>
    /// If left <see langword="null"/> the existing title will be retained.
    /// </para>
    /// </param>
    /// <param name="path">The relative path to the file.</param>
    /// <param name="fileSize">The size of the file, in bytes.</param>
    /// <param name="type">
    /// <para>
    /// The MIME type of the file.
    /// </para>
    /// <para>
    /// If left <see langword="null"/> the existing type will be retained.
    /// </para>
    /// </param>
    /// <param name="markdown">The markdown.</param>
    /// <param name="revisionComment">
    /// An optional comment supplied for this revision (e.g. to explain the changes).
    /// </param>
    /// <param name="isDeleted">Indicates that this file has been marked as deleted.</param>
    /// <param name="owner">
    /// <para>
    /// The new owner of the file.
    /// </para>
    /// <para>
    /// May be a user, a group, or <see langword="null"/>.
    /// </para>
    /// </param>
    /// <param name="allowedEditors">
    /// <para>
    /// The user(s) and/or group(s) allowed to edit this file.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the file can be edited by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the file can only be edited by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to make edits.
    /// </para>
    /// </param>
    /// <param name="allowedViewers">
    /// <para>
    /// The user(s) and/or group(s) allowed to view this file.
    /// </para>
    /// <para>
    /// If <see langword="null"/> the file can be viewed by anyone.
    /// </para>
    /// <para>
    /// If non-<see langword="null"/> the file can only be viewed by those listed, plus its
    /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
    /// langword="null"/>) list allows only the owner to view the file.
    /// </para>
    /// </param>
    public async Task ReviseAsync(
        IWikiOptions options,
        IDataStore dataStore,
        string editor,
        string? title = null,
        string? path = null,
        int? fileSize = null,
        string? type = null,
        string? markdown = null,
        string? revisionComment = null,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null)
    {
        title ??= title?.ToWikiTitleCase() ?? Title;

        var newFile = false;
        if (!string.IsNullOrWhiteSpace(path))
        {
            newFile = path != FilePath;
            FilePath = path;
        }
        if (fileSize.HasValue)
        {
            newFile |= fileSize.Value != FileSize;
            FileSize = fileSize.Value;
        }
        if (!string.IsNullOrEmpty(type))
        {
            newFile |= type != FileType;
            FileType = type;
        }

        if (newFile)
        {
            Uploader = editor;
        }

        var previousTitle = Title;
        Title = title;
        var sameTitle = string.Equals(previousTitle, title, StringComparison.Ordinal);

        var previousMarkdown = MarkdownContent;
        var wasDeleted = IsDeleted || string.IsNullOrWhiteSpace(previousMarkdown);

        if (isDeleted || string.IsNullOrWhiteSpace(markdown))
        {
            if (!wasDeleted)
            {
                Html = string.Empty;
                IsDeleted = true;
                MarkdownContent = string.Empty;
                Preview = string.Empty;
                Categories = new List<string>().AsReadOnly();

                if (Transclusions is not null)
                {
                    await RemovePageTransclusionsAsync(dataStore, Id, Transclusions)
                        .ConfigureAwait(false);
                }

                await RemovePageLinksAsync(dataStore, Id, WikiLinks)
                    .ConfigureAwait(false);

                Transclusions = null;
                WikiLinks = new List<WikiLink>().AsReadOnly();
            }
        }
        else
        {
            IsDeleted = false;
        }

        if (!sameTitle && !IsDeleted)
        {
            await CreatePageReferenceAsync(dataStore, Id, title, options.FileNamespace)
                .ConfigureAwait(false);
        }
        if (!sameTitle)
        {
            await RemovePageReferenceAsync(dataStore, Id, previousTitle, options.FileNamespace)
                .ConfigureAwait(false);
        }

        var changed = wasDeleted != IsDeleted
            || !string.Equals(previousMarkdown, markdown, StringComparison.Ordinal);

        if (!IsDeleted && changed)
        {
            MarkdownContent = markdown!;

            var previousTransclusions = Transclusions?.ToList() ?? new List<Transclusion>();
            var md = TransclusionParser.Transclude(
                options,
                dataStore,
                title,
                $"{options.FileNamespace}:{title}",
                markdown!,
                out var transclusions);
            Transclusions = transclusions.Count == 0
                ? null
                : transclusions.AsReadOnly();
            await RemovePageTransclusionsAsync(dataStore, Id, previousTransclusions.Except(transclusions))
                .ConfigureAwait(false);
            await AddPageTransclusionsAsync(dataStore, Id, transclusions.Except(previousTransclusions))
                .ConfigureAwait(false);

            var previousWikiLinks = WikiLinks.ToList();
            WikiLinks = GetWikiLinks(options, dataStore, md, title, options.FileNamespace).AsReadOnly();
            await RemovePageLinksAsync(dataStore, Id, previousWikiLinks.Except(WikiLinks))
                .ConfigureAwait(false);
            await AddPageLinksAsync(dataStore, Id, WikiLinks.Except(previousWikiLinks))
                .ConfigureAwait(false);
        }

        if (changed)
        {
            Categories = (await UpdateCategoriesAsync(
                options,
                dataStore,
                Id,
                editor,
                owner,
                allowedEditors,
                allowedViewers,
                WikiLinks,
                Categories)
                .ConfigureAwait(false))
                .AsReadOnly();

            Update(options, dataStore);
        }

        var oldOwner = Owner;
        Owner = owner;
        AllowedEditors = allowedEditors?.ToList().AsReadOnly();
        AllowedViewers = allowedViewers?.ToList().AsReadOnly();

        var revision = new Revision(
            Id,
            editor,
            title,
            options.FileNamespace,
            previousMarkdown,
            MarkdownContent,
            revisionComment);
        await dataStore.StoreItemAsync(revision).ConfigureAwait(false);

        TimestampTicks = revision.TimestampTicks;

        await dataStore.StoreItemAsync(this).ConfigureAwait(false);

        await UpdateReferencesAsync(
            options,
            dataStore,
            title,
            options.FileNamespace,
            IsDeleted,
            sameTitle,
            previousTitle,
            options.FileNamespace,
            false,
            false)
            .ConfigureAwait(false);

        if (isDeleted && !wasDeleted)
        {
            if (options.OnDeleted is not null)
            {
                await options.OnDeleted(this, oldOwner, Owner).ConfigureAwait(false);
            }
            else if (options.OnEdited is not null)
            {
                await options.OnEdited(this, revision, oldOwner, Owner).ConfigureAwait(false);
            }
        }
        else if (options.OnEdited is not null)
        {
            await options.OnEdited(this, revision, oldOwner, Owner).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets a string which shows the given file <paramref name="size"/> in bytes if &lt; 1KB,
    /// in KB if &lt; 1GB, or in GB otherwise.
    /// </summary>
    /// <param name="size">A file size, in bytes.</param>
    /// <returns>
    /// A string which shows the given file <paramref name="size"/> in bytes if &lt; 1KB, in KB
    /// if &lt; 1GB, or in GB otherwise.
    /// </returns>
    public static string GetFileSizeString(int size)
    {
        if (size < 1000)
        {
            return $"{size} B";
        }
        else if (size < 1000000)
        {
            return $"{size / 1000:N0} KB";
        }
        else
        {
            return $"{size / 1000000.0:N} GB";
        }
    }
}
