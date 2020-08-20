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
    /// A wiki category revision.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    [Newtonsoft.Json.JsonConverter(typeof(Converters.NewtonsoftJson.NoConverter))]
    public sealed class Category : Article
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string CategoryIdItemTypeName = ":Article:Category:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public override string IdItemTypeName => CategoryIdItemTypeName;

        /// <summary>
        /// The list of IDs of the items (including child <see cref="Category"/> items) which belong
        /// to this category.
        /// </summary>
        /// <remarks>
        /// Updates to this cache do not count as a revision.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public ICollection<string> ChildIds { get; } = new List<string>();

        /// <summary>
        /// Gets the full title of this item (including namespace).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override string FullTitle => $"{WikiConfig.CategoryNamespace}:{Title}";

        /// <summary>
        /// The namespace to which this category belongs.
        /// </summary>
        public override string WikiNamespace => WikiConfig.CategoryNamespace;

        /// <summary>
        /// Initializes a new instance of <see cref="Category"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="title">
        /// The title of this article. Must be unique within its namespace, and non-empty.
        /// </param>
        /// <param name="markdownContent">The raw markdown.</param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        /// <param name="childIds">
        /// The list of IDs of the items (including child <see cref="Category"/> items) which belong
        /// to this category.
        /// </param>
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
        public Category(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: Used by deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string title,
            string markdownContent,
            IReadOnlyCollection<WikiLink> wikiLinks,
            ICollection<string> childIds,
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
                markdownContent,
                wikiLinks,
                timestampTicks,
                WikiConfig.CategoryNamespace,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                null,
                null,
                false,
                false,
                categories,
                transclusions) => ChildIds = childIds;

        private Category(
            string id,
            string title,
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
                WikiConfig.CategoryNamespace,
                isDeleted,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions)
        { }

        private Category(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            CategoryIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>)) ?? new ReadOnlyCollection<WikiLink>(new WikiLink[0]),
            (ICollection<string>?)info.GetValue(nameof(ChildIds), typeof(ICollection<string>)) ?? new List<string>(),
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (bool?)info.GetValue(nameof(IsDeleted), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(Owner), typeof(string)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedEditors), typeof(IReadOnlyCollection<string>)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedViewers), typeof(IReadOnlyCollection<string>)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(Categories), typeof(IReadOnlyCollection<string>)) ?? new ReadOnlyCollection<string>(new string[0]),
            (IReadOnlyList<Transclusion>?)info.GetValue(nameof(Transclusions), typeof(IReadOnlyList<Transclusion>)))
        { }

        /// <summary>
        /// Gets the latest revision for the article with the given title.
        /// </summary>
        /// <param name="title">The title of the article to retrieve.</param>
        /// <returns>The latest revision for the article with the given title; or <see
        /// langword="null"/> if no such article exists.</returns>
        public static Category? GetCategory(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            var category = WikiConfig.DataStore.Query<Category>()
                .Where(x => x.Title == title)
                .OrderBy(x => x.TimestampTicks, descending: true)
                .FirstOrDefault();
            // If no exact match exists, ignore case if only one such match exists.
            if (category is null)
            {
                var categories = WikiConfig.DataStore.Query<Category>()
                    .Where(x => x.Title.ToLower() == title.ToLower())
                    .OrderBy(x => x.TimestampTicks, descending: true)
                    .ToList();
                if (categories.Count == 1)
                {
                    category = categories[0];
                }
            }
            return category;
        }

        /// <summary>
        /// Gets a particular revision for the article with the given title.
        /// </summary>
        /// <param name="title">The title of the article to retrieve.</param>
        /// <param name="timestamp">The timestamp of the revision.</param>
        /// <returns>The revision of the article with the given title and timestamp; or <see
        /// langword="null"/> if no such article exists.</returns>
        public static Category? GetCategory(string? title, DateTimeOffset timestamp)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            var ticks = timestamp.ToUniversalTime().Ticks;
            var category = WikiConfig.DataStore.Query<Category>()
                .FirstOrDefault(x => x.Title == title && x.TimestampTicks == ticks);
            // If no exact match exists, ignore case if only one such match exists.
            if (category is null)
            {
                var categories = WikiConfig.DataStore.Query<Category>()
                    .Where(x => x.Title.ToLower() == title.ToLower()
                        && x.TimestampTicks == ticks)
                    .ToList();
                if (categories.Count == 1)
                {
                    category = categories[0];
                }
            }
            return category;
        }

        /// <summary>
        /// Gets a new <see cref="Category"/> instance.
        /// </summary>
        /// <param name="title">The title of the category. Must be unique within its namespace, and
        /// non-empty.</param>
        /// <param name="editor">
        /// The ID of the user who made this revision.
        /// </param>
        /// <param name="markdown">The raw markdown content.</param>
        /// <param name="owner">
        /// <para>
        /// The owner of the category.
        /// </para>
        /// <para>
        /// May be a user, a group, or <see langword="null"/>.
        /// </para>
        /// </param>
        /// <param name="allowedEditors">
        /// <para>
        /// The user(s) and/or group(s) allowed to edit this category.
        /// </para>
        /// <para>
        /// If <see langword="null"/> the category can be edited by anyone.
        /// </para>
        /// <para>
        /// If non-<see langword="null"/> the category can only be edited by those listed, plus its
        /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
        /// langword="null"/>) list allows only the owner to make edits.
        /// </para>
        /// </param>
        /// <param name="allowedViewers">
        /// <para>
        /// The user(s) and/or group(s) allowed to view this category.
        /// </para>
        /// <para>
        /// If <see langword="null"/> the category can be viewed by anyone.
        /// </para>
        /// <para>
        /// If non-<see langword="null"/> the category can only be viewed by those listed, plus its
        /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
        /// langword="null"/>) list allows only the owner to view the category.
        /// </para>
        /// </param>
        public static async Task<Category> NewAsync(
            string title,
            string editor,
            string? markdown = null,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            title = title.ToWikiTitleCase();
            if (WikiConfig.DataStore.Query<Category>().Any(x => x.Title == title))
            {
                throw new ArgumentException("The given category title is already in use", nameof(title));
            }
            var wikiId = WikiConfig.DataStore.CreateNewIdFor<Category>();

            var revision = new WikiRevision(
                wikiId,
                editor,
                title,
                WikiConfig.CategoryNamespace,
                null,
                markdown);
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
                    GetFullTitle(title, WikiConfig.CategoryNamespace),
                    markdown,
                    out transclusions);
            }

            var wikiLinks = GetWikiLinks(md);
            foreach (var link in wikiLinks.Where(x => x.Missing))
            {
                var existing = await WikiConfig.DataStore.Query<MissingPage>()
                    .FirstOrDefaultAsync(x => x.Title == link.Title && x.WikiNamespace == link.WikiNamespace)
                    .ConfigureAwait(false);
                if (existing is null)
                {
                    var missingPage = await MissingPage.NewAsync(
                        WikiConfig.DataStore.CreateNewIdFor<MissingPage>(),
                        link.Title,
                        link.WikiNamespace,
                        wikiId)
                        .ConfigureAwait(false);
                }
            }

            var categories = new List<string>();
            var categoryTitles = wikiLinks
                .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                .Select(x => x.Title)
                .ToList();
            foreach (var categoryTitle in categoryTitles)
            {
                var childCategory = GetCategory(categoryTitle)
                    ?? await NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                await childCategory.AddArticleAsync(wikiId).ConfigureAwait(false);
                categories.Add(childCategory.Title);
            }

            var category = new Category(
                wikiId,
                title,
                markdown,
                new ReadOnlyCollection<WikiLink>(wikiLinks),
                revision.TimestampTicks,
                isDeleted: false,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions);
            await WikiConfig.DataStore.StoreItemAsync(category).ConfigureAwait(false);

            var missing = await WikiConfig.DataStore.Query<MissingPage>()
                .FirstOrDefaultAsync(x => x.Title == title && x.WikiNamespace == WikiConfig.CategoryNamespace)
                .ConfigureAwait(false);
            if (!(missing is null))
            {
                await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
            }

            return category;
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
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(ChildIds), ChildIds);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
            info.AddValue(nameof(IsDeleted), IsDeleted);
            info.AddValue(nameof(Owner), Owner);
            info.AddValue(nameof(AllowedEditors), AllowedEditors);
            info.AddValue(nameof(AllowedViewers), AllowedViewers);
            info.AddValue(nameof(Categories), Categories);
            info.AddValue(nameof(Transclusions), Transclusions);
        }

        /// <summary>
        /// Revises this <see cref="Category"/> instance.
        /// </summary>
        /// <param name="editor">
        /// The ID of the user who made this revision.
        /// </param>
        /// <param name="title">
        /// <para>
        /// The optional new title of the category. Must be unique and non-empty.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the existing title will be retained.
        /// </para>
        /// </param>
        /// <param name="markdown">
        /// <para>
        /// The raw markdown content.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the existing markdown will be retained.
        /// </para>
        /// </param>
        /// <param name="revisionComment">
        /// An optional comment supplied for this revision (e.g. to explain the changes).
        /// </param>
        /// <param name="isDeleted">Indicates that this category has been marked as deleted.</param>
        /// <param name="owner">
        /// <para>
        /// The new owner of the category.
        /// </para>
        /// <para>
        /// May be a user, a group, or <see langword="null"/>.
        /// </para>
        /// </param>
        /// <param name="allowedEditors">
        /// <para>
        /// The user(s) and/or group(s) allowed to edit this category.
        /// </para>
        /// <para>
        /// If <see langword="null"/> the category can be edited by anyone.
        /// </para>
        /// <para>
        /// If non-<see langword="null"/> the category can only be edited by those listed, plus its
        /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
        /// langword="null"/>) list allows only the owner to make edits.
        /// </para>
        /// </param>
        /// <param name="allowedViewers">
        /// <para>
        /// The user(s) and/or group(s) allowed to view this category.
        /// </para>
        /// <para>
        /// If <see langword="null"/> the category can be viewed by anyone.
        /// </para>
        /// <para>
        /// If non-<see langword="null"/> the category can only be viewed by those listed, plus its
        /// owner (regardless of whether the owner is explicitly listed). An empty (but non-<see
        /// langword="null"/>) list allows only the owner to view the category.
        /// </para>
        /// </param>
        public async Task ReviseAsync(
            string editor,
            string? title = null,
            string? markdown = null,
            string? revisionComment = null,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null)
        {
            if (isDeleted && ChildIds.Count > 0)
            {
                throw new ArgumentException("Non-empty categories cannot be deleted", nameof(isDeleted));
            }

            title ??= Title;

            if (WikiConfig.DataStore.Query<Category>()
                .Any(x => x.Id != Id && x.Title == title))
            {
                throw new ArgumentException("The given category title is already in use", nameof(title));
            }

            var previousTitle = Title;
            Title = title;

            var previousMarkdown = MarkdownContent;

            if (isDeleted)
            {
                IsDeleted = true;
                MarkdownContent = string.Empty;
                WikiLinks = new List<WikiLink>().AsReadOnly();
                Categories = new List<string>().AsReadOnly();
                Transclusions = null;

                foreach (var link in WikiLinks)
                {
                    var missing = await WikiConfig.DataStore.Query<MissingPage>()
                        .FirstOrDefaultAsync(x => x.Title == link.Title
                            && x.WikiNamespace == link.WikiNamespace
                            && x.References.Count == 1
                            && x.References.Contains(Id))
                        .ConfigureAwait(false);
                    if (!(missing is null))
                    {
                        await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
                    }
                }
            }
            else if (!(markdown is null))
            {
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    IsDeleted = true;
                    MarkdownContent = string.Empty;
                    WikiLinks = new List<WikiLink>().AsReadOnly();
                    Categories = new List<string>().AsReadOnly();
                    Transclusions = null;

                    foreach (var link in WikiLinks)
                    {
                        var missingLink = await WikiConfig.DataStore.Query<MissingPage>()
                            .FirstOrDefaultAsync(x => x.Title == link.Title
                                && x.WikiNamespace == link.WikiNamespace
                                && x.References.Count == 1
                                && x.References.Contains(Id))
                            .ConfigureAwait(false);
                        if (!(missingLink is null))
                        {
                            await WikiConfig.DataStore.RemoveItemAsync(missingLink).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    MarkdownContent = markdown;

                    var md = TransclusionParser.Transclude(
                        title,
                        GetFullTitle(title, WikiConfig.CategoryNamespace),
                        markdown,
                        out var transclusions);
                    Transclusions = transclusions.Count == 0
                        ? null
                        : transclusions.AsReadOnly();

                    var previousWikiLinks = WikiLinks.ToList();
                    WikiLinks = GetWikiLinks(md).AsReadOnly();
                    foreach (var link in previousWikiLinks.Except(WikiLinks))
                    {
                        var missingLink = await WikiConfig.DataStore.Query<MissingPage>()
                            .FirstOrDefaultAsync(x => x.Title == link.Title
                                && x.WikiNamespace == link.WikiNamespace
                                && x.References.Count == 1
                                && x.References.Contains(Id))
                            .ConfigureAwait(false);
                        if (!(missingLink is null))
                        {
                            await WikiConfig.DataStore.RemoveItemAsync(missingLink).ConfigureAwait(false);
                        }
                    }
                    foreach (var link in WikiLinks.Except(previousWikiLinks).Where(x => x.Missing))
                    {
                        var existing = await WikiConfig.DataStore.Query<MissingPage>()
                            .FirstOrDefaultAsync(x => x.Title == link.Title && x.WikiNamespace == link.WikiNamespace)
                            .ConfigureAwait(false);
                        if (existing is null)
                        {
                            var missingPage = await MissingPage.NewAsync(
                                WikiConfig.DataStore.CreateNewIdFor<MissingPage>(),
                                link.Title,
                                link.WikiNamespace,
                                Id)
                                .ConfigureAwait(false);
                        }
                    }

                    var categories = Categories.ToList();
                    var categoryTitles = WikiLinks
                        .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                        .Select(x => x.Title)
                        .ToList();
                    var newCategories = new List<string>();
                    foreach (var categoryTitle in categoryTitles)
                    {
                        var childCategory = GetCategory(categoryTitle)
                            ?? await NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                        if (!childCategory.ChildIds.Contains(Id))
                        {
                            await childCategory.AddArticleAsync(this).ConfigureAwait(false);
                            newCategories.Add(childCategory.Title);
                        }
                    }
                    foreach (var removedCategory in categories.Except(newCategories).ToList())
                    {
                        var childCategory = GetCategory(removedCategory);
                        if (childCategory is null)
                        {
                            continue;
                        }
                        await childCategory.RemoveChildAsync(this).ConfigureAwait(false);
                        categories.Remove(childCategory.Title);
                    }
                    if (newCategories.Count > 0)
                    {
                        categories.AddRange(newCategories);
                    }
                    Categories = categories.AsReadOnly();
                }

                if (!string.Equals(title, previousTitle, StringComparison.Ordinal))
                {
                    var missing = await WikiConfig.DataStore.Query<MissingPage>()
                        .FirstOrDefaultAsync(x => x.Title == title && x.WikiNamespace == WikiConfig.CategoryNamespace)
                        .ConfigureAwait(false);
                    if (!(missing is null))
                    {
                        await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
                    }
                }
            }

            Owner = owner;
            AllowedEditors = allowedEditors?.ToList().AsReadOnly();
            AllowedViewers = allowedViewers?.ToList().AsReadOnly();

            var revision = new WikiRevision(
                Id,
                editor,
                title,
                WikiConfig.CategoryNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            TimestampTicks = revision.TimestampTicks;

            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);
        }

        internal async Task AddArticleAsync(string wikiId)
        {
            ChildIds.Add(wikiId);
            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);
        }

        internal Task AddArticleAsync(Article child) => AddArticleAsync(child.Id);

        internal async Task RemoveChildAsync(Article child)
        {
            ChildIds.Remove(child.Id);
            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);
        }
    }
}
