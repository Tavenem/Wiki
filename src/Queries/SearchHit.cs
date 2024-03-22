namespace Tavenem.Wiki.Queries;

/// <summary>
/// A search hit.
/// </summary>
/// <param name="Title">
/// The title of the matching wiki item.
/// </param>
/// <param name="Excerpt">
/// An optional excerpt from the matching article, possibly with some sections highlighted with HTML
/// <c>em</c> tags to indicate query matches.
/// </param>
public record SearchHit(PageTitle Title, string? Excerpt = null);