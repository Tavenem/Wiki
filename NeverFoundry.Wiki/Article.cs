using NeverFoundry.DataStorage;
using NeverFoundry.DiffPatchMerge;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
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
    [Serializable]
    public class Article : MarkdownItem
    {
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
        public IReadOnlyList<string>? AllowedEditors { get; private protected set; }

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
        public IReadOnlyList<string>? AllowedViewers { get; private protected set; }

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
        public IReadOnlyList<string> Categories { get; private protected set; } = new List<string>().AsReadOnly();

        /// <summary>
        /// Gets the full title of this article (including namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        public virtual string FullTitle => string.CompareOrdinal(WikiNamespace, WikiConfig.DefaultNamespace) == 0
            ? Title
            : $"{WikiNamespace}:{Title}";

        /// <summary>
        /// Indicates that this article has been marked as deleted.
        /// </summary>
        public bool IsDeleted { get; private protected set; }

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
        /// The title of this article. Must be unique and non-empty.
        /// </summary>
        public string Title { get; private protected set; }

        /// <summary>
        /// The transclusions within this article.
        /// </summary>
        public IReadOnlyList<string> Transclusions { get; private protected set; } = new List<string>().AsReadOnly();

        /// <summary>
        /// The namespace to which this article belongs.
        /// </summary>
        public virtual string WikiNamespace { get; private protected set; }

        private protected Article(
            string id,
            string title,
            string? markdown,
            IList<WikiLink> wikiLinks,
            long timestampTicks,
            string? wikiNamespace = null,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null,
            IList<string>? categories = null,
            IList<string>? transclusions = null,
            string? redirectNamespace = null,
            string? redirectTitle = null) : base(id, markdown, wikiLinks)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            wikiNamespace ??= WikiConfig.DefaultNamespace;

            if (!string.IsNullOrEmpty(owner))
            {
                AllowedEditors = allowedEditors?.ToList();
                AllowedViewers = allowedViewers?.ToList();
            }
            Categories = (IReadOnlyList<string>?)categories ?? new List<string>();
            IsDeleted = isDeleted;
            Owner = owner;
            RedirectNamespace = redirectNamespace;
            RedirectTitle = redirectTitle;
            TimestampTicks = timestampTicks;
            Title = title;
            Transclusions = (IReadOnlyList<string>?)transclusions ?? new List<string>();
            WikiNamespace = wikiNamespace;
        }

        private protected Article(
            string id,
            string title,
            string markdown,
            IList<WikiLink> wikiLinks,
            long timestampTicks,
            string? wikiNamespace = null,
            bool isDeleted = false,
            string? owner = null,
            IEnumerable<string>? allowedEditors = null,
            IEnumerable<string>? allowedViewers = null,
            string? redirectNamespace = null,
            string? redirectTitle = null,
            IList<string>? categories = null,
            IList<string>? transclusions = null) : base(id, markdown, wikiLinks)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
            }
            wikiNamespace ??= WikiConfig.DefaultNamespace;

            if (!string.IsNullOrEmpty(owner))
            {
                AllowedEditors = allowedEditors?.ToList();
                AllowedViewers = allowedViewers?.ToList();
            }
            Categories = (IReadOnlyList<string>?)categories ?? new List<string>();
            IsDeleted = isDeleted;
            Owner = owner;
            RedirectNamespace = redirectNamespace;
            RedirectTitle = redirectTitle;
            TimestampTicks = timestampTicks;
            Title = title;
            Transclusions = (IReadOnlyList<string>?)transclusions ?? new List<string>();
            WikiNamespace = wikiNamespace;
        }

        private Article(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (IList<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IList<WikiLink>)) ?? new WikiLink[0],
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty,
            (bool?)info.GetValue(nameof(IsDeleted), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(Owner), typeof(string)),
            (IList<string>?)info.GetValue(nameof(AllowedEditors), typeof(IList<string>)),
            (IList<string>?)info.GetValue(nameof(AllowedViewers), typeof(IList<string>)),
            (string?)info.GetValue(nameof(RedirectNamespace), typeof(string)),
            (string?)info.GetValue(nameof(RedirectTitle), typeof(string)),
            (IList<string>?)info.GetValue(nameof(Categories), typeof(IList<string>)),
            (IList<string>?)info.GetValue(nameof(Transclusions), typeof(IList<string>)))
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
        /// <returns>The latest revision for the article with the given title; or <see
        /// langword="null"/> if no such article exists.</returns>
        public static Article? GetArticle(string? title, string? wikiNamespace = null)
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
                article = DataStore.GetFirstItemWhereOrderedBy<Article, long>(
                    x => x.WikiNamespace == wikiNamespace && x.Title == title,
                    x => x.TimestampTicks,
                    descending: true);
                // If no exact match exists, ignore case if only one such match exists.
                if (article is null)
                {
                    var articles = DataStore.GetItemsWhereOrderedBy<Article, long>(
                        x => string.Equals(x.WikiNamespace, wikiNamespace, StringComparison.OrdinalIgnoreCase)
                            && string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase),
                        x => x.TimestampTicks,
                        descending: true)
                        .ToList();
                    if (articles.Count == 1)
                    {
                        article = articles[0];
                    }
                }
                if (!string.IsNullOrEmpty(article?.RedirectTitle))
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
            var article = DataStore.GetFirstItemWhere<Article>(x => x.WikiNamespace == wikiNamespace
                && x.Title == title
                && x.TimestampTicks == ticks);
            // If no exact match exists, ignore case if only one such match exists.
            if (article is null)
            {
                var articles = DataStore.GetItemsWhere<Article>(x => string.Equals(x.WikiNamespace, wikiNamespace, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase)
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
        /// If this is equal to <see cref="WikiConfig.CategoriesTitle"/>, the result will be a <see
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
            if (string.Equals(wikiNamespace, WikiConfig.CategoriesTitle, StringComparison.CurrentCultureIgnoreCase))
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
            if (DataStore.GetItemsWhere<Article>(x => x.WikiNamespace == wikiNamespace
                && x.Title == title).Count > 0)
            {
                throw new ArgumentException("The given title is already in use for this namespace", nameof(title));
            }

            var wikiId = Guid.NewGuid().ToString();

            var revision = new WikiRevision(
                wikiId,
                editor,
                title,
                wikiNamespace,
                null,
                markdown);
            await revision.SaveAsync().ConfigureAwait(false);

            string? redirectNamespace = null;
            string? redirectTitle = null;
            if (markdown?.StartsWith("{{redirect|", StringComparison.OrdinalIgnoreCase) == true
                && markdown.IndexOf("}}") != -1)
            {
                var (newNamespace, newTitle, _, _) = GetTitleParts(markdown[11..markdown.IndexOf("}}")]);
                redirectTitle = newTitle;
                redirectNamespace = newNamespace;
            }

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
                    GetFullTitle(title, wikiNamespace),
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

            var article = new Article(
                wikiId,
                title,
                markdown,
                wikiLinks,
                revision.TimestampTicks,
                wikiNamespace,
                isDeleted: false,
                owner,
                allowedEditors,
                allowedViewers,
                categories,
                transclusions,
                redirectNamespace,
                redirectTitle);
            await article.SaveAsync().ConfigureAwait(false);
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
            return await DataStore.GetPageWhereOrderedByAsync(
                exp,
                x => x.TimestampTicks,
                pageNumber,
                pageSize,
                descending: true)
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

            if (DataStore.GetItemsWhere<Article>(x => x.Id != Id
                && x.WikiNamespace == wikiNamespace
                && x.Title == title).Count > 0)
            {
                throw new ArgumentException("The given title is already in use for this namespace", nameof(title));
            }

            Title = title;
            WikiNamespace = wikiNamespace;

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

                    if (MarkdownContent.StartsWith("{{redirect|", StringComparison.OrdinalIgnoreCase)
                        && MarkdownContent.IndexOf("}}") != -1)
                    {
                        var (newNamespace, newTitle, _, _) = GetTitleParts(MarkdownContent[11..MarkdownContent.IndexOf("}}")]);
                        RedirectTitle = newTitle;
                        RedirectNamespace = newNamespace;
                    }

                    var md = TransclusionParser.Transclude(
                        title,
                        GetFullTitle(title, wikiNamespace),
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
                wikiNamespace,
                previousMarkdown,
                MarkdownContent,
                revisionComment);
            await revision.SaveAsync().ConfigureAwait(false);

            TimestampTicks = revision.TimestampTicks;

            await SaveAsync().ConfigureAwait(false);
        }

        private async Task<IList<WikiRevision>> GetRevisionsUntilAsync(DateTimeOffset time)
        {
            var ticks = time.ToUniversalTime().Ticks;
            var lastMilestone = await DataStore
                .GetFirstItemWhereOrderedByAsync<WikiRevision, long>(x => x.WikiId == Id && x.TimestampTicks <= ticks && x.IsMilestone, x => x.TimestampTicks, descending: true)
                .ConfigureAwait(false);
            if (lastMilestone is null)
            {
                return new List<WikiRevision>();
            }
            return await DataStore
                .GetItemsWhereOrderedByAsync<WikiRevision, long>(x => x.WikiId == Id && x.TimestampTicks >= lastMilestone.TimestampTicks && x.TimestampTicks <= ticks, x => x.TimestampTicks)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private protected override string PostprocessMarkdown(string markdown, bool isPreview = false)
        {
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

                SaveAsync().GetAwaiter().GetResult();
            }

            return md;
        }
    }
}
