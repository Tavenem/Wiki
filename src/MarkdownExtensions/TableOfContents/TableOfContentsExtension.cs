using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;
using System.Linq;

namespace Tavenem.Wiki.MarkdownExtensions.TableOfContents
{
    /// <summary>
    /// An extension to add automatic table of contents generation.
    /// </summary>
    public class TableOfContentsExtension : IMarkdownExtension
    {
        /// <summary>
        /// <para>
        /// A format string used to produce a valid table of contents markdown entry.
        /// </para>
        /// <para>
        /// The parameters are, in order: the depth, starting level, and title.
        /// </para>
        /// </summary>
        public const string ToCFormat = "<!-- TOC {0} {1} {2} -->";
        private const string DefaultToCKey = "DefaultToC";

        /// <summary>
        /// The options set for this instance.
        /// </summary>
        public TableOfContentsOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TableOfContentsExtension"/>.
        /// </summary>
        public TableOfContentsExtension() => Options = new TableOfContentsOptions();

        /// <summary>
        /// Initializes a new instance of <see cref="TableOfContentsExtension"/>.
        /// </summary>
        /// <param name="options">
        /// The options set for this instance.
        /// </param>
        public TableOfContentsExtension(TableOfContentsOptions options) => Options = options;

        /// <summary>
        /// Setups this extension for the specified pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<TableOfContentsBlockParser>())
            {
                pipeline.BlockParsers.InsertBefore<HtmlBlockParser>(new TableOfContentsBlockParser(Options));
            }

            pipeline.DocumentProcessed -= PipelineOnDocumentProcessed;
            pipeline.DocumentProcessed += PipelineOnDocumentProcessed;

            var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
            if (headingBlockParser != null)
            {
                headingBlockParser.Closed -= BlockParser_Closed;
                headingBlockParser.Closed += BlockParser_Closed;
            }
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
            if (paragraphBlockParser != null)
            {
                paragraphBlockParser.Closed -= BlockParser_Closed;
                paragraphBlockParser.Closed += BlockParser_Closed;
            }
        }

        /// <summary>
        /// Setups this extension for the specified renderer.
        /// </summary>
        /// <param name="pipeline">The pipeline used to parse the document.</param>
        /// <param name="renderer">The renderer.</param>
        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
            => renderer.ObjectRenderers.AddIfNotAlready<TableOfContentsRenderer>();

        private void BlockParser_Closed(BlockProcessor processor, Block block)
        {
            if (block is null)
            {
                return;
            }

            var doc = processor.Document;
            if (doc.GetData(DefaultToCKey) is not string)
            {
                doc.SetData(DefaultToCKey, DefaultToCKey);

                if (block.Parent != null)
                {
                    // Insert before the first heading, or after the first paragraph (whichever
                    // comes first).
                    var index = block.Parent.IndexOf(block);
                    if (block is HeadingBlock)
                    {
                        if (index > 0)
                        {
                            index--;
                        }
                    }
                    else
                    {
                        index++;
                    }
                    block.Parent.Insert(index, new TableOfContentsBlock(null)
                    {
                        IsDefault = true,
                        Depth = Options.DefaultDepth,
                        StartingLevel = Options.DefaultStartingLevel,
                    });
                }
            }
        }

        private void PipelineOnDocumentProcessed(MarkdownDocument document)
        {
            var toCs = document.Descendants<TableOfContentsBlock>().ToList();

            // Mark any default entries as no-ToC when an explicit entry or a no-ToC entry is found,
            // to avoid rendering.
            if (toCs.Any(x => !x.IsDefault || x.IsNoToc))
            {
                foreach (var toC in toCs.Where(x => x.IsDefault))
                {
                    toC.IsNoToc = true;
                }
            }

            foreach (var toC in toCs.Where(x => !x.IsNoToc))
            {
                var levelOffset = toC.Parent.Descendants<HeadingBlock>()
                    .Where(x => x.Line < toC.Line)
                    .OrderByDescending(x => x.Line)
                    .FirstOrDefault()?.Level ?? 0;
                toC.LevelOffset = levelOffset;

                var headings = toC.Parent.Descendants<HeadingBlock>()
                    .Where(x => x.Line > toC.Line)
                    .OrderBy(x => x.Line)
                    .TakeWhile(x => x.Level > levelOffset)
                    .Where(x => x.Level >= levelOffset + toC.StartingLevel
                        && x.Level < levelOffset + toC.StartingLevel + toC.Depth)
                    .ToList();

                if (!toC.IsDefault || headings.Count >= Options.MinimumTopLevel)
                {
                    toC.Headings = headings;
                }
            }
        }
    }
}
