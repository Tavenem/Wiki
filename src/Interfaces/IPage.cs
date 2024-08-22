using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A common base interface for wiki pages.
/// </summary>
public interface IPage<TSelf> where TSelf : class, IIdItem, IPage<TSelf>?
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string PageIdItemTypeName = ":Page:";

    /// <summary>
    /// <para>
    /// Indicates whether this is a redirect to a page which does not exist.
    /// </para>
    /// <para>
    /// Updates to this property do not constitute a revision.
    /// </para>
    /// </summary>
    bool IsBrokenRedirect { get; }

    /// <summary>
    /// <para>
    /// Indicates whether this is a redirect to a page which is also a redirect.
    /// </para>
    /// <para>
    /// Updates to this property do not constitute a revision.
    /// </para>
    /// </summary>
    bool IsDoubleRedirect { get; }

    /// <summary>
    /// If this is a redirect, contains the title of the destination.
    /// </summary>
    PageTitle? RedirectTitle { get; }

    /// <summary>
    /// The title of this page.
    /// </summary>
    PageTitle Title { get; }

    /// <summary>
    /// Gets the ID for an <see cref="IPage{TSelf}"/> given its <paramref name="title"/>.
    /// </summary>
    /// <param name="title">
    /// The title of the page.
    /// </param>
    /// <returns>
    /// The ID which should be used for an <see cref="IPage{TSelf}"/> given the parameters.
    /// </returns>
    public static string GetId(PageTitle title) => PageIdItemTypeName + title.ToString();

    /// <summary>
    /// Gets the latest revision for the page with the given title.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the page to retrieve.</param>
    /// <param name="exactMatch">
    /// If <see langword="true"/> a case-insensitive match will not be attempted if an exact match
    /// is not found.
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/>, redirects will be ignored, and the literal content of a redirect
    /// article will be returned.
    /// </param>
    /// <param name="typeInfo">
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// The latest revision for the page with the given title.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Note: this will only return a value when the given page has some data associated with it. In
    /// some cases, it may not have any content/revisions. The data may consist only of reference
    /// properties (e.g. the <see cref="Page.References"/> collection, if other pages link to it).
    /// It may return <see langword="null"/> if no data at all exists in association with the page.
    /// In some cases, an empty object may be returned even if no data currently exists, in cases
    /// where data formerly existed but has been removed.
    /// </para>
    /// <para>
    /// Use <see cref="GetPageAsync{T}(IDataStore, PageTitle, bool, bool, JsonTypeInfo{T}?)"/> to retrieve a
    /// placeholder object even if no data has ever existed for a page.
    /// </para>
    /// </remarks>
    public static Task<T?> GetExistingPageAsync<T>(
        IDataStore dataStore,
        PageTitle title,
        bool exactMatch = true,
        bool noRedirect = true,
        JsonTypeInfo<T>? typeInfo = null) where T : class, IIdItem, IPage<T>
        => GetPageAsync(dataStore, title, false, exactMatch, noRedirect, typeInfo);

    /// <summary>
    /// Gets the latest revision for the page with the given title.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the page to retrieve.</param>
    /// <param name="exactMatch">
    /// If <see langword="true"/> a case-insensitive match will not be attempted if an exact match
    /// is not found.
    /// </param>
    /// <param name="noRedirect">
    /// If <see langword="true"/>, redirects will be ignored, and the literal content of a
    /// redirect article will be returned.
    /// </param>
    /// <param name="typeInfo">
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// The latest revision for the page with the given title.
    /// </returns>
    /// <remarks>
    /// Note: this always returns a value. If no such page exists, a placeholder instance is
    /// returned. In some cases, the placeholder for a page which does not exist may even have
    /// non-default values for some properties (e.g. the <see cref="Page.References"/> collection,
    /// if other pages link to it).
    /// </remarks>
    public static Task<T> GetPageAsync<T>(
        IDataStore dataStore,
        PageTitle title,
        bool exactMatch = false,
        bool noRedirect = false,
        JsonTypeInfo<T>? typeInfo = null) where T : class, IIdItem, IPage<T>
        => GetPageAsync<T>(dataStore, title, true, exactMatch, noRedirect, typeInfo)!;

    /// <summary>
    /// Gets an empty instance of the current type.
    /// </summary>
    /// <returns>An empty instance of <typeparamref name="TSelf" />.</returns>
    static abstract TSelf Empty(PageTitle title);

    private static async Task<T?> GetPageAsync<T>(
        IDataStore dataStore,
        PageTitle title,
        bool usePlaceholder,
        bool exactMatch,
        bool noRedirect,
        JsonTypeInfo<T>? typeInfo = null) where T : class, IIdItem, IPage<T>
    {
        var id = GetId(title);

        T? page;
        var count = 0;
        var ids = new HashSet<string>();
        bool redirect;
        do
        {
            redirect = false;
            page = await dataStore.GetItemAsync<T>(id, typeInfo)
                .ConfigureAwait(false);

            // If no exact match exists, ignore case if only one such match exists.
            if (!exactMatch && page is null)
            {
                var normalizedReference = await NormalizedPageReference
                    .GetNormalizedPageReferenceAsync(dataStore, title)
                    .ConfigureAwait(false);
                if (normalizedReference?.References.Count == 1)
                {
                    page = await dataStore
                        .GetItemAsync(normalizedReference.References[0], typeInfo)
                        .ConfigureAwait(false);
                }
            }

            if (!noRedirect
                && page?.IsBrokenRedirect == false
                && page.RedirectTitle.HasValue)
            {
                redirect = true;
                if (ids.Contains(page.Id))
                {
                    break; // abort if a cycle is detected
                }
                ids.Add(page.Id);
                id = GetId(page.RedirectTitle.Value);
            }

            count++;
        } while (redirect && count < 100); // abort if redirection nests 100 levels

        if (page is null && usePlaceholder)
        {
            // Get a placeholder for pages which do not exist yet at all.
            return T.Empty(title);
        }

        return page;
    }

    /// <summary>
    /// Gets a copy of this instance.
    /// </summary>
    /// <param name="newNamespace">A new namespace to assign to the copied page.</param>
    /// <returns>
    /// A new instance of <typeparamref name="TSelf"/> with the same properties as this instance.
    /// </returns>
    TSelf Copy(string? newNamespace = null);

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
    Task RenameAsync(
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
        IMemoryCache? cache = null);
}
