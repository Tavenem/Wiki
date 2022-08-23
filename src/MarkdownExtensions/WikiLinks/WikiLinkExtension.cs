using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// An extension for wiki links.
/// </summary>
public class WikiLinkExtension : IMarkdownExtension
{
    /// <summary>
    /// The <see cref="IDataStore"/> used by this instance.
    /// </summary>
    public IDataStore DataStore { get; }

    /// <summary>
    /// The options for this instance.
    /// </summary>
    public WikiOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="WikiLinkExtension"/>.
    /// </summary>
    public WikiLinkExtension(WikiOptions options, IDataStore dataStore)
    {
        Options = options;
        DataStore = dataStore;
    }

    /// <summary>
    /// Setups this extension for the specified pipeline.
    /// </summary>
    /// <param name="pipeline">The pipeline.</param>
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        if (!pipeline.InlineParsers.Contains<WikiLinkInlineParser>())
        {
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new WikiLinkInlineParser(Options, DataStore));
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
            htmlRenderer.ObjectRenderers.InsertBefore<LinkInlineRenderer>(new WikiLinkInlineRenderer(Options));
        }
    }
}
