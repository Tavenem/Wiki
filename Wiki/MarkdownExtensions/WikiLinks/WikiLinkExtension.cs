using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace NeverFoundry.Wiki.MarkdownExtensions.WikiLinks
{
    /// <summary>
    /// An extension for wiki links.
    /// </summary>
    public class WikiLinkExtension : IMarkdownExtension
    {
        /// <summary>
        /// Setups this extension for the specified pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<WikiLinkInlineParser>())
            {
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new WikiLinkInlineParser());
            }
        }

        /// <summary>
        /// Setups this extension for the specified renderer.
        /// </summary>
        /// <param name="pipeline">The pipeline used to parse the document.</param>
        /// <param name="renderer">The renderer.</param>
        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer
                && !htmlRenderer.ObjectRenderers.Contains<WikiLinkInlineRenderer>())
            {
                htmlRenderer.ObjectRenderers.InsertBefore<LinkInlineRenderer>(new WikiLinkInlineRenderer());
            }
        }
    }
}
