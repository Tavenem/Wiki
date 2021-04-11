using Markdig.Helpers;
using Markdig.Parsers;
using System.Text;

namespace Tavenem.Wiki.MarkdownExtensions.TableOfContents
{
    /// <summary>
    /// A block parser for table of contents markers.
    /// </summary>
    public class TableOfContentsBlockParser : BlockParser
    {
        /// <summary>
        /// The options set for this instance.
        /// </summary>
        public TableOfContentsOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TableOfContentsBlockParser"/>.
        /// </summary>
        /// <param name="options">
        /// The options used during parsing.
        /// </param>
        public TableOfContentsBlockParser(TableOfContentsOptions options)
        {
            Options = options;

            OpeningCharacters = new[] { '<', '>' };
        }

        /// <summary>
        /// Tries to match a block opening.
        /// </summary>
        /// <param name="processor">The parser processor.</param>
        /// <returns>The result of the match</returns>
        public override BlockState TryOpen(BlockProcessor processor)
        {
            var start = processor.Start;
            var line = processor.Line;
            if (!line.Match("<!--") || line.End < 9)
            {
                return BlockState.None;
            }
            line.Start += 4;

            var c = line.CurrentChar;
            while (c.IsWhitespace())
            {
                c = line.NextChar();
            }

            var isNoToc = false;
            if (line.MatchLowercase("notoc"))
            {
                line.Start += 2;
                isNoToc = true;
            }

            if (!line.MatchLowercase("toc"))
            {
                return BlockState.None;
            }
            line.Start += 3;
            if (line.Start >= line.End)
            {
                return BlockState.None;
            }

            c = line.CurrentChar;
            while (c.IsWhitespace())
            {
                c = line.NextChar();
            }

            int depth, startingLevel;
            var buffer = new StringBuilder();
            while (c.IsDigit())
            {
                buffer.Append(c);
                c = line.NextChar();
            }
            if (buffer.Length > 0 || c == '*')
            {
                depth = c == '*' || !int.TryParse(buffer.ToString(), out var d)
                    ? Options.DefaultDepth
                    : d;
                buffer.Length = 0;

                c = line.NextChar();
                while (c.IsWhitespace())
                {
                    c = line.NextChar();
                }

                while (c.IsDigit())
                {
                    buffer.Append(c);
                    c = line.NextChar();
                }
                if (buffer.Length > 0 || c == '*')
                {
                    startingLevel = c == '*' || !int.TryParse(buffer.ToString(), out var s)
                        ? Options.DefaultStartingLevel
                        : s;
                    buffer.Length = 0;

                    c = line.NextChar();
                    while (c.IsWhitespace())
                    {
                        c = line.NextChar();
                    }
                }
                else
                {
                    startingLevel = Options.DefaultStartingLevel;
                }
            }
            else
            {
                depth = Options.DefaultDepth;
                startingLevel = Options.DefaultStartingLevel;
            }

            string? title = null;
            var foundEnd = false;
            if (c != '-')
            {
                buffer.Length = 0;
                while (c != '\0')
                {
                    if (c == '-' && line.Match("-->"))
                    {
                        foundEnd = true;
                        line.Start += 3;
                        break;
                    }
                    buffer.Append(c);
                    c = line.NextChar();
                }
                if (!foundEnd)
                {
                    return BlockState.None;
                }
                if (buffer.Length > 0)
                {
                    for (var i = buffer.Length - 1; i >= 0; i--)
                    {
                        if (buffer[i].IsWhitespace())
                        {
                            buffer.Length--;
                        }
                        else
                        {
                            break;
                        }
                    }
                    title = buffer.ToString();
                }
            }
            if (!foundEnd)
            {
                if (!line.Match("-->"))
                {
                    return BlockState.None;
                }
                line.Start += 3;
            }

            var block = new TableOfContentsBlock(this)
            {
                Column = processor.Column,
                Depth = depth,
                IsNoToc = isNoToc,
                StartingLevel = startingLevel,
                Title = title ?? Options.DefaultTitle,
            };
            block.Span.Start = start;
            block.Span.End = line.Start - 1;
            processor.NewBlocks.Push(block);
            return BlockState.BreakDiscard;
        }
    }
}
