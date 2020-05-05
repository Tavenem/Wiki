using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeverFoundry.Wiki.MarkdownExtensions.HeadingIds
{
    internal class HeadingIdExtension : IMarkdownExtension
    {
        private const string HeadingIdKey = "HeadingId";

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
            if (headingBlockParser != null)
            {
                headingBlockParser.Closed -= HeadingBlockParser_Closed;
                headingBlockParser.Closed += HeadingBlockParser_Closed;
            }
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
            if (paragraphBlockParser != null)
            {
                paragraphBlockParser.Closed -= HeadingBlockParser_Closed;
                paragraphBlockParser.Closed += HeadingBlockParser_Closed;
            }
        }

        public void Setup(MarkdownPipeline _, IMarkdownRenderer __) { }

        private void HeadingBlockParser_Closed(BlockProcessor processor, Block block)
        {
            if (!(block is HeadingBlock headingBlock))
            {
                return;
            }

            var headingLine = headingBlock.Lines.Lines[0];

            var text = headingLine.ToString();

            var linkRef = new HeadingLinkReferenceDefinition()
            {
                HeadingBlock = headingBlock,
                CreateLinkInline = CreateLinkInlineForHeading
            };

            var doc = processor.Document;
            if (!(doc.GetData(this) is Dictionary<string, HeadingLinkReferenceDefinition> dictionary))
            {
                dictionary = new Dictionary<string, HeadingLinkReferenceDefinition>();
                doc.SetData(this, dictionary);
                doc.ProcessInlinesBegin += Document_ProcessInlinesBegin;
            }
            dictionary[text] = linkRef;

            headingBlock.ProcessInlinesEnd += HeadingBlock_ProcessInlinesEnd;
        }

        private void Document_ProcessInlinesBegin(InlineProcessor processor, Inline inline)
        {
            var doc = processor.Document;
            doc.ProcessInlinesBegin -= Document_ProcessInlinesBegin;
            foreach (var keyPair in (Dictionary<string, HeadingLinkReferenceDefinition>)doc.GetData(this))
            {
                if (!doc.TryGetLinkReferenceDefinition(keyPair.Key, out var linkDef))
                {
                    doc.SetLinkReferenceDefinition(keyPair.Key, keyPair.Value);
                }
            }
            doc.RemoveData(this);
        }

        private Inline CreateLinkInlineForHeading(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline child)
        {
            var headingRef = (HeadingLinkReferenceDefinition)linkRef;
            return new LinkInline()
            {
                GetDynamicUrl = () => HtmlHelper.Unescape("#" + headingRef.HeadingBlock?.GetAttributes().Id),
                Title = HtmlHelper.Unescape(linkRef.Title),
            };
        }

        private void HeadingBlock_ProcessInlinesEnd(InlineProcessor processor, Inline inline)
        {
            if (!(processor.Document.GetData(HeadingIdKey) is HashSet<string> identifiers))
            {
                identifiers = new HashSet<string>();
                processor.Document.SetData(HeadingIdKey, identifiers);
            }

            var headingBlock = (HeadingBlock)processor.Block;
            if (headingBlock.Inline == null)
            {
                return;
            }

            var attributes = processor.Block.GetAttributes();
            if (attributes.Id != null)
            {
                return;
            }

            string headingText;
            using (var sw = new StringWriter())
            {
                var stripRenderer = new HtmlRenderer(sw);
                stripRenderer.Render(headingBlock.Inline);
                headingText = stripRenderer.Writer.ToString();
            }

            var baseHeadingId = string.IsNullOrEmpty(headingText) ? "section" : headingText;

            var index = 0;
            var headingId = baseHeadingId;
            var headingBuffer = new StringBuilder();
            while (!identifiers.Add(headingId))
            {
                index++;
                headingBuffer.Append(baseHeadingId);
                headingBuffer.Append('-');
                headingBuffer.Append(index);
                headingId = headingBuffer.ToString();
                headingBuffer.Length = 0;
            }

            attributes.Id = headingId;
        }
    }
}
