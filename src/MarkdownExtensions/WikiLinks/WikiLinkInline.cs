using Markdig.Syntax.Inlines;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// A wiki link inline.
/// </summary>
public class WikiLinkInline : LinkInline
{
    /// <summary>
    /// The linked article (if any).
    /// </summary>
    public Article? Article { get; set; }

    /// <summary>
    /// The display text.
    /// </summary>
    public string? Display { get; set; }

    /// <summary>
    /// The domain of the linked article (if any).
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// The text outside the link which is to be included.
    /// </summary>
    public string? Endmatter { get; set; }

    /// <summary>
    /// Whether this is a link to a missing page.
    /// </summary>
    public bool Missing { get; set; }

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
    /// Whether a leading ':' preceded the title.
    /// </summary>
    public bool IsNamespaceEscaped { get; set; }

    /// <summary>
    /// Whether this is a link to a discussion page.
    /// </summary>
    public bool IsTalk { get; set; }

    /// <summary>
    /// Whether this is a link to Wikipedia.
    /// </summary>
    public bool IsWikipedia { get; set; }

    /// <summary>
    /// The namespace for the linked article (if any).
    /// </summary>
    public string? WikiNamespace { get; set; }
}
