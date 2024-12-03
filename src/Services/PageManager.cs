namespace Tavenem.Wiki.Services;

/// <summary>
/// A default implementation of <see cref="IPageManager"/> which performs no actions.
/// </summary>
public class PageManager : IPageManager
{
    /// <summary>
    /// A callback invoked when a new page is created (no-op in this default implementation).
    /// </summary>
    /// <param name="page">The new <see cref="Page"/>.</param>
    /// <param name="editor">The ID of the user who created the new page.</param>
    /// <remarks>
    /// This function is also invoked when a previously deleted article is re-created.
    /// </remarks>
    public ValueTask OnCreatedAsync(Page page, string editor) => ValueTask.CompletedTask;

    /// <summary>
    /// A callback invoked when a page is deleted (no-op in this default implementation).
    /// </summary>
    /// <param name="page">The deleted page.</param>
    /// <param name="oldOwner">The original <see cref="Page.Owner"/>.</param>
    /// <param name="newOwner">The new <see cref="Page.Owner"/>.</param>
    public ValueTask OnDeletedAsync(Page page, string? oldOwner, string? newOwner) => ValueTask.CompletedTask;

    /// <summary>
    /// A callback invoked when a <see cref="Page"/> is edited (not including deletion; no-op in this default implementation).
    /// </summary>
    /// <param name="page">The edited page.</param>
    /// <param name="revision">The revision applied.</param>
    /// <param name="oldOwner">The original <see cref="Page.Owner"/>.</param>
    /// <param name="newOwner">The new <see cref="Page.Owner"/>.</param>
    public ValueTask OnEditedAsync(Page page, Revision revision, string? oldOwner, string? newOwner) => ValueTask.CompletedTask;

    /// <summary>
    /// A callback invoked when a <see cref="Page"/> is renamed (no-op in this default implementation).
    /// </summary>
    /// <param name="page">The renamed page.</param>
    /// <param name="oldTitle">The original title.</param>
    /// <param name="oldOwner">The original <see cref="Page.Owner"/>.</param>
    /// <param name="newOwner">The new <see cref="Page.Owner"/>.</param>
    /// <remarks>
    /// Note that in addition to this method, <see cref="OnDeletedAsync"/> is called for the
    /// original page, and <see cref="OnCreatedAsync"/> for the new page, since a rename is actually
    /// the deletion of the original page plus the creation of a new page with a new title.
    /// </remarks>
    public ValueTask OnRenamedAsync(Page page, PageTitle oldTitle, string? oldOwner, string? newOwner) => ValueTask.CompletedTask;
}
