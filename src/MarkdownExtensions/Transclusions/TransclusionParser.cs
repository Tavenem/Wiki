using Markdig.Helpers;
using System.Globalization;
using System.Text;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.MarkdownExtensions.Transclusions;

/// <summary>
/// A parser for transcluded wiki items.
/// </summary>
public static class TransclusionParser
{
    internal const char ParameterCloseChar = ')';
    internal const char ParameterOpenChar = '(';
    internal const char TransclusionCloseChar = '}';
    internal const char TransclusionOpenChar = '{';

    private const char CodeBlockChar = '`';
    private const char SeparatorChar = '|';
    private const int TransclusionMaxDepth = 100;

    /// <summary>
    /// Replaces all the transclusions in the given <paramref name="markdown"/> with their contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the top-level article being generated.</param>
    /// <param name="markdown">A markdown string.</param>
    /// <param name="isTemplate">Whether the article is being rendered as a transclusion.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <param name="parameterValues">
    /// A collection of supplied parameter values, if the markdown is itself a transclusion.
    /// </param>
    /// <returns>
    /// The markdown with all transclusions substituted.
    /// </returns>
    public static async ValueTask<string> TranscludeAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle? title,
        string markdown,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        Dictionary<string, string>? parameterValues = null)
    {
        var (result, _) = await TranscludeInnerAsync(
            options,
            dataStore,
            title,
            markdown,
            isTemplate,
            isPreview,
            isTalk,
            parameterValues);
        return result;
    }

    /// <summary>
    /// Replaces all the transclusions in the given <paramref name="markdown"/> with their
    /// contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the top-level page being generated.</param>
    /// <param name="markdown">A markdown string.</param>
    /// <param name="isTemplate">Whether the page is being rendered as a
    /// transclusion.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <param name="parameterValues">
    /// A collection of supplied parameter values, if the markdown is itself a transclusion.
    /// </param>
    /// <returns>
    /// A tuple containing the markdown with all transclusions substituted, and a <see
    /// cref="List{T}"/> of the full titles of all pages referenced by the transclusions within
    /// the given <paramref name="markdown"/> (including nested transclusions).
    /// </returns>
    internal static async ValueTask<(string markdown, List<PageTitle> transcludedPages)> TranscludeInnerAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle? title,
        string markdown,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        Dictionary<string, string>? parameterValues = null)
    {
        var transcludedPages = new List<PageTitle>();

        if (string.IsNullOrWhiteSpace(markdown))
        {
            return (markdown, transcludedPages);
        }

        var parameterInclusions = new List<Transclusion>();
        var transclusions = new List<Transclusion>();

        var lineReader = new LineReader(markdown);
        var codeFenced = false;
        while (true)
        {
            var lineText = lineReader.ReadLine();
            if (lineText.Text is null)
            {
                break;
            }

            var lineTransclusions = Parse(lineText, out var lineParameters, out var isCodeFence);

            if (isCodeFence)
            {
                codeFenced = false;
            }
            if (codeFenced)
            {
                continue;
            }

            if (parameterValues != null)
            {
                foreach (var parameter in lineParameters)
                {
                    var name = lineText.Text[(parameter.Start + 2)..(parameter.End - 2)].ToLower(CultureInfo.CurrentCulture);
                    parameter.Content = parameterValues.TryGetValue(name, out var value)
                        ? value
                        : string.Empty;
                }
            }

            lineTransclusions.Sort((x, y) => x.Start.CompareTo(y.Start));
            for (var i = 0; i < lineTransclusions.Count; i++)
            {
                (lineTransclusions[i].Content, var linePages) = await GetContentAsync(
                    options,
                    dataStore,
                    title,
                    lineText.Text[lineTransclusions[i].Start..lineTransclusions[i].End],
                    offset: lineTransclusions[i].Start,
                    end: lineTransclusions[i].End - 2,
                    depth: 0,
                    isTemplate,
                    isPreview,
                    isTalk,
                    lineTransclusions,
                    lineParameters,
                    parameterValues);
                transcludedPages = transcludedPages.Union(linePages).ToList();
            }

            transclusions.AddRange(lineTransclusions);
            parameterInclusions.AddRange(lineParameters);
        }

        return (Render(markdown, 0, transclusions, parameterInclusions),
            transcludedPages);
    }

    private static async ValueTask<(string, List<PageTitle>)> GetContentAsync(
        WikiOptions options,
        IDataStore dataStore,
        PageTitle? title,
        string markdown,
        int offset,
        int end,
        int depth,
        bool isTemplate,
        bool isPreview,
        bool isTalk,
        List<Transclusion> transclusions,
        List<Transclusion> parameterInclusions,
        Dictionary<string, string>? passedParameterValues = null)
    {
        var transcludedPages = new List<PageTitle>();

        var includedTransclusions = new List<Transclusion>();
        var j = 0;
        while (j < transclusions.Count
            && transclusions[j].Start < offset + 2)
        {
            j++;
        }
        while (j < transclusions.Count
            && transclusions[j].Start < end)
        {
            includedTransclusions.Add(transclusions[j]);
            j++;
        }

        var includedParameters = new List<Transclusion>();
        j = 0;
        while (j < parameterInclusions.Count
            && parameterInclusions[j].Start < offset + 2)
        {
            j++;
        }
        while (j < parameterInclusions.Count
            && parameterInclusions[j].Start < end)
        {
            includedParameters.Add(parameterInclusions[j]);
            j++;
        }
        for (var i = 0; i < includedTransclusions.Count; i++)
        {
            if (depth >= TransclusionMaxDepth)
            {
                includedTransclusions[i].Content = markdown[(includedTransclusions[i].Start - offset)..(includedTransclusions[i].End - offset)];
                continue;
            }

            (includedTransclusions[i].Content, var nestedArticles) = await GetContentAsync(
                options,
                dataStore,
                title,
                markdown[(includedTransclusions[i].Start - offset)..(includedTransclusions[i].End - offset)],
                offset: includedTransclusions[i].Start,
                end: includedTransclusions[i].End - 2,
                depth + 1,
                isTemplate,
                isPreview,
                isTalk,
                includedTransclusions,
                includedParameters,
                passedParameterValues);
            transcludedPages = transcludedPages.Union(nestedArticles).ToList();
        }

        var template = Render(markdown, offset, includedTransclusions, includedParameters);
        var templateContent = template[2..^2];

        var separatorIndex = templateContent.IndexOfUnescaped(SeparatorChar);
        if (separatorIndex == 0)
        {
            return (template, transcludedPages);
        }

        string? reference;
        string? parameters;
        if (separatorIndex == -1)
        {
            reference = templateContent;
            parameters = null;
        }
        else
        {
            reference = templateContent[..separatorIndex];
            if (separatorIndex == templateContent.Length - 1)
            {
                parameters = null;
            }
            else
            {
                parameters = templateContent[(separatorIndex + 1)..];
            }
        }
        reference = reference.Trim();

        Dictionary<string, string> parameterValues;
        var invariantReference = reference.ToLowerInvariant();
        if (TransclusionFunctions._Functions.TryGetValue(invariantReference, out var func))
        {
            // If the function uses any parameters, return it as-is so that it can be invoked
            // with those parameters at a higher level.
            parameterValues = ParseParameters(parameters);
            if (parameterValues.Any(x => x.Value.Length > 4
                && x.Value[0] == ParameterOpenChar
                && x.Value[1] == ParameterOpenChar
                && x.Value[^1] == ParameterCloseChar
                && x.Value[^2] == ParameterCloseChar))
            {
                return (template, transcludedPages);
            }
            if (passedParameterValues is not null
                && (invariantReference == "eval"
                || invariantReference == "exec"))
            {
                var index = 1;
                foreach (var (key, value) in passedParameterValues)
                {
                    if (!parameterValues.ContainsKey(key))
                    {
                        parameterValues.Add(key, value);
                    }
                    else if (int.TryParse(key, out var i))
                    {
                        while (parameterValues.ContainsKey(index.ToString()))
                        {
                            index++;
                        }
                        parameterValues.Add(index.ToString(), value);
                    }
                }
            }
            if (invariantReference == "exec")
            {
                string? script = null;
                if (!parameterValues.TryGetValue("code", out var code)
                    && parameterValues.TryGetValue("1", out code))
                {
                    parameterValues.Remove("1");
                }
                parameterValues.Remove("code");
                if (!string.IsNullOrWhiteSpace(code))
                {
                    var codeTitle = PageTitle.Parse(code);
                    if (string.IsNullOrEmpty(codeTitle.Namespace))
                    {
                        codeTitle = codeTitle.WithNamespace(options.ScriptNamespace);
                    }
                    if (string.CompareOrdinal(codeTitle.Namespace, options.ScriptNamespace) == 0)
                    {
                        var scriptPage = await IPage<Article>
                            .GetExistingPageAsync<Article>(dataStore, codeTitle, true, false)
                            .ConfigureAwait(false);
                        script = scriptPage?.MarkdownContent;
                    }
                }
                if (!string.IsNullOrWhiteSpace(script))
                {
                    parameterValues["code"] = script;
                }
            }
            return (func.Invoke(options, parameterValues, title, isTemplate, isPreview, isTalk),
                transcludedPages);
        }

        if (depth >= TransclusionMaxDepth)
        {
            return (template, transcludedPages);
        }

        var pageTitle = PageTitle.Parse(reference);
        if (string.IsNullOrEmpty(pageTitle.Namespace))
        {
            pageTitle = pageTitle.WithNamespace(options.TransclusionNamespace);
        }
        else if (string.CompareOrdinal(pageTitle.Namespace, "-") == 0)
        {
            pageTitle = pageTitle.WithNamespace(null);
        }
        else if (string.CompareOrdinal(pageTitle.Namespace, "\\-") == 0)
        {
            pageTitle = pageTitle.WithNamespace("-");
        }
        transcludedPages.Add(pageTitle);

        var page = await IPage<Page>.GetExistingPageAsync<Page>(
            dataStore,
            pageTitle,
            true,
            false)
            .ConfigureAwait(false);
        if (page is null
            || page.AllowedViewers?.Count > 0)
        {
            return (template, transcludedPages);
        }

        parameterValues = ParseParameters(parameters);

        var (pageContent, pageTransclusions) = await TranscludeInnerAsync(
            options,
            dataStore,
            title,
            page.MarkdownContent,
            isTemplate: true,
            isPreview,
            isTalk,
            parameterValues);

        transcludedPages = transcludedPages.Union(pageTransclusions).ToList();

        return (pageContent ?? string.Empty, transcludedPages);
    }

    private static List<Transclusion> Parse(StringSlice slice, out List<Transclusion> parameterInclusions, out bool isCodeFence)
    {
        isCodeFence = false;

        parameterInclusions = new List<Transclusion>();
        var transclusions = new List<Transclusion>();
        if (slice.IsEmptyOrWhitespace())
        {
            return transclusions;
        }

        var c = slice.CurrentChar;

        var transclusionOpenerIndexes = new Stack<int>();
        var parameterOpenerIndexes = new Stack<int>();
        var isCodeBlock = false;
        var leadingSpaces = 0;
        var codeFenceCharacters = 0;
        while (c != '\0')
        {
            if (leadingSpaces > 0)
            {
                if (c.IsWhitespace())
                {
                    leadingSpaces++;
                    if (leadingSpaces >= 4)
                    {
                        return transclusions;
                    }
                }
                else
                {
                    leadingSpaces = 0;
                }
            }
            if (c == CodeBlockChar)
            {
                if (codeFenceCharacters > 0)
                {
                    codeFenceCharacters++;
                    if (codeFenceCharacters >= 3)
                    {
                        isCodeFence = true;
                        return transclusions;
                    }
                }
                if (isCodeBlock)
                {
                    isCodeBlock = false;
                }
                else if (slice.IndexOf(CodeBlockChar) != -1)
                {
                    isCodeBlock = true;
                }
            }
            else
            {
                codeFenceCharacters = 0;
                if (!isCodeBlock)
                {
                    if (c == TransclusionOpenChar)
                    {
                        if (slice.PeekChar() == TransclusionOpenChar)
                        {
                            transclusionOpenerIndexes.Push(slice.Start);
                            slice.NextChar();
                        }
                    }
                    else if (c == ParameterOpenChar)
                    {
                        if (slice.PeekChar() == ParameterOpenChar)
                        {
                            parameterOpenerIndexes.Push(slice.Start);
                            slice.NextChar();
                        }
                    }
                    else if (c == TransclusionCloseChar)
                    {
                        if (slice.PeekChar() == TransclusionCloseChar)
                        {
                            if (transclusionOpenerIndexes.Count > 0)
                            {
                                var transclusionOpener = transclusionOpenerIndexes.Count > 0 ? transclusionOpenerIndexes.Peek() : -1;
                                var parameterOpener = parameterOpenerIndexes.Count > 0 ? parameterOpenerIndexes.Peek() : -1;
                                if (transclusionOpener > parameterOpener && transclusionOpener != -1)
                                {
                                    transclusions.Add(new Transclusion(
                                        transclusionOpenerIndexes.Pop(),
                                        slice.Start + 2));
                                }
                            }
                            slice.NextChar();
                        }
                    }
                    else if (c == ParameterCloseChar)
                    {
                        if (slice.PeekChar() == ParameterCloseChar)
                        {
                            if (parameterOpenerIndexes.Count > 0)
                            {
                                var transclusionOpener = transclusionOpenerIndexes.Count > 0 ? transclusionOpenerIndexes.Peek() : -1;
                                var parameterOpener = parameterOpenerIndexes.Count > 0 ? parameterOpenerIndexes.Peek() : -1;
                                if (parameterOpener > transclusionOpener && parameterOpener != -1)
                                {
                                    parameterInclusions.Add(new Transclusion(
                                        parameterOpenerIndexes.Pop(),
                                        slice.Start + 2));
                                }
                            }
                            slice.NextChar();
                        }
                    }
                }
            }
            c = slice.NextChar();
        }

        return transclusions;
    }

    private static Dictionary<string, string> ParseParameters(ReadOnlySpan<char> span)
    {
        var parameters = new Dictionary<string, string>();
        if (span.Length == 0)
        {
            return parameters;
        }

        var index = 1;
        var separatorIndex = -1;
        do
        {
            if (separatorIndex != -1 && separatorIndex < span.Length - 1)
            {
                span = span[(separatorIndex + 1)..];
            }
            separatorIndex = span.IndexOfUnescaped(SeparatorChar);
            if (separatorIndex != -1) // skip separators inside links
            {
                var linkIndex = span.IndexOfUnescaped(WikiLinks.WikiLinkInlineParser.LinkOpenChar);
                if (linkIndex != -1
                    && linkIndex < separatorIndex
                    && linkIndex < span.Length - 4
                    && span[linkIndex + 1] == WikiLinks.WikiLinkInlineParser.LinkOpenChar)
                {
                    var linkCloseIndex = linkIndex + 2 + span[(linkIndex + 2)..].IndexOfUnescaped(WikiLinks.WikiLinkInlineParser.LinkCloseChar);
                    if (linkCloseIndex != -1
                        && linkCloseIndex < span.Length - 1
                        && span[linkCloseIndex + 1] == WikiLinks.WikiLinkInlineParser.LinkCloseChar)
                    {
                        separatorIndex = linkCloseIndex == span.Length - 2
                            ? -1
                            : linkCloseIndex + 2 + span[(linkCloseIndex + 2)..].IndexOfUnescaped(SeparatorChar);
                    }
                }
            }
            var parameter = separatorIndex == -1
                ? span
                : span[..separatorIndex];

            var equalIndex = parameter.IndexOfUnescaped('=');
            string name;
            ReadOnlySpan<char> value;
            if (equalIndex == -1)
            {
                name = index.ToString();
                value = parameter;
                index++;
            }
            else if (equalIndex == 0)
            {
                name = index.ToString();
                value = parameter[1..];
                index++;
            }
            else if (equalIndex == parameter.Length - 1)
            {
                name = parameter[..equalIndex].ToString();
                value = new ReadOnlySpan<char>();
            }
            else
            {
                name = parameter[..equalIndex].ToString();
                value = parameter[(equalIndex + 1)..];
            }
            parameters[name.Trim().ToLower()] = value.Trim().ToString();
        }
        while (separatorIndex != -1 && separatorIndex < span.Length - 1);
        if (separatorIndex == span.Length - 1)
        {
            parameters[index.ToString()] = string.Empty;
        }

        return parameters;
    }

    private static string Render(
        string markdown,
        int offset,
        List<Transclusion> transclusions,
        List<Transclusion> parameterInclusions)
    {
        var sb = new StringBuilder();
        var endIndex = 0;
        var transclusionIndex = 0;
        var parameterIndex = 0;
        while (endIndex < markdown.Length)
        {
            if (parameterIndex >= parameterInclusions.Count
                && transclusionIndex >= transclusions.Count)
            {
                sb.Append(markdown[endIndex..]);
                break;
            }

            var isTransclusion = parameterIndex >= parameterInclusions.Count
                || (transclusions.Count > transclusionIndex
                && transclusions[transclusionIndex].Start < parameterInclusions[parameterIndex].Start);
            var transclusion = isTransclusion
                ? transclusions[transclusionIndex]
                : parameterInclusions[parameterIndex];
            while (transclusionIndex < transclusions.Count - 1
                && transclusions[transclusionIndex + 1].Start < transclusion.End)
            {
                transclusionIndex++;
            }
            while (parameterIndex < parameterInclusions.Count - 1
                && parameterInclusions[parameterIndex + 1].Start < transclusion.End)
            {
                parameterIndex++;
            }

            if ((transclusion.Start - offset) > endIndex)
            {
                sb.Append(markdown[endIndex..(transclusion.Start - offset)]);
            }

            if (transclusion.Content is null)
            {
                sb.Append(markdown[(transclusion.Start - offset)..(transclusion.End - offset)]);
            }
            else
            {
                sb.Append(transclusion.Content);
            }

            endIndex = transclusion.End - offset;
            if (isTransclusion)
            {
                transclusionIndex++;
            }
            else
            {
                parameterIndex++;
            }
            while (transclusionIndex < transclusions.Count
                && (transclusions[transclusionIndex].Start - offset) < endIndex)
            {
                transclusionIndex++;
            }
            while (parameterIndex < parameterInclusions.Count
                && (parameterInclusions[parameterIndex].Start - offset) < endIndex)
            {
                parameterIndex++;
            }
        }

        return sb.ToString();
    }
}
