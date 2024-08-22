using Ganss.Xss;
using Lucene.Net.Util;
using Markdig;
using Tavenem.Wiki.MarkdownExtensions;
using Tavenem.Wiki.MarkdownExtensions.TableOfContents;

namespace Tavenem.Wiki;

/// <summary>
/// A static class containing various customization and configuration options for the wiki
/// system.
/// </summary>
internal static class WikiConfig
{
    internal const LuceneVersion WikiLuceneVersion = LuceneVersion.LUCENE_48;

    private static IHtmlSanitizer? _HtmlSanitizerNoTemplate;
    private static Dictionary<TableOfContentsOptions, MarkdownPipeline>? _PipelineCache;
    private static Dictionary<string, IHtmlSanitizer>? _SanitizerCache;

    private static IHtmlSanitizer? _HtmlSanitizerFull;
    internal static IHtmlSanitizer HtmlSanitizerFull
        => _HtmlSanitizerFull ??= new HtmlSanitizer(new HtmlSanitizerOptions())
        {
            KeepChildNodes = true
        };

    internal static MarkdownPipeline? _markdownPipelinePlainText;
    internal static MarkdownPipeline MarkdownPipelinePlainText
        => _markdownPipelinePlainText ??= GetMarkdownPipelinePlainText();

    internal static IHtmlSanitizer GetHtmlSanitizer(WikiOptions options)
    {
        if (string.IsNullOrEmpty(options.LinkTemplate))
        {
            if (_HtmlSanitizerNoTemplate is not null)
            {
                return _HtmlSanitizerNoTemplate;
            }
        }
        else if (_SanitizerCache?.TryGetValue(options.LinkTemplate, out var sanitizer) == true)
        {
            return sanitizer;
        }

        var htmlSanitizer = new HtmlSanitizer();
        htmlSanitizer.AllowedAttributes.Add("class");
        htmlSanitizer.AllowedAttributes.Add("role");
        htmlSanitizer.AllowedAttributes.Add("id");

        htmlSanitizer.RemovingAttribute += (_, e) =>
        {
            if (e.Tag.TagName == "A" && options.LinkTemplate?.Contains(e.Attribute.Name) == true)
            {
                e.Cancel = true;
            }
            e.Cancel |= e.Attribute.Name.StartsWith("data-");
        };

        if (string.IsNullOrEmpty(options.LinkTemplate))
        {
            _HtmlSanitizerNoTemplate = htmlSanitizer;
        }
        else
        {
            (_SanitizerCache ??= [])[options.LinkTemplate] = htmlSanitizer;
        }

        return htmlSanitizer;
    }

    internal static MarkdownPipeline GetMarkdownPipeline(WikiOptions options)
    {
        var tableOfContentsOptions = new TableOfContentsOptions
        {
            DefaultDepth = options.DefaultTableOfContentsDepth,
            DefaultStartingLevel = 1,
            MinimumTopLevel = options.MinimumTableOfContentsHeadings,
            DefaultTitle = options.DefaultTableOfContentsTitle,
        };
        if (_PipelineCache?.TryGetValue(tableOfContentsOptions, out var pipeline) == true)
        {
            return pipeline;
        }
        pipeline = new MarkdownPipelineBuilder()
            .UseWikiLinks()
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseTableOfContents(new TableOfContentsOptions
            {
                DefaultDepth = options.DefaultTableOfContentsDepth,
                DefaultStartingLevel = 1,
                MinimumTopLevel = options.MinimumTableOfContentsHeadings,
                DefaultTitle = options.DefaultTableOfContentsTitle,
            })
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseAutoLinks()
            .UseGenericAttributes()
            .UseSmartyPants()
            .Build();
        (_PipelineCache ??= new()).Add(tableOfContentsOptions, pipeline);
        return pipeline;
    }

    private static MarkdownPipeline GetMarkdownPipelinePlainText()
        => new MarkdownPipelineBuilder()
        .UseWikiLinks()
        .UseAbbreviations()
        .UseAutoIdentifiers()
        .UseCitations()
        .UseCustomContainers()
        .UseDefinitionLists()
        .UseEmphasisExtras()
        .UseFigures()
        .UseFooters()
        .UseFootnotes()
        .UseGridTables()
        .UseMathematics()
        .UseMediaLinks()
        .UsePipeTables()
        .UseListExtras()
        .UseTaskLists()
        .UseGenericAttributes()
        .UseSmartyPants()
        .Build();
}
