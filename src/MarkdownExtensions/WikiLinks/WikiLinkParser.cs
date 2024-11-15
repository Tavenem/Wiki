using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Text;
using System.Web;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// A parser for <see cref="WikiLink"/> items.
/// </summary>
public static class WikiLinkParser
{
    private const char ActionSeparatorChar = '/';
    private const char LabelSeparatorChar = '|';

    /// <summary>
    /// Populates wiki page info for <see cref="WikiLinkInline"/> instances in the given <paramref
    /// name="document"/>, and gets the list of referenced pages.
    /// </summary>
    /// <returns>
    /// The list of <see cref="WikiLink"/> instances in the document.
    /// </returns>
    public static List<WikiLink>? ReplaceWikiLinks(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle title,
        Page? page,
        MarkdownDocument document)
    {
        List<WikiLink>? wikiLinks = null;
        foreach (var link in document.Descendants<WikiLinkInline>())
        {
            ParseLabel(options, dataStore, title, page, link);
            if (link.PageTitle.HasValue)
            {
                (wikiLinks ??= []).Add(new(
                    link.Page,
                    link.Action,
                    link.Fragment,
                    link.IsCategory,
                    link.IsEscaped,
                    link.IsMissingIgnored,
                    link.PageTitle.Value));
            }
        }

        return wikiLinks;
    }

    private static string? GetUrl(WikiOptions options, WikiLinkInline link)
    {
        if (string.IsNullOrEmpty(link.Title))
        {
            return string.IsNullOrEmpty(link.Fragment)
                ? null
                : $"#{link.Fragment}";
        }

        var renderer = new StringBuilder();
        if (link.IsWikipedia)
        {
            renderer.Append("https://wikipedia.org/wiki/");
            renderer.Append(link.Title);
        }
        else if (link.IsCommons)
        {
            renderer.Append("https://commons.wikimedia.org/wiki/File:");
            renderer.Append(link.Title);
        }
        else
        {
            if (string.IsNullOrEmpty(options.WikiLinkPrefix))
            {
                renderer.Append("./");
            }
            else
            {
                if (options.WikiLinkPrefix[0] == '/')
                {
                    renderer.Append('.');
                }
                else if (!options.WikiLinkPrefix.StartsWith("./"))
                {
                    renderer.Append("./");
                }
                renderer.Append(options.WikiLinkPrefix);
                if (options.WikiLinkPrefix[^1] != '/')
                {
                    renderer.Append('/');
                }
            }
            if (link.PageTitle.HasValue)
            {
                renderer.Append(link.PageTitle.Value.ToString());
            }
            else
            {
                renderer.Append(link.Title);
            }
        }

        if (!string.IsNullOrEmpty(link.Action))
        {
            renderer.Append('/');
            renderer.Append(link.Action);
        }

        if (!string.IsNullOrEmpty(link.Fragment))
        {
            renderer.Append('#');
            renderer.Append(link.Fragment.ToLowerInvariant().Replace(' ', '-'));
        }

        return renderer.ToString();
    }

    private static void ParseLabel(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle currentPageTitle,
        Page? currentPage,
        WikiLinkInline inline)
    {
        var slice = inline.LabelWithTrivia.IsEmpty
            ? inline.Label.AsSpan()
            : inline.LabelWithTrivia.AsSpan();

        var actionSeparatorIndex = slice.IndexOf(ActionSeparatorChar);
        if (actionSeparatorIndex >= 0)
        {
            if (actionSeparatorIndex < slice.Length - 1)
            {
                inline.Action = slice[(actionSeparatorIndex + 1)..].ToString();
            }
            slice = slice[..actionSeparatorIndex];
        }

        string? labelAddendum = null;
        var hasCaseIndicator = false;
        var inlineTitle = inline.Title;
        var labelSeparatorIndex = slice.IndexOf(LabelSeparatorChar);
        if (labelSeparatorIndex >= 0)
        {
            if (slice.Length > labelSeparatorIndex + 1)
            {
                hasCaseIndicator = slice[labelSeparatorIndex + 1] == LabelSeparatorChar;
                if (!hasCaseIndicator || slice.Length > labelSeparatorIndex + 2)
                {
                    labelAddendum = slice[(labelSeparatorIndex + (hasCaseIndicator ? 2 : 1))..].ToString();
                }
            }
            slice = slice[..labelSeparatorIndex];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                    ? null
                    : slice.ToString();
            }
        }

        inline.IsMissingIgnored = !string.IsNullOrEmpty(inline.Action);
        if (!slice.IsEmpty
            && slice[0] == '~')
        {
            inline.IsMissingIgnored = true;
            slice = slice[1..];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                ? null
                : slice.ToString();
            }
        }

        if (!slice.IsEmpty
            && slice[0] == ':')
        {
            inline.IsEscaped = true;
            slice = slice[1..];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                ? null
                : slice.ToString();
            }
        }

        if (slice.Length >= 2
            && slice[0..2].Equals("w:", StringComparison.OrdinalIgnoreCase))
        {
            inline.IsWikipedia = true;
            slice = slice[2..];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                    ? null
                    : slice.ToString();
            }
        }

        if (!inline.IsWikipedia
            && slice.Length >= 3
            && slice[0..3].Equals("cc:", StringComparison.OrdinalIgnoreCase))
        {
            inline.IsCommons = true;
            slice = slice[3..];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                    ? null
                    : slice.ToString();
            }
        }

        // Record and remove fragment from title.
        var fragmentIndex = slice.LastIndexOf('#');
        if (fragmentIndex >= 0)
        {
            if (fragmentIndex == 0)
            {
                inline.IsMissingIgnored = true;
            }
            inline.Fragment = slice[(fragmentIndex + 1)..].ToString();
            slice = slice[..fragmentIndex];
            if (inline.IsShortcut)
            {
                inlineTitle = slice.IsEmpty
                    ? null
                    : slice.ToString();
            }
        }

        var pageTitle = PageTitle.Parse(HtmlHelper.Unescape(slice.ToString()));
        var titlePart = (pageTitle.Title ?? string.Empty).AsSpan();
        var title = pageTitle.ToString();

        inline.HasDisplay = !string.IsNullOrWhiteSpace(inlineTitle)
            && (!inline.IsShortcut
            || !inlineTitle.Equals(title, StringComparison.Ordinal));
        if (inline.HasDisplay)
        {
            inline.Display = inlineTitle;
        }
        else
        {
            var endIndex = titlePart.Length;

            // Remove anything in parenthesis at the end.
            var openParen = titlePart.IndexOf('(');
            if (openParen != -1
                && titlePart[openParen..].TrimEnd()[^1] == ')')
            {
                endIndex = openParen;
            }

            if (inline.Fragment is not null)
            {
                inline.Display = fragmentIndex == 0
                    ? inline.Fragment.ToWikiTitleCase() + labelAddendum
                    : $"{titlePart[..endIndex].Trim()}{labelAddendum} § {inline.Fragment.ToWikiTitleCase()}";
            }
            else
            {
                inline.Display = titlePart[..endIndex].Trim().ToString() + labelAddendum;
            }

            if (hasCaseIndicator)
            {
                inline.Display = inline.Display.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            }

            if (labelSeparatorIndex >= 0)
            {
                inline.HasDisplay = true;
            }
        }

        var isGroupPage = false;
        var isUserPage = false;
        if (inline.IsWikipedia)
        {
            if (!inline.HasDisplay)
            {
                inline.Display = inline.Display?.Replace('_', ' ');
                inline.HasDisplay = !string.IsNullOrEmpty(inline.Display);
            }
        }
        else if (inline.IsCommons)
        {
            if (!inline.HasDisplay && inline.Display is not null)
            {
                var extIndex = inline.Display.IndexOf('.');
                if (extIndex != -1)
                {
                    inline.Display = inline.Display[..extIndex].Replace('_', ' ');
                    inline.HasDisplay = true;
                }
            }
        }
        else
        {
            inline.IsCategory = string.Equals(
                pageTitle.Namespace,
                options.CategoryNamespace,
                StringComparison.OrdinalIgnoreCase);
            if (inline.IsCategory)
            {
                pageTitle = pageTitle.WithNamespace(options.CategoryNamespace); // normalize casing
            }
            else
            {
                isGroupPage = string.Equals(
                    pageTitle.Namespace,
                    options.GroupNamespace,
                    StringComparison.OrdinalIgnoreCase);
                if (isGroupPage)
                {
                    pageTitle = pageTitle.WithNamespace(options.GroupNamespace);
                }
                else
                {
                    isUserPage = string.Equals(
                        pageTitle.Namespace,
                        options.UserNamespace,
                        StringComparison.OrdinalIgnoreCase);
                    if (isUserPage)
                    {
                        pageTitle = pageTitle.WithNamespace(options.UserNamespace);
                    }
                }

                inline.Fragment = inline.Fragment?.ToLowerInvariant().Replace(' ', '-');
            }
        }

        inline.Title = title;

        if (!inline.IsWikipedia
            && !inline.IsCommons)
        {
            inline.PageTitle = pageTitle;

            if (!inline.IsCategory
                && !isGroupPage
                && !isUserPage)
            {
                if (pageTitle.Equals(currentPageTitle))
                {
                    if (currentPage is null)
                    {
                        var id = IPage<Page>.GetId(pageTitle);
                        currentPage = dataStore.GetItem(id, WikiJsonSerializerContext.Default.Page);
                    }
                    inline.Page = currentPage;
                }
                else
                {
                    var id = IPage<Page>.GetId(pageTitle);
                    inline.Page = dataStore.GetItem(id, WikiJsonSerializerContext.Default.Page);
                    if (!inline.IsMissingIgnored)
                    {
                        inline.IsMissing = inline.Page?.Revision?.IsDeleted != false;
                    }
                }
            }
        }

        inline.Url = GetUrl(options, inline);

        if (!inline.IsWikipedia && !inline.IsCommons)
        {
            inline.GetAttributes().AddClass(inline.IsMissing ? "wiki-link-missing" : "wiki-link-exists");

            if ((string.IsNullOrEmpty(inline.Url)
                || inline.Url[0] != '#')
                && !string.IsNullOrEmpty(options.LinkTemplate))
            {
                inline.LinkTemplate = options.LinkTemplate.Replace("{LINK}", HttpUtility.HtmlEncode(inline.Url));
            }
        }
    }
}
