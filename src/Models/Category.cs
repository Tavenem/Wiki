using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;

namespace Tavenem.Wiki;

/// <summary>
/// A wiki category revision.
/// </summary>
public sealed class Category : Article
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string CategoryIdItemTypeName = ":Article:Category:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => CategoryIdItemTypeName;

    /// <summary>
    /// The list of IDs of the items (including child <see cref="Category"/> items) which belong
    /// to this category.
    /// </summary>
    /// <remarks>
    /// Updates to this cache do not count as a revision.
    /// </remarks>
    public ICollection<string> ChildIds { get; } = new List<string>();

    /// <summary>
    /// Initializes a new instance of <see cref="Category"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="title">
    /// The title of this article. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="html">The rendered HTML content.</param>
    /// <param name="markdownContent">The raw markdown.</param>
    /// <param name="preview">A preview of this item's rendered HTML.</param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    /// <param name="childIds">
    /// The list of IDs of the items (including child <see cref="Category"/> items) which belong
    /// to this category.
    /// </param>
    /// <param name="timestampTicks">
    /// The timestamp when this message was sent, in UTC Ticks.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace to which this article belongs.
    /// </param>
    /// <param name="domain">
    /// The domain to which this article belongs (if any).
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
    /// The users allowed to edit this category.
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
    /// The users allowed to view this category.
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
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this category.
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
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this category.
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
    public Category(
        string id,
        string title,
        string html,
        string markdownContent,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        ICollection<string> childIds,
        long timestampTicks,
        string wikiNamespace,
        string? domain,
        bool isDeleted,
        string? owner,
        IReadOnlyCollection<string>? allowedEditors,
        IReadOnlyCollection<string>? allowedViewers,
        IReadOnlyCollection<string>? allowedEditorGroups,
        IReadOnlyCollection<string>? allowedViewerGroups,
        IReadOnlyCollection<string> categories,
        IReadOnlyList<Transclusion>? transclusions) : base(
            id,
            title,
            html,
            markdownContent,
            preview,
            wikiLinks,
            timestampTicks,
            wikiNamespace,
            domain,
            isDeleted,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            null,
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
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        long timestampTicks,
        string wikiNamespace,
        string? domain,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null,
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
            domain,
            isDeleted,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            categories,
            transclusions)
    { }

    /// <summary>
    /// Gets the latest revision for the article with the given title.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the article to retrieve.</param>
    /// <param name="domain">The domain of the article to retrieve (if any).</param>
    /// <param name="allowCaseInsenstive">
    /// If <see langword="true"/> a case-insensitive match will be returned if no exact match is
    /// found, but only if there is only one such match. If there is more than one possible
    /// match when disregarding case, no result is returned.
    /// </param>
    /// <returns>
    /// The latest revision for the article with the given title; or <see langword="null"/> if
    /// no such article exists.
    /// </returns>
    public static async Task<Category?> GetCategoryAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? title,
        string? domain = null,
        bool allowCaseInsenstive = true)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        Category? category = null;
        var reference = await PageReference.GetPageReferenceAsync(
            dataStore,
            title,
            options.CategoryNamespace,
            domain);
        if (reference is not null)
        {
            category = await dataStore.GetItemAsync<Category>(reference.Reference);
        }
        // If no exact match exists, ignore case if only one such match exists.
        if (category is null && allowCaseInsenstive)
        {
            var normalizedReference = await NormalizedPageReference.GetNormalizedPageReferenceAsync(
                dataStore,
                title,
                options.CategoryNamespace,
                domain);
            if (normalizedReference?.References.Count == 1)
            {
                category = await dataStore.GetItemAsync<Category>(normalizedReference.References[0]);
            }
        }

        return category;
    }

    /// <summary>
    /// Gets a new <see cref="Category"/> instance.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the category. Must be unique within its namespace, and
    /// non-empty.</param>
    /// <param name="editor">
    /// The ID of the user who made this revision.
    /// </param>
    /// <param name="markdown">The raw markdown content.</param>
    /// <param name="domain">The domain to which this category belongs (if any).</param>
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
    /// The users allowed to edit this category.
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
    /// The users allowed to view this category.
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
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this category.
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
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this category.
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
        WikiOptions options,
        IDataStore dataStore,
        string title,
        string editor,
        string? markdown = null,
        string? domain = null,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
        }
        title = title.ToWikiTitleCase();

        var wikiId = dataStore.CreateNewIdFor<Category>();

        await CreatePageReferenceAsync(dataStore, wikiId, title, options.CategoryNamespace, domain)
            .ConfigureAwait(false);

        var revision = new Revision(
            wikiId,
            editor,
            title,
            options.CategoryNamespace,
            domain,
            null,
            markdown);
        await dataStore.StoreItemAsync(revision).ConfigureAwait(false);

        var md = markdown;
        List<Transclusion> transclusions;
        if (string.IsNullOrEmpty(markdown))
        {
            transclusions = new List<Transclusion>();
        }
        else
        {
            (md, transclusions) = await TransclusionParser.TranscludeInnerAsync(
                options,
                dataStore,
                title,
                $"{options.CategoryNamespace}:{title}",
                markdown);
        }

        var wikiLinks = GetWikiLinks(options, dataStore, md, title, options.CategoryNamespace, domain);

        var categories = await UpdateCategoriesAsync(
            options,
            dataStore,
            wikiId,
            editor,
            owner,
            domain,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            wikiLinks)
            .ConfigureAwait(false);

        var category = new Category(
            wikiId,
            title,
            markdown,
            RenderHtml(
                options,
                dataStore,
                await PostprocessArticleMarkdownAsync(
                    options,
                    dataStore,
                    title,
                    options.CategoryNamespace,
                    domain,
                    markdown)),
            RenderPreview(
                options,
                dataStore,
                await PostprocessArticleMarkdownAsync(
                    options,
                    dataStore,
                    title,
                    options.CategoryNamespace,
                    domain,
                    markdown,
                    true)),
            new ReadOnlyCollection<WikiLink>(wikiLinks),
            revision.TimestampTicks,
            options.CategoryNamespace,
            domain,
            isDeleted: false,
            owner,
            allowedEditors,
            allowedViewers,
            allowedEditorGroups,
            allowedViewerGroups,
            categories,
            transclusions);
        await dataStore.StoreItemAsync(category).ConfigureAwait(false);

        await AddPageTransclusionsAsync(dataStore, wikiId, transclusions).ConfigureAwait(false);

        await AddPageLinksAsync(dataStore, wikiId, wikiLinks).ConfigureAwait(false);

        await UpdateReferencesAsync(
            options,
            dataStore,
            title,
            options.CategoryNamespace,
            domain,
            false,
            true,
            null,
            null,
            null,
            false,
            false)
            .ConfigureAwait(false);

        if (options.OnCreated is not null)
        {
            await options.OnCreated.Invoke(category, editor).ConfigureAwait(false);
        }

        return category;
    }

    /// <summary>
    /// Revises this <see cref="Category"/> instance.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
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
    /// <param name="domain">
    /// <para>
    /// The optional new domain to which this article belongs.
    /// </para>
    /// <para>
    /// If left <see langword="null"/> the existing domain will be retained (if any).
    /// </para>
    /// <para>
    /// To clear the domain, set this to an empty string instead, which will assign <see langword="null"/>.
    /// </para>
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
    /// The users allowed to edit this category.
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
    /// The users allowed to view this category.
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
    /// <param name="allowedEditorGroups">
    /// <para>
    /// The groups allowed to edit this category.
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
    /// <param name="allowedViewerGroups">
    /// <para>
    /// The groups allowed to view this category.
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
        WikiOptions options,
        IDataStore dataStore,
        string editor,
        string? title = null,
        string? markdown = null,
        string? revisionComment = null,
        string? domain = null,
        bool isDeleted = false,
        string? owner = null,
        IEnumerable<string>? allowedEditors = null,
        IEnumerable<string>? allowedViewers = null,
        IEnumerable<string>? allowedEditorGroups = null,
        IEnumerable<string>? allowedViewerGroups = null)
    {
        if (isDeleted && ChildIds.Count > 0)
        {
            throw new ArgumentException("Non-empty categories cannot be deleted", nameof(isDeleted));
        }

        title ??= title?.ToWikiTitleCase() ?? Title;
        var previousTitle = Title;
        Title = title;

        if (domain is null)
        {
            domain = Domain;
        }
        else if (string.IsNullOrWhiteSpace(domain))
        {
            domain = null;
        }
        else
        {
            domain = domain.Trim();
        }
        var previousDomain = Domain;
        Domain = domain;

        var sameTitle = string.Equals(previousTitle, title, StringComparison.Ordinal)
            && string.Equals(previousDomain, domain, StringComparison.Ordinal);

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
            await CreatePageReferenceAsync(dataStore, Id, title, options.CategoryNamespace, domain)
                .ConfigureAwait(false);
        }
        if (!sameTitle)
        {
            await RemovePageReferenceAsync(dataStore, Id, previousTitle, options.CategoryNamespace, previousDomain)
                .ConfigureAwait(false);
        }

        var changed = wasDeleted != IsDeleted
            || !string.Equals(previousMarkdown, markdown, StringComparison.Ordinal);

        if (!IsDeleted && changed)
        {
            MarkdownContent = markdown!;

            var previousTransclusions = Transclusions?.ToList() ?? new List<Transclusion>();
            var (md, transclusions) = await TransclusionParser.TranscludeInnerAsync(
                options,
                dataStore,
                title,
                $"{options.CategoryNamespace}:{title}",
                markdown!);
            Transclusions = transclusions.Count == 0
                ? null
                : transclusions.AsReadOnly();
            await RemovePageTransclusionsAsync(dataStore, Id, previousTransclusions.Except(transclusions))
                .ConfigureAwait(false);
            await AddPageTransclusionsAsync(dataStore, Id, transclusions.Except(previousTransclusions))
                .ConfigureAwait(false);

            var previousWikiLinks = WikiLinks.ToList();
            WikiLinks = GetWikiLinks(options, dataStore, md, title, options.CategoryNamespace, domain).AsReadOnly();
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
                domain,
                allowedEditors,
                allowedViewers,
                allowedEditorGroups,
                allowedViewerGroups,
                WikiLinks,
                Categories)
                .ConfigureAwait(false))
                .AsReadOnly();

            await UpdateAsync(options, dataStore);
        }

        var oldOwner = Owner;
        Owner = owner;
        AllowedEditors = allowedEditors?.ToList().AsReadOnly();
        AllowedViewers = allowedViewers?.ToList().AsReadOnly();

        var revision = new Revision(
            Id,
            editor,
            title,
            options.CategoryNamespace,
            domain,
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
            options.CategoryNamespace,
            domain,
            IsDeleted,
            sameTitle,
            previousTitle,
            options.CategoryNamespace,
            previousDomain,
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

    internal async Task AddArticleAsync(IDataStore dataStore, string wikiId)
    {
        ChildIds.Add(wikiId);
        await dataStore.StoreItemAsync(this).ConfigureAwait(false);
    }

    internal Task AddArticleAsync(IDataStore dataStore, Article child) => AddArticleAsync(dataStore, child.Id);

    internal async Task RemoveChildAsync(IDataStore dataStore, Article child)
    {
        ChildIds.Remove(child.Id);
        await dataStore.StoreItemAsync(this).ConfigureAwait(false);
    }

    internal async Task RemoveChildIdAsync(IDataStore dataStore, string childId)
    {
        ChildIds.Remove(childId);
        await dataStore.StoreItemAsync(this).ConfigureAwait(false);
    }

    internal async Task RestoreAsync(
        WikiOptions options,
        IDataStore dataStore,
        string editor)
    {
        WikiNamespace = options.CategoryNamespace;

        var existing = await GetCategoryAsync(options, dataStore, Title, Domain, false);
        if (existing is not null)
        {
            Id = existing.Id;
        }

        await CreatePageReferenceAsync(
            dataStore,
            Id,
            Title,
            WikiNamespace,
            Domain)
            .ConfigureAwait(false);

        var md = MarkdownContent;
        List<Transclusion> transclusions;
        if (string.IsNullOrEmpty(MarkdownContent))
        {
            transclusions = new List<Transclusion>();
        }
        else
        {
            (md, transclusions) = await TransclusionParser.TranscludeInnerAsync(
                options,
                dataStore,
                Title,
                $"{options.CategoryNamespace}:{Title}",
                MarkdownContent);
        }

        var wikiLinks = GetWikiLinks(options, dataStore, md, Title, options.CategoryNamespace, Domain);

        var categories = await UpdateCategoriesAsync(
            options,
            dataStore,
            Id,
            editor,
            Owner,
            Domain,
            AllowedEditors,
            AllowedViewers,
            AllowedEditorGroups,
            AllowedViewerGroups,
            wikiLinks)
            .ConfigureAwait(false);

        Html = RenderHtml(
            options,
            dataStore,
            await PostprocessArticleMarkdownAsync(
                options,
                dataStore,
                Title,
                options.CategoryNamespace,
                Domain,
                MarkdownContent));
        Preview = RenderPreview(
            options,
            dataStore,
            await PostprocessArticleMarkdownAsync(
                options,
                dataStore,
                Title,
                options.CategoryNamespace,
                Domain,
                MarkdownContent,
                true));
        WikiLinks = new ReadOnlyCollection<WikiLink>(wikiLinks);
        Categories = categories;
        Transclusions = transclusions;
        await dataStore.StoreItemAsync(this).ConfigureAwait(false);

        await AddPageTransclusionsAsync(dataStore, Id, transclusions).ConfigureAwait(false);

        await AddPageLinksAsync(dataStore, Id, wikiLinks).ConfigureAwait(false);

        await UpdateReferencesAsync(
            options,
            dataStore,
            Title,
            options.CategoryNamespace,
            Domain,
            IsDeleted,
            true,
            null,
            null,
            null,
            false,
            false)
            .ConfigureAwait(false);
    }
}
