using Microsoft.CodeAnalysis.CSharp.Scripting;
using NeverFoundry.MathAndScience;
using NeverFoundry.Wiki.MarkdownExtensions.TableOfContents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MarkdownExtensions.Transclusions
{
    internal static class TransclusionFunctions
    {
        internal const string PreviewClass = "wiki-preview";

        private const int ScriptExecutionTimeoutInMilliseconds = 5000;

        internal static readonly Dictionary<string, Func<Dictionary<string, string>, string?, string?, bool, bool, string>> _Functions
            = new Dictionary<string, Func<Dictionary<string, string>, string?, string?, bool, bool, string>>
            {
                ["exec"] = Exec,
                ["format"] = Format,
                ["fullpagename"] = (_, __, fullTitle, ___, ____) => fullTitle ?? string.Empty,
                ["if"] = If,
                ["ifeq"] = IfEq,
                ["ifnottemplate"] = IfNotTemplate,
                ["iftemplate"] = IfTemplate,
                ["notoc"] = (_, __, ___, ____, _____) => "<!-- NOTOC -->",
                ["padleft"] = PadLeft,
                ["padright"] = PadRight,
                ["pagename"] = (_, title, __, ___, ____) => title ?? string.Empty,
                ["preview"] = Preview,
                ["toc"] = TableOfContents,
                ["tolower"] = (parameters, _, __, ___, ____) => parameters.TryGetValue("1", out var value) ? value.ToLower() : string.Empty,
                ["totitlecase"] = TitleCase,
                ["toupper"] = (parameters, _, __, ___, ____) => parameters.TryGetValue("1", out var value) ? value.ToUpper() : string.Empty,
            };

        private static string Exec(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (parameters.Count == 0)
            {
                return string.Empty;
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

            var state = Task.Run(() => CSharpScript.RunAsync("", Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default.WithImports("System", "System.Globalization"))).GetAwaiter().GetResult();
            foreach (var (name, value) in numberedParameters)
            {
                try
                {
                    if (double.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var _{name} = {value};")).GetAwaiter().GetResult();
                    }
                    else if (bool.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var _{name} = {value.ToLowerInvariant()};")).GetAwaiter().GetResult();
                    }
                    else if (DateTimeOffset.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var _{name} = DateTimeOffset.Parse({value});")).GetAwaiter().GetResult();
                    }
                    else
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var _{name} = \"{value}\";")).GetAwaiter().GetResult();
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
            foreach (var (name, value) in namedParameters)
            {
                try
                {
                    if (double.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var {name} = {value};")).GetAwaiter().GetResult();
                    }
                    else if (bool.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var {name} = {value.ToLowerInvariant()};")).GetAwaiter().GetResult();
                    }
                    else if (DateTimeOffset.TryParse(value, out var _))
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var {name} = DateTimeOffset.Parse({value});")).GetAwaiter().GetResult();
                    }
                    else
                    {
                        state = Task.Run(() => state.ContinueWithAsync($"var {name} = \"{value}\";")).GetAwaiter().GetResult();
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }

            try
            {
                var cts = new CancellationTokenSource(ScriptExecutionTimeoutInMilliseconds);
                state = Task.Run(() => state.ContinueWithAsync(code, cancellationToken: cts.Token)).GetAwaiter().GetResult();
            }
            catch
            {
                return string.Empty;
            }
            return state.ReturnValue?.ToString() ?? string.Empty;
        }

        private static string Format(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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

        private static string If(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
        {
            if (!parameters.TryGetValue("1", out var first)
                || string.IsNullOrWhiteSpace(first))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("2", out var second))
            {
                return string.Empty;
            }
            if (!parameters.TryGetValue("3", out var elseValue))
            {
                elseValue = string.Empty;
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
                return elseValue;
            }
        }

        private static string IfEq(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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
                return firstFloatValue.IsNearlyEqualTo(secondFloatValue)
                    ? ifValue
                    : elseValue;
            }
            else
            {
                return elseValue;
            }
        }

        private static string IfNotTemplate(Dictionary<string, string> parameters, string? _, string? __, bool isTemplate, bool ___)
        {
            if (isTemplate)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var first) ? first : string.Empty;
        }

        private static string IfTemplate(Dictionary<string, string> parameters, string? _, string? __, bool isTemplate, bool ___)
        {
            if (!isTemplate)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var first) ? first : string.Empty;
        }

        private static string PadLeft(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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

        private static string PadRight(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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

        private static string Preview(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool isPreview)
        {
            if (!isPreview)
            {
                return string.Empty;
            }
            return parameters.TryGetValue("1", out var value)
                ? $"::{value}::{{.{PreviewClass}}}"
                : string.Empty;
        }

        private static string TableOfContents(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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

        private static string TitleCase(Dictionary<string, string> parameters, string? _, string? __, bool ___, bool ____)
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
