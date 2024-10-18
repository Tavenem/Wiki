using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

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
        if (!link.IsWikipedia
            && !link.IsCommons
            && link.IsCategory
            && !link.IsEscaped)
        {
            return; // do not render unescaped category links
        }

        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("<a href=\"");
            renderer.WriteEscapeUrl(link.Url);
            renderer.Write("\"");
            renderer.WriteAttributes(link);

            if (!string.IsNullOrEmpty(link.LinkTemplate))
            {
                renderer.Write(" ");
                renderer.Write(link.LinkTemplate);
            }
        }

        if (link.IsImage)
        {
            WriteImage(renderer, link);
            return;
        }

        if (renderer.EnableHtmlForInline)
        {
            renderer.Write(">");
        }

        if (link.HasDisplay && !string.IsNullOrEmpty(link.Display))
        {
            renderer.Write(link.Display);
        }
        else if (link.PageTitle.HasValue)
        {
            if (!string.IsNullOrEmpty(link.PageTitle.Value.Domain))
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("<span class=\"wiki-link-domain\">");
                }
                else
                {
                    renderer.Write('(');
                }
                renderer.Write(link.PageTitle.Value.Domain);
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("</span>");
                }
                else
                {
                    renderer.Write("):");
                }
            }
            if (!string.IsNullOrEmpty(link.PageTitle.Value.Namespace))
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("<span class=\"wiki-link-namespace\">");
                }
                renderer.Write(link.PageTitle.Value.Namespace);
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("</span>");
                }
                else
                {
                    renderer.Write(':');
                }
            }
            if (!string.IsNullOrEmpty(link.PageTitle.Value.Title))
            {
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("<span class=\"wiki-link-title\">");
                }
                renderer.Write(link.PageTitle.Value.Title);
                if (renderer.EnableHtmlForInline)
                {
                    renderer.Write("</span>");
                }
            }
        }
        else
        {
            renderer.WriteChildren(link);
        }
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write("</a>");
        }
    }

    private static void WriteImage(HtmlRenderer renderer, WikiLinkInline link)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.Write(" target=\"_blank\"");

            if (link.IsCommons)
            {
                renderer.Write("><img src=\"https://commons.wikimedia.org/wiki/Special:Redirect/file/File:");
                renderer.WriteEscapeUrl(link.Title);
                renderer.Write("\" alt=\"");
            }
            else
            {
                renderer.Write("><img src=\"");
                renderer.WriteEscapeUrl(link.Url);
                renderer.Write("\" alt=\"");
            }
        }
        var wasEnableHtmlForInline = renderer.EnableHtmlForInline;
        renderer.EnableHtmlForInline = false;
        renderer.WriteChildren(link);
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
            renderer.Write(" /></a>");
        }
    }
}
