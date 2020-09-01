using NeverFoundry.DataStorage;
using NeverFoundry.DiffPatchMerge;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A wiki article revision.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    [System.Text.Json.Serialization.JsonConverter(typeof(Converters.ArticleConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(Converters.NewtonsoftJson.ArticleConverter))]
    public class Article : MarkdownItem
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string ArticleIdItemTypeName = ":Article:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public virtual string IdItemTypeName => ArticleIdItemTypeName;

        /// <summary>
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
        /// </summary>
        /// <remarks>
        /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyCollection<string>? AllowedEditors { get; private protected set; }

        /// <summary>
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
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cannot be set if the <see cref="Owner"/> is <see langword="null"/>.
        /// </para>
        /// <para>
        /// A user who cannot view an article is still able to see that an article by that title
        /// exists (to avoid confusion about creating a new article with that title).
        /// </para>
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyCollection<string>? AllowedViewers { get; private protected set; }

        /// <summary>
        /// <para>
        /// The titles of the categories to which this article belongs.
        /// </para>
        /// <para>
        /// Note that this list is a cache that is filled when the article is rendered, and will not
        /// be accurate prior to rendering.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Updates to this list (due to changes in transcluded articles) do not count as revisions.
        /// </remarks>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyCollection<string> Categories { get; private protected set; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Gets the full title of this article (including namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual string FullTitle => string.CompareOrdinal(WikiNamespace, WikiConfig.DefaultNamespace) == 0
            ? Title
            : $"{WikiNamespace}:{Title}";

        /// <summary>
        /// <para>
        /// Indicates whether this is a redirect to a page which does not exist.
        /// </para>
        /// <para>
        /// Updates to this property do not constitute a revision.
        /// </para>
        /// </summary>
        public bool IsBrokenRedirect { get; private protected set; }

        /// <summary>
        /// Indicates that this article has been marked as deleted.
        /// </summary>
        public bool IsDeleted { get; private protected set; }

        /// <summary>
        /// <para>
        /// Indicates whether this is a redirect to a page which is also a redirect.
        /// </para>
        /// <para>
        /// Updates to this property do not constitute a revision.
        /// </para>
        /// </summary>
        public bool IsDoubleRedirect { get; private protected set; }

        /// <summary>
        /// <para>
        /// The owner of this article.
        /// </para>
        /// <para>
        /// May be a user, a group, or <see langword="null"/>.
        /// </para>
        /// </summary>
        public string? Owner { get; private protected set; }

        /// <summary>
        /// If this is a redirect, optionally contains the namespace.
        /// </summary>
        public string? RedirectNamespace { get; private set; }

        /// <summary>
        /// If this is a redirect, contains the title of the destination.
        /// </summary>
        public string? RedirectTitle { get; private set; }

        /// <summary>
        /// The timestamp of the latest revision to this item, in UTC.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get => new DateTimeOffset(TimestampTicks, TimeSpan.Zero);
            set => TimestampTicks = value.ToUniversalTime().Ticks;
        }

        /// <summary>
        /// The timestamp of the latest revision to this item, in UTC Ticks.
        /// </summary>
        public long TimestampTicks { get; private protected set; }

        /// <summary>
        /// The title of this article. Must be unique within its namespace, and non-empty.
        /// </summary>
        public string Title { get; private protected set; }

        /// <summary>
        /// The transclusions within this article.
        /// </summary>
        [Newtonsoft.Json.JsonProperty(TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None)]
        public IReadOnlyList<Transclusion>? Transclusions { get; private protected set; }

        /// <summary>
        /// The namespace to which this article belongs.
        /// </summary>
        public virtual string WikiNamespace { get; private protected set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Article"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="title">
        /// The title of this article. Must be unique within its namespace, and non-empty.
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
        /// <param name="redirectNamespace">
        /// If this is a redirect, optionally contains the namespace.
        /// </param>
        /// <param name="redirectTitle">
        /// If this is a redirect, contains the title of the destination.
        /// </param>
        /// <param name="isBrokenRedirect">
        /// Indicates whether this is a redirect to a page which does not exist.
        /// </param>
        /// <param name="isDoubleRedirect">
        /// Indicates whether this is a redirect to a page which is also a redirect.
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
        [Newtonsoft.Json.JsonConstructor]
        public Article(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: Used by deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string title,
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
            string? redirectNamespace,
            string? redirectTitle,
            bool isBrokenRedirect,
            bool isDoubleRedirect,
            IReadOnlyCollection<string> categories,
            IReadOnlyList<Transclusion>? transclusions) : base(id, html, markdownContent, preview, wikiLinks)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            wikiNamespace ??= WikiConfig.DefaultNamespace;

            if (!string.IsNullOrEmpty(owner))
            {
                AllowedEditors = allowedEditors;
                AllowedViewers = allowedViewers;
            }
            Categories = categories;
            IsBrokenRedirect = isBrokenRedirect;
            IsDeleted = isDeleted;
            IsDoubleRedirect = isDoubleRedirect;
            Owner = owner;
            RedirectNamespace = redirectNamespace;
            RedirectTitle = redirectTitle;
            TimestampTicks = timestampTicks;
            Title = title;
            Transclusions = transclusions;
            WikiNamespace = wikiNamespace;
        }

        private protected Article(
            string id,
            string title,
            string? markdown,
            IReadOnlyCollection<WikiLink> wikiLinks,
            long timestampTicks,
            string? wikiNamespace = null,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null,
            IList<string>? categories = null,
            IList<Transclusion>? transclusions = null,
            string? redirectNamespace = null,
            string? redirectTitle = null,
            bool isBrokenRedirect = false,
            bool isDoubleRedirect = false) : base(id, markdown, wikiLinks)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            wikiNamespace ??= WikiConfig.DefaultNamespace;

            if (!string.IsNullOrEmpty(owner))
            {
                AllowedEditors = allowedEditors?.ToList().AsReadOnly();
                AllowedViewers = allowedViewers?.ToList().AsReadOnly();
            }
            Categories = new ReadOnlyCollection<string>(categories ?? new List<string>());
            IsBrokenRedirect = isBrokenRedirect;
            IsDeleted = isDeleted;
            IsDoubleRedirect = isDoubleRedirect;
            Owner = owner;
            RedirectNamespace = redirectNamespace;
            RedirectTitle = redirectTitle;
            TimestampTicks = timestampTicks;
            Title = title;
            Transclusions = transclusions is null || transclusions.Count == 0
                ? null
                : new ReadOnlyCollection<Transclusion>(transclusions);
            WikiNamespace = wikiNamespace;
            Update();
        }

        private Article(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            ArticleIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Html), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Preview), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>)) ?? new ReadOnlyCollection<WikiLink>(Array.Empty<WikiLink>()),
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (bool?)info.GetValue(nameof(IsDeleted), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(Owner), typeof(string)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedEditors), typeof(IReadOnlyCollection<string>)),
            (IReadOnlyCollection<string>?)info.GetValue(nameof(AllowedViewers), typeof(IReadOnlyCollection<string>)),
            (string?)info.GetValue(nameof(RedirectNamespace), typeof(string)),
            (string?)info.GetValue(nameof(RedirectTitle), typeof(string)),
            (bool?)info.GetValue(nameof(IsBrokenRedirect), typeof(bool)) ?? default,
            (bool?)info.GetValue(nameof(IsDoubleRedirect), typeof(bool)) ?? default,
            (IReadOnlyCollection<string>?)info.GetValue(nameof(Categories), typeof(IReadOnlyCollection<string>)) ?? new ReadOnlyCollection<string>(Array.Empty<string>()),
            (IReadOnlyList<Transclusion>?)info.GetValue(nameof(Transclusions), typeof(IReadOnlyList<Transclusion>)))
        { }

        /// <summary>
        /// Gets the latest revision for the article with the given title.
        /// </summary>
        /// <param name="title">The title of the article to retrieve.</param>
        /// <param name="wikiNamespace">
        /// <para>
        /// The namespace of the article to retrieve.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the default namespace (<see
        /// cref="WikiConfig.DefaultNamespace"/>) will be assumed.
        /// </para>
        /// </param>
        /// <param name="noRedirect">
        /// If <see langword="true"/>, redirects will be ignored, and the literal content of a
        /// redirect article will be returned. Useful when a redirect itself is to be edited.
        /// </param>
        /// <returns>The latest revision for the article with the given title; or <see
        /// langword="null"/> if no such article exists.</returns>
        public static Article? GetArticle(string? title, string? wikiNamespace = null, bool noRedirect = false)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            wikiNamespace ??= WikiConfig.DefaultNamespace;
            Article? article = null;
            var count = 0;
            var ids = new HashSet<string>();
            bool redirect;
            do
            {
                redirect = false;

                var reference = PageReference.GetPageReference(title, wikiNamespace);
                if (reference is not null)
                {
                    article = WikiConfig.DataStore.GetItem<Article>(reference.Reference);
                }
                // If no exact match exists, ignore case if only one such match exists.
                if (article is null)
                {
                    var normalizedReference = NormalizedPageReference.GetNormalizedPageReference(title, wikiNamespace);
                    if (normalizedReference is not null
                        && normalizedReference.References.Count == 1)
                    {
                        article = WikiConfig.DataStore.GetItem<Article>(normalizedReference.References[0]);
                    }
                }

                if (!noRedirect && article?.IsBrokenRedirect != true && !string.IsNullOrEmpty(article?.RedirectTitle))
                {
                    redirect = true;
                    if (ids.Contains(article.Id))
                    {
                        break; // abort if a cycle is detected
                    }
                    ids.Add(article.Id);
                    title = article.RedirectTitle;
                    wikiNamespace = article.RedirectNamespace ?? WikiConfig.DefaultNamespace;
                }

                count++;
            } while (redirect && count < 100); // abort if redirection nests 100 levels
            return article;
        }

        /// <summary>
        /// Gets the full title from the given parts (includes namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        /// <returns>The full title from the given parts.</returns>
        public static string GetFullTitle(string title, string? wikiNamespace = null, bool talk = false)
        {
            if (talk)
            {
                return string.IsNullOrWhiteSpace(wikiNamespace)
                    || string.CompareOrdinal(wikiNamespace, WikiConfig.DefaultNamespace) == 0
                  ? $"{WikiConfig.TalkNamespace}:{title}"
                  : $"{WikiConfig.TalkNamespace}:{wikiNamespace}:{title}";
            }
            else
            {
                return string.IsNullOrWhiteSpace(wikiNamespace)
                    || string.CompareOrdinal(wikiNamespace, WikiConfig.DefaultNamespace) == 0
                  ? title
                  : $"{wikiNamespace}:{title}";
            }
        }

        /// <summary>
        /// Breaks the given title string into parts.
        /// </summary>
        /// <param name="text">The full title string.</param>
        /// <returns>
        /// The namespace and title, and <see cref="bool"/> flags indicating whether the title
        /// indicates a discussion page, as well as whether the namespace was omitted.
        /// </returns>
        public static (string wikiNamespace, string title, bool isTalk, bool defaultNamespace) GetTitleParts(string? text)
        {
            string wikiNamespace, title;
            var isTalk = false;
            var defaultNamespace = false;
            if (string.IsNullOrWhiteSpace(text))
            {
                wikiNamespace = WikiConfig.DefaultNamespace;
                title = WikiConfig.MainPageTitle;
            }
            else
            {
                var parts = text.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 2)
                {
                    if (string.CompareOrdinal(parts[0], WikiConfig.TalkNamespace) == 0)
                    {
                        isTalk = true;
                        wikiNamespace = parts[1].ToWikiTitleCase();
                        title = string.Join(':', parts[2..^0]).ToWikiTitleCase();
                    }
                    else
                    {
                        wikiNamespace = parts[0].ToWikiTitleCase();
                        title = string.Join(':', parts[1..^0]).ToWikiTitleCase();
                    }
                }
                else if (parts.Length == 2)
                {
                    wikiNamespace = parts[0].ToWikiTitleCase();
                    title = parts[1].ToWikiTitleCase();
                }
                else
                {
                    wikiNamespace = WikiConfig.DefaultNamespace;
                    defaultNamespace = true;
                    title = text.ToWikiTitleCase();
                }
            }
            if (!isTalk && string.CompareOrdinal(wikiNamespace, WikiConfig.TalkNamespace) == 0)
            {
                isTalk = true;
                wikiNamespace = WikiConfig.DefaultNamespace;
                defaultNamespace = true;
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                title = WikiConfig.MainPageTitle;
                defaultNamespace = string.CompareOrdinal(wikiNamespace, WikiConfig.DefaultNamespace) != 0;
                wikiNamespace = WikiConfig.DefaultNamespace;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(wikiNamespace))
                {
                    wikiNamespace = WikiConfig.DefaultNamespace;
                    defaultNamespace = true;
                }
            }
            return (wikiNamespace, title, isTalk, defaultNamespace);
        }

        /// <summary>
        /// Gets a new <see cref="Article"/> instance.
        /// </summary>
        /// <param name="title">The title of the article. Must be unique within its namespace, and
        /// non-empty.</param>
        /// <param name="editor">
        /// The ID of the user who created this article.
        /// </param>
        /// <param name="markdown">The raw markdown content.</param>
        /// <param name="wikiNamespace">
        /// <para>
        /// The namespace to which this article belongs.
        /// </para>
        /// <para>
        /// If this is equal to <see cref="WikiConfig.CategoryNamespace"/>, the result will be a <see
        /// cref="Category"/> rather than an <see cref="Article"/>.
        /// </para>
        /// </param>
        /// <param name="owner">
        /// <para>
        /// The ID of the intended owner of the article.
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
        /// </param>
        public static async Task<Article> NewAsync(
            string title,
            string editor,
            string? markdown = null,
            string? wikiNamespace = null,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            wikiNamespace = wikiNamespace?.ToWikiTitleCase();
            if (string.Equals(wikiNamespace, WikiConfig.CategoryNamespace, StringComparison.CurrentCultureIgnoreCase))
            {
                return await Category.NewAsync(title, editor, markdown, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
            }
            if (WikiConfig.ReservedNamespaces.Any(x => string.Equals(wikiNamespace, x, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ArgumentException("Value for namespace was a reserved name", nameof(wikiNamespace));
            }
            if (string.IsNullOrWhiteSpace(wikiNamespace))
            {
                wikiNamespace = WikiConfig.DefaultNamespace;
            }

            title = title.ToWikiTitleCase();

            var wikiId = WikiConfig.DataStore.CreateNewIdFor<Article>();

            await CreatePageReferenceAsync(wikiId, title, wikiNamespace).ConfigureAwait(false);

            var revision = new Revision(
                wikiId,
                editor,
                title,
                wikiNamespace,
                null,
                markdown);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            var (
                redirectNamespace,
                redirectTitle,
                isRedirect,
                isBrokenRedirect,
                isDoubleRedirect) = await IdentifyRedirectAsync(wikiId, markdown).ConfigureAwait(false);

            var md = markdown;
            List<Transclusion> transclusions;
            if (isRedirect || string.IsNullOrEmpty(markdown))
            {
                transclusions = new List<Transclusion>();
            }
            else
            {
                md = TransclusionParser.Transclude(
                    title,
                    GetFullTitle(title, wikiNamespace),
                    markdown,
                    out transclusions);
            }

            var wikiLinks = isRedirect
                ? new List<WikiLink>()
                : GetWikiLinks(md, title, wikiNamespace);

            var categories = await UpdateCategoriesAsync(
                wikiId,
                editor,
                owner,
                allowedEditors,
                allowedViewers,
                wikiLinks)
                .ConfigureAwait(false);

            var article = new Article(
                wikiId,
                title,
                markdown,
                new ReadOnlyCollection<WikiLink>(wikiLinks),
                revision.TimestampTicks,
                wikiNamespace,
                isDeleted: false,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions,
                redirectNamespace,
                redirectTitle,
                isBrokenRedirect,
                isDoubleRedirect);
            await WikiConfig.DataStore.StoreItemAsync(article).ConfigureAwait(false);

            await AddPageTransclusionsAsync(wikiId, transclusions).ConfigureAwait(false);

            await AddPageLinksAsync(wikiId, wikiLinks).ConfigureAwait(false);

            await UpdateReferencesAsync(
                title,
                wikiNamespace,
                false,
                true,
                null,
                null,
                true,
                isRedirect)
                .ConfigureAwait(false);

            return article;
        }

        private protected static async Task AddPageLinksAsync(string id, IEnumerable<WikiLink> wikiLinks)
        {
            foreach (var link in wikiLinks.Where(x => !x.IsCategory))
            {
                var linkReference = await PageLinks
                    .GetPageLinksAsync(link.Title, link.WikiNamespace)
                    .ConfigureAwait(false);
                if (linkReference is null)
                {
                    await PageLinks.NewAsync(link.Title, link.WikiNamespace, id).ConfigureAwait(false);
                }
                else
                {
                    await linkReference.AddReferenceAsync(id).ConfigureAwait(false);
                }

                if (link.Missing)
                {
                    var existing = await MissingPage
                        .GetMissingPageAsync(link.Title, link.WikiNamespace)
                        .ConfigureAwait(false);
                    if (existing is null)
                    {
                        var missingPage = await MissingPage.NewAsync(
                            link.Title,
                            link.WikiNamespace,
                            id)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await existing.AddReferenceAsync(id).ConfigureAwait(false);
                    }
                }
            }
        }

        private protected static async Task AddPageTransclusionsAsync(string id, IEnumerable<Transclusion> transclusions)
        {
            foreach (var transclusion in transclusions)
            {
                var reference = await PageTransclusions
                    .GetPageTransclusionsAsync(transclusion.Title, transclusion.WikiNamespace)
                    .ConfigureAwait(false);
                if (reference is null)
                {
                    await PageTransclusions.NewAsync(transclusion.Title, transclusion.WikiNamespace, id).ConfigureAwait(false);
                }
                else
                {
                    await reference.AddReferenceAsync(id).ConfigureAwait(false);
                }
            }
        }

        private protected static async Task CreatePageReferenceAsync(string id, string title, string wikiNamespace)
        {
            var existingPage = await PageReference
                .GetPageReferenceAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (existingPage is null)
            {
                await PageReference.NewAsync(id, title, wikiNamespace).ConfigureAwait(false);
            }
            else if (existingPage.Reference != id)
            {
                throw new ArgumentException("The given title is already in use for this namespace", nameof(title));
            }

            var existingNormalizedReference = await NormalizedPageReference
                .GetNormalizedPageReferenceAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (existingNormalizedReference is null)
            {
                await NormalizedPageReference.NewAsync(id, title, wikiNamespace).ConfigureAwait(false);
            }
            else
            {
                await existingNormalizedReference.AddReferenceAsync(id).ConfigureAwait(false);
            }

            var missing = await MissingPage
                .GetMissingPageAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (missing is not null)
            {
                await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
            }
        }

        private static async Task<(
            HashSet<string> idsToUpdate,
            HashSet<string> idsToUpdateRecursively,
            HashSet<string> redirectsToUpdate)> GetIdsToUpdateAsync(
            string title,
            string wikiNamespace,
            bool sameTitle,
            string? previousTitle = null,
            string? previousNamespace = null,
            bool processRedirects = false)
        {
            var idsToUpdate = new HashSet<string>();
            var idsToUpdateRecursively = new HashSet<string>();
            var redirectsToUpdate = new HashSet<string>();

            if (processRedirects)
            {
                if (!sameTitle)
                {
                    var previousRedirectReference = await PageRedirects
                        .GetPageRedirectsAsync(previousTitle!, previousNamespace)
                        .ConfigureAwait(false);
                    if (previousRedirectReference is not null)
                    {
                        foreach (var redirect in previousRedirectReference.References)
                        {
                            idsToUpdate.Add(redirect);
                            idsToUpdateRecursively.Add(redirect);
                            redirectsToUpdate.Add(redirect);
                        }
                    }
                }

                var redirectReference = await PageRedirects
                    .GetPageRedirectsAsync(title, wikiNamespace)
                    .ConfigureAwait(false);
                if (redirectReference is not null)
                {
                    foreach (var redirect in redirectReference.References)
                    {
                        idsToUpdate.Add(redirect);
                        idsToUpdateRecursively.Add(redirect);
                        redirectsToUpdate.Add(redirect);
                    }
                }
            }

            if (!sameTitle)
            {
                var previousPageTransclusions = await PageTransclusions
                    .GetPageTransclusionsAsync(previousTitle!, previousNamespace)
                    .ConfigureAwait(false);
                if (previousPageTransclusions is not null)
                {
                    foreach (var reference in previousPageTransclusions.References)
                    {
                        idsToUpdate.Add(reference);
                        idsToUpdateRecursively.Add(reference);
                    }
                }

                var previousReferences = await PageLinks
                    .GetPageLinksAsync(previousTitle!, previousNamespace)
                    .ConfigureAwait(false);
                if (previousReferences is not null)
                {
                    foreach (var reference in previousReferences.References)
                    {
                        idsToUpdate.Add(reference);
                    }
                }
            }

            var pageTransclusions = await PageTransclusions
                .GetPageTransclusionsAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (pageTransclusions is not null)
            {
                foreach (var reference in pageTransclusions.References)
                {
                    idsToUpdate.Add(reference);
                    idsToUpdateRecursively.Add(reference);
                }
            }

            var references = await PageLinks
                .GetPageLinksAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (references is not null)
            {
                foreach (var reference in references.References)
                {
                    idsToUpdate.Add(reference);
                }
            }

            return (
                idsToUpdate,
                idsToUpdateRecursively,
                redirectsToUpdate);
        }

        private static async Task<(
            string? redirectNamespace,
            string? redirectTitle,
            bool isRedirect,
            bool isBrokenRedirect,
            bool isDoubleRedirect)> IdentifyRedirectAsync(
            string id,
            string? markdown)
        {
            if ((markdown?.StartsWith("{{redirect|", StringComparison.OrdinalIgnoreCase)) != true
                || !markdown.Contains("}}", StringComparison.Ordinal))
            {
                return (
                    null,
                    null,
                    false,
                    false,
                    false);
            }

            var (redirectNamespace, redirectTitle, _, defaultNamespace) = GetTitleParts(markdown[11..markdown.IndexOf("}}")]);

            // Redirect to a category or file from an article is not valid.
            if (!defaultNamespace
                && (string.Equals(redirectNamespace, WikiConfig.CategoryNamespace, StringComparison.Ordinal)
                || string.Equals(redirectNamespace, WikiConfig.FileNamespace, StringComparison.Ordinal)))
            {
                return (
                    redirectNamespace,
                    redirectTitle,
                    true,
                    true,
                    false);
            }

            var isBrokenRedirect = false;
            var isDoubleRedirect = false;

            var redirect = await PageReference
                .GetPageReferenceAsync(redirectTitle, redirectNamespace)
                .ConfigureAwait(false);
            if (redirect is null)
            {
                isBrokenRedirect = true;
            }
            else
            {
                var redirectArticle = await WikiConfig.DataStore
                    .GetItemAsync<Article>(redirect.Reference)
                    .ConfigureAwait(false);
                if (redirectArticle is null)
                {
                    isBrokenRedirect = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(redirectArticle.RedirectTitle))
                    {
                        isDoubleRedirect = true;
                    }

                    var existingRedirect = await PageRedirects
                        .GetPageRedirectsAsync(redirectTitle, redirectNamespace)
                        .ConfigureAwait(false);
                    if (existingRedirect is null)
                    {
                        await PageRedirects.NewAsync(redirectTitle!, redirectNamespace!, id).ConfigureAwait(false);
                    }
                    else
                    {
                        await existingRedirect.AddReferenceAsync(id).ConfigureAwait(false);
                    }
                }
            }

            return (
                redirectNamespace,
                redirectTitle,
                true,
                isBrokenRedirect,
                isDoubleRedirect);
        }

        private protected static async Task RemovePageLinksAsync(string id, IEnumerable<WikiLink> wikiLinks)
        {
            foreach (var link in wikiLinks.Where(x => !x.IsCategory))
            {
                var linkReference = await PageLinks
                    .GetPageLinksAsync(link.Title, link.WikiNamespace)
                    .ConfigureAwait(false);
                if (linkReference is not null)
                {
                    await linkReference.RemoveReferenceAsync(id).ConfigureAwait(false);
                }

                var missing = await MissingPage
                    .GetMissingPageAsync(link.Title, link.WikiNamespace)
                    .ConfigureAwait(false);
                if (missing is not null)
                {
                    await missing.RemoveReferenceAsync(id).ConfigureAwait(false);
                }
            }
        }

        private protected static async Task RemovePageReferenceAsync(string id, string title, string wikiNamespace)
        {
            var existingReference = await PageReference
                .GetPageReferenceAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (existingReference is not null)
            {
                await WikiConfig.DataStore.RemoveItemAsync(existingReference).ConfigureAwait(false);
            }

            var existingNormalizedReference = await NormalizedPageReference
                .GetNormalizedPageReferenceAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (existingNormalizedReference is not null)
            {
                await existingNormalizedReference.RemoveReferenceAsync(id).ConfigureAwait(false);
            }

            var linkReference = await PageLinks
                .GetPageLinksAsync(title, wikiNamespace)
                .ConfigureAwait(false);
            if (linkReference is not null && linkReference.References.Count > 0)
            {
                var missing = await MissingPage
                    .GetMissingPageAsync(title, wikiNamespace)
                    .ConfigureAwait(false);
                if (missing is not null)
                {
                    await MissingPage.NewAsync(title, wikiNamespace, linkReference.References).ConfigureAwait(false);
                }
            }
        }

        private protected static async Task RemovePageTransclusionsAsync(string id, IEnumerable<Transclusion> transclusions)
        {
            foreach (var transclusion in transclusions)
            {
                var reference = await PageTransclusions
                    .GetPageTransclusionsAsync(transclusion.Title, transclusion.WikiNamespace)
                    .ConfigureAwait(false);
                if (reference is not null)
                {
                    await reference.RemoveReferenceAsync(id).ConfigureAwait(false);
                }
            }
        }

        private protected static async Task<List<string>> UpdateCategoriesAsync(
            string id,
            string editor,
            string? owner,
            IEnumerable<string>? allowedEditors,
            IEnumerable<string>? allowedViewers,
            IEnumerable<WikiLink> wikiLinks,
            IEnumerable<string>? previousCategories = null)
        {
            var currentCategories = wikiLinks
                .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                .Select(x => x.Title)
                .ToList();

            var oldCategories = previousCategories?.ToList() ?? new List<string>();

            var newCategories = new List<string>();
            foreach (var categoryTitle in currentCategories.Except(oldCategories))
            {
                var category = Category.GetCategory(categoryTitle)
                    ?? await Category.NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                if (!category.ChildIds.Contains(id))
                {
                    await category.AddArticleAsync(id).ConfigureAwait(false);
                    newCategories.Add(category.Title);
                }
            }

            var retainedCategories = oldCategories
                .Intersect(currentCategories)
                .ToList();
            foreach (var removedCategory in oldCategories.Except(currentCategories))
            {
                var category = Category.GetCategory(removedCategory);
                if (category is not null)
                {
                    await category.RemoveChildIdAsync(id).ConfigureAwait(false);
                    retainedCategories.Remove(category.Title);
                }
            }

            retainedCategories.AddRange(newCategories);

            return retainedCategories;
        }

        private protected static async Task UpdateReferencesAsync(
            string title,
            string wikiNamespace,
            bool deleted,
            bool sameTitle,
            string? previousTitle = null,
            string? previousNamespace = null,
            bool processRedirects = false,
            bool isRedirect = false)
        {
            var idsUpdated = new HashSet<string>();
            var idsUpdatedRecursively = new HashSet<string>();

            var (idsToUpdate,
                idsToUpdateRecursively,
                redirectsToUpdate) = await GetIdsToUpdateAsync(
                    title,
                    wikiNamespace,
                    sameTitle,
                    previousTitle,
                    previousNamespace,
                    processRedirects)
                .ConfigureAwait(false);

            while (idsToUpdate.Count > 0)
            {
                var idToUpdate = idsToUpdate.First();
                idsToUpdate.Remove(idToUpdate);

                var referringArticle = await WikiConfig.DataStore.GetItemAsync<Article>(idToUpdate).ConfigureAwait(false);
                if (referringArticle is null)
                {
                    idsUpdated.Add(idToUpdate);
                    idsUpdatedRecursively.Add(idToUpdate);
                    continue;
                }

                if (!idsUpdated.Contains(idToUpdate))
                {
                    if (redirectsToUpdate.Contains(idToUpdate))
                    {
                        referringArticle.IsBrokenRedirect = deleted;
                        referringArticle.IsDoubleRedirect = isRedirect;
                        redirectsToUpdate.Remove(idToUpdate);
                    }

                    referringArticle.Update();
                    await WikiConfig.DataStore.StoreItemAsync(referringArticle).ConfigureAwait(false);

                    idsUpdated.Add(idToUpdate);
                }

                if (idsToUpdateRecursively.Contains(idToUpdate))
                {
                    idsToUpdateRecursively.Remove(idToUpdate);
                    idsUpdatedRecursively.Add(idToUpdate);

                    var (childIdsToUpdate,
                        childIdsToUpdateRecursively,
                        _) = await GetIdsToUpdateAsync(
                            referringArticle.Title,
                            referringArticle.WikiNamespace,
                            true,
                            null,
                            null,
                            referringArticle.IdItemTypeName != Category.CategoryIdItemTypeName
                                && referringArticle.IdItemTypeName != WikiFile.WikiFileIdItemTypeName)
                        .ConfigureAwait(false);
                    foreach (var id in childIdsToUpdate.Except(idsUpdated))
                    {
                        idsToUpdate.Add(id);
                    }
                    foreach (var id in childIdsToUpdateRecursively.Except(idsUpdatedRecursively))
                    {
                        idsToUpdate.Add(id);
                        idsToUpdateRecursively.Add(id);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a diff which represents the final revision at the given <paramref name="time"/>, as
        /// rendered HTML.
        /// </summary>
        /// <param name="time">
        /// The time of the final revision.
        /// </param>
        /// <param name="format">
        /// <para>
        /// The format used.
        /// </para>
        /// <para>
        /// Can be either "delta" (the default), "gnu", "md", or "html" (case insensitive).
        /// </para>
        /// <para>
        /// The "delta" format (the default, used if an empty string or whitespace is passed)
        /// renders a compact, encoded string which describes each diff operation. The first
        /// character is '=' for unchanged text, '+' for an insertion, and '-' for deletion.
        /// Unchanged text and deletions are followed by their length only; insertions are followed
        /// by a compressed version of their full text. Each diff is separated by a tab character
        /// ('\t').
        /// </para>
        /// <para>
        /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
        /// nothing if the text was unchanged. Each diff is separated by a newline.
        /// </para>
        /// <para>
        /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
        /// nothing if the text was unchanged. Diffs are concatenated without separators.
        /// </para>
        /// <para>
        /// The "html" format renders the text surrounded by a span with class "diff-deleted" for
        /// deletion, "diff-inserted" for addition, or without a wrapping span if the text was
        /// unchanged. Diffs are concatenated without separators.
        /// </para>
        /// </param>
        /// <returns>
        /// A string representing the final revision at the given time, as rendered HTML.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        /// <remarks>
        /// The <paramref name="time"/> does not need to be exact. All revisions up to and including
        /// the given time will be included.
        /// </remarks>
        public async Task<string> GetDiffAsync(DateTimeOffset time, string format = "md")
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            if (revisions.Count == 0)
            {
                return string.Empty;
            }
            return RenderHtml(TransclusionParser.Transclude(
                revisions[revisions.Count - 1].Title,
                revisions[revisions.Count - 1].FullTitle,
                Revision.GetDiff(revisions, format),
                out _));
        }

        /// <summary>
        /// Gets a diff which represents the final revision at the given <paramref name="time"/>, as
        /// rendered HTML.
        /// </summary>
        /// <param name="time">
        /// The time of the final revision.
        /// </param>
        /// <returns>
        /// A string representing the final revision at the given time, as rendered HTML.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        /// <remarks>
        /// The <paramref name="time"/> does not need to be exact. All revisions up to and including
        /// the given time will be included.
        /// </remarks>
        public async Task<string> GetDiffHtmlAsync(DateTimeOffset time)
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            if (revisions.Count == 0)
            {
                return string.Empty;
            }
            return RenderHtml(TransclusionParser.Transclude(
                revisions[revisions.Count - 1].Title,
                revisions[revisions.Count - 1].FullTitle,
                Revision.GetDiff(revisions, "html"),
                out _));
        }

        /// <summary>
        /// Gets a diff between the text at the given <paramref name="time"/> and the current
        /// version of the text.
        /// </summary>
        /// <param name="time">
        /// The time of the final revision.
        /// </param>
        /// <param name="format">
        /// <para>
        /// The format used.
        /// </para>
        /// <para>
        /// Can be either "delta" (the default), "gnu", "md", or "html" (case insensitive).
        /// </para>
        /// <para>
        /// The "delta" format (the default, used if an empty string or whitespace is passed)
        /// renders a compact, encoded string which describes each diff operation. The first
        /// character is '=' for unchanged text, '+' for an insertion, and '-' for deletion.
        /// Unchanged text and deletions are followed by their length only; insertions are followed
        /// by a compressed version of their full text. Each diff is separated by a tab character
        /// ('\t').
        /// </para>
        /// <para>
        /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
        /// nothing if the text was unchanged. Each diff is separated by a newline.
        /// </para>
        /// <para>
        /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
        /// nothing if the text was unchanged. Diffs are concatenated without separators.
        /// </para>
        /// <para>
        /// The "html" format renders the text surrounded by a span with class "diff-deleted" for
        /// deletion, "diff-inserted" for addition, or without a wrapping span if the text was
        /// unchanged. Diffs are concatenated without separators.
        /// </para>
        /// </param>
        /// <returns>
        /// A string representing the difference between the resulting text after the given sequence
        /// of revisions and the current version of the text.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        public async Task<string> GetDiffWithCurrentAsync(DateTimeOffset time, string format = "md")
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            return Diff.GetWordDiff(Revision.GetText(revisions), MarkdownContent).ToString(format);
        }

        /// <summary>
        /// Gets a diff between the text at the given <paramref name="time"/> and the current
        /// version of the text, as rendered HTML.
        /// </summary>
        /// <param name="time">
        /// The time of the final revision.
        /// </param>
        /// <returns>
        /// A string representing the difference between the resulting text after the given sequence
        /// of revisions and the current version of the text, as rendered HTML.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        public async Task<string> GetDiffWithCurrentHtmlAsync(DateTimeOffset time)
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            var diff = Diff.GetWordDiff(Revision.GetText(revisions), MarkdownContent).ToString("html");
            return RenderHtml(TransclusionParser.Transclude(
                revisions[revisions.Count - 1].Title,
                revisions[revisions.Count - 1].FullTitle,
                diff,
                out _));
        }

        /// <summary>
        /// Gets a diff between the text at two given times.
        /// </summary>
        /// <param name="firstTime">
        /// The first revision time to compare.
        /// </param>
        /// <param name="secondTime">
        /// The second revision time to compare.
        /// </param>
        /// <param name="format">
        /// <para>
        /// The format used.
        /// </para>
        /// <para>
        /// Can be either "delta" (the default), "gnu", "md", or "html" (case insensitive).
        /// </para>
        /// <para>
        /// The "delta" format (the default, used if an empty string or whitespace is passed)
        /// renders a compact, encoded string which describes each diff operation. The first
        /// character is '=' for unchanged text, '+' for an insertion, and '-' for deletion.
        /// Unchanged text and deletions are followed by their length only; insertions are followed
        /// by a compressed version of their full text. Each diff is separated by a tab character
        /// ('\t').
        /// </para>
        /// <para>
        /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
        /// nothing if the text was unchanged. Each diff is separated by a newline.
        /// </para>
        /// <para>
        /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
        /// nothing if the text was unchanged. Diffs are concatenated without separators.
        /// </para>
        /// <para>
        /// The "html" format renders the text surrounded by a span with class "diff-deleted" for
        /// deletion, "diff-inserted" for addition, or without a wrapping span if the text was
        /// unchanged. Diffs are concatenated without separators.
        /// </para>
        /// </param>
        /// <returns>
        /// A string representing the difference between the text at two given times.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        /// <remarks>
        /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values
        /// are swapped. In other words, the diff is always from an earlier version to a later version.
        /// </remarks>
        public async Task<string> GetDiffWithOtherAsync(DateTimeOffset firstTime, DateTimeOffset secondTime, string format = "md")
        {
            if (secondTime < firstTime)
            {
                var tmp = secondTime;
                secondTime = firstTime;
                firstTime = tmp;
            }
            var firstRevisions = await GetRevisionsUntilAsync(firstTime).ConfigureAwait(false);
            var secondRevisions = await GetRevisionsUntilAsync(secondTime).ConfigureAwait(false);
            return Diff.GetWordDiff(Revision.GetText(firstRevisions), Revision.GetText(secondRevisions)).ToString(format);
        }

        /// <summary>
        /// Gets a diff between the text at two given times, as rendered HTML.
        /// </summary>
        /// <param name="firstTime">
        /// The first revision time to compare.
        /// </param>
        /// <param name="secondTime">
        /// The second revision time to compare.
        /// </param>
        /// <returns>
        /// A string representing the difference between the text at two given times, as rendered
        /// HTML.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// A revision was incorrectly formatted; or, the sequence of revisions is not a
        /// well-ordered set of revisions which start with a milestone and apply seamlessly in the
        /// order given.
        /// </exception>
        /// <remarks>
        /// If <paramref name="secondTime"/> is before <paramref name="firstTime"/>, their values
        /// are swapped. In other words, the diff is always from an earlier version to a later version.
        /// </remarks>
        public async Task<string> GetDiffWithOtherHtmlAsync(DateTimeOffset firstTime, DateTimeOffset secondTime)
        {
            if (secondTime < firstTime)
            {
                var tmp = secondTime;
                secondTime = firstTime;
                firstTime = tmp;
            }
            var firstRevisions = await GetRevisionsUntilAsync(firstTime).ConfigureAwait(false);
            var secondRevisions = await GetRevisionsUntilAsync(secondTime).ConfigureAwait(false);
            var diff = Diff.GetWordDiff(Revision.GetText(firstRevisions), Revision.GetText(secondRevisions)).ToString("html");
            return RenderHtml(TransclusionParser.Transclude(
                secondRevisions[secondRevisions.Count - 1].Title,
                secondRevisions[secondRevisions.Count - 1].FullTitle,
                diff,
                out _));
        }

        /// <summary>
        /// Gets this item's content at the given <paramref name="time"/>, rendered as HTML.
        /// </summary>
        /// <returns>The rendered HTML.</returns>
        public async Task<string> GetHtmlAsync(DateTimeOffset time)
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            if (revisions.Count == 0)
            {
                return string.Empty;
            }
            return RenderHtml(TransclusionParser.Transclude(
                revisions[revisions.Count - 1].Title,
                revisions[revisions.Count - 1].FullTitle,
                Revision.GetText(revisions),
                out _));
        }

        /// <summary>
        /// Gets a subset of the revision history of this item, in reverse chronological order (most recent
        /// first).
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="start">The earliest <see cref="Timestamp"/> to retrieve.</param>
        /// <param name="end">The most recent <see cref="Timestamp"/> to retrieve.</param>
        /// <param name="condition">An optional condition which the items must satisfy.</param>
        /// <returns>An <see cref="IOrderedQueryable{T}"/> of <see cref="Article"/>
        /// instances.</returns>
        public async Task<IPagedList<Revision>> GetHistoryAsync(
            int pageNumber,
            int pageSize,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null,
            Expression<Func<Revision, bool>>? condition = null)
        {
            Expression<Func<Revision, bool>> exp = x => x.WikiId == Id;
            if (start.HasValue)
            {
                exp = exp.AndAlso(x => x.TimestampTicks >= start.Value.ToUniversalTime().Ticks);
            }
            if (end.HasValue)
            {
                exp = exp.AndAlso(x => x.TimestampTicks <= end.Value.ToUniversalTime().Ticks);
            }
            exp = condition is null ? exp : exp.AndAlso(condition);
            return await WikiConfig.DataStore.Query<Revision>()
                .Where(exp)
                .OrderBy(x => x.TimestampTicks, descending: true)
                .GetPageAsync(pageNumber, pageSize)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets this item's content at the given <paramref name="time"/>.
        /// </summary>
        /// <returns>The markdown as it was at the given <paramref name="time"/>.</returns>
        public async Task<string> GetMarkdownAsync(DateTimeOffset time)
        {
            var revisions = await GetRevisionsUntilAsync(time).ConfigureAwait(false);
            if (revisions.Count == 0)
            {
                return string.Empty;
            }
            return Revision.GetText(revisions);
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
            info.AddValue(nameof(Html), Html);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(Preview), Preview);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
            info.AddValue(nameof(IsDeleted), IsDeleted);
            info.AddValue(nameof(Owner), Owner);
            info.AddValue(nameof(AllowedEditors), AllowedEditors);
            info.AddValue(nameof(AllowedViewers), AllowedViewers);
            info.AddValue(nameof(RedirectNamespace), RedirectNamespace);
            info.AddValue(nameof(RedirectTitle), RedirectTitle);
            info.AddValue(nameof(IsBrokenRedirect), IsBrokenRedirect);
            info.AddValue(nameof(IsDoubleRedirect), IsDoubleRedirect);
            info.AddValue(nameof(Categories), Categories);
            info.AddValue(nameof(Transclusions), Transclusions);
        }

        /// <summary>
        /// Revises this <see cref="Article"/> instance.
        /// </summary>
        /// <param name="editor">
        /// The ID of the user who made this revision.
        /// </param>
        /// <param name="title">
        /// <para>
        /// The optional new title of the article. Must be unique within its namespace, and
        /// non-empty.
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
        /// <param name="wikiNamespace">
        /// <para>
        /// The optional new namespace to which this article belongs.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the existing namespace will be retained.
        /// </para>
        /// </param>
        /// <param name="isDeleted">Indicates that this article has been marked as deleted.</param>
        /// <param name="owner">
        /// <para>
        /// The new owner of the article.
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
        /// </param>
        public async Task ReviseAsync(
            string editor,
            string? title = null,
            string? markdown = null,
            string? revisionComment = null,
            string? wikiNamespace = null,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null)
        {
            title ??= title?.ToWikiTitleCase() ?? Title;
            if (string.IsNullOrWhiteSpace(wikiNamespace))
            {
                wikiNamespace = WikiNamespace;
            }
            wikiNamespace ??= wikiNamespace?.ToWikiTitleCase() ?? WikiNamespace;

            var previousTitle = Title;
            var previousNamespace = WikiNamespace;
            Title = title;
            WikiNamespace = wikiNamespace;
            var sameTitle = string.Equals(previousTitle, title, StringComparison.Ordinal)
                && string.Equals(previousNamespace, wikiNamespace, StringComparison.Ordinal);

            var previousMarkdown = MarkdownContent;
            var wasDeleted = IsDeleted || string.IsNullOrWhiteSpace(previousMarkdown);
            var isRedirect = !string.IsNullOrEmpty(RedirectTitle);

            if (isDeleted || string.IsNullOrWhiteSpace(markdown))
            {
                if (!wasDeleted)
                {
                    Html = string.Empty;
                    IsBrokenRedirect = false;
                    IsDeleted = true;
                    IsDoubleRedirect = false;
                    MarkdownContent = string.Empty;
                    Preview = string.Empty;
                    Categories = new List<string>().AsReadOnly();
                    RedirectTitle = null;
                    RedirectNamespace = null;

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
                await CreatePageReferenceAsync(Id, title, wikiNamespace).ConfigureAwait(false);
            }
            if (!sameTitle)
            {
                await RemovePageReferenceAsync(Id, previousTitle, previousNamespace).ConfigureAwait(false);
            }

            var changed = wasDeleted != IsDeleted
                || !string.Equals(previousMarkdown, markdown, StringComparison.Ordinal);

            if (!IsDeleted && changed)
            {
                MarkdownContent = markdown!;

                string? redirectNamespace;
                string? redirectTitle;
                (
                    redirectNamespace,
                    redirectTitle,
                    isRedirect,
                    _,
                    _) = await IdentifyRedirectAsync(Id, markdown).ConfigureAwait(false);
                if (isRedirect)
                {
                    RedirectTitle = redirectTitle;
                    RedirectNamespace = redirectNamespace;
                }

                var previousTransclusions = Transclusions?.ToList() ?? new List<Transclusion>();
                List<Transclusion> transclusions;
                var md = markdown ?? string.Empty;
                if (isRedirect)
                {
                    transclusions = new List<Transclusion>();
                }
                else
                {
                    md = TransclusionParser.Transclude(
                        title,
                        GetFullTitle(title, wikiNamespace),
                        markdown!,
                        out transclusions);
                }
                Transclusions = transclusions.Count == 0
                    ? null
                    : transclusions.AsReadOnly();
                await RemovePageTransclusionsAsync(Id, previousTransclusions.Except(transclusions)).ConfigureAwait(false);
                await AddPageTransclusionsAsync(Id, transclusions.Except(previousTransclusions)).ConfigureAwait(false);

                var previousWikiLinks = WikiLinks.ToList();
                WikiLinks = isRedirect
                    ? new List<WikiLink>()
                    : GetWikiLinks(md, title, wikiNamespace).AsReadOnly();
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
                wikiNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            TimestampTicks = revision.TimestampTicks;

            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);

            await UpdateReferencesAsync(
                title,
                wikiNamespace,
                IsDeleted,
                sameTitle,
                previousTitle,
                previousNamespace,
                true,
                isRedirect)
                .ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<Revision>> GetRevisionsUntilAsync(DateTimeOffset time)
        {
            var ticks = time.ToUniversalTime().Ticks;
            var lastMilestone = await WikiConfig.DataStore.Query<Revision>()
                .Where(x => x.WikiId == Id && x.TimestampTicks <= ticks && x.IsMilestone)
                .OrderBy(x => x.TimestampTicks, descending: true)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (lastMilestone is null)
            {
                return new List<Revision>();
            }
            return await WikiConfig.DataStore.Query<Revision>()
                .Where(x => x.WikiId == Id && x.TimestampTicks >= lastMilestone.TimestampTicks && x.TimestampTicks <= ticks)
                .OrderBy(x => x.TimestampTicks)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private protected override string PostprocessMarkdown(string? markdown, bool isPreview = false)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            return TransclusionParser.Transclude(
                  Title,
                  FullTitle,
                  markdown,
                  out _,
                  isPreview: isPreview);
        }
    }
}
