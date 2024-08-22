using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Tavenem.Wiki.MarkdownExtensions.WikiLinks;

/// <summary>
/// An inline parser for <see cref="WikiLinkInline"/>.
/// </summary>
public class WikiLinkInlineParser : LinkInlineParser
{
    /// <inheritdoc />
    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        // The following methods are inspired by the "An algorithm for parsing nested emphasis and links"
        // at the end of the CommonMark specs.

        var c = slice.CurrentChar;

        var startPosition = processor.GetSourcePosition(slice.Start, out var line, out var column);

        var isImage = false;
        if (c == '!')
        {
            isImage = true;
            c = slice.NextChar();
            if (c != '[')
            {
                return false;
            }
        }
        string? label;
        var labelWithTriviaSpan = SourceSpan.Empty;
        switch (c)
        {
            case '[':
                // If this is not an image, we may have a reference link shortcut
                // so we try to resolve it here
                var saved = slice;

                SourceSpan labelSpan;
                // If the label is followed by either a ( or a [, this is not a shortcut
                if (processor.TrackTrivia)
                {
                    if (LinkHelper.TryParseLabelTrivia(ref slice, out label, out labelSpan))
                    {
                        labelWithTriviaSpan.Start = labelSpan.Start; // skip opening [
                        labelWithTriviaSpan.End = labelSpan.End; // skip closing ]
                    }
                }
                else
                {
                    _ = LinkHelper.TryParseLabel(ref slice, out label, out labelSpan);
                }
                slice = saved;

                // Else we insert a LinkDelimiter
                slice.SkipChar();
                var linkDelimiter = new LinkDelimiterInline(this)
                {
                    Type = DelimiterType.Open,
                    Label = label,
                    LabelSpan = processor.GetSourcePositionFromLocalSpan(labelSpan),
                    IsImage = isImage,
                    Span = new SourceSpan(startPosition, processor.GetSourcePosition(slice.Start - 1)),
                    Line = line,
                    Column = column
                };

                if (processor.TrackTrivia)
                {
                    linkDelimiter.LabelWithTrivia = new StringSlice(slice.Text, labelWithTriviaSpan.Start, labelWithTriviaSpan.End);
                }

                processor.Inline = linkDelimiter;
                return true;

            case ']':
                slice.SkipChar();
                if (processor.Inline is not null
                    && TryProcessLinkOrImage(processor, ref slice))
                {
                    return true;
                }

                // If we don’t find one, we return a literal slice node ].
                // (Done after by the LiteralInline parser)
                return false;
        }

        // We don't have an emphasis
        return false;
    }

    private static void MarkParentAsInactive(Inline? inline)
    {
        while (inline is not null)
        {
            if (inline is LinkDelimiterInline linkInline)
            {
                if (linkInline.IsImage)
                {
                    break;
                }

                linkInline.IsActive = false;
            }

            inline = inline.Parent;
        }
    }

    private static bool ProcessLinkReference(
        InlineProcessor state,
        StringSlice text,
        string label,
        SourceSpan labelWithTriviaSpan,
        bool isShortcut,
        SourceSpan labelSpan,
        LinkDelimiterInline parent,
        int endPosition,
        LocalLabel localLabel)
    {
        // Ordinary reference link, rather than a wiki link.
        if (state.Document.TryGetLinkReferenceDefinition(label, out _))
        {
            return false;
        }

        // Inline Link
        var link = new WikiLinkInline()
        {
            Title = localLabel == LocalLabel.Empty
                ? null
                : HtmlHelper.Unescape(parent.Label),
            TitleSpan = localLabel == LocalLabel.Empty
                ? SourceSpan.Empty
                : parent.LabelSpan,
            Label = label,
            LabelSpan = labelSpan,
            IsImage = parent.IsImage,
            IsShortcut = isShortcut,
            Span = new SourceSpan(parent.Span.Start, endPosition),
            Line = parent.Line,
            Column = parent.Column,
        };

        if (state.TrackTrivia)
        {
            link.LabelWithTrivia = new StringSlice(text.Text, labelWithTriviaSpan.Start, labelWithTriviaSpan.End);
            link.LocalLabel = localLabel;
        }

        var child = parent.FirstChild;
        if (child is null)
        {
            child = new LiteralInline()
            {
                Content = StringSlice.Empty,
                IsClosed = true,
                // Not exact but we leave it like this
                Span = parent.Span,
                Line = parent.Line,
                Column = parent.Column,
            };
            link.AppendChild(child);
        }
        else
        {
            // Insert all child into the link
            while (child is not null)
            {
                var next = child.NextSibling;
                child.Remove();
                link.AppendChild(child);
                child = next;
            }
        }

        link.IsClosed = true;

        // Process emphasis delimiters
        state.PostProcessInlines(0, link, null, false);

        state.Inline = link;

        return true;
    }

    private static bool TryProcessLinkOrImage(InlineProcessor inlineState, ref StringSlice text)
    {
        var openParent = inlineState.Inline!.FirstParentOfType<LinkDelimiterInline>();
        if (openParent is null)
        {
            return false;
        }

        // If we do find one, but it’s not active,
        // we remove the inactive delimiter from the stack,
        // and return a literal text node ].
        if (!openParent.IsActive)
        {
            inlineState.Inline = new LiteralInline()
            {
                Content = new StringSlice("["),
                Span = openParent.Span,
                Line = openParent.Line,
                Column = openParent.Column,
            };
            openParent.ReplaceBy(inlineState.Inline);
            return false;
        }

        // Inline link, not wiki link.
        if (text.CurrentChar == '(')
        {
            return false;
        }

        // If we find one and it’s active, then we parse ahead to see if we have a reference
        // link/image, compact reference link/image, or shortcut reference link/image.
        var parentDelimiter = openParent.Parent;
        var labelSpan = SourceSpan.Empty;
        string? label = null;
        var isLabelSpanLocal = true;
        var isShortcut = false;
        var localLabel = LocalLabel.Local;

        // Handle Collapsed links
        if (text.CurrentChar == '[')
        {
            if (text.PeekChar() == ']')
            {
                label = openParent.Label;
                labelSpan = openParent.LabelSpan;
                isLabelSpanLocal = false;
                localLabel = LocalLabel.Empty;
                text.SkipChar(); // Skip [
                text.SkipChar(); // Skip ]
            }
        }
        else
        {
            localLabel = LocalLabel.None;
            label = openParent.Label;
            isShortcut = true;
        }

        if (label is not null || LinkHelper.TryParseLabelTrivia(ref text, true, out label, out labelSpan))
        {
            var labelWithTrivia = new SourceSpan(labelSpan.Start, labelSpan.End);
            if (isLabelSpanLocal)
            {
                labelSpan = inlineState.GetSourcePositionFromLocalSpan(labelSpan);
            }

            if (ProcessLinkReference(
                inlineState,
                text,
                label!,
                labelWithTrivia,
                isShortcut,
                labelSpan,
                openParent,
                inlineState.GetSourcePosition(text.Start - 1),
                localLabel))
            {
                // Remove the open parent
                openParent.Remove();
                if (!openParent.IsImage)
                {
                    MarkParentAsInactive(parentDelimiter);
                }
                return true;
            }
            else if (text.CurrentChar is not ']' and not '[')
            {
                return false;
            }
        }

        // We have a nested [ ]
        // firstParent.Remove();
        // The opening [ will be transformed to a literal followed by all the children of the [

        var literal = new LiteralInline()
        {
            Span = openParent.Span,
            Content = new StringSlice(openParent.IsImage ? "![" : "[")
        };

        inlineState.Inline = openParent.ReplaceBy(literal);
        return false;
    }
}