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
            var title = link.Title;
            var isWikipedia = link.IsWikipedia;
            var isCommons = link.IsCommons;
            var articleExists = false;

            if (!isWikipedia && !isCommons)
            {
                if (link.IsCategory)
                {
                    if (!link.IsNamespaceEscaped)
                    {
                        return; // do not render unescaped category links
                    }
                    articleExists = Category.GetCategory(title)?.IsDeleted == false;
                }
                else if (string.Equals(link.WikiNamespace, WikiConfig.FileNamespace, StringComparison.CurrentCultureIgnoreCase))
                {
                    articleExists = WikiFile.GetFile(title)?.IsDeleted == false;
                }
                else
                {
                    articleExists = Article.GetArticle(title, link.WikiNamespace)?.IsDeleted == false;
                }
                title = Article.GetFullTitle(title, link.WikiNamespace, link.IsTalk);
            }

            if (renderer.EnableHtmlForInline)
            {
                if (isWikipedia)
                {
                    renderer.Write("<a href=\"http://wikipedia.org/wiki/");
                }
                else if (isCommons)
                {
                    renderer.Write("<a href=\"https://commons.wikimedia.org/wiki/File:");
                }
                else
                {
                    renderer.Write(link.IsImage ? "<img src=\"/" : "<a href=\"Wiki/");
                    link.GetAttributes().AddClass(articleExists ? "wiki-link-exists" : "wiki-link-missing");
                }
                renderer.WriteEscapeUrl(title);
                renderer.Write("\"");
                renderer.WriteAttributes(link);

                if (!isWikipedia && !isCommons && !string.IsNullOrEmpty(WikiConfig.LinkTemplate))
                {
                    renderer.Write(" ");
                    renderer.Write(WikiConfig.LinkTemplate.Replace("{LINK}", HttpUtility.HtmlEncode(title)));
                }
            }
            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    if (isWikipedia)
                    {
                        renderer.Write("><img src=\"http://wikipedia.org/wiki/");
                        renderer.WriteEscapeUrl(title);
                        renderer.Write("\" alt=\"");
                    }
                    else if (isCommons)
                    {
                        renderer.Write("><img src=\"https://commons.wikimedia.org/wiki/Special:Redirect/file/");
                        renderer.WriteEscapeUrl(title);
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
                    if (isWikipedia || isCommons)
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
                    renderer.Write(title);
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
