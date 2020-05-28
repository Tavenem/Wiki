using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using NeverFoundry.DataStorage;
using NeverFoundry.DiffPatchMerge;
using NeverFoundry.Wiki.MarkdownExtensions.TableOfContents;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using NeverFoundry.Wiki.MarkdownExtensions.WikiLinks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace NeverFoundry.Wiki
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
        /// The markdown content.
        /// </summary>
        public string MarkdownContent { get; private protected set; }

        /// <summary>
        /// The wiki links within this content.
        /// </summary>
        public IReadOnlyList<WikiLink> WikiLinks { get; private protected set; } = new List<WikiLink>().AsReadOnly();

        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownItem"/>.
        /// </summary>
        /// <param name="markdown">The raw markdown.</param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        protected MarkdownItem(string? markdown, List<WikiLink> wikiLinks)
        {
            MarkdownContent = markdown ?? string.Empty;
            WikiLinks = wikiLinks;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownItem"/>.
        /// </summary>
        /// <param name="markdown">The raw markdown.</param>
        protected MarkdownItem(string? markdown) : this(markdown, GetWikiLinks(markdown)) { }

        private protected MarkdownItem(string id, string? markdown, IList<WikiLink> wikiLinks) : base(id)
        {
            MarkdownContent = markdown ?? string.Empty;
            WikiLinks = new ReadOnlyCollection<WikiLink>(wikiLinks);
        }

        private MarkdownItem(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (WikiLink[]?)info.GetValue(nameof(WikiLinks), typeof(WikiLink[])) ?? new WikiLink[0])
        { }

        /// <summary>
        /// Gets the given markdown content as plain text (i.e. strips all formatting).
        /// </summary>
        /// <param name="markdown">The markdown content.</param>
        /// <param name="characterLimit">The maximum number of characters to return.</param>
        /// <param name="singleParagraph">
        /// If true, stops after the first paragraph break, even still under the allowed character limit.
        /// </param>
        /// <returns>The plain text.</returns>
        public static string GetPlainText(string markdown, int? characterLimit = 200, bool singleParagraph = true)
        {
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

            var html = Markdown.ToHtml(markdown, WikiConfig.MarkdownPipelinePlainText);
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            if (!(WikiConfig.Postprocessors is null))
            {
                foreach (var preprocessor in WikiConfig.Postprocessors)
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
        /// <param name="markdown">A markdown-formatted string.</param>
        /// <returns>The rendered HTML.</returns>
        public static string RenderHtml(string markdown)
        {
            var html = Markdown.ToHtml(markdown, WikiConfig.MarkdownPipeline);
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            if (!(WikiConfig.Postprocessors is null))
            {
                foreach (var preprocessor in WikiConfig.Postprocessors)
                {
                    html = preprocessor.Process.Invoke(html);
                }
            }

            return WikiConfig.HtmlSanitizer.Sanitize(html, WikiConfig.ServerUrl);
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(WikiLinks), WikiLinks.ToArray());
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
        /// <param name="other">The other <see cref="MarkdownItem"/> insteance.</param>
        /// <returns>
        /// A string representing the diff between this instance and the <paramref name="other"/>
        /// instance, as rendered HTML.
        /// </returns>
        public string GetDiffHtml(MarkdownItem other)
            => RenderHtml(PostprocessMarkdown(GetDiff(other, "html")));

        /// <summary>
        /// Gets this item's content rendered as HTML.
        /// </summary>
        /// <returns>The rendered HTML.</returns>
        public string GetHtml() => RenderHtml(PostprocessMarkdown(MarkdownContent));

        /// <summary>
        /// Gets this item's content as plain text (i.e. strips all formatting).
        /// </summary>
        /// <param name="characterLimit">The maximum number of characters to return.</param>
        /// <param name="singleParagraph">
        /// If true, stops after the first paragraph break, even still under the allowed character limit.
        /// </param>
        /// <returns>The plain text.</returns>
        public string GetPlainText(int? characterLimit = 200, bool singleParagraph = true)
            => GetPlainText(PostprocessMarkdown(MarkdownContent), characterLimit, singleParagraph);

        /// <summary>
        /// Gets a preview of this item's rendered HTML.
        /// </summary>
        /// <returns>A preview of this item's rendered HTML.</returns>
        public string GetPreview()
        {
            var markdown = PostprocessMarkdown(MarkdownContent, isPreview: true);

            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }

            var document = Markdown.Parse(markdown, WikiConfig.MarkdownPipeline);
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
                WikiConfig.MarkdownPipeline.Setup(renderer);
                renderer.Render(document);
                html = writer.ToString();
            }

            if (!string.IsNullOrWhiteSpace(html)
                && !(WikiConfig.Postprocessors is null))
            {
                foreach (var preprocessor in WikiConfig.Postprocessors)
                {
                    html = preprocessor.Process.Invoke(html);
                }
            }

            return WikiConfig.HtmlSanitizer.Sanitize(html, WikiConfig.ServerUrl) ?? string.Empty;
        }

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

        private protected static List<WikiLink> GetWikiLinks(string? markdown)
            => string.IsNullOrEmpty(markdown)
            ? new List<WikiLink>()
            : Markdown.Parse(markdown, WikiConfig.MarkdownPipeline)
            .Descendants<WikiLinkInline>()
            .Where(x => !x.IsWikipedia && !x.IsCommons)
            .Select(x => new WikiLink(x.IsCategory, x.IsNamespaceEscaped, x.IsTalk, x.Title, x.WikiNamespace))
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
                var nextLine = span.Slice(i).IndexOf('\n');
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

        private protected virtual string PostprocessMarkdown(string markdown, bool isPreview = false) => markdown;
    }
}
