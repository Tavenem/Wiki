using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a link to or from a wiki page.
/// </summary>
/// <param name="Title">The title of the page.</param>
/// <param name="ChildCount">
/// The number of child items if the link is a <see cref="Category"/>.
/// </param>
/// <param name="FileSize">
/// The file size if the link is a <see cref="WikiFile"/>.
/// </param>
/// <param name="FileType">
/// The file type if the link is a <see cref="WikiFile"/>.
/// </param>
public record LinkInfo(
    PageTitle Title,
    int ChildCount,
    int FileSize,
    string? FileType);
