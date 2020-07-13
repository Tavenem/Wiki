using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Web;

namespace NeverFoundry.Wiki.MarkdownExtensions.WikiLinks
{
    /// <summary>
    /// An inline renderer for wiki links.
    /// </summary>
    public class WikiLinkInlineRenderer : HtmlObjectRenderer<WikiLinkInline>
    {
        /// <summary>
        /// Writes the specified Markdown object to the renderer.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="link">The markdown object.</param>
        protected override void Write(HtmlRenderer renderer, WikiLinkInline link)
        {
            if (!link.IsWikipedia && !link.IsCommons && link.IsCategory && !link.IsNamespaceEscaped)
            {
                return; // do not render unescaped category links
            }
            var fullTitle = !link.IsCommons && !link.IsWikipedia
                ? Article.GetFullTitle(link.Title, link.WikiNamespace, link.IsTalk)
                : link.Title;

            if (renderer.EnableHtmlForInline)
            {
                if (link.IsWikipedia)
                {
                    renderer.Write("<a href=\"http://wikipedia.org/wiki/");
                }
                else if (link.IsCommons)
                {
                    renderer.Write("<a href=\"https://commons.wikimedia.org/wiki/File:");
                }
                else
                {
                    renderer.Write(link.IsImage ? "<img src=\"/" : "<a href=\"Wiki/");
                    link.GetAttributes().AddClass(link.Missing ? "wiki-link-missing" : "wiki-link-exists");
                }
                renderer.WriteEscapeUrl(fullTitle);
                renderer.Write("\"");
                renderer.WriteAttributes(link);

                if (!link.IsWikipedia && !link.IsCommons && !string.IsNullOrEmpty(WikiConfig.LinkTemplate))
                {
                    renderer.Write(" ");
                    renderer.Write(WikiConfig.LinkTemplate.Replace("{LINK}", HttpUtility.HtmlEncode(fullTitle)));
                }
            }
            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    if (link.IsWikipedia)
                    {
                        renderer.Write("><img src=\"http://wikipedia.org/wiki/");
                        renderer.WriteEscapeUrl(link.Title);
                        renderer.Write("\" alt=\"");
                    }
                    else if (link.IsCommons)
                    {
                        renderer.Write("><img src=\"https://commons.wikimedia.org/wiki/Special:Redirect/file/");
                        renderer.WriteEscapeUrl(link.Title);
                        renderer.Write("\" alt=\"");
                    }
                    else
                    {
                        renderer.Write(" alt=\"");
                    }
                }
                var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
                renderer.EnableHtmlForInline = false;
                renderer.WriteChildren(link);
                if (!string.IsNullOrEmpty(link.Endmatter))
                {
                    renderer.Write(link.Endmatter);
                }
                renderer.EnableHtmlForInline = wasEnableHtmlForInline;
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("\"");
                }
            }

            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write(" />");
                    if (link.IsWikipedia || link.IsCommons)
                    {
                        renderer.Write("</a>");
                    }
                }
            }
            else
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write(">");
                }
                if (!string.IsNullOrEmpty(link.Display) && link.HasDisplay)
                {
                    renderer.Write(link.Display);
                }
                else
                {
                    if (!string.Equals(link.WikiNamespace, WikiConfig.DefaultNamespace, StringComparison.OrdinalIgnoreCase))
                    {
                        if (renderer.EnableHtmlForInline)
                        {
                            renderer.Write("<span class=\"wiki-link-namespace\">");
                        }
                        renderer.Write(link.WikiNamespace);
                        if (renderer.EnableHtmlForInline)
                        {
                            renderer.Write("</span>");
                        }
                    }
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("<span class=\"wiki-link-title\">");
                    }
                    renderer.Write(link.Title);
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("</span>");
                    }
                    //renderer.WriteChildren(link);
                }
                if (!string.IsNullOrEmpty(link.Endmatter))
                {
                    renderer.Write(link.Endmatter);
                }
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("</a>");
                }
            }
        }
    }
}
