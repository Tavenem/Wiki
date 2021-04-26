using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Web;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks
{
    /// <summary>
    /// An inline renderer for wiki links.
    /// </summary>
    public class WikiLinkInlineRenderer : HtmlObjectRenderer<WikiLinkInline>
    {
        /// <summary>
        /// The options for this instance.
        /// </summary>
        public IWikiOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiLinkInlineRenderer"/>.
        /// </summary>
        public WikiLinkInlineRenderer(IWikiOptions options) => Options = options;

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
            var fullTitle = !link.IsCommons
                && !link.IsWikipedia
                && (string.IsNullOrEmpty(link.Title) || link.Title[0] != '#')
                ? Article.GetFullTitle(Options, link.Title ?? string.Empty, link.WikiNamespace, link.IsTalk)
                : link.Title;

            if (renderer.EnableHtmlForInline)
            {
                if (link.IsWikipedia)
                {
                    renderer.Write("<a href=\"https://wikipedia.org/wiki/");
                }
                else if (link.IsCommons)
                {
                    renderer.Write("<a href=\"https://commons.wikimedia.org/wiki/File:");
                }
                else
                {
                    if (link.IsImage)
                    {
                        renderer.Write("<img src=\"/");
                    }
                    else if (!string.IsNullOrEmpty(link.Title) && link.Title[0] == '#')
                    {
                        renderer.Write("<a href=\"");
                    }
                    else
                    {
                        renderer.Write($"<a href=\"/{Options.WikiLinkPrefix}/");
                    }
                    link.GetAttributes().AddClass(link.Missing ? "wiki-link-missing" : "wiki-link-exists");
                }
                renderer.WriteEscapeUrl(fullTitle);
                renderer.Write("\"");
                renderer.WriteAttributes(link);

                if (!link.IsWikipedia
                    && !link.IsCommons
                    && (string.IsNullOrEmpty(link.Title) || link.Title[0] != '#')
                    && !string.IsNullOrEmpty(Options.LinkTemplate))
                {
                    renderer.Write(" ");
                    renderer.Write(Options.LinkTemplate.Replace("{LINK}", HttpUtility.HtmlEncode(fullTitle)));
                }
            }
            if (link.IsImage)
            {
                if (renderer.EnableHtmlForInline)
                {
                    if (link.IsWikipedia)
                    {
                        renderer.Write("><img src=\"https://wikipedia.org/wiki/");
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

                var properties = link.GetAttributes()?.Properties;
                var heightIndex = properties?.FindIndex(x => x.Key.Equals("height", StringComparison.OrdinalIgnoreCase)) ?? -1;
                if (heightIndex != -1)
                {
                    if (int.TryParse(properties![heightIndex].Value, out var heightInt))
                    {
                        renderer.Write("height=\"");
                        renderer.Write(heightInt.ToString());
                        renderer.Write("\"");
                    }
                    else if (double.TryParse(properties[heightIndex].Value, out var heightFloat))
                    {
                        renderer.Write("height=\"");
                        renderer.Write(heightFloat.ToString());
                        renderer.Write("\"");
                    }
                }

                var widthIndex = properties?.FindIndex(x => x.Key.Equals("width", StringComparison.OrdinalIgnoreCase)) ?? -1;
                if (widthIndex != -1)
                {
                    if (int.TryParse(properties![widthIndex].Value, out var widthInt))
                    {
                        renderer.Write("width=\"");
                        renderer.Write(widthInt.ToString());
                        renderer.Write("\"");
                    }
                    else if (double.TryParse(properties[widthIndex].Value, out var widthFloat))
                    {
                        renderer.Write("width=\"");
                        renderer.Write(widthFloat.ToString());
                        renderer.Write("\"");
                    }
                }

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
                    if (!string.IsNullOrEmpty(link.WikiNamespace)
                        && !string.Equals(link.WikiNamespace, Options.DefaultNamespace, StringComparison.OrdinalIgnoreCase))
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
