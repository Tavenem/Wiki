using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;

namespace NeverFoundry.Wiki.MarkdownExtensions.WikiLinks
{
    public class WikiLinkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<WikiLinkInlineParser>())
            {
                pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new WikiLinkInlineParser());
            }
        }

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
