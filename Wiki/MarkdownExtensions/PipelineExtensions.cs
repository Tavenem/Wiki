using Markdig;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions.TableOfContents;
using NeverFoundry.Wiki.MarkdownExtensions.WikiLinks;

namespace NeverFoundry.Wiki.MarkdownExtensions
{
    internal static class PipelineExtensions
    {
        /// <summary>
        /// Adds a table of contents above the first heading, or after the first paragraph
        /// (whichever comes first).
        /// </summary>
        /// <param name="pipeline">The <see cref="MarkdownPipelineBuilder"/>.</param>
        /// <returns>The <see cref="MarkdownPipelineBuilder"/>.</returns>
        internal static MarkdownPipelineBuilder UseTableOfContents(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.ReplaceOrAdd<TableOfContentsExtension>(new TableOfContentsExtension());
            return pipeline;
        }

        /// <summary>
        /// Adds a table of contents above the first heading, or after the first paragraph
        /// (whichever comes first).
        /// </summary>
        /// <param name="pipeline">The <see cref="MarkdownPipelineBuilder"/>.</param>
        /// <param name="options">
        /// The options to set for the <see cref="TableOfContentsExtension"/> instance.
        /// </param>
        /// <returns>The <see cref="MarkdownPipelineBuilder"/>.</returns>
        internal static MarkdownPipelineBuilder UseTableOfContents(this MarkdownPipelineBuilder pipeline, TableOfContentsOptions options)
        {
            pipeline.Extensions.ReplaceOrAdd<TableOfContentsExtension>(new TableOfContentsExtension(options));
            return pipeline;
        }

        /// <summary>
        /// Adds wiki links.
        /// </summary>
        /// <returns>The <see cref="MarkdownPipelineBuilder"/>.</returns>
        internal static MarkdownPipelineBuilder UseWikiLinks(this MarkdownPipelineBuilder pipeline, IWikiOptions options, IDataStore dataStore)
        {
            pipeline.Extensions.ReplaceOrAdd<WikiLinkExtension>(new WikiLinkExtension(options, dataStore));
            return pipeline;
        }
    }
}
