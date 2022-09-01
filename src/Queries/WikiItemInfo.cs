namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a wiki item.
/// </summary>
/// <param name="DisplayTitle">
/// The title to display (may be different than the actual <see cref="Article.Title"/>).
/// </param>
/// <param name="Html">
/// <para>
/// The content of the item.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the item has content, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="IsDiff">
/// Whether the content shows a diff.
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
/// <param name="Permission">
/// The permission(s) the requesting user has for this item.
/// </param>
public record WikiItemInfo(
    string? DisplayTitle,
    string? Html,
    bool IsDiff,
    Article? Item,
    WikiPermission Permission);
