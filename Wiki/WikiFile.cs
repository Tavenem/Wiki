using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
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
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    [Newtonsoft.Json.JsonConverter(typeof(Converters.NewtonsoftJson.NoConverter))]
    public sealed class WikiFile : Article
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string WikiFileIdItemTypeName = ":Article:WikiFile:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
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
        /// Gets the full title of this item (including namespace).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override string FullTitle => $"{WikiConfig.FileNamespace}:{Title}";

        /// <summary>
        /// The namespace to which this file belongs.
        /// </summary>
        public override string WikiNamespace => WikiConfig.FileNamespace;

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
        /// <param name="html">The rendered HTML content.</param>
        /// <param name="markdownContent">The raw markdown.</param>
        /// <param name="preview">A preview of this item's rendered HTML.</param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        /// <param name="timestampTicks">
        /// The timestamp when this message was sent, in UTC Ticks.
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
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public WikiFile(
            string id,
            string idItemTypeName,
            string title,
            string filePath,
            int fileSize,
            string fileType,
            string html,
            string markdownContent,
            string preview,
            IReadOnlyCollection<WikiLink> wikiLinks,
            long timestampTicks,
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
                WikiConfig.FileNamespace,
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
        }

        private WikiFile(
            string id,
            string title,
            string filePath,
            int fileSize,
            string fileType,
            string? markdown,
            IReadOnlyCollection<WikiLink> wikiLinks,
            long timestampTicks,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null,
            IList<string>? categories = null,
            IList<Transclusion>? transclusions = null) : base(
                id,
                title,
                markdown,
                wikiLinks,
                timestampTicks,
                WikiConfig.FileNamespace,
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
        }

        private WikiFile(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            WikiFileIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(FilePath), typeof(string)) ?? string.Empty,
            (int?)info.GetValue(nameof(FileSize), typeof(int)) ?? default,
            (string?)info.GetValue(nameof(FileType), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Html), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Preview), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>))
                ?? new ReadOnlyCollection<WikiLink>(Array.Empty<WikiLink>()),
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (bool?)info.GetValue(nameof(IsDeleted), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(Owner), typeof(string)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedEditors), typeof(IReadOnlyCollection<string>)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedViewers), typeof(IReadOnlyCollection<string>)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(Categories), typeof(IReadOnlyCollection<string>))
                ?? new ReadOnlyCollection<string>(Array.Empty<string>()),
            (IReadOnlyList<Transclusion>?)info.GetValue(nameof(Transclusions), typeof(IReadOnlyList<Transclusion>)))
        { }

        /// <summary>
        /// Gets the latest revision for the file with the given title.
        /// </summary>
        /// <param name="title">The title of the file to retrieve.</param>
        /// <returns>The latest revision for the file with the given title; or <see
        /// langword="null"/> if no such file exists.</returns>
        public static WikiFile? GetFile(string title)
        {
            WikiFile? file = null;
            var reference = PageReference.GetPageReference(title, WikiConfig.FileNamespace);
            if (reference is not null)
            {
                file = WikiConfig.DataStore.GetItem<WikiFile>(reference.Reference);
            }
            // If no exact match exists, ignore case if only one such match exists.
            if (file is null)
            {
                var normalizedReference = NormalizedPageReference.GetNormalizedPageReference(title, WikiConfig.FileNamespace);
                if (normalizedReference is not null
                    && normalizedReference.References.Count == 1)
                {
                    file = WikiConfig.DataStore.GetItem<WikiFile>(normalizedReference.References[0]);
                }
            }

            return file;
        }

        /// <summary>
        /// Gets a new <see cref="WikiFile"/> instance.
        /// </summary>
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

            var wikiId = WikiConfig.DataStore.CreateNewIdFor<WikiFile>();

            await CreatePageReferenceAsync(wikiId, title, WikiConfig.FileNamespace).ConfigureAwait(false);

            var revision = new Revision(
                wikiId,
                editor,
                title,
                WikiConfig.FileNamespace,
                null,
                markdown,
                revisionComment);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            var md = markdown;
            List<Transclusion> transclusions;
            if (string.IsNullOrEmpty(markdown))
            {
                transclusions = new List<Transclusion>();
            }
            else
            {
                md = TransclusionParser.Transclude(
                    title,
                    $"{WikiConfig.FileNamespace}:{title}",
                    markdown,
                    out transclusions);
            }

            var wikiLinks = GetWikiLinks(md, title, WikiConfig.FileNamespace);

            var categories = await UpdateCategoriesAsync(
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
                type,
                markdown,
                new ReadOnlyCollection<WikiLink>(wikiLinks),
                revision.TimestampTicks,
                isDeleted: false,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions);
            await WikiConfig.DataStore.StoreItemAsync(file).ConfigureAwait(false);

            await AddPageTransclusionsAsync(wikiId, transclusions).ConfigureAwait(false);

            await AddPageLinksAsync(wikiId, wikiLinks).ConfigureAwait(false);

            await UpdateReferencesAsync(
                title,
                WikiConfig.FileNamespace,
                false,
                true,
                null,
                null,
                false,
                false)
                .ConfigureAwait(false);

            return file;
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(FilePath), FilePath);
            info.AddValue(nameof(FileSize), FileSize);
            info.AddValue(nameof(FileType), FileType);
            info.AddValue(nameof(Html), Html);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(Preview), Preview);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
            info.AddValue(nameof(IsDeleted), IsDeleted);
            info.AddValue(nameof(Owner), Owner);
            info.AddValue(nameof(AllowedEditors), AllowedEditors);
            info.AddValue(nameof(AllowedViewers), AllowedViewers);
            info.AddValue(nameof(Categories), Categories);
            info.AddValue(nameof(Transclusions), Transclusions);
        }

        /// <summary>
        /// Revises this <see cref="WikiFile"/> instance.
        /// </summary>
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

            if (!string.IsNullOrWhiteSpace(path))
            {
                FilePath = path;
            }
            if (fileSize.HasValue)
            {
                FileSize = fileSize.Value;
            }
            if (!string.IsNullOrEmpty(type))
            {
                FileType = type;
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
                        await RemovePageTransclusionsAsync(Id, Transclusions).ConfigureAwait(false);
                    }

                    await RemovePageLinksAsync(Id, WikiLinks).ConfigureAwait(false);

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
                await CreatePageReferenceAsync(Id, title, WikiConfig.FileNamespace).ConfigureAwait(false);
            }
            if (!sameTitle)
            {
                await RemovePageReferenceAsync(Id, previousTitle, WikiConfig.FileNamespace).ConfigureAwait(false);
            }

            var changed = wasDeleted != IsDeleted
                || !string.Equals(previousMarkdown, markdown, StringComparison.Ordinal);

            if (!IsDeleted && changed)
            {
                MarkdownContent = markdown!;

                var previousTransclusions = Transclusions?.ToList() ?? new List<Transclusion>();
                var md = TransclusionParser.Transclude(
                    title,
                    $"{WikiConfig.FileNamespace}:{title}",
                    markdown!,
                    out var transclusions);
                Transclusions = transclusions.Count == 0
                    ? null
                    : transclusions.AsReadOnly();
                await RemovePageTransclusionsAsync(Id, previousTransclusions.Except(transclusions)).ConfigureAwait(false);
                await AddPageTransclusionsAsync(Id, transclusions.Except(previousTransclusions)).ConfigureAwait(false);

                var previousWikiLinks = WikiLinks.ToList();
                WikiLinks = GetWikiLinks(md, title, WikiConfig.FileNamespace).AsReadOnly();
                await RemovePageLinksAsync(Id, previousWikiLinks.Except(WikiLinks)).ConfigureAwait(false);
                await AddPageLinksAsync(Id, WikiLinks.Except(previousWikiLinks)).ConfigureAwait(false);
            }

            if (changed)
            {
                Categories = (await UpdateCategoriesAsync(
                    Id,
                    editor,
                    owner,
                    allowedEditors,
                    allowedViewers,
                    WikiLinks,
                    Categories)
                    .ConfigureAwait(false))
                    .AsReadOnly();

                Update();
            }

            Owner = owner;
            AllowedEditors = allowedEditors?.ToList().AsReadOnly();
            AllowedViewers = allowedViewers?.ToList().AsReadOnly();

            var revision = new Revision(
                Id,
                editor,
                title,
                WikiConfig.FileNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            TimestampTicks = revision.TimestampTicks;

            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);

            await UpdateReferencesAsync(
                title,
                WikiConfig.FileNamespace,
                IsDeleted,
                sameTitle,
                previousTitle,
                WikiConfig.FileNamespace,
                false,
                false)
                .ConfigureAwait(false);
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
}
