using Ganss.XSS;
using Markdig;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions;
using System;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// <para>
    /// A static class containing various customization and configuration options for the wiki
    /// system.
    /// </para>
    /// <para>
    /// Note: this class' members are not thread-safe. It is expected that these configuration
    /// values will be set once per application, during startup or initialization procedures. If
    /// your use-case requires changing any of these values during the lifetime of your application,
    /// you must implement thread-safe operations around those properties yourself.
    /// </para>
    /// </summary>
    internal static class WikiConfig
    {
        private static IHtmlSanitizer? _HtmlSanitizerNoTemplate;

        private static IHtmlSanitizer? _HtmlSanitizerFull;
        internal static IHtmlSanitizer HtmlSanitizerFull
            => _HtmlSanitizerFull ??= new HtmlSanitizer(
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>())
            {
                KeepChildNodes = true
            };

        internal static IHtmlSanitizer GetHtmlSanitizer(IWikiOptions options)
        {
            if (string.IsNullOrEmpty(options.LinkTemplate)
                && _HtmlSanitizerNoTemplate is not null)
            {
                return _HtmlSanitizerNoTemplate;
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

            return htmlSanitizer;
        }

        internal static MarkdownPipeline GetMarkdownPipeline(IWikiOptions options, IDataStore dataStore) =>
            new MarkdownPipelineBuilder()
            .UseWikiLinks(options, dataStore)
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseTableOfContents(new MarkdownExtensions.TableOfContents.TableOfContentsOptions
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

        internal static MarkdownPipeline GetMarkdownPipelinePlainText(IWikiOptions options, IDataStore dataStore) =>
            new MarkdownPipelineBuilder()
            .UseWikiLinks(options, dataStore)
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
}
