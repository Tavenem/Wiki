using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NeverFoundry.Wiki.MarkdownExtensions.TableOfContents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MarkdownExtensions.Transclusions
{
    internal static class TransclusionFunctions
    {
        internal const string PreviewClass = "wiki-preview";

        private const int ScriptExecutionTimeoutInMilliseconds = 5000;

        internal static readonly Dictionary<string, Func<IWikiOptions, Dictionary<string, string>, string?, string?, bool, bool, string>> _Functions
            = new Dictionary<string, Func<IWikiOptions, Dictionary<string, string>, string?, string?, bool, bool, string>>
            {
                ["exec"] = Exec,
                ["format"] = Format,
                ["fullpagename"] = (_, _, _, fullTitle, _, _) => fullTitle ?? string.Empty,
                ["if"] = If,
                ["ifcategory"] = IfCategory,
                ["ifeq"] = IfEq,
                ["ifnottemplate"] = IfNotTemplate,
                ["iftalk"] = IfTalk,
                ["iftemplate"] = IfTemplate,
                ["namespace"] = (options, _, _, fullTitle, _, _) => Article.GetTitleParts(options, fullTitle).wikiNamespace,
                ["notoc"] = (_, _, _, _, _, _) => "<!-- NOTOC -->",
                ["padleft"] = PadLeft,
                ["padright"] = PadRight,
                ["pagename"] = (_, _, title, _, _, _) => title ?? string.Empty,
                ["preview"] = Preview,
                ["toc"] = TableOfContents,
                ["tolower"] = (_, parameters, _, _, _, _) => parameters.TryGetValue("1", out var value) ? value.ToLower() : string.Empty,
                ["totitlecase"] = TitleCase,
                ["toupper"] = (_, parameters, _, __, ___, ____) => parameters.TryGetValue("1", out var value) ? value.ToUpper() : string.Empty,
            };

        private static readonly SemaphoreSlim _ScriptLock = new SemaphoreSlim(1);
        private static ScriptState<object>? _BaseScriptState;

        private static string Exec(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (parameters.Count == 0)
            {
                return string.Empty;
            }

            if (_BaseScriptState is null)
            {
                _ScriptLock.Wait();
                _BaseScriptState ??= Task.Run(() => CSharpScript.RunAsync("", ScriptOptions.Default
                    .AddReferences(typeof(Enumerable).Assembly)
                    .WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text"))).GetAwaiter().GetResult();
                _ScriptLock.Release();
            }

            var numberedParameters = parameters.Where(x => int.TryParse(x.Key, out var _)).ToList();
            List<KeyValuePair<string, string>> namedParameters;
            if (parameters.TryGetValue("code", out var code))
            {
                namedParameters = parameters
                    .Where(x => x.Key != "code")
                    .Except(numberedParameters)
                    .ToList();
            }
            else
            {
                if (numberedParameters.Count == 0)
                {
                    return string.Empty;
                }
                namedParameters = parameters
                    .Except(numberedParameters)
                    .ToList();
                code = numberedParameters[^1].Value;
                numberedParameters.RemoveAt(numberedParameters.Count - 1);
            }
            var scriptCode = new StringBuilder();

            foreach (var (name, value) in numberedParameters)
            {
                scriptCode.Append("var _")
                    .Append(name)
                    .Append(" = ");
                if (double.TryParse(value, out var _))
                {
                    scriptCode.Append(value);
                }
                else if (bool.TryParse(value, out var b))
                {
                    scriptCode.Append(b);
                }
                else if (DateTimeOffset.TryParse(value, out var _))
                {
                    scriptCode.Append("DateTimeOffset.Parse(")
                        .Append(value)
                        .Append(')');
                }
                else
                {
                    scriptCode.Append('"')
                        .Append(value)
                        .Append('"');
                }
                scriptCode.AppendLine(";");
            }
            foreach (var (name, value) in namedParameters)
            {
                scriptCode.Append("var ")
                    .Append(name)
                    .Append(" = ");
                if (double.TryParse(value, out var _))
                {
                    scriptCode.Append(value);
                }
                else if (bool.TryParse(value, out var b))
                {
                    scriptCode.Append(b);
                }
                else if (DateTimeOffset.TryParse(value, out var _))
                {
                    scriptCode.Append("DateTimeOffset.Parse(")
                        .Append(value)
                        .Append(')');
                }
                else
                {
                    scriptCode.Append('"')
                        .Append(value)
                        .Append('"');
                }
                scriptCode.AppendLine(";");
            }

            scriptCode.Append(code);
            try
            {
                var cts = new CancellationTokenSource(ScriptExecutionTimeoutInMilliseconds);
                var state = Task.Run(() => _BaseScriptState.ContinueWithAsync(scriptCode.ToString(), cancellationToken: cts.Token)).GetAwaiter().GetResult();
                return state.ReturnValue?.ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string Format(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var first))
            {
                return string.Empty;
            }
            string? format = null;
            if (parameters.TryGetValue("2", out var second))
            {
                format = second;
            }
            var result = string.Empty;
            if (long.TryParse(first, out var intValue))
            {
                var success = false;
                try
                {
                    result = intValue.ToString(format ?? "N0");
                    success = true;
                }
                catch { }
                if (!success && format is null)
                {
                    result = intValue.ToString("N0");
                }
            }
            else if (double.TryParse(first, out var floatValue))
            {
                var success = false;
                try
                {
                    result = floatValue.ToString(format ?? "N");
                    success = true;
                }
                catch { }
                if (!success && format is null)
                {
                    result = floatValue.ToString("N");
                }
            }
            else if (DateTimeOffset.TryParse(first, out var timestamp))
            {
                var success = false;
                try
                {
                    result = timestamp.ToString(format ?? "g");
                    success = true;
                }
                catch { }
                if (!success && format is null)
                {
                    result = timestamp.ToString("g");
                }
            }
            else
            {
                result = first;
            }
            return result;
        }

        private static string If(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("2", out var second))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("3", out var elseValue))
            {
                elseValue = string.Empty;
            }
            if (!parameters.TryGetValue("1", out var first)
                || string.IsNullOrWhiteSpace(first))
            {
                return elseValue;
            }
            if (bool.TryParse(first, out var value))
            {
                return value ? second : elseValue;
            }
            else if (long.TryParse(first, out var intValue))
            {
                return intValue > 0 ? second : elseValue;
            }
            else if (double.TryParse(first, out var floatValue))
            {
                return floatValue > 0 ? second : elseValue;
            }
            else
            {
                return second;
            }
        }

        private static string IfCategory(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? fullTitle, bool __, bool ___)
        {
            if (Article.GetTitleParts(options, fullTitle).wikiNamespace != options.CategoryNamespace)
            {
                return parameters.TryGetValue("2", out var second) ? second : string.Empty;
            }
            return parameters.TryGetValue("1", out var first) ? first : string.Empty;
        }

        private static string IfEq(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var first))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("2", out var second))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("3", out var ifValue))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("4", out var elseValue))
            {
                elseValue = string.Empty;
            }
            if (string.CompareOrdinal(first, second) == 0)
            {
                return ifValue;
            }
            else if (bool.TryParse(first, out var firstValue)
                && bool.TryParse(second, out var secondValue))
            {
                return firstValue == secondValue ? ifValue : elseValue;
            }
            else if (long.TryParse(first, out var firstIntValue)
                && long.TryParse(second, out var secondIntValue))
            {
                return firstIntValue == secondIntValue ? ifValue : elseValue;
            }
            else if (double.TryParse(first, out var firstFloatValue)
                && double.TryParse(second, out var secondFloatValue))
            {
                if (firstFloatValue == secondFloatValue)
                {
                    return ifValue;
                }

                var diff = Math.Abs(firstFloatValue - secondFloatValue);
                return diff < firstFloatValue * 1e-15
                    ? ifValue
                    : elseValue;
            }
            else
            {
                return elseValue;
            }
        }

        private static string IfNotTemplate(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool isTemplate, bool ___)
        {
            if (isTemplate)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var first) ? first : string.Empty;
        }

        private static string IfTalk(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? fullTitle, bool __, bool ___)
        {
            if (Article.GetTitleParts(options, fullTitle).isTalk)
            {
                return parameters.TryGetValue("1", out var first) ? first : string.Empty;
            }
            return parameters.TryGetValue("2", out var second) ? second : string.Empty;
        }

        private static string IfTemplate(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool isTemplate, bool ___)
        {
            if (!isTemplate)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var first) ? first : string.Empty;
        }

        private static string PadLeft(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var value))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("2", out var second)
                || !int.TryParse(second, out var totalWidth))
            {
                return value;
            }
            var paddingChar = '0';
            if (parameters.TryGetValue("3", out var third)
                && third.Length >= 1)
            {
                paddingChar = third[0];
            }
            return value.PadLeft(totalWidth, paddingChar);
        }

        private static string PadRight(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var value))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("2", out var second)
                || !int.TryParse(second, out var totalWidth))
            {
                return value;
            }
            var paddingChar = '0';
            if (parameters.TryGetValue("3", out var third)
                && third.Length >= 1)
            {
                paddingChar = third[0];
            }
            return value.PadRight(totalWidth, paddingChar);
        }

        private static string Preview(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool isPreview)
        {
            if (!isPreview)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var value)
                ? $"::{value}::{{.{PreviewClass}}}"
                : string.Empty;
        }

        private static string TableOfContents(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            string depth;
            if (!parameters.TryGetValue("1", out var first)
                || string.IsNullOrWhiteSpace(first)
                || first == "*")
            {
                depth = "*";
            }
            else if (int.TryParse(first, out var d))
            {
                depth = d.ToString();
            }
            else
            {
                return string.Empty;
            }

            string startingLevel;
            if (!parameters.TryGetValue("2", out var second)
                || string.IsNullOrWhiteSpace(second)
                || second == "*")
            {
                startingLevel = "*";
            }
            else if (int.TryParse(second, out var s))
            {
                startingLevel = s.ToString();
            }
            else
            {
                return string.Empty;
            }

            var title = parameters.TryGetValue("3", out var third) ? third : string.Empty;

            return string.Format(TableOfContentsExtension.ToCFormat, depth, startingLevel, title);
        }

        private static string TitleCase(IWikiOptions options, Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var value))
            {
                return string.Empty;
            }
            if (parameters.TryGetValue("2", out var second))
            {
                if (bool.TryParse(second, out var b))
                {
                    if (b)
                    {
                        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                    }
                }
                else if (double.TryParse(second, out var n))
                {
                    if (n > 0)
                    {
                        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
                    }
                }
            }
            return value.ToWikiTitleCase();
        }
    }
}
