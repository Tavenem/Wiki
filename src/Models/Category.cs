using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A wiki category revision.
/// </summary>
public sealed class Category : Page, IPage<Category>
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string CategoryIdItemTypeName = ":Page:Category:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => CategoryIdItemTypeName;

    /// <summary>
    /// The pages which belong to this category (including child categories).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Updates to this cache do not count as revisions.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </para>
    /// </remarks>
    public IReadOnlyCollection<PageTitle>? Children { get; set; }

    /// <summary>
    /// Whether this page exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Categories are always considered to exist implicitly. They are enumerations of their members
    /// (even when they have no current members), even if they have no other content.
    /// </para>
    /// <para>
    /// This property has a public setter for serialization support, but setting it has no effect.
    /// </para>
    /// </remarks>
    public override bool Exists
    {
        get => true;
        set { }
    }

    /// <summary>
    /// The files contained directly in this category.
    /// </summary>
    /// <remarks>
    /// Note: this property is not persisted. It is dynamically built when the category is retrieved.
    /// </remarks>
    public Dictionary<string, List<CategoryFile>>? Files { get; set; }

    /// <summary>
    /// Whether this category contains no children.
    /// </summary>
    public bool IsEmpty => Children is null || Children.Count == 0;

    /// <summary>
    /// The pages contained directly within this category.
    /// </summary>
    /// <remarks>
    /// Note: this property is not persisted. It is dynamically built when the category is retrieved.
    /// </remarks>
    public Dictionary<string, List<PageTitle>>? Pages { get; set; }

    /// <summary>
    /// The categories contained directly in this category.
    /// </summary>
    /// <remarks>
    /// Note: this property is not persisted. It is dynamically built when the category is retrieved.
    /// </remarks>
    public Dictionary<string, List<Subcategory>>? Subcategories { get; set; }

    /// <summary>
    /// Constructs a new instance of <see cref="Category"/>.
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
    public Category(
        PageTitle title,
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        string? markdownContent = null,
        string? owner = null,
        Revision? revision = null,
        IReadOnlyCollection<string>? allowedEditors = null,
        IReadOnlyCollection<string>? allowedViewers = null,
        IReadOnlyCollection<string>? allowedEditorGroups = null,
        IReadOnlyCollection<string>? allowedViewerGroups = null,
        IReadOnlyCollection<PageTitle>? categories = null,
        IReadOnlyCollection<Heading>? headings = null,
        IReadOnlyCollection<PageTitle>? redirectReferences = null,
        IReadOnlyCollection<PageTitle>? references = null,
        IReadOnlyCollection<PageTitle>? transclusionReferences = null,
        IReadOnlyCollection<PageTitle>? transclusions = null,
        bool isBrokenRedirect = false,
        bool isDoubleRedirect = false,
        PageTitle? redirectTitle = null,
        IReadOnlyCollection<PageTitle>? children = null) : base(
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
        headings,
        redirectReferences,
        references,
        transclusionReferences,
        transclusions,
        isBrokenRedirect,
        isDoubleRedirect,
        redirectTitle) => Children = children;

    private Category(PageTitle title) : base(title) { }

    /// <summary>
    /// Gets an empty instance of <see cref="Category"/>.
    /// </summary>
    /// <returns>An empty instance of <see cref="Category"/>.</returns>
    public static new Category Empty(PageTitle title) => new(title);

    /// <summary>
    /// Gets a copy of this instance.
    /// </summary>
    /// <param name="newNamespace">A new namespace to assign to the copied page.</param>
    /// <returns>
    /// A new instance of <see cref="Category"/> with the same properties as this instance.
    /// </returns>
    public override Category Copy(string? newNamespace = null)
        => Copy<Category>(newNamespace);

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
        PageTitle? redirectTitle = null) => RenameAsync<Category>(
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

    internal async ValueTask AddPageAsync(IDataStore dataStore, PageTitle title)
    {
        if (Children?.Contains(title) == true)
        {
            return;
        }

        var children = Children?.ToList() ?? [];
        children.Add(title);
        Children = children.AsReadOnly();
        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }

    internal override Category GetArchiveCopy()
    {
        var page = base.GetArchiveCopy();
        if (page is Category category)
        {
            category.Children = null;
            return category;
        }
        throw new InvalidOperationException();
    }

    internal async ValueTask RemovePageAsync(IDataStore dataStore, PageTitle title)
    {
        if (Children?.Contains(title) != true)
        {
            return;
        }

        Children = Children.ToImmutableList().Remove(title);
        await dataStore.StoreItemAsync(this)
            .ConfigureAwait(false);
    }
}
