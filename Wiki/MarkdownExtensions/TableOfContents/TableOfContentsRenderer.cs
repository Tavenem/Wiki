using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeverFoundry.Wiki.MarkdownExtensions.TableOfContents
{
    /// <summary>
    /// Renderer for <see cref="TableOfContentsBlock"/>.
    /// </summary>
    public class TableOfContentsRenderer : HtmlObjectRenderer<TableOfContentsBlock>
    {
        private const string Tab = "   ";

        /// <summary>
        /// Writes the specified Markdown object to the renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="block">The markdown object.</param>
        protected override void Write(HtmlRenderer renderer, TableOfContentsBlock block)
        {
            if (block.Headings is null || block.Headings.Count == 0)
            {
                return;
            }

            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("<div class=\"toc\" role=\"navigation\">");
                renderer.PushIndent(Tab);
                renderer.Write("<h2 class=\"toc-title\">");
            }
            renderer.Write(block.Title);
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</h2>");
                renderer.Write("<ul>");
                renderer.PushIndent(Tab);
            }

            var ords = new List<int>(0);
            var ordLevel = 0;
            var headings = block.Headings.OrderBy(x => x.Line).ToList();
            var childList = 0;
            for (var i = 0; i < headings.Count; i++)
            {
                var adjustedLevel = headings[i].Level - block.LevelOffset - (block.StartingLevel - 1);

                renderer.EnsureLine();

                while (ordLevel > adjustedLevel)
                {
                    ords.RemoveAt(ords.Count - 1);
                    ordLevel--;
                }
                if (ordLevel == adjustedLevel)
                {
                    var ord = ords[^1] + 1;
                    ords.RemoveAt(ords.Count - 1);
                    ords.Add(ord);
                }
                while (ordLevel < adjustedLevel)
                {
                    ords.Add(1);
                    ordLevel++;
                }

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("<li>");
                }

                string? id = null;
                var attr = headings[i].TryGetAttributes();
                if (attr != null)
                {
                    id = attr.Id;
                }

                if (renderer.EnableHtmlForBlock)
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        renderer.Write("<a href=\"#");
                        renderer.WriteEscapeUrl(id);
                        renderer.Write("\">");
                    }
                    renderer.Write("<span class='toc-number'>");
                }
                renderer.Write(ords[0].ToString());
                for (var j = 1; j < ords.Count; j++)
                {
                    renderer.Write(".");
                    renderer.Write(ords[j].ToString());
                }
                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("</span><span class=\"toc-heading\">");
                }

                string headingText;
                using (var sw = new StringWriter())
                {
                    var stripRenderer = new HtmlRenderer(sw);
                    stripRenderer.Render(headings[i].Inline);
                    headingText = stripRenderer.Writer.ToString() ?? string.Empty;
                }

                renderer.Write(headingText);

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.Write("</span>");
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        renderer.Write("</a>");
                    }
                }
                if (i < headings.Count - 1
                    && headings[i + 1].Level - block.LevelOffset > adjustedLevel)
                {
                    childList++;
                    renderer.EnsureLine();
                    renderer.PushIndent(Tab);
                    if (renderer.EnableHtmlForBlock)
                    {
                        renderer.Write("<ul>");
                        renderer.PushIndent(Tab);
                    }
                }
                else
                {
                    if (renderer.EnableHtmlForBlock)
                    {
                        renderer.Write("</li>");
                    }
                }
                var n = i + 1;
                while (childList > 0
                    && (n > headings.Count - 1
                    || headings[n].Level - block.LevelOffset < adjustedLevel))
                {
                    renderer.EnsureLine();
                    renderer.PopIndent();
                    if (renderer.EnableHtmlForBlock)
                    {
                        renderer.Write("</ul>");
                    }
                    renderer.EnsureLine();
                    renderer.PopIndent();
                    if (renderer.EnableHtmlForBlock)
                    {
                        renderer.Write("</li>");
                    }
                    childList--;
                }
            }

            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.PopIndent();
                renderer.WriteLine("</ul>");
                renderer.PopIndent();
                renderer.WriteLine("</div>");
            }
        }
    }
}
