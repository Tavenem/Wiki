using Markdig.Syntax.Inlines;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// A wiki link inline.
/// </summary>
public class WikiLinkInline : LinkInline
{
    /// <summary>
    /// An action segment which follows the link.
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// The display text.
    /// </summary>
    public string? Display { get; set; }

    /// <summary>
    /// Any fragment which follows the link.
    /// </summary>
    public string? Fragment { get; set; }

    /// <summary>
    /// Whether this link has an explicit display label.
    /// </summary>
    public bool HasDisplay { get; set; }

    /// <summary>
    /// Whether this is a link to a category.
    /// </summary>
    public bool IsCategory { get; set; }

    /// <summary>
    /// Whether this is a link to an item in the Creative Commons.
    /// </summary>
    public bool IsCommons { get; set; }

    /// <summary>
    /// Whether a leading ':' precedes the link.
    /// </summary>
    public bool IsEscaped { get; set; }

    /// <summary>
    /// Whether this is a link to a missing page.
    /// </summary>
    public bool IsMissing { get; set; }

    /// <summary>
    /// Whether this is a link to Wikipedia.
    /// </summary>
    public bool IsWikipedia { get; set; }

    /// <summary>
    /// A string added to this wiki link, if non-empty.
    /// </summary>
    public string? LinkTemplate { get; set; }

    /// <summary>
    /// The linked page (if any).
    /// </summary>
    public Page? Page { get; set; }

    /// <summary>
    /// The title of the linked article.
    /// </summary>
    public PageTitle? PageTitle { get; set; }
}
