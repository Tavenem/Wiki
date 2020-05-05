using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A file tracked by the wiki system.
    /// </summary>
    [Serializable]
    public sealed class WikiFile : Article
    {
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
        public override string FullTitle => $"{WikiConfig.FileNamespace}:{Title}";

        /// <summary>
        /// The namespace to which this file belongs.
        /// </summary>
        public override string WikiNamespace => WikiConfig.FileNamespace;

        private WikiFile(
            string id,
            string title,
            string filePath,
            int fileSize,
            string fileType,
            string? markdown,
            IList<WikiLink> wikiLinks,
            DateTimeOffset timestamp,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null,
            IList<string>? categories = null,
            IList<string>? transclusions = null) : base(
                id,
                title,
                markdown,
                wikiLinks,
                timestamp,
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
            (string)info.GetValue(nameof(Id), typeof(string)),
            (string)info.GetValue(nameof(Title), typeof(string)),
            (string)info.GetValue(nameof(FilePath), typeof(string)),
            (int)info.GetValue(nameof(FileSize), typeof(int)),
            (string)info.GetValue(nameof(FileType), typeof(string)),
            (string)info.GetValue(nameof(MarkdownContent), typeof(string)),
            (IList<WikiLink>)info.GetValue(nameof(WikiLinks), typeof(IList<WikiLink>)),
            (DateTimeOffset)info.GetValue(nameof(Timestamp), typeof(DateTimeOffset)),
            (bool)info.GetValue(nameof(IsDeleted), typeof(bool)),
            (string?)info.GetValue(nameof(Owner), typeof(string)),
            (IList<string>?)info.GetValue(nameof(AllowedEditors), typeof(IList<string>)),
            (IList<string>?)info.GetValue(nameof(AllowedViewers), typeof(IList<string>)),
            (IList<string>?)info.GetValue(nameof(Categories), typeof(IList<string>)),
            (IList<string>?)info.GetValue(nameof(Transclusions), typeof(IList<string>)))
        { }

        /// <summary>
        /// Gets the latest revision for the file with the given title.
        /// </summary>
        /// <param name="title">The title of the file to retrieve.</param>
        /// <returns>The latest revision for the file with the given title; or <see
        /// langword="null"/> if no such file exists.</returns>
        public static WikiFile? GetFile(string title)
        {
            var file = DataStore.GetFirstItemWhereOrderedBy<WikiFile, DateTimeOffset>(
                x => x.Title == title,
                x => x.Timestamp,
                descending: true);
            // If no exact match exists, ignore case if only one such match exists.
            if (file is null)
            {
                var files = DataStore.GetItemsWhereOrderedBy<WikiFile, DateTimeOffset>(
                    x => string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase),
                    x => x.Timestamp,
                    descending: true);
                if (files.Count == 1)
                {
                    file = files[0];
                }
            }
            return file;
        }

        /// <summary>
        /// Gets a particular revision for the file with the given title.
        /// </summary>
        /// <param name="title">The title of the file to retrieve.</param>
        /// <param name="timestamp">The timestamp of the revision.</param>
        /// <returns>The revision of the file with the given title and timestamp; or <see
        /// langword="null"/> if no such file exists.</returns>
        public static WikiFile? GetFile(string title, DateTimeOffset timestamp)
        {
            var file = DataStore.GetFirstItemWhere<WikiFile>(x => x.Title == title
                && x.Timestamp == timestamp);
            // If no exact match exists, ignore case if only one such match exists.
            if (file is null)
            {
                var files = DataStore.GetItemsWhere<WikiFile>(x => string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase)
                    && x.Timestamp == timestamp);
                if (files.Count == 1)
                {
                    file = files[0];
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
            if (DataStore.GetItemsWhere<WikiFile>(x => x.Title == title).Count > 0)
            {
                throw new ArgumentException("The given title is already in use", nameof(title));
            }
            var wikiId = Guid.NewGuid().ToString();

            var revision = new WikiRevision(
                wikiId,
                editor,
                title,
                WikiConfig.FileNamespace,
                null,
                markdown,
                revisionComment);
            await revision.SaveAsync().ConfigureAwait(false);

            var md = markdown;
            List<string> transclusions;
            if (string.IsNullOrEmpty(markdown))
            {
                transclusions = new List<string>();
            }
            else
            {
                md = TransclusionParser.Transclude(
                    title,
                    GetFullTitle(title, WikiConfig.FileNamespace),
                    markdown,
                    out transclusions);
            }

            var wikiLinks = GetWikiLinks(md);

            var categories = new List<string>();
            var categoryTitles = wikiLinks
                .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                .Select(x => x.Title)
                .ToList();
            foreach (var categoryTitle in categoryTitles)
            {
                var category = Category.GetCategory(categoryTitle)
                    ?? await Category.NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                await category.AddArticleAsync(wikiId).ConfigureAwait(false);
                categories.Add(category.Title);
            }

            var file = new WikiFile(
                wikiId,
                title,
                filePath,
                fileSize,
                type,
                markdown,
                wikiLinks,
                revision.Timestamp,
                isDeleted: false,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions);
            await file.SaveAsync().ConfigureAwait(false);
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
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(Timestamp), Timestamp);
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
            title ??= Title;
            if (DataStore.GetItemsWhere<WikiFile>(x => x.Id != Id && x.Title == title).Count > 0)
            {
                throw new ArgumentException("The given title is already in use", nameof(title));
            }

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

            Title = title;

            var previousMarkdown = MarkdownContent;
            if (isDeleted)
            {
                IsDeleted = true;
                MarkdownContent = string.Empty;
                WikiLinks = new List<WikiLink>().AsReadOnly();
                Categories = new List<string>().AsReadOnly();
                Transclusions = new List<string>().AsReadOnly();
            }
            else if (!(markdown is null))
            {
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    IsDeleted = true;
                    MarkdownContent = string.Empty;
                    WikiLinks = new List<WikiLink>().AsReadOnly();
                    Categories = new List<string>().AsReadOnly();
                    Transclusions = new List<string>().AsReadOnly();
                }
                else
                {
                    MarkdownContent = markdown;

                    var md = TransclusionParser.Transclude(
                        title,
                        GetFullTitle(title, WikiConfig.FileNamespace),
                        markdown,
                        out var transclusions);
                    Transclusions = transclusions.AsReadOnly();

                    WikiLinks = GetWikiLinks(md).AsReadOnly();

                    var categories = Categories.ToList();
                    var categoryTitles = WikiLinks
                        .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                        .Select(x => x.Title)
                        .ToList();
                    var newCategories = new List<string>();
                    foreach (var categoryTitle in categoryTitles)
                    {
                        var category = Category.GetCategory(categoryTitle)
                            ?? await Category.NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                        if (!category.ChildIds.Contains(Id))
                        {
                            await category.AddArticleAsync(Id).ConfigureAwait(false);
                            newCategories.Add(category.Title);
                        }
                    }
                    foreach (var removedCategory in categories.Except(newCategories).ToList())
                    {
                        var category = Category.GetCategory(removedCategory);
                        if (category is null)
                        {
                            continue;
                        }
                        await category.RemoveChildAsync(this).ConfigureAwait(false);
                        categories.Remove(category.Title);
                    }
                    if (newCategories.Count > 0)
                    {
                        categories.AddRange(newCategories);
                    }
                    Categories = categories.AsReadOnly();
                }
            }

            Owner = owner;
            AllowedEditors = allowedEditors?.ToList().AsReadOnly();
            AllowedViewers = allowedViewers?.ToList().AsReadOnly();

            var revision = new WikiRevision(
                Id,
                editor,
                title,
                WikiConfig.FileNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await revision.SaveAsync().ConfigureAwait(false);

            Timestamp = revision.Timestamp;

            await SaveAsync().ConfigureAwait(false);
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
