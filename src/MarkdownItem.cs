using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Tavenem.DataStorage;
using Tavenem.DiffPatchMerge;
using Tavenem.Wiki.MarkdownExtensions.TableOfContents;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;
using Tavenem.Wiki.MarkdownExtensions.WikiLinks;

namespace Tavenem.Wiki
{
    /// <summary>
    /// An item which contains markdown.
    /// </summary>
    [Serializable]
    public abstract class MarkdownItem : IdItem, ISerializable
    {
        /// <summary>
        /// The mininum number of characters taken when generating a preview.
        /// </summary>
        public const int PreviewCharacterMin = 100;

        /// <summary>
        /// The maxinum number of characters taken when generating a preview.
        /// </summary>
        public const int PreviewCharacterMax = 500;

        private const int NewLineWeight = 4;

        /// <summary>
        /// The rendered HTML content.
        /// </summary>
        public string Html { get; private protected set; } = null!; // Always initialized during ctor, but in one instance by the subclass.

        /// <summary>
        /// The markdown content.
        /// </summary>
        public string MarkdownContent { get; private protected set; }

        /// <summary>
        /// A preview of this item's rendered HTML.
        /// </summary>
        public string Preview { get; private protected set; } = null!; // Always initialized during ctor, but in one instance by the subclass.

        /// <summary>
        /// The wiki links within this content.
        /// </summary>
        public IReadOnlyCollection<WikiLink> WikiLinks { get; private protected set; } = new List<WikiLink>().AsReadOnly();

        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownItem"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="markdownContent">The raw markdown.</param>
        /// <param name="html">The rendered HTML content.</param>
        /// <param name="preview">A preview of this item's rendered HTML.</param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        /// <remarks>
        /// Note: this constructor is most useful for deserializers.
        /// </remarks>
        private protected MarkdownItem(string id, string? markdownContent, string html, string preview, IReadOnlyCollection<WikiLink> wikiLinks) : base(id)
        {
            Html = html;
            MarkdownContent = markdownContent ?? string.Empty;
            Preview = preview;
            WikiLinks = wikiLinks;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownItem"/>.
        /// </summary>
        /// <param name="markdown">The raw markdown.</param>
        /// <param name="html">
        /// The rendered HTML content.
        /// </param>
        /// <param name="preview">
        /// A preview of this item's rendered HTML.
        /// </param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        private protected MarkdownItem(string? markdown, string? html, string? preview, IReadOnlyCollection<WikiLink> wikiLinks)
        {
            MarkdownContent = markdown ?? string.Empty;
            Html = html ?? string.Empty;
            Preview = preview ?? string.Empty;
            WikiLinks = wikiLinks;
        }

        private MarkdownItem(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)),
            (string?)info.GetValue(nameof(Html), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Preview), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>)) ?? new ReadOnlyCollection<WikiLink>(Array.Empty<WikiLink>()))
        { }

        /// <summary>
        /// Gets the given markdown content as plain text (i.e. strips all formatting).
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="markdown">The markdown content.</param>
        /// <param name="characterLimit">The maximum number of characters to return.</param>
        /// <param name="singleParagraph">
        /// If true, stops after the first paragraph break, even still under the allowed character limit.
        /// </param>
        /// <returns>The plain text.</returns>
        public static string FormatPlainText(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            int? characterLimit = 200,
            bool singleParagraph = true)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            if (singleParagraph && markdown.Length > 1)
            {
                var paraIndex = markdown.IndexOf(Environment.NewLine + Environment.NewLine, 1);
                if (paraIndex > 0)
                {
                    markdown = markdown.Substring(0, paraIndex);
                }
            }

            if (characterLimit.HasValue && markdown.Length > characterLimit.Value * 5)
            {
                markdown = markdown.Substring(0, characterLimit.Value * 5);
            }

            var html = Markdown.ToHtml(markdown, WikiConfig.GetMarkdownPipelinePlainText(options, dataStore));
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            if (options.Postprocessors is not null)
            {
                foreach (var preprocessor in options.Postprocessors)
                {
                    html = preprocessor.Process.Invoke(html);
                }
            }

            var sanitized = WikiConfig.HtmlSanitizerFull.Sanitize(html);
            if (characterLimit.HasValue && sanitized.Length > characterLimit)
            {
                var substring = sanitized.Substring(0, characterLimit.Value);
                var i = substring.Length - 1;
                var whitespace = false;
                for (; i > 0; i--)
                {
                    if (substring[i].IsWhiteSpaceOrZero())
                    {
                        whitespace = true;
                    }
                    else if (whitespace)
                    {
                        break;
                    }
                }
                sanitized = whitespace
                    ? substring.Substring(0, i + 1)
                    : substring;
            }
            return sanitized;
        }

        /// <summary>
        /// Renders the given <paramref name="markdown"/> as HTML.
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="markdown">The markdown content.</param>
        /// <returns>The rendered HTML.</returns>
        public static string RenderHtml(IWikiOptions options, IDataStore dataStore, string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }

            var html = Markdown.ToHtml(markdown, WikiConfig.GetMarkdownPipeline(options, dataStore));
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            if (options.Postprocessors is not null)
            {
                foreach (var preprocessor in options.Postprocessors)
                {
                    html = preprocessor.Process.Invoke(html);
                }
            }

            return WikiConfig.GetHtmlSanitizer(options).Sanitize(html);
        }

        /// <summary>
        /// Gets a preview of the given markdown's rendered HTML.
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="markdown">The markdown content.</param>
        /// <returns>A preview of the rendered HTML.</returns>
        public static string RenderPreview(IWikiOptions options, IDataStore dataStore, string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }

            var document = Markdown.Parse(markdown, WikiConfig.GetMarkdownPipeline(options, dataStore));
            if (AnyPreviews(document))
            {
                TrimNonPreview(document);
            }
            else
            {
                var minCharactersAvailable = PreviewCharacterMin;
                var maxCharactersAvailable = PreviewCharacterMax;
                Trim(document, ref minCharactersAvailable, ref maxCharactersAvailable);
            }

            string html;
            using (var writer = new StringWriter())
            {
                var renderer = new HtmlRenderer(writer);
                WikiConfig.GetMarkdownPipeline(options, dataStore).Setup(renderer);
                renderer.Render(document);
                html = writer.ToString();
            }

            if (!string.IsNullOrWhiteSpace(html)
                && options.Postprocessors is not null)
            {
                foreach (var preprocessor in options.Postprocessors)
                {
                    html = preprocessor.Process.Invoke(html);
                }
            }

            return WikiConfig.GetHtmlSanitizer(options).Sanitize(html) ?? string.Empty;
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(Html), Html);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(Preview), Preview);
            info.AddValue(nameof(WikiLinks), WikiLinks);
        }

        /// <summary>
        /// Gets a diff between the <see cref="MarkdownContent"/> of this item and the given one.
        /// </summary>
        /// <param name="other">The other <see cref="MarkdownItem"/> insteance.</param>
        /// <param name="format">
        /// <para>
        /// The format used.
        /// </para>
        /// <para>
        /// Can be either "delta" (the default), "gnu", "md", or "html" (case insensitive).
        /// </para>
        /// <para>
        /// The "delta" format (the default, used if an empty string or whitespace is passed)
        /// renders a compact, encoded string which describes each diff operation. The first
        /// character is '=' for unchanged text, '+' for an insertion, and '-' for deletion.
        /// Unchanged text and deletions are followed by their length only; insertions are followed
        /// by a compressed version of their full text. Each diff is separated by a tab character
        /// ('\t').
        /// </para>
        /// <para>
        /// The "gnu" format renders the text preceded by "- " for deletion, "+ " for addition, or
        /// nothing if the text was unchanged. Each diff is separated by a newline.
        /// </para>
        /// <para>
        /// The "md" format renders the text surrounded by "~~" for deletion, "++" for addition, or
        /// nothing if the text was unchanged. Diffs are concatenated without separators.
        /// </para>
        /// <para>
        /// The "html" format renders the text surrounded by a span with class "diff-deleted" for
        /// deletion, "diff-inserted" for addition, or without a wrapping span if the text was
        /// unchanged. Diffs are concatenated without separators.
        /// </para>
        /// </param>
        /// <returns>
        /// A string representing the diff between this instance and the <paramref name="other"/>
        /// instance.
        /// </returns>
        public string GetDiff(MarkdownItem other, string format = "md")
            => Diff.GetWordDiff(MarkdownContent, other.MarkdownContent).ToString(format);

        /// <summary>
        /// Gets a diff between the <see cref="MarkdownContent"/> of this item and the given one, as
        /// rendered HTML.
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="other">The other <see cref="MarkdownItem"/> insteance.</param>
        /// <returns>
        /// A string representing the diff between this instance and the <paramref name="other"/>
        /// instance, as rendered HTML.
        /// </returns>
        public string GetDiffHtml(IWikiOptions options, IDataStore dataStore, MarkdownItem other)
            => RenderHtml(options, dataStore, PostprocessMarkdown(options, dataStore, GetDiff(other, "html")));

        /// <summary>
        /// Gets this item's content rendered as HTML.
        /// </summary>
        /// <returns>The rendered HTML.</returns>
        public string GetHtml(IWikiOptions options, IDataStore dataStore)
            => RenderHtml(options, dataStore, PostprocessMarkdown(options, dataStore, MarkdownContent));

        /// <summary>
        /// Gets the given markdown content as plain text (i.e. strips all formatting).
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="markdown">The markdown content.</param>
        /// <param name="characterLimit">The maximum number of characters to return.</param>
        /// <param name="singleParagraph">
        /// If true, stops after the first paragraph break, even still under the allowed character limit.
        /// </param>
        /// <returns>The plain text.</returns>
        public string GetPlainText(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            int? characterLimit = 200,
            bool singleParagraph = true)
            => FormatPlainText(options, dataStore, PostprocessMarkdown(options, dataStore, markdown), characterLimit, singleParagraph);

        /// <summary>
        /// Gets this item's content as plain text (i.e. strips all formatting).
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
        /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
        /// <param name="characterLimit">The maximum number of characters to return.</param>
        /// <param name="singleParagraph">
        /// If true, stops after the first paragraph break, even still under the allowed character limit.
        /// </param>
        /// <returns>The plain text.</returns>
        public string GetPlainText(
            IWikiOptions options,
            IDataStore dataStore,
            int? characterLimit = 200,
            bool singleParagraph = true)
            => FormatPlainText(options, dataStore, PostprocessMarkdown(options, dataStore, MarkdownContent), characterLimit, singleParagraph);

        /// <summary>
        /// Gets a preview of this item's rendered HTML.
        /// </summary>
        /// <returns>A preview of this item's rendered HTML.</returns>
        public string GetPreview(IWikiOptions options, IDataStore dataStore)
            => RenderPreview(options, dataStore, PostprocessMarkdown(options, dataStore, MarkdownContent, isPreview: true));

        private static bool AnyPreviews(MarkdownObject obj)
        {
            if (obj is ContainerBlock containerBlock)
            {
                for (var i = 0; i < containerBlock.Count; i++)
                {
                    if (AnyPreviews(containerBlock[i]))
                    {
                        return true;
                    }
                }
            }
            else if (obj is ContainerInline containerInline)
            {
                if (obj is CustomContainerInline cc
                    && cc.TryGetAttributes()?.Classes?.Contains(TransclusionFunctions.PreviewClass) == true)
                {
                    return true;
                }

                var inline = containerInline.FirstChild;

                while (inline != null)
                {
                    if (AnyPreviews(inline))
                    {
                        return true;
                    }
                    inline = inline.NextSibling;
                }
            }
            else if (obj is LeafBlock leafBlock)
            {
                if (leafBlock.Inline != null
                    && AnyPreviews(leafBlock.Inline))
                {
                    return true;
                }
            }
            return false;
        }

        private protected static List<WikiLink> GetWikiLinks(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            string? title = null,
            string? wikiNamespace = null)
            => string.IsNullOrEmpty(markdown)
            ? new List<WikiLink>()
            : Markdown.Parse(markdown, WikiConfig.GetMarkdownPipeline(options, dataStore))
            .Descendants<WikiLinkInline>()
            .Where(x => !x.IsWikipedia
                && !x.IsCommons
                && (x.Title.Length < 5
                || ((x.Title[0] != TransclusionParser.TransclusionOpenChar
                || x.Title[1] != TransclusionParser.TransclusionOpenChar
                || x.Title[^1] != TransclusionParser.TransclusionCloseChar
                || x.Title[^2] != TransclusionParser.TransclusionCloseChar)
                && (x.Title[0] != TransclusionParser.ParameterOpenChar
                || x.Title[1] != TransclusionParser.ParameterOpenChar
                || x.Title[^1] != TransclusionParser.ParameterCloseChar
                || x.Title[^2] != TransclusionParser.ParameterCloseChar))))
            .Select(x =>
            {
                var anchorIndex = x.Title.LastIndexOf('#');
                return new WikiLink(
                    x.Article,
                    x.Missing && (x.Title != title || x.WikiNamespace != wikiNamespace),
                    x.IsCategory,
                    x.IsNamespaceEscaped,
                    x.IsTalk,
                    anchorIndex == -1 ? x.Title : x.Title[..anchorIndex],
                    x.WikiNamespace ?? options.DefaultNamespace);
            })
            .ToList();

        private static void Trim(MarkdownObject obj, ref int minCharactersAvailable, ref int maxCharactersAvailable)
        {
            if (obj is ContainerBlock containerBlock)
            {
                int i;
                for (i = 0; i < containerBlock.Count && minCharactersAvailable > 0; i++)
                {
                    Trim(containerBlock[i], ref minCharactersAvailable, ref maxCharactersAvailable);
                }

                for (var removeIndex = containerBlock.Count - 1; removeIndex >= i; removeIndex--)
                {
                    containerBlock.RemoveAt(removeIndex);
                }
            }
            else if (obj is ContainerInline containerInline)
            {
                var inline = containerInline.FirstChild;

                while (inline != null && maxCharactersAvailable > 0)
                {
                    Trim(inline, ref minCharactersAvailable, ref maxCharactersAvailable);
                    inline = inline.NextSibling;
                }

                if (maxCharactersAvailable <= 0)
                {
                    while (inline != null)
                    {
                        var next = inline.NextSibling;
                        inline.Remove();
                        inline = next;
                    }
                }
            }
            else if (obj is LeafBlock leafBlock)
            {
                if (leafBlock is TableOfContentsBlock toc)
                {
                    toc.Headings = null;
                }
                else
                {
                    if (leafBlock.Inline != null)
                    {
                        Trim(leafBlock.Inline, ref minCharactersAvailable, ref maxCharactersAvailable);
                    }

                    ref var lines = ref leafBlock.Lines;
                    var stringLines = lines.Lines;

                    if (stringLines != null)
                    {
                        int i;
                        for (i = 0; i < lines.Count && maxCharactersAvailable > 0; i++)
                        {
                            var original = maxCharactersAvailable;
                            TrimStringSlice(ref stringLines[i].Slice, ref maxCharactersAvailable);
                            var delta = original - maxCharactersAvailable;
                            minCharactersAvailable -= delta;
                            minCharactersAvailable -= NewLineWeight;
                            maxCharactersAvailable -= NewLineWeight;
                        }

                        for (var removeIndex = lines.Count - 1; removeIndex >= i; removeIndex--)
                        {
                            lines.RemoveAt(removeIndex);
                        }
                    }

                    minCharactersAvailable -= NewLineWeight;
                    maxCharactersAvailable -= NewLineWeight;
                }
            }
            else if (obj is LeafInline leafInline)
            {
                if (leafInline is LiteralInline literal)
                {
                    var original = maxCharactersAvailable;
                    TrimStringSlice(ref literal.Content, ref maxCharactersAvailable);
                    var delta = original - maxCharactersAvailable;
                    minCharactersAvailable -= delta;
                }
                else if (leafInline is CodeInline code)
                {
                    var original = maxCharactersAvailable;
                    code.Content = TrimString(code.Content, ref maxCharactersAvailable);
                    var delta = original - maxCharactersAvailable;
                    minCharactersAvailable -= delta;
                }
                else if (leafInline is AutolinkInline autoLink)
                {
                    minCharactersAvailable -= autoLink.Url.Length;
                    maxCharactersAvailable -= autoLink.Url.Length;
                }
                else if (leafInline is LineBreakInline)
                {
                    minCharactersAvailable -= NewLineWeight;
                    maxCharactersAvailable -= NewLineWeight;
                }
            }
        }

        private static bool TrimNonPreview(MarkdownObject obj)
        {
            if (obj is CustomContainerInline cc
                && cc.TryGetAttributes()?.Classes?.Contains(TransclusionFunctions.PreviewClass) == true)
            {
                return true;
            }

            var anyPreviewChildren = false;
            if (obj is ContainerBlock containerBlock)
            {
                var removable = new List<int>();
                for (var i = 0; i < containerBlock.Count; i++)
                {
                    if (TrimNonPreview(containerBlock[i]))
                    {
                        anyPreviewChildren = true;
                    }
                    else
                    {
                        removable.Add(i);
                    }
                }

                removable.Sort((x, y) => y.CompareTo(x));
                foreach (var index in removable)
                {
                    containerBlock.RemoveAt(index);
                }

                return anyPreviewChildren;
            }

            if (obj is ContainerInline containerInline)
            {
                var inline = containerInline.FirstChild;

                while (inline != null)
                {
                    if (TrimNonPreview(inline))
                    {
                        anyPreviewChildren = true;
                        inline = inline.NextSibling;
                    }
                    else
                    {
                        var removed = inline;
                        inline = inline.NextSibling;
                        removed.Remove();
                    }
                }

                return anyPreviewChildren;
            }

            if (obj is LeafBlock leafBlock)
            {
                if (leafBlock.Inline is null)
                {
                    return false;
                }

                if (TrimNonPreview(leafBlock.Inline))
                {
                    return true;
                }

                leafBlock.Inline.Remove();
            }

            return false;
        }

        private static ReadOnlySpan<char> TrimSpan(ReadOnlySpan<char> span, ref int charactersAvailable)
        {
            var i = 0;
            while (i < span.Length && charactersAvailable > 0)
            {
                var nextLine = span[i..].IndexOf('\n');
                var lineLength = nextLine == -1 ? span.Length - i : nextLine - i;

                if (lineLength <= charactersAvailable)
                {
                    i += lineLength;
                    charactersAvailable -= lineLength;

                    if (charactersAvailable > NewLineWeight && nextLine != -1)
                    {
                        i++;
                        charactersAvailable -= NewLineWeight;
                    }
                    else
                    {
                        if (nextLine != -1)
                        {
                            charactersAvailable = 0;
                        }
                        break;
                    }
                }
                else
                {
                    i += charactersAvailable;
                    charactersAvailable = 0;
                }
            }
            return span.Slice(0, i);
        }

        private static string TrimString(string text, ref int charactersAvailable)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            else if (charactersAvailable <= 0)
            {
                return string.Empty;
            }
            else
            {
                var span = TrimSpan(text.AsSpan(), ref charactersAvailable);
                return span.Length == text.Length ? text : span.ToString();
            }
        }

        private static void TrimStringSlice(ref StringSlice slice, ref int charactersAvailable)
        {
            if (slice.IsEmpty)
            {
                return;
            }
            if (charactersAvailable <= 0)
            {
                slice = new StringSlice(null);
            }
            else
            {
                var span = slice.Text.AsSpan(slice.Start, slice.Length);
                var trimmed = TrimSpan(span, ref charactersAvailable);
                slice.End = slice.Start + trimmed.Length - 1;
            }
        }

        [MemberNotNull(nameof(Html), nameof(Preview))]
        internal void Update(IWikiOptions options, IDataStore dataStore)
        {
            Html = GetHtml(options, dataStore);
            Preview = GetPreview(options, dataStore);
        }

        private protected virtual string PostprocessMarkdown(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            bool isPreview = false) => markdown ?? string.Empty;
    }
}
