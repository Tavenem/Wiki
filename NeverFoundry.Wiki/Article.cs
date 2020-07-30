using NeverFoundry.DataStorage;
using NeverFoundry.DiffPatchMerge;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="markdownContent">The raw markdown.</param>
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
            string markdownContent,
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
            IReadOnlyList<Transclusion>? transclusions) : base(id, markdownContent, wikiLinks)
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
        }

        private Article(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            ArticleIdItemTypeName,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>)) ?? new ReadOnlyCollection<WikiLink>(new WikiLink[0]),
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
            (IReadOnlyCollection<string>?)info.GetValue(nameof(Categories), typeof(IReadOnlyCollection<string>)) ?? new ReadOnlyCollection<string>(new string[0]),
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
            var redirect = false;
            var count = 0;
            var ids = new HashSet<string>();
            do
            {
                redirect = false;
                article = WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.WikiNamespace == wikiNamespace && x.Title == title)
                    .OrderBy(x => x.TimestampTicks, descending: true)
                    .FirstOrDefault();
                // If no exact match exists, ignore case if only one such match exists.
                if (article is null)
                {
                    var articles = WikiConfig.DataStore.Query<Article>()
                        .Where(x => x.WikiNamespace.ToLower() == wikiNamespace.ToLower()
                            && x.Title.ToLower() == title.ToLower())
                        .OrderBy(x => x.TimestampTicks, descending: true)
                        .ToList();
                    if (articles.Count == 1)
                    {
                        article = articles[0];
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
        /// Gets a particular revision for the article with the given title.
        /// </summary>
        /// <param name="title">The title of the article to retrieve.</param>
        /// <param name="timestamp">The timestamp of the revision.</param>
        /// <param name="wikiNamespace">
        /// <para>
        /// The namespace of the article to retrieve.
        /// </para>
        /// <para>
        /// If left <see langword="null"/> the default namespace (<see
        /// cref="WikiConfig.DefaultNamespace"/>) will be assumed.
        /// </para>
        /// </param>
        /// <returns>The revision of the article with the given title and timestamp; or <see
        /// langword="null"/> if no such article exists.</returns>
        public static Article? GetArticle(string? title, DateTimeOffset timestamp, string? wikiNamespace = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            wikiNamespace ??= WikiConfig.DefaultNamespace;
            var ticks = timestamp.ToUniversalTime().Ticks;
            var article = WikiConfig.DataStore.Query<Article>()
                .Where(x => x.WikiNamespace == wikiNamespace
                    && x.Title == title
                    && x.TimestampTicks == ticks)
                .FirstOrDefault();
            // If no exact match exists, ignore case if only one such match exists.
            if (article is null)
            {
                var articles = WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.WikiNamespace.ToLower() == wikiNamespace.ToLower()
                        && x.Title.ToLower() == title.ToLower()
                        && x.TimestampTicks == ticks)
                    .ToList();
                if (articles.Count == 1)
                {
                    article = articles[0];
                }
            }
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
        /// <returns>The namespace and title, and <see cref="bool"/> flags indicating whether the
        /// title indicates a dioscussion page, as well as whether the namespace was omitted.</returns>
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
            if (WikiConfig.DataStore.Query<Article>().Any(x => x.WikiNamespace == wikiNamespace && x.Title == title))
            {
                throw new ArgumentException("The given title is already in use for this namespace", nameof(title));
            }

            var wikiId = WikiConfig.DataStore.CreateNewIdFor<Article>();

            var revision = new WikiRevision(
                wikiId,
                editor,
                title,
                wikiNamespace,
                null,
                markdown);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            string? redirectNamespace = null;
            string? redirectTitle = null;
            var isBrokenRedirect = false;
            var isDoubleRedirect = false;
            if (markdown?.StartsWith("{{redirect|", StringComparison.OrdinalIgnoreCase) == true
                && markdown.IndexOf("}}") != -1)
            {
                var (newNamespace, newTitle, _, _) = GetTitleParts(markdown[11..markdown.IndexOf("}}")]);
                redirectTitle = newTitle;
                redirectNamespace = newNamespace;

                // Redirect to a category or file from an article is not valid.
                if (string.Equals(redirectNamespace, WikiConfig.CategoryNamespace, StringComparison.Ordinal)
                    || string.Equals(redirectNamespace, WikiConfig.FileNamespace, StringComparison.Ordinal))
                {
                    isBrokenRedirect = true;
                }
                else
                {
                    var redirect = GetArticle(redirectTitle, redirectNamespace);
                    if (redirect is null)
                    {
                        isBrokenRedirect = true;
                    }
                    else if (!string.IsNullOrEmpty(redirect.RedirectTitle))
                    {
                        isDoubleRedirect = true;
                    }
                }
            }

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
                    GetFullTitle(title, wikiNamespace),
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
                var category = Category.GetCategory(categoryTitle)
                    ?? await Category.NewAsync(categoryTitle, editor, null, owner, allowedEditors, allowedViewers).ConfigureAwait(false);
                await category.AddArticleAsync(wikiId).ConfigureAwait(false);
                categories.Add(category.Title);
            }

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

            var missing = await WikiConfig.DataStore.Query<MissingPage>()
                .FirstOrDefaultAsync(x => x.Title == title && x.WikiNamespace == wikiNamespace)
                .ConfigureAwait(false);
            if (!(missing is null))
            {
                await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
            }

            var brokenRedirect = await WikiConfig.DataStore.Query<Article>()
                .FirstOrDefaultAsync(x => x.IsBrokenRedirect && x.RedirectTitle == title && x.RedirectNamespace == wikiNamespace)
                .ConfigureAwait(false);
            if (!(brokenRedirect is null))
            {
                brokenRedirect.IsBrokenRedirect = false;
                await WikiConfig.DataStore.StoreItemAsync(brokenRedirect).ConfigureAwait(false);
            }

            return article;
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
                WikiRevision.GetDiff(revisions, format),
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
                WikiRevision.GetDiff(revisions, "html"),
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
            return Diff.GetWordDiff(WikiRevision.GetText(revisions), MarkdownContent).ToString(format);
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
            var diff = Diff.GetWordDiff(WikiRevision.GetText(revisions), MarkdownContent).ToString("html");
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
            return Diff.GetWordDiff(WikiRevision.GetText(firstRevisions), WikiRevision.GetText(secondRevisions)).ToString(format);
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
            var diff = Diff.GetWordDiff(WikiRevision.GetText(firstRevisions), WikiRevision.GetText(secondRevisions)).ToString("html");
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
                WikiRevision.GetText(revisions),
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
        public async Task<IPagedList<WikiRevision>> GetHistoryAsync(
            int pageNumber,
            int pageSize,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null,
            Expression<Func<WikiRevision, bool>>? condition = null)
        {
            Expression<Func<WikiRevision, bool>> exp = x => x.WikiId == Id;
            if (start.HasValue)
            {
                exp = exp.AndAlso(x => x.TimestampTicks >= start.Value.ToUniversalTime().Ticks);
            }
            if (end.HasValue)
            {
                exp = exp.AndAlso(x => x.TimestampTicks <= end.Value.ToUniversalTime().Ticks);
            }
            exp = condition is null ? exp : exp.AndAlso(condition);
            return await WikiConfig.DataStore.Query<WikiRevision>()
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
            return WikiRevision.GetText(revisions);
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
            title ??= Title;
            if (string.IsNullOrWhiteSpace(wikiNamespace))
            {
                wikiNamespace = null;
            }
            wikiNamespace ??= WikiNamespace;

            if (WikiConfig.DataStore.Query<Article>()
                .Any(x => x.Id != Id
                    && x.WikiNamespace == wikiNamespace
                    && x.Title == title))
            {
                throw new ArgumentException("The given title is already in use for this namespace", nameof(title));
            }

            var previousTitle = Title;
            var previousNamespace = WikiNamespace;
            Title = title;
            WikiNamespace = wikiNamespace;

            var previousMarkdown = MarkdownContent;
            if (isDeleted)
            {
                IsBrokenRedirect = false;
                IsDeleted = true;
                IsDoubleRedirect = false;
                MarkdownContent = string.Empty;
                WikiLinks = new List<WikiLink>().AsReadOnly();
                Categories = new List<string>().AsReadOnly();
                Transclusions = null;
                RedirectTitle = null;
                RedirectNamespace = null;

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

                await foreach (var redirect in WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.RedirectTitle == previousTitle && x.RedirectNamespace == previousNamespace)
                    .AsAsyncEnumerable())
                {
                    redirect.IsBrokenRedirect = true;
                    redirect.IsDoubleRedirect = false;
                    await WikiConfig.DataStore.StoreItemAsync(redirect).ConfigureAwait(false);
                }
            }
            else if (!(markdown is null))
            {
                if (string.IsNullOrWhiteSpace(markdown))
                {
                    IsBrokenRedirect = false;
                    IsDeleted = true;
                    IsDoubleRedirect = false;
                    MarkdownContent = string.Empty;
                    WikiLinks = new List<WikiLink>().AsReadOnly();
                    Categories = new List<string>().AsReadOnly();
                    Transclusions = null;
                    RedirectTitle = null;
                    RedirectNamespace = null;

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

                    var wasRedirect = !string.IsNullOrEmpty(RedirectTitle);
                    if (MarkdownContent.StartsWith("{{redirect|", StringComparison.OrdinalIgnoreCase)
                        && MarkdownContent.IndexOf("}}") != -1)
                    {
                        var (newNamespace, newTitle, _, _) = GetTitleParts(MarkdownContent[11..MarkdownContent.IndexOf("}}")]);
                        RedirectTitle = newTitle;
                        RedirectNamespace = newNamespace;

                        if (string.Equals(title, previousTitle, StringComparison.Ordinal)
                            && string.Equals(wikiNamespace, previousNamespace, StringComparison.Ordinal))
                        {
                            await foreach (var redirect in WikiConfig.DataStore.Query<Article>()
                                .Where(x => x.RedirectTitle == title && x.RedirectNamespace == wikiNamespace)
                                .AsAsyncEnumerable())
                            {
                                redirect.IsDoubleRedirect = true;
                                await WikiConfig.DataStore.StoreItemAsync(redirect).ConfigureAwait(false);
                            }
                        }
                    }
                    else if (wasRedirect
                        && string.Equals(title, previousTitle, StringComparison.Ordinal)
                        && string.Equals(wikiNamespace, previousNamespace, StringComparison.Ordinal))
                    {
                        await foreach (var redirect in WikiConfig.DataStore.Query<Article>()
                            .Where(x => x.IsDoubleRedirect && x.RedirectTitle == title && x.RedirectNamespace == wikiNamespace)
                            .AsAsyncEnumerable())
                        {
                            redirect.IsDoubleRedirect = false;
                            await WikiConfig.DataStore.StoreItemAsync(redirect).ConfigureAwait(false);
                        }
                    }

                    var md = TransclusionParser.Transclude(
                        title,
                        GetFullTitle(title, wikiNamespace),
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

                if (!string.Equals(title, previousTitle, StringComparison.Ordinal)
                    || !string.Equals(wikiNamespace, previousNamespace, StringComparison.Ordinal))
                {
                    var missing = await WikiConfig.DataStore.Query<MissingPage>()
                        .FirstOrDefaultAsync(x => x.Title == title && x.WikiNamespace == wikiNamespace)
                        .ConfigureAwait(false);
                    if (!(missing is null))
                    {
                        await WikiConfig.DataStore.RemoveItemAsync(missing).ConfigureAwait(false);
                    }

                    await foreach (var redirect in WikiConfig.DataStore.Query<Article>()
                        .Where(x => x.RedirectTitle == previousTitle && x.RedirectNamespace == previousNamespace)
                        .AsAsyncEnumerable())
                    {
                        redirect.IsBrokenRedirect = true;
                        redirect.IsDoubleRedirect = false;
                        await WikiConfig.DataStore.StoreItemAsync(redirect).ConfigureAwait(false);
                    }

                    await foreach (var redirect in WikiConfig.DataStore.Query<Article>()
                        .Where(x => x.RedirectTitle == title && x.RedirectNamespace == wikiNamespace)
                        .AsAsyncEnumerable())
                    {
                        redirect.IsBrokenRedirect = false;
                        await WikiConfig.DataStore.StoreItemAsync(redirect).ConfigureAwait(false);
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
                wikiNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await WikiConfig.DataStore.StoreItemAsync(revision).ConfigureAwait(false);

            TimestampTicks = revision.TimestampTicks;

            await WikiConfig.DataStore.StoreItemAsync(this).ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<WikiRevision>> GetRevisionsUntilAsync(DateTimeOffset time)
        {
            var ticks = time.ToUniversalTime().Ticks;
            var lastMilestone = await WikiConfig.DataStore.Query<WikiRevision>()
                .Where(x => x.WikiId == Id && x.TimestampTicks <= ticks && x.IsMilestone)
                .OrderBy(x => x.TimestampTicks, descending: true)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (lastMilestone is null)
            {
                return new List<WikiRevision>();
            }
            return await WikiConfig.DataStore.Query<WikiRevision>()
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

            var md = TransclusionParser.Transclude(
                  Title,
                  FullTitle,
                  markdown,
                  out _,
                  isPreview: isPreview);

            // Update wikilinks in case any embedded in transclusions have changed.
            var wikiLinks = GetWikiLinks(md);
            if (!WikiLinks.SequenceEqual(wikiLinks))
            {
                WikiLinks = wikiLinks.AsReadOnly();

                // Update categories in case any specified in transclusions have changed.
                var categories = Categories.ToList();
                var categoryTitles = WikiLinks
                    .Where(x => x.IsCategory && !x.IsNamespaceEscaped)
                    .Select(x => x.Title)
                    .ToList();
                var newCategories = new List<string>();
                foreach (var categoryTitle in categoryTitles)
                {
                    var category = Category.GetCategory(categoryTitle)
                        ?? Category.NewAsync(categoryTitle, "System", null).GetAwaiter().GetResult();
                    if (!category.ChildIds.Contains(Id))
                    {
                        category.AddArticleAsync(Id).GetAwaiter().GetResult();
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
                    category.RemoveChildAsync(this).GetAwaiter().GetResult();
                    categories.Remove(category.Title);
                }
                if (newCategories.Count > 0)
                {
                    categories.AddRange(newCategories);
                }
                Categories = categories.AsReadOnly();

                WikiConfig.DataStore.StoreItemAsync(this).GetAwaiter().GetResult();
            }

            return md;
        }
    }
}
