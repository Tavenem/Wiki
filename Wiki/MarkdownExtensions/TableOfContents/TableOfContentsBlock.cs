using Markdig.Parsers;
using Markdig.Syntax;
using System.Collections.Generic;

namespace NeverFoundry.Wiki.MarkdownExtensions.TableOfContents
{
    /// <summary>
    /// A leaf block representing a table of contents.
    /// </summary>
    public class TableOfContentsBlock : LeafBlock
    {
        /// <summary>
        /// The number of levels of hierarchy which will be shown.
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Whether this is the default Table of Contents marker.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Whether this is a marker indicating that the default Table of Contents should be
        /// suppressed.
        /// </summary>
        public bool IsNoToc { get; set; }

        /// <summary>
        /// The first level of hierarchy (1-based) which will be shown.
        /// </summary>
        public int StartingLevel { get; set; }

        /// <summary>
        /// The title of this table of contents.
        /// </summary>
        public string Title { get; set; } = "Contents";

        internal List<HeadingBlock>? Headings { get; set; }

        internal int LevelOffset { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TableOfContentsBlock"/>.
        /// </summary>
        /// <param name="parser">The <see cref="BlockParser"/> instance.</param>
        public TableOfContentsBlock(BlockParser? parser) : base(parser) { }
    }
}
