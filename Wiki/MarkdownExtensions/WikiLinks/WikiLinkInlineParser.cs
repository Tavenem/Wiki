using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Linq;
using System.Text;

namespace NeverFoundry.Wiki.MarkdownExtensions.WikiLinks
{
    /// <summary>
    /// An inline parser for <see cref="WikiLink"/> items.
    /// </summary>
    public class WikiLinkInlineParser : InlineParser
    {
        internal const char LinkCloseChar = ']';
        internal const char LinkOpenChar = '[';

        private const char SeparatorChar = '|';

        /// <summary>
        /// Initializes a new instance of <see cref="WikiLinkInlineParser"/>.
        /// </summary>
        public WikiLinkInlineParser() => OpeningCharacters = new[] { LinkOpenChar, LinkCloseChar, '!' };

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
                    if (TryParseLink(ref slice, out var title, out var display, out var hasDisplay, out var endPosition))
                    {
                        processor.Inline = new WikiLinkDelimiterInline(this)
                        {
                            Type = DelimiterType.Open,
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

        private string? ParseEndmatter(ref StringSlice lines)
        {
            string? endmatter = null;
            var buffer = new StringBuilder();

            var c = lines.CurrentChar;
            while (true)
            {
                if (!c.IsAlphaNumeric())
                {
                    if (buffer.Length > 0 && buffer.Length <= 999)
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

        private bool TryParseLink(ref StringSlice lines, out string? title, out string? display, out bool hasDisplay, out int endPosition)
        {
            lines.NextChar(); // skip second opening char, which has already been confirmed

            endPosition = lines.Start;
            hasDisplay = false;
            title = null;
            display = null;
            var buffer = new StringBuilder();

            char c;
            var hasEscape = false;
            var previousWhitespace = true;
            var hasNonWhitespace = false;
            var hasSeparator = false;
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
                    if (c != LinkOpenChar && c != LinkCloseChar && c != SeparatorChar && c != '\\')
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
                                display = buffer.ToString();
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

            if (isValid)
            {
                if (!hasSeparator)
                {
                    title = display;
                }
                else
                {
                    if (hasNonWhitespace)
                    {
                        hasDisplay = true;
                    }
                    else if (!hasNonWhitespace && title != null)
                    {
                        hasDisplay = true;

                        // Remove namespace(s).
                        var startIndex = title.LastIndexOf(':') + 1;
                        var endIndex = title.Length;

                        // Remove anchor.
                        var anchor = title.LastIndexOf('#');
                        if (anchor != -1)
                        {
                            endIndex = anchor;
                        }

                        // Remove anything in parenthesis at the end.
                        var openParen = title.IndexOf('(');
                        if (openParen != -1
                            && openParen < endIndex
                            && title.TrimEnd()[^1] == ')')
                        {
                            endIndex = openParen;
                        }

                        display = title[startIndex..endIndex].Trim();

                        if (anchor != -1)
                        {
                            display = openParen == -1 || openParen < anchor
                                ? $"{display} § {title[(anchor + 1)..]}"
                                : $"{display} § {title[(anchor + 1)..openParen]}";
                        }

                        if (hasDoubleSeparator)
                        {
                            display = display.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                        }
                    }
                }
            }

            buffer.Length = 0;

            return isValid;
        }

        private bool TryProcessLinkOrImage(InlineProcessor inlineState, ref StringSlice text)
        {
            var openParent = inlineState.Inline.FindParentOfType<WikiLinkDelimiterInline>().FirstOrDefault();
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

            if (openParent.Title != null)
            {
                string? endmatter = null;
                if (!openParent.HasDisplay)
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
                var isWikipedia = openParent.Title.StartsWith("w:", StringComparison.OrdinalIgnoreCase);
                var isCommons = !isWikipedia && openParent.Title.StartsWith("cc:", StringComparison.OrdinalIgnoreCase);
                var isCategory = false;
                var isNamespaceEscaped = false;
                var isTalk = false;
                string? wikiNamespace = null;

                var mainTitle = title;
                if (isWikipedia)
                {
                    title = openParent.Title[2..];
                    if (!openParent.HasDisplay)
                    {
                        display = display?[2..];
                        openParent.HasDisplay = true;
                    }
                }
                else if (isCommons)
                {
                    title = openParent.Title[3..];

                    if (!openParent.HasDisplay && display is not null)
                    {
                        var extIndex = display.IndexOf('.');
                        {
                            if (extIndex != -1)
                            {
                                display = display[3..extIndex];
                                openParent.HasDisplay = true;
                            }
                        }
                    }
                }
                else
                {
                    var anchorIndex = title.IndexOf('#');
                    if (anchorIndex != -1)
                    {
                        mainTitle = title[..anchorIndex];
                    }
                    var (wWikiNamespace, wTitle, wIsTalk, _) = Article.GetTitleParts(mainTitle);
                    isTalk = wIsTalk;
                    wikiNamespace = wWikiNamespace;
                    isCategory = string.Equals(wikiNamespace, WikiConfig.CategoryNamespace, StringComparison.CurrentCultureIgnoreCase);
                    if (isCategory)
                    {
                        isNamespaceEscaped = title.StartsWith(':');
                        wikiNamespace = WikiConfig.CategoryNamespace; // normalize casing
                    }
                    mainTitle = wTitle;
                    title = anchorIndex == -1 || anchorIndex >= title.Length - 1
                        ? wTitle
                        : wTitle + title[anchorIndex..];
                }

                var articleMissing = false;
                if (!isCategory && !isWikipedia && !isCommons)
                {
                    var reference = PageReference.GetPageReference(mainTitle, wikiNamespace);
                    if (reference is null)
                    {
                        articleMissing = true;
                    }
                    else
                    {
                        var article = WikiConfig.DataStore.GetItem<Article>(reference.Reference);
                        articleMissing = article?.IsDeleted == true;
                    }
                }

                var wikiLink = new WikiLinkInline()
                {
                    Title = HtmlHelper.Unescape(title),
                    Display = display,
                    HasDisplay = openParent.HasDisplay,
                    IsImage = openParent.IsImage,
                    IsCategory = isCategory,
                    IsCommons = isCommons,
                    IsWikipedia = isWikipedia,
                    IsNamespaceEscaped = isNamespaceEscaped,
                    IsTalk = isTalk,
                    Missing = articleMissing,
                    Endmatter = endmatter,
                    WikiNamespace = wikiNamespace,
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

            return false;
        }
    }
}
