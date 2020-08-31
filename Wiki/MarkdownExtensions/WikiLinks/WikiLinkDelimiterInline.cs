using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace NeverFoundry.Wiki.MarkdownExtensions.WikiLinks
{
    /// <summary>
    /// A delimiter for a wiki link.
    /// </summary>
    public class WikiLinkDelimiterInline : LinkDelimiterInline
    {
        /// <summary>
        /// Whether this link's display label was auto-generated.
        /// </summary>
        public bool AutoDisplay { get; set; }

        /// <summary>
        /// The display text.
        /// </summary>
        public string? Display { get; set; }

        /// <summary>
        /// Whether this link has an explicit display label.
        /// </summary>
        public bool HasDisplay { get; set; }

        /// <summary>
        /// The title of the linked article.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiLinkDelimiterInline"/>.
        /// </summary>
        /// <param name="parser">The <see cref="InlineParser"/> instance.</param>
        public WikiLinkDelimiterInline(InlineParser parser) : base(parser) { }
    }
}
