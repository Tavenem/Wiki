﻿using Microsoft.Extensions.Caching.Memory;
using SmartComponents.LocalEmbeddings;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A user page.
/// </summary>
public class UserPage : OwnerPage, IPage<UserPage>
{
    /// <summary>
    /// The groups to which the associated user belongs (if any).
    /// </summary>
    public List<IWikiGroup>? Groups { get; set; }

    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string UserPageIdItemTypeName = ":Page:Article:OwnerPage:UserPage:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => UserPageIdItemTypeName;

    /// <summary>
    /// Constructs a new instance of <see cref="UserPage"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor should only be used for deserialization or for testing purposes.
    /// </para>
    /// <para>
    /// To create a new page as part of a normal user interaction, use the <see
    /// cref="WikiExtensions.AddOrReviseWikiPageAsync"/> method.
    /// </para>
    /// <para>
    /// To create a page programmatically, you can use a combination of <see
    /// cref="WikiExtensions.GetWikiPageAsync(IDataStore, WikiOptions, PageTitle, bool, bool)"/> (to
    /// get the current page, or a new page if one does not already exist), and <see
    /// cref="Page.UpdateAsync"/> to update the result with the intended properties.
    /// </para>
    /// </remarks>
    [JsonConstructor]
    public UserPage(
        PageTitle title,
        string? html = null,
        string? preview = null,
        string? text = null,
        string? markdownContent = null,
        string? owner = null,
        Revision? revision = null,
        IReadOnlyCollection<string>? allowedEditors = null,
        IReadOnlyCollection<string>? allowedViewers = null,
        IReadOnlyCollection<string>? allowedEditorGroups = null,
        IReadOnlyCollection<string>? allowedViewerGroups = null,
        IReadOnlyCollection<PageTitle>? categories = null,
        IReadOnlyCollection<IReadOnlyCollection<byte>>? embeddingI1Bytes = null,
        IReadOnlyCollection<Heading>? headings = null,
        IReadOnlyCollection<PageTitle>? redirectReferences = null,
        IReadOnlyCollection<PageTitle>? references = null,
        IReadOnlyCollection<byte>? titleEmbeddingI1Bytes = null,
        IReadOnlyCollection<PageTitle>? transclusionReferences = null,
        IReadOnlyCollection<PageTitle>? transclusions = null,
        bool isBrokenRedirect = false,
        bool isDoubleRedirect = false,
        PageTitle? redirectTitle = null) : base(
        title,
        html,
        preview,
        text,
        markdownContent,
        owner,
        revision,
        allowedEditors,
        allowedViewers,
        allowedEditorGroups,
        allowedViewerGroups,
        categories,
        embeddingI1Bytes,
        headings,
        redirectReferences,
        references,
        titleEmbeddingI1Bytes,
        transclusionReferences,
        transclusions,
        isBrokenRedirect,
        isDoubleRedirect,
        redirectTitle)
    { }

    private protected UserPage(PageTitle title) : base(title) { }

    /// <summary>
    /// Gets an empty instance of <see cref="UserPage"/>.
    /// </summary>
    /// <returns>An empty instance of <see cref="UserPage"/>.</returns>
    public static new UserPage Empty(PageTitle title) => new(title);

    /// <summary>
    /// Gets a copy of this instance.
    /// </summary>
    /// <param name="newNamespace">A new namespace to assign to the copied page.</param>
    /// <returns>
    /// A new instance of <see cref="UserPage"/> with the same properties as this instance.
    /// </returns>
    public override UserPage Copy(string? newNamespace = null)
        => Copy<UserPage>(newNamespace);

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
    /// <param name="embedder">
    /// <para>
    /// An instance of <see cref="LocalEmbedder"/> to use for embedding.
    /// </para>
    /// <para>
    /// If omitted, a default static instance will be created, used, and then disposed. This is
    /// highly inefficient and can slow performance considerably. A singleton instance should be
    /// passed whenever possible.
    /// </para>
    /// </param>
    /// <param name="cache">
    /// <para>
    /// An <see cref="IMemoryCache"/> instance used to cache a mapping of wiki page titles to search
    /// embeddings. This should normally be a singleton instance supplied by dependency injection.
    /// </para>
    /// <para>
    /// If no cache is supplied, the entire database of wiki pages will be read and its contents
    /// parsed for embeddings on every search. For very small wikis with highly responsive data
    /// persistence mechanisms, this may be desirable.
    /// </para>
    /// <para>
    /// The cache will only be updated if it has been built (lazily, as a result of a search).
    /// </para>
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
        PageTitle? redirectTitle = null,
        LocalEmbedder? embedder = null,
        IMemoryCache? cache = null) => RenameAsync(
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
            redirectTitle,
            WikiJsonSerializerContext.Default.UserPage,
            embedder,
            cache);
}
