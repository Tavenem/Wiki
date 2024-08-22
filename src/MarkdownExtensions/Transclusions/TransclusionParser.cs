using Markdig.Helpers;
using System.Text;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.MarkdownExtensions.Transclusions;

/// <summary>
/// A parser for transcluded wiki items.
/// </summary>
public static class TransclusionParser
{
    internal const char CodeBlockChar = '`';
    internal const char TransclusionCloseChar = '}';
    internal const char TransclusionOpenChar = '{';

    private const string TransclusionBlockOpenSequence = "{{#>";
    private const string TransclusionBlockEndOpenSequence = "{{/";
    private const string TransclusionCloseSequence = "}}";
    private const string TransclusionOpenSequence = "{{>";

    private const int TransclusionMaxDepth = 100;

    /// <summary>
    /// Replaces all the transclusions in the given <paramref name="markdown"/> with their contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="markdown">A markdown string.</param>
    /// <param name="title">The title of the top-level article being generated.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <returns>
    /// The markdown with all transclusions substituted.
    /// </returns>
    public static async ValueTask<string> TranscludeAsync(
        WikiOptions options,
        IDataStore dataStore,
        string markdown,
        PageTitle? title = null,
        bool isPreview = false,
        bool isTalk = false)
    {
        var (result, _) = await TranscludeInnerAsync(
            options,
            dataStore,
            markdown,
            title,
            false,
            isPreview,
            isTalk);
        return result!;
    }

    /// <summary>
    /// Replaces all the transclusions in the given <paramref name="markdown"/> with their
    /// contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="markdown">A markdown string.</param>
    /// <param name="title">The title of the top-level page being generated.</param>
    /// <param name="isTemplate">Whether the page is being rendered as a
    /// transclusion.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <param name="arguments">
    /// A collection of supplied arguments, if the markdown is itself a transclusion.
    /// </param>
    /// <param name="parameters">
    /// A collection of supplied parameter values, if the markdown is itself a transclusion.
    /// </param>
    /// <returns>
    /// A tuple containing the markdown with all transclusions substituted, and a <see
    /// cref="List{T}"/> of the full titles of all pages referenced by the transclusions within
    /// the given <paramref name="markdown"/> (including nested transclusions).
    /// </returns>
    internal static async ValueTask<(string? markdown, HashSet<PageTitle> transcludedPages)> TranscludeInnerAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? markdown,
        PageTitle? title = null,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        List<object>? arguments = null,
        Dictionary<string, object>? parameters = null)
    {
        var transcludedPages = new HashSet<PageTitle>();

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return (string.Empty, transcludedPages);
        }

        var writer = new StringBuilder();
        await TranscludeInnerAsync(
            options,
            dataStore,
            writer,
            transcludedPages,
            markdown,
            title,
            isTemplate,
            isPreview,
            isTalk,
            arguments,
            parameters);
        return (writer.ToString(), transcludedPages);
    }

    internal static async ValueTask TranscludeInnerAsync(
        WikiOptions options,
        IDataStore dataStore,
        StringBuilder writer,
        HashSet<PageTitle> transcludedPages,
        string? markdown,
        PageTitle? title = null,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        List<object>? arguments = null,
        Dictionary<string, object>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return;
        }

        var transclusions = GetTransclusions(markdown);
        if (transclusions.Count == 0)
        {
            writer.Append(TransclusionPreprocessor.Transclude(
                options,
                title,
                markdown,
                isTemplate,
                isPreview,
                isTalk,
                arguments,
                parameters));
            return;
        }

        RemoveInvalidTransclusions(options, markdown, transclusions);
        if (transclusions.Count == 0)
        {
            writer.Append(TransclusionPreprocessor.Transclude(
                options,
                title,
                markdown,
                isTemplate,
                isPreview,
                isTalk,
                arguments,
                parameters));
            return;
        }

        for (var i = 0; i < transclusions.Count; i++)
        {
            if (!transclusions[i].Title.HasValue)
            {
                transclusions.RemoveAt(i--);
                continue;
            }

            if (transclusions[i].Title!.Value.Title == "@partial-block")
            {
                continue;
            }

            // Always add the page to the list of transclusions,
            // even if it doesn't exist or can't be transcluded,
            // since missing transclusions are obtained from this list.
            if (!transclusions[i].IsBlockEnd)
            {
                transcludedPages.Add(transclusions[i].Title!.Value);
            }

            var page = await IPage<Page>.GetExistingPageAsync(
                dataStore,
                transclusions[i].Title!.Value,
                true,
                false,
                WikiJsonSerializerContext.Default.Page)
                .ConfigureAwait(false);
            // If an anonymous user cannot read the page, do not transclude it.
            // The transclusion process occurs as part of the generation of page content,
            // not dynamically at render time (when consideration for individual permissions might be possible).
            if (page?.Exists == true
                && !string.IsNullOrWhiteSpace(page.MarkdownContent)
                && page.GetPermission(options).HasFlag(WikiPermission.Read))
            {
                transclusions[i].Page = page;
            }
            else
            {
                transclusions.RemoveAt(i--);
            }
        }
        if (transclusions.Count == 0)
        {
            writer.Append(TransclusionPreprocessor.Transclude(
                options,
                title,
                markdown,
                isTemplate,
                isPreview,
                isTalk,
                arguments,
                parameters));
            return;
        }

        var builder = new StringBuilder();
        var markdownIndex = 0;
        var transclusionIndex = 0;
        while (transclusionIndex < transclusions.Count)
        {
            (transclusionIndex, markdownIndex) = await WriteTransclusionContentAsync(
                options,
                dataStore,
                builder,
                transcludedPages,
                transclusions,
                transclusionIndex,
                markdownIndex,
                0,
                markdown,
                title,
                isPreview,
                isTalk,
                arguments,
                parameters);
        }
        if (transclusions[^1].End < markdown.Length)
        {
            builder.Append(markdown[transclusions[^1].End..]);
        }
        writer.Append(TransclusionPreprocessor.Transclude(
            options,
            title,
            builder,
            isTemplate,
            isPreview,
            isTalk,
            arguments,
            parameters));
    }

    private static async ValueTask<(int newTransclusionIndex, int newMarkdownIndex)> WriteTransclusionContentAsync(
        WikiOptions options,
        IDataStore dataStore,
        StringBuilder writer,
        HashSet<PageTitle> transcludedPages,
        List<Transclusion> transclusions,
        int transclusionIndex,
        int markdownIndex,
        int depth,
        string markdown,
        PageTitle? title,
        bool isPreview,
        bool isTalk,
        List<object>? arguments,
        Dictionary<string, object>? parameters)
    {
        var transclusion = transclusions[transclusionIndex];
        transclusionIndex++;

        // write content prior to the start of the transclusion
        if (transclusion.Start > markdownIndex)
        {
            writer.Append(markdown[markdownIndex..transclusion.Start]);
        }

        if (transclusion.Title!.Value.Title == "@partial-block")
        {
            if (parameters?.TryGetValue("@partial-block", out var p) == true
                && p is string md)
            {
                writer.Append(md);
            }
            else
            {
                writer.Append(markdown, transclusion.Start, transclusion.End - transclusion.Start);
            }
            markdownIndex = transclusion.End;
            return (transclusionIndex, markdownIndex);
        }

        if (transclusion.IsBlockEnd)
        {
            // if this block is not associated with a starting block (should not happen),
            // or its starting block was not successfully transcluded,
            // write the literal content
            if (transclusion.BlockStart?.WasTranscluded != true)
            {
                writer.Append(markdown, transclusion.Start, transclusion.End - transclusion.Start);
            }
            markdownIndex = transclusion.End;
            return (transclusionIndex, markdownIndex);
        }

        if (depth >= TransclusionMaxDepth
            || transclusion.Page is null
            || string.IsNullOrWhiteSpace(transclusion.Page.MarkdownContent)
            || (transclusion.IsBlockStart && transclusion.BlockEnd is null))
        {
            writer.Append(markdown, transclusion.Start, transclusion.End - transclusion.Start);
            markdownIndex = transclusion.End;
            return (transclusionIndex, markdownIndex);
        }

        var (transclusionArguments, transclusionParameters) = transclusion.Parameters.HasValue
            ? ParseParameters(markdown, transclusion.Parameters.Value)
            : (null, null);
        if (arguments is not null)
        {
            (transclusionArguments ??= []).AddRange(arguments);
        }
        if (parameters is not null)
        {
            foreach (var (key, value) in parameters)
            {
                if (transclusionParameters is null)
                {
                    (transclusionParameters = [])[key] = value;
                }
                else if (!transclusionParameters.ContainsKey(key))
                {
                    transclusionParameters[key] = value;
                }
            }
        }

        markdownIndex = transclusion.End;

        if (transclusion.BlockEnd is not null
            && transclusion.End < transclusion.BlockEnd.Start - 1)
        {
            var content = new StringBuilder();
            while (transclusionIndex < transclusions.Count)
            {
                if (transclusions[transclusionIndex].End > transclusion.BlockEnd.Start)
                {
                    break;
                }
                if (transclusions[transclusionIndex].Start < transclusion.End)
                {
                    transclusionIndex++;
                    continue;
                }
                (transclusionIndex, markdownIndex) = await WriteTransclusionContentAsync(
                    options,
                    dataStore,
                    content,
                    transcludedPages,
                    transclusions,
                    transclusionIndex,
                    markdownIndex,
                    depth + 1,
                    markdown,
                    title,
                    isPreview,
                    isTalk,
                    transclusionArguments,
                    transclusionParameters);
            }

            if (transclusions[transclusionIndex - 1].End < transclusion.BlockEnd.Start - 1)
            {
                content.Append(
                    markdown,
                    transclusions[transclusionIndex - 1].End,
                    transclusion.BlockEnd.Start - transclusions[transclusionIndex - 1].End);
            }

            if (content.Length > 0)
            {
                (transclusionParameters ??= [])["@partial-block"] = TransclusionPreprocessor
                    .Transclude(
                    options,
                    title,
                    content.ToString(),
                    true,
                    isPreview,
                    isTalk,
                    transclusionArguments,
                    transclusionParameters);
            }

            markdownIndex = transclusion.BlockEnd.End;
        }

        await TranscludeInnerAsync(
            options,
            dataStore,
            writer,
            transcludedPages,
            transclusion.Page.MarkdownContent,
            title,
            true,
            isPreview,
            isTalk,
            transclusionArguments,
            transclusionParameters);

        return (transclusionIndex, markdownIndex);
    }

    private static ReadOnlySpan<char> GetTransclusionName(string markdown, Transclusion transclusion, out int separatorIndex)
    {
        if (transclusion.IsBlockEnd)
        {
            separatorIndex = -1;
            return markdown.AsSpan()[transclusion.ContentStart..transclusion.ContentEnd];
        }
        else
        {
            var span = markdown.AsSpan();
            separatorIndex = span[transclusion.ContentStart..transclusion.ContentEnd].IndexOf(' ');
            if (separatorIndex != -1)
            {
                separatorIndex += transclusion.ContentStart;
            }
            return separatorIndex == -1
                ? span[transclusion.ContentStart..transclusion.ContentEnd]
                : span[transclusion.ContentStart..separatorIndex];
        }
    }

    private static List<Transclusion> GetTransclusions(string markdown)
    {
        var transclusions = new List<Transclusion>();

        var lineReader = new LineReader(markdown);
        var isCodeFenced = false;
        StringSlice lineText;
        while ((lineText = lineReader.ReadLine()).Text is not null)
        {
            Parse(transclusions, ref lineText, ref isCodeFenced);
        }

        transclusions.Sort((x, y) => x.Start.CompareTo(y.Start));
        return transclusions;
    }

    private static void Parse(List<Transclusion> transclusions, ref StringSlice slice, ref bool isCodeFenced)
    {
        if (slice.IsEmptyOrWhitespace())
        {
            return;
        }

        var c = slice.CurrentChar;

        var transclusionOpenerIndex = -1;
        var transclusionContentOpenIndex = -1;
        var inCodeSpan = false;
        var inBlockOpen = false;
        var inBlockEndOpen = false;
        var isEscaped = false;
        var leadingSpaces = 0;
        var transclusionOpenSpaces = false;
        var transclusionCloseSpaces = -1;
        var codeFenceCharacters = 0;
        while (c != '\0')
        {
            if (leadingSpaces >= 0)
            {
                if (c.IsWhitespace())
                {
                    leadingSpaces++;
                    if (leadingSpaces >= 4)
                    {
                        return;
                    }
                    c = slice.NextChar();
                    continue;
                }
                else
                {
                    leadingSpaces = -1;
                }
            }
            else if (transclusionOpenSpaces)
            {
                if (c.IsWhitespace())
                {
                    transclusionContentOpenIndex++;
                    c = slice.NextChar();
                    continue;
                }
                else
                {
                    transclusionOpenSpaces = false;
                    transclusionCloseSpaces = 0;
                }
            }
            else if (transclusionCloseSpaces >= 0)
            {
                if (c.IsWhitespace())
                {
                    transclusionCloseSpaces++;
                    c = slice.NextChar();
                    continue;
                }
                else
                {
                    transclusionCloseSpaces = 0;
                }
            }

            if (c == CodeBlockChar)
            {
                codeFenceCharacters++;
                if (codeFenceCharacters >= 3)
                {
                    isCodeFenced = !isCodeFenced;
                }
                if (inCodeSpan)
                {
                    inCodeSpan = false;
                }
                else if (slice.IndexOf(CodeBlockChar) != -1)
                {
                    inCodeSpan = true;
                }
                c = slice.NextChar();
                continue;
            }

            if (c == '\\')
            {
                isEscaped = !isEscaped;
                c = slice.NextChar();
                continue;
            }

            codeFenceCharacters = 0;
            if (inCodeSpan
                || isCodeFenced
                || isEscaped)
            {
                isEscaped = false;
                c = slice.NextChar();
                continue;
            }

            if (slice.Match(TransclusionOpenSequence))
            {
                transclusionOpenerIndex = slice.Start;
                transclusionContentOpenIndex = slice.Start + TransclusionOpenSequence.Length;
                for (var i = 1; i < TransclusionOpenSequence.Length; i++)
                {
                    slice.SkipChar();
                }
                transclusionOpenSpaces = true;
            }
            else if (slice.Match(TransclusionBlockOpenSequence))
            {
                transclusionOpenerIndex = slice.Start;
                transclusionContentOpenIndex = slice.Start + TransclusionBlockOpenSequence.Length;
                inBlockOpen = true;
                for (var i = 1; i < TransclusionBlockOpenSequence.Length; i++)
                {
                    slice.SkipChar();
                }
                transclusionOpenSpaces = true;
            }
            else if (slice.Match(TransclusionBlockEndOpenSequence))
            {
                transclusionOpenerIndex = slice.Start;
                transclusionContentOpenIndex = slice.Start + TransclusionBlockEndOpenSequence.Length;
                inBlockEndOpen = true;
                for (var i = 1; i < TransclusionBlockEndOpenSequence.Length; i++)
                {
                    slice.SkipChar();
                }
                transclusionOpenSpaces = true;
            }
            else if (slice.Match(TransclusionCloseSequence))
            {
                if (transclusionOpenerIndex != -1)
                {
                    var contentEnd = slice.Start;
                    contentEnd -= transclusionCloseSpaces;

                    if (contentEnd > transclusionContentOpenIndex)
                    {
                        transclusions.Add(new Transclusion(
                            transclusionOpenerIndex,
                            transclusionContentOpenIndex,
                            contentEnd,
                            slice.Start + TransclusionCloseSequence.Length,
                            inBlockEndOpen,
                            inBlockOpen));
                    }

                    transclusionOpenerIndex = -1;
                    transclusionContentOpenIndex = -1;
                    transclusionCloseSpaces = -1;
                    inBlockEndOpen = false;
                    inBlockOpen = false;
                }
                for (var i = 1; i < TransclusionCloseSequence.Length; i++)
                {
                    slice.SkipChar();
                }
            }

            isEscaped = false;
            c = slice.NextChar();
        }
    }

    private static (List<object>? arguments, Dictionary<string, object>? parameters) ParseParameters(string value, Range range)
    {
        if (value.Length == 0)
        {
            return (null, null);
        }

        List<object>? arguments = null;
        Dictionary<string, object>? parameters = null;

        static int FindUnescaped(string searchString, char value, int startIndex, int endIndex)
        {
            var escaped = false;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (escaped)
                {
                    escaped = false;
                }
                else if (searchString[i] == '\\')
                {
                    escaped = true;
                }
                else if (searchString[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        static object GetParameterValue(in ReadOnlySpan<char> span)
        {
            if ((span[0] == '"' && span[^1] == '"')
                || (span[0] == '\'' && span[^1] == '\''))
            {
                return HtmlHelper.Unescape(span[1..^1].ToString());
            }
            else if (span[0] == '['
                && span[^1] == ']')
            {
                var array = new List<object>();
                var startIndex = 0;
                int separatorIndex;
                ReadOnlySpan<char> current;
                do
                {
                    separatorIndex = span[startIndex..].IndexOf(',');
                    if (separatorIndex == 0)
                    {
                        startIndex++;
                        continue;
                    }

                    current = separatorIndex == -1
                        ? span[startIndex..]
                        : span[startIndex..separatorIndex];

                    array.Add(GetParameterValue(current));

                    startIndex = separatorIndex == -1
                        ? span.Length
                        : separatorIndex + 1;
                }
                while (startIndex < span.Length);
                return array.ToArray();
            }
            else if (bool.TryParse(span, out var b))
            {
                return b;
            }
            else if (DateTimeOffset.TryParse(span, out var dt))
            {
                return dt;
            }
            else if (long.TryParse(span, out var l))
            {
                return l;
            }
            else if (double.TryParse(span, out var d))
            {
                return d;
            }
            else
            {
                return HtmlHelper.Unescape(span.ToString());
            }
        }

        var startIndex = range.Start.Value;
        int equalIndex, separatorIndex;
        ReadOnlySpan<char> current;
        do
        {
            if (value[startIndex] == '"')
            {
                separatorIndex = FindUnescaped(value, '"', startIndex + 1, range.End.Value);
                current = separatorIndex == -1
                    ? value.AsSpan()[startIndex..]
                    : value.AsSpan()[startIndex..(separatorIndex + 1)];
            }
            else if (value[startIndex] == '\'')
            {
                separatorIndex = FindUnescaped(value, '\'', startIndex + 1, range.End.Value);
                current = separatorIndex == -1
                    ? value.AsSpan()[startIndex..]
                    : value.AsSpan()[startIndex..(separatorIndex + 1)];
            }
            else
            {
                separatorIndex = FindUnescaped(value, ' ', startIndex, range.End.Value);
                current = separatorIndex == -1
                    ? value.AsSpan()[startIndex..range.End.Value]
                    : value.AsSpan()[startIndex..separatorIndex];
            }
            if (current.Length == 0)
            {
                startIndex++;
                continue;
            }

            equalIndex = current.IndexOf('=');
            if (equalIndex == -1)
            {
                (arguments ??= []).Add(GetParameterValue(current));
            }
            else if (equalIndex == 0)
            {
                (arguments ??= []).Add(GetParameterValue(current[1..]));
            }
            else if (equalIndex == current.Length - 1)
            {
                (parameters ??= []).Add(current[..equalIndex].ToString(), string.Empty);
            }
            else
            {
                (parameters ??= []).Add(
                    current[..equalIndex].ToString(),
                    GetParameterValue(current[(equalIndex + 1)..]));
            }

            startIndex = separatorIndex == -1
                ? range.End.Value
                : separatorIndex + 1;
        }
        while (startIndex < range.End.Value);

        return (arguments, parameters);
    }

    private static void RemoveInvalidTransclusions(
        WikiOptions options,
        string markdown,
        List<Transclusion> transclusions)
    {
        var matchedEnds = new HashSet<int>();
        for (var i = 0; i < transclusions.Count; i++)
        {
            if (transclusions[i].IsBlockEnd)
            {
                if (!matchedEnds.Contains(i))
                {
                    transclusions.RemoveAt(i--);
                }
                continue;
            }

            var name = GetTransclusionName(markdown, transclusions[i], out var separatorIndex);
            if (!TryGetTransclusionPageTitle(options, name, out var title))
            {
                transclusions.RemoveAt(i--);
                continue;
            }
            transclusions[i].Title = title;
            if (separatorIndex != -1)
            {
                transclusions[i].Parameters = new(separatorIndex + 1, transclusions[i].ContentEnd);
            }

            if (!transclusions[i].IsBlockStart)
            {
                continue;
            }

            var matched = false;
            for (var j = i + 1; j < transclusions.Count; j++)
            {
                if (!transclusions[j].IsBlockEnd)
                {
                    continue;
                }
                if (matchedEnds.Contains(j))
                {
                    break;
                }

                var otherName = GetTransclusionName(markdown, transclusions[j], out _);
                if (!TryGetTransclusionPageTitle(options, otherName, out var otherTitle))
                {
                    continue;
                }
                transclusions[j].Title = otherTitle;
                if (otherName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    matched = true;
                    matchedEnds.Add(j);
                    transclusions[j].BlockStart = transclusions[i];
                    transclusions[i].BlockEnd = transclusions[j];
                    break;
                }
            }
            if (!matched)
            {
                transclusions.RemoveAt(i--);
            }
        }
    }

    private static bool TryGetTransclusionPageTitle(WikiOptions options, in ReadOnlySpan<char> name, out PageTitle title)
    {
        title = PageTitle.Parse(name);
        if (title.IsEmpty)
        {
            return false;
        }
        if (string.IsNullOrEmpty(title.Namespace))
        {
            title = title.WithNamespace(options.TransclusionNamespace);
        }
        else if (string.CompareOrdinal(title.Namespace, "-") == 0)
        {
            title = title.WithNamespace(null);
        }
        else if (string.CompareOrdinal(title.Namespace, "\\-") == 0)
        {
            title = title.WithNamespace("-");
        }
        if (title.Title?.Equals(options.MainPageTitle) == true)
        {
            title = title.WithTitle(null);
        }
        return true;
    }
}
