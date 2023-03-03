using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Tavenem.Wiki.MarkdownExtensions.TableOfContents;

/// <summary>
/// A markdown parser used to obtain headings from automatic or manual tables of contents.
/// </summary>
public static class TableOfContentsParser
{
    /// <summary>
    /// Parse the given <paramref name="markdown"/> and obtain the list of headings within it,
    /// respecting any table of contents markers.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="markdown">The markdown text to parse.</param>
    /// <returns>
    /// A <see cref="List{T}"/> of <see cref="Heading"/> objects.
    /// </returns>
    public static List<Heading> Parse(WikiOptions options, string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return new();
        }

        var document = Markdig.Markdown.Parse(markdown, WikiConfig.GetMarkdownPipelineWithoutLinks(options));

        var toCs = document.Descendants<TableOfContentsBlock>().ToList();

        // Mark any default entries as no-ToC when an explicit entry or a no-ToC entry is found,
        // to avoid rendering.
        if (toCs.Any(x => !x.IsDefault || x.IsNoToc))
        {
            foreach (var toC in toCs.Where(x => x.IsDefault))
            {
                toC.IsNoToc = true;
            }
        }

        var headingLines = new List<(int line, Heading heading)>();
        foreach (var toC in toCs.Where(x => !x.IsNoToc))
        {
            var levelOffset = toC.Parent is null
                ? 0
                : toC.Parent.Descendants<HeadingBlock>()
                    .Where(x => x.Line < toC.Line)
                    .OrderByDescending(x => x.Line)
                    .FirstOrDefault()?.Level ?? 0;
            toC.LevelOffset = levelOffset;

            var toCHeadingBlocks = toC.Parent is null
                ? new List<HeadingBlock>()
                : toC.Parent.Descendants<HeadingBlock>()
                    .Where(x => x.Line > toC.Line)
                    .OrderBy(x => x.Line)
                    .TakeWhile(x => x.Level > levelOffset)
                    .Where(x => x.Level >= levelOffset + toC.StartingLevel
                        && x.Level < levelOffset + toC.StartingLevel + toC.Depth)
                    .ToList();

            if (!toC.IsDefault || toCHeadingBlocks.Count >= options.MinimumTableOfContentsHeadings)
            {
                foreach (var block in toCHeadingBlocks)
                {
                    string? id = null;
                    var attributes = block.TryGetAttributes();
                    if (attributes != null)
                    {
                        id = attributes.Id;
                    }

                    string? headingText = null;
                    if (block.Inline is not null)
                    {
                        using var sw = new StringWriter();
                        var stripRenderer = new HtmlRenderer(sw);
                        stripRenderer.Render(block.Inline);
                        headingText = stripRenderer.Writer.ToString();
                    }

                    var heading = new Heading()
                    {
                        Id = id,
                        Level = block.Level,
                        OffsetLevel = block.Level - toC.LevelOffset - (toC.StartingLevel - 1),
                        Text = headingText,
                    };
                    headingLines.Add((block.Line, heading));
                }
            }
        }
        if (headingLines.Count == 0)
        {
            return new();
        }
        headingLines.Sort((x, y) => x.line.CompareTo(y.line));
        return headingLines.Select(x => x.heading).ToList();
    }
}
