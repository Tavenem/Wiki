using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Web;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// An inline renderer for wiki links.
/// </summary>
public class WikiLinkInlineRenderer : HtmlObjectRenderer<WikiLinkInline>
{
    /// <summary>
    /// The options for this instance.
    /// </summary>
    public WikiOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="WikiLinkInlineRenderer"/>.
    /// </summary>
    public WikiLinkInlineRenderer(WikiOptions options) => Options = options;

    /// <summary>
    /// Writes the specified Markdown object to the renderer.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    /// <param name="link">The markdown object.</param>
    protected override void Write(HtmlRenderer renderer, WikiLinkInline link)
    {
        if (!link.IsWikipedia && !link.IsCommons && link.IsCategory && !link.IsEscaped)
        {
            return; // do not render unescaped category links
        }
        var url = link.IsCommons
            || link.IsWikipedia
            ? link.Title
            : link.PageTitle.ToString();

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
                    renderer.Write("<img src=\"./");
                }
                else if (!string.IsNullOrEmpty(link.Title) && link.Title[0] == '#')
                {
                    renderer.Write("<a href=\"");
                }
                else if (string.IsNullOrEmpty(Options.WikiLinkPrefix))
                {
                    renderer.Write("<a href=\"./");
                }
                else
                {
                    renderer.Write($"<a href=\"./{Options.WikiLinkPrefix}/");
                }
                link.GetAttributes().AddClass(link.IsMissing ? "wiki-link-missing" : "wiki-link-exists");
            }
            renderer.WriteEscapeUrl(url);
            if (!string.IsNullOrEmpty(link.Action))
            {
                renderer.Write('/');
                renderer.Write(link.Action);
            }
            if (!string.IsNullOrEmpty(link.Fragment))
            {
                renderer.Write('#');
                renderer.Write(link.Fragment.ToLowerInvariant().Replace(' ', '-'));
            }
            renderer.Write("\"");
            renderer.WriteAttributes(link);

            if (!link.IsWikipedia
                && !link.IsCommons
                && (string.IsNullOrEmpty(link.Title) || link.Title[0] != '#')
                && !string.IsNullOrEmpty(Options.LinkTemplate))
            {
                renderer.Write(" ");
                renderer.Write(Options.LinkTemplate.Replace("{LINK}", HttpUtility.HtmlEncode(url)));
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
                var hasDomain = !string.IsNullOrEmpty(link.PageTitle.Domain);
                if (hasDomain)
                {
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("<span class=\"wiki-link-domain\">");
                    }
                    else
                    {
                        renderer.Write('(');
                    }
                    renderer.Write(link.PageTitle.Domain);
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("</span>");
                    }
                    else
                    {
                        renderer.Write("):");
                    }
                }
                if (!string.IsNullOrEmpty(link.PageTitle.Namespace))
                {
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("<span class=\"wiki-link-namespace\">");
                    }
                    renderer.Write(link.PageTitle.Namespace);
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("</span>");
                    }
                    else
                    {
                        renderer.Write(':');
                    }
                }
                if (!string.IsNullOrEmpty(link.PageTitle.Title))
                {
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("<span class=\"wiki-link-title\">");
                    }
                    renderer.Write(link.PageTitle.Title);
                    if (renderer.EnableHtmlForInline)
                    {
                        renderer.Write("</span>");
                    }
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
