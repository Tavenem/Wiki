namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a wiki page.
/// </summary>
/// <param name="DisplayTitle">
/// The title to display (may be different than the actual <see cref="Page.Title"/>).
/// </param>
/// <param name="Html">
/// <para>
/// The content of the page.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the page has content, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="IsDiff">
/// Whether the content shows a diff.
/// </param>
/// <param name="Page">
/// <para>
/// The <see cref="Wiki.Page"/>.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the page exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for this page.
/// </param>
public record WikiPageInfo(
    string? DisplayTitle,
    string? Html,
    bool IsDiff,
    Page? Page,
    WikiPermission Permission);
