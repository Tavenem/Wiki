namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a wiki item, with additional information suited to editing the item.
/// </summary>
/// <param name="AllowedEditors">
/// <para>
/// Information about the users currently permitted to edit the item.
/// </para>
/// <para>
/// Will be <see langword="null"/> even if the list is non-empty if the requesting user does not
/// have permission to view the item.
/// </para>
/// </param>
/// <param name="AllowedEditorGroups">
/// <para>
/// Information about the groups currently permitted to edit the item.
/// </para>
/// <para>
/// Will be <see langword="null"/> even if the list is non-empty if the requesting user does not
/// have permission to view the item.
/// </para>
/// </param>
/// <param name="AllowedViewers">
/// <para>
/// Information about the users currently permitted to view the item.
/// </para>
/// <para>
/// Will be <see langword="null"/> even if the list is non-empty if the requesting user does not
/// have permission to view the item.
/// </para>
/// </param>
/// <param name="AllowedViewerGroups">
/// <para>
/// Information about the groups currently permitted to view the item.
/// </para>
/// <para>
/// Will be <see langword="null"/> even if the list is non-empty if the requesting user does not
/// have permission to view the item.
/// </para>
/// </param>
/// <param name="DisplayTitle">
/// The title to display (may be different than the actual <see cref="Article.Title"/>).
/// </param>
/// <param name="Item">
/// <para>
/// The <see cref="Article"/>, <see cref="Category"/>, or <see cref="WikiFile"/>.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the item exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Owner">
/// <para>
/// Information about the current owner of the item.
/// </para>
/// <para>
/// Will be <see langword="null"/> even if there is a current owner if the requesting user does not
/// have permission to view the item.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for this item.
/// </param>
public record WikiEditInfo(
    List<WikiUserInfo>? AllowedEditors,
    List<WikiUserInfo>? AllowedEditorGroups,
    List<WikiUserInfo>? AllowedViewers,
    List<WikiUserInfo>? AllowedViewerGroups,
    string? DisplayTitle,
    Article? Item,
    WikiUserInfo? Owner,
    WikiPermission Permission);
