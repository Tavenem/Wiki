using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Text;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// An inline parser for <see cref="WikiLink"/> items.
/// </summary>
public class WikiLinkInlineParser : InlineParser
{
    private const char ActionSeparatorChar = '/';

    internal const char LinkCloseChar = ']';
    internal const char LinkOpenChar = '[';

    private const char SeparatorChar = '|';

    /// <summary>
    /// The <see cref="IDataStore"/> used by this instance.
    /// </summary>
    public IDataStore DataStore { get; }

    /// <summary>
    /// The options for this instance.
    /// </summary>
    public WikiOptions Options { get; }

    /// <summary>
    /// The title of the page being parsed/rendered.
    /// </summary>
    public PageTitle Title { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="WikiLinkInlineParser"/>.
    /// </summary>
    public WikiLinkInlineParser(WikiOptions options, IDataStore dataStore, PageTitle title)
    {
        DataStore = dataStore;
        OpeningCharacters = [LinkOpenChar, LinkCloseChar, '!'];
        Options = options;
        Title = title;
    }

    /// <summary>
    /// Tries to match the specified slice.
    /// </summary>
    /// <param name="processor">The parser processor.</param>
    /// <param name="slice">The text slice.</param>
    /// <returns><c>true</c> if this parser found a match; <c>false</c> otherwise</returns>
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        var c = slice.CurrentChar;
        var startPosition = processor.GetSourcePosition(slice.Start, out var line, out var column);

        var isImage = false;
        if (c == '!')
        {
            isImage = true;
            c = slice.NextChar();
            if (c != LinkOpenChar)
            {
                return false;
            }
        }

        switch (c)
        {
            case LinkOpenChar:
                if (slice.PeekChar() != LinkOpenChar)
                {
                    return false;
                }
                var saved = slice;
                var currentPosition = slice.Start;
                if (TryParseLink(
                    ref slice,
                    out var title,
                    out var display,
                    out var action,
                    out var hasDisplay,
                    out var autoDisplay,
                    out var endPosition))
                {
                    processor.Inline = new WikiLinkDelimiterInline(this)
                    {
                        Type = DelimiterType.Open,
                        Action = action,
                        AutoDisplay = autoDisplay,
                        Display = display,
                        HasDisplay = hasDisplay,
                        Title = title,
                        IsImage = isImage,
                        Span = new SourceSpan(startPosition, processor.GetSourcePosition(slice.Start - 1)),
                        Line = line,
                        Column = column,
                    };
                    slice = saved;
                    if (endPosition == currentPosition)
                    {
                        slice.NextChar();
                        slice.NextChar();
                    }
                    else
                    {
                        slice.Start = endPosition;
                        slice.NextChar();
                    }
                }
                else
                {
                    slice = saved;
                    return false;
                }
                return true;
            case LinkCloseChar:
                if (slice.PeekChar() != LinkCloseChar)
                {
                    return false;
                }
                slice.NextChar();
                slice.NextChar();
                return processor.Inline != null && TryProcessLinkOrImage(processor, ref slice);
        }

        return false;
    }

    private static string? ParseEndmatter(ref StringSlice lines)
    {
        string? endmatter = null;
        var buffer = new StringBuilder();

        var c = lines.CurrentChar;
        while (true)
        {
            if (!c.IsAlphaNumeric())
            {
                if (buffer.Length is > 0 and <= 999)
                {
                    endmatter = buffer.ToString();
                }
                break;
            }

            buffer.Append(c);
            c = lines.NextChar();
        }

        buffer.Length = 0;

        return endmatter;
    }

    private static bool TryParseLink(
        ref StringSlice lines,
        out string? title,
        out string? display,
        out string? action,
        out bool hasDisplay,
        out bool autoDisplay,
        out int endPosition)
    {
        lines.NextChar(); // skip second opening char, which has already been confirmed

        endPosition = lines.Start;
        autoDisplay = false;
        hasDisplay = false;
        title = null;
        display = null;
        action = null;
        var buffer = new StringBuilder();

        char c;
        var hasEscape = false;
        var previousWhitespace = true;
        var hasNonWhitespace = false;
        var hasSeparator = false;
        var hasActionSeparator = false;
        var hasDoubleSeparator = false;
        var isValid = false;
        while (true)
        {
            c = lines.NextChar();
            if (c == '\0')
            {
                break;
            }

            if (hasEscape)
            {
                if (c is not LinkOpenChar
                    and not LinkCloseChar
                    and not SeparatorChar
                    and not ActionSeparatorChar
                    and not '\\')
                {
                    break;
                }
            }
            else
            {
                if (c == LinkOpenChar)
                {
                    break;
                }

                if (c == SeparatorChar)
                {
                    if (!hasNonWhitespace)
                    {
                        if (hasSeparator)
                        {
                            hasDoubleSeparator = true;
                            endPosition = lines.Start;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!hasSeparator)
                    {
                        endPosition = lines.Start;
                    }
                    hasSeparator = true;
                    for (var i = buffer.Length - 1; i >= 0; i--)
                    {
                        if (!buffer[i].IsWhitespace())
                        {
                            break;
                        }
                        buffer.Length = i;
                    }

                    if (buffer.Length <= 999)
                    {
                        if (hasActionSeparator)
                        {
                            action = buffer.ToString();
                        }
                        else
                        {
                            title = buffer.ToString();
                        }
                        buffer.Length = 0;
                        hasNonWhitespace = false;
                        previousWhitespace = true;
                        isValid = true;
                    }

                    continue;
                }

                if (c == ActionSeparatorChar
                    && (buffer.Length == 0
                    || buffer[^1] != '<'))
                {
                    if (!hasNonWhitespace)
                    {
                        if (hasActionSeparator)
                        {
                            endPosition = lines.Start;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!hasActionSeparator)
                    {
                        endPosition = lines.Start;
                    }
                    hasActionSeparator = true;
                    for (var i = buffer.Length - 1; i >= 0; i--)
                    {
                        if (!buffer[i].IsWhitespace())
                        {
                            break;
                        }
                        buffer.Length = i;
                    }

                    if (buffer.Length <= 999)
                    {
                        title = buffer.ToString();
                        buffer.Length = 0;
                        hasNonWhitespace = false;
                        previousWhitespace = true;
                        isValid = true;
                    }

                    continue;
                }

                if (c == LinkCloseChar && lines.PeekChar() == LinkCloseChar)
                {
                    lines.NextChar();
                    lines.NextChar();
                    if (hasNonWhitespace)
                    {
                        for (var i = buffer.Length - 1; i >= 0; i--)
                        {
                            if (!buffer[i].IsWhitespace())
                            {
                                break;
                            }
                            buffer.Length = i;
                        }

                        if (buffer.Length <= 999)
                        {
                            if (hasSeparator)
                            {
                                display = buffer.ToString();
                            }
                            else if (hasActionSeparator)
                            {
                                action = buffer.ToString();
                            }
                            else
                            {
                                title = buffer.ToString();
                            }
                            isValid = true;
                        }
                    }
                    break;
                }
            }

            var isWhitespace = c.IsWhitespace();
            if (isWhitespace)
            {
                c = ' ';
            }

            if (!hasEscape && c == '\\')
            {
                hasEscape = true;
            }
            else
            {
                hasEscape = false;

                if (!previousWhitespace || !isWhitespace)
                {
                    if (hasDoubleSeparator)
                    {
                        hasDoubleSeparator = false;
                    }
                    buffer.Append(c);
                    if (!isWhitespace)
                    {
                        hasNonWhitespace = true;
                    }
                }
            }
            previousWhitespace = isWhitespace;
        }

        if (!isValid)
        {
            return false;
        }

        if (!hasSeparator)
        {
            return true;
        }

        if (hasNonWhitespace)
        {
            hasDisplay = true;
            return true;
        }

        if (string.IsNullOrEmpty(title))
        {
            return true;
        }

        autoDisplay = true;
        hasDisplay = true;

        // Remove domain and namespace.
        var pageTitle = PageTitle.Parse(title);
        title = pageTitle.ToString(); // Normalize casing.
        var titlePart = pageTitle.Title ?? string.Empty;
        var endIndex = titlePart.Length;

        // Remove fragment.
        var fragmentIndex = titlePart.LastIndexOf('#');
        if (fragmentIndex != -1)
        {
            endIndex = fragmentIndex;
        }

        // Remove anything in parenthesis at the end.
        var openParen = titlePart.IndexOf('(');
        if (openParen != -1
            && openParen < endIndex
            && titlePart.TrimEnd()[^1] == ')')
        {
            endIndex = openParen;
        }

        display = titlePart[..endIndex].Trim();

        if (fragmentIndex >= 0)
        {
            var fragment = openParen == -1 || openParen < fragmentIndex
                ? titlePart[(fragmentIndex + 1)..]
                : titlePart[(fragmentIndex + 1)..openParen];
            display = fragmentIndex == 0
                ? fragment
                : $"{display} § {fragment}";
        }

        if (hasDoubleSeparator)
        {
            display = display.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        }

        return true;
    }

    private bool TryProcessLinkOrImage(InlineProcessor inlineState, ref StringSlice text)
    {
        var openParent = inlineState.Inline?.FindParentOfType<WikiLinkDelimiterInline>().FirstOrDefault();
        if (openParent is null)
        {
            return false;
        }

        if (!openParent.IsActive)
        {
            inlineState.Inline = new LiteralInline()
            {
                Content = new StringSlice("[["),
                Span = openParent.Span,
                Line = openParent.Line,
                Column = openParent.Column,
            };
            openParent.ReplaceBy(inlineState.Inline);
            return false;
        }

        var parentDelimiter = openParent.Parent;
        var savedText = text;

        string? endmatter = null;
        if (!openParent.HasDisplay || openParent.AutoDisplay)
        {
            endmatter = ParseEndmatter(ref text);
            text = savedText;
            if (endmatter?.Length > 0)
            {
                text.Start += endmatter.Length;
            }
        }

        var title = openParent.Title;
        var display = openParent.Display;
        string? fragment = null;
        var isWikipedia = title?.StartsWith("w:", StringComparison.OrdinalIgnoreCase) == true;
        var isCommons = !isWikipedia
            && title?.StartsWith("cc:", StringComparison.OrdinalIgnoreCase) == true;
        var isCategory = false;
        var isGroupPage = false;
        var isUserPage = false;
        var isEscaped = false;
        var ignoreMissing = !string.IsNullOrEmpty(openParent.Action);
        var pageTitle = new PageTitle();

        if (isWikipedia)
        {
            title = title![2..];
            if (!openParent.HasDisplay)
            {
                display = display?[2..].Replace('_', ' ');
                openParent.HasDisplay = true;
            }
            else if (openParent.AutoDisplay)
            {
                display = display?.Replace('_', ' ');
            }
        }
        else if (isCommons)
        {
            title = title![3..];

            if (!openParent.HasDisplay)
            {
                if (display is not null)
                {
                    var extIndex = display.IndexOf('.');
                    {
                        if (extIndex != -1)
                        {
                            display = display[3..extIndex].Replace('_', ' ');
                            openParent.HasDisplay = true;
                        }
                    }
                }
            }
            else if (openParent.AutoDisplay)
            {
                display = display?.Replace('_', ' ');
            }
        }
        else if (!string.IsNullOrWhiteSpace(title))
        {
            if (title[0] == '~')
            {
                ignoreMissing = true;
                title = title[1..];
            }

            var fragmentIndex = title.IndexOf('#');
            if (fragmentIndex >= 0)
            {
                fragment = title[(fragmentIndex + 1)..];
            }
            if (fragmentIndex == 0)
            {
                ignoreMissing = true;
                if (string.IsNullOrEmpty(display))
                {
                    display = title[1..];
                    openParent.HasDisplay = true;
                }
            }
            else
            {
                isEscaped = title.StartsWith(':');
                if (isEscaped)
                {
                    title = title[1..];
                }

                var mainTitle = title;
                if (fragmentIndex != -1)
                {
                    mainTitle = title[..fragmentIndex];
                }

                pageTitle = PageTitle.Parse(HtmlHelper.Unescape(mainTitle));

                isCategory = string.Equals(
                    pageTitle.Namespace,
                    Options.CategoryNamespace,
                    StringComparison.OrdinalIgnoreCase);
                if (isCategory)
                {
                    pageTitle = pageTitle.WithNamespace(Options.CategoryNamespace); // normalize casing
                }
                else
                {
                    isGroupPage = string.Equals(
                        pageTitle.Namespace,
                        Options.GroupNamespace,
                        StringComparison.OrdinalIgnoreCase);
                    if (isGroupPage)
                    {
                        pageTitle = pageTitle.WithNamespace(Options.GroupNamespace);
                    }
                    else
                    {
                        isUserPage = string.Equals(
                            pageTitle.Namespace,
                            Options.UserNamespace,
                            StringComparison.OrdinalIgnoreCase);
                        if (isUserPage)
                        {
                            pageTitle = pageTitle.WithNamespace(Options.UserNamespace);
                        }
                    }
                }

                title = fragmentIndex == -1 || fragmentIndex >= title.Length - 1
                    ? pageTitle.Title
                    : pageTitle.Title + title[fragmentIndex..].ToLowerInvariant().Replace(' ', '-');
            }
        }

        Page? page = null;
        var pageMissing = false;
        if (!isCategory
            && !isGroupPage
            && !isUserPage
            && !isWikipedia
            && !isCommons)
        {
            var id = IPage<Page>.GetId(pageTitle);
            page = DataStore.GetItem(id, WikiJsonSerializerContext.Default.Page);
            if (!ignoreMissing && !pageTitle.Equals(Title))
            {
                pageMissing = page?.Revision?.IsDeleted != false;
            }
        }

        var wikiLink = new WikiLinkInline()
        {
            Page = page,
            PageTitle = pageTitle,
            Action = openParent.Action,
            Title = HtmlHelper.Unescape(title),
            Display = display,
            Fragment = fragment,
            HasDisplay = openParent.HasDisplay,
            IsImage = openParent.IsImage,
            IsCategory = isCategory,
            IsCommons = isCommons,
            IsWikipedia = isWikipedia,
            IsEscaped = isEscaped,
            IsMissing = pageMissing,
            Endmatter = endmatter,
            Span = new SourceSpan(openParent.Span.Start, inlineState.GetSourcePosition(text.Start - 1)),
            Line = openParent.Line,
            Column = openParent.Column,
        };

        openParent.ReplaceBy(wikiLink);
        inlineState.Inline = wikiLink;

        inlineState.PostProcessInlines(0, wikiLink, null, false);

        if (!openParent.IsImage && parentDelimiter != null)
        {
            foreach (var parent in parentDelimiter.FindParentOfType<WikiLinkDelimiterInline>())
            {
                if (parent.IsImage)
                {
                    break;
                }

                parent.IsActive = false;
            }
        }

        wikiLink.IsClosed = true;

        return true;
    }
}
