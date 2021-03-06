using Tavenem.DataStorage;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;

namespace Tavenem.Wiki.Test;

/// <summary>
/// Test subclass of <see cref="MarkdownItem"/>.
/// </summary>
public class MarkdownItemTestSubclass : MarkdownItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    public MarkdownItemTestSubclass() { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    public MarkdownItemTestSubclass(string id) : base(id) { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="markdownContent">The raw markdown.</param>
    /// <param name="html">The rendered HTML content.</param>
    /// <param name="preview">A preview of this item's rendered HTML.</param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers.
    /// </remarks>
    public MarkdownItemTestSubclass(string id, string? markdownContent, string html, string preview, IReadOnlyCollection<WikiLink> wikiLinks)
        : base(id, markdownContent, html, preview, wikiLinks)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    /// <param name="markdown">The raw markdown.</param>
    /// <param name="html">
    /// The rendered HTML content.
    /// </param>
    /// <param name="preview">
    /// A preview of this item's rendered HTML.
    /// </param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    public MarkdownItemTestSubclass(string? markdown, string? html, string? preview, IReadOnlyCollection<WikiLink> wikiLinks)
        : base(markdown, html, preview, wikiLinks)
    { }

    public static MarkdownItemTestSubclass New(IWikiOptions options, IDataStore dataStore, string? markdown)
    {
        var md = string.IsNullOrEmpty(markdown)
            ? null
            : TransclusionParser.Transclude(
                options,
                dataStore,
                null,
                null,
                markdown,
                out _);
        var wikiLinks = GetWikiLinks(options, dataStore, md);
        return new MarkdownItemTestSubclass(
            md,
            RenderHtml(options, dataStore, md),
            RenderPreview(
                options,
                dataStore,
                string.IsNullOrEmpty(markdown)
                    ? string.Empty
                    : TransclusionParser.Transclude(
                        options,
                        dataStore,
                        null,
                        null,
                        markdown,
                        out _,
                        isPreview: true)),
            wikiLinks);
    }
}
