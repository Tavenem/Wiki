﻿using HandlebarsDotNet;
using Jint;
using System.Globalization;
using System.Text;
using Tavenem.Wiki.MarkdownExtensions.TableOfContents;

namespace Tavenem.Wiki.MarkdownExtensions.Transclusions;

internal static class TransclusionPreprocessor
{
    internal const string PreviewClass = "wiki-preview";

    private const int ScriptExecutionTimeoutInSeconds = 5;
    private const int ScriptMemoryLimitInBytes = 10_000_000;
    private const int ScriptRecursionLimit = 100;
    private const int ScriptStatementLimit = 10000;

    private static readonly IHandlebars _context;

    static TransclusionPreprocessor()
    {
        _context = Handlebars.Create();
        using (_context.Configure())
        {
            _context.Configuration.MissingPartialTemplateHandler = new MissingPartialHandler();

            _context.RegisterHelper(
                "domain",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (context["title"] is PageTitle title)
                    {
                        output.Write(title.Domain);
                    }
                });

            _context.RegisterHelper(
                "exec",
                (
                    EncodedTextWriter output,
                    BlockHelperOptions options,
                    Context context,
                    Arguments arguments
                ) =>
                {
                    var script = options.Template();
                    if (string.IsNullOrWhiteSpace(script))
                    {
                        return;
                    }
                    output.Write(
                        GetScriptResult(
                            script,
                            false,
                            options,
                            context,
                            arguments),
                        !arguments.Hash.TryGetValue("safe", out var v)
                            || v is not bool s
                            || !s);
                });

            _context.RegisterHelper(
                "eval",
                (
                    EncodedTextWriter output,
                    BlockHelperOptions options,
                    Context context,
                    Arguments arguments
                ) =>
                {
                    var script = options.Template();
                    if (string.IsNullOrWhiteSpace(script))
                    {
                        return;
                    }
                    output.Write(
                        GetScriptResult(
                            script,
                            true,
                            options,
                            context,
                            arguments),
                        !arguments.Hash.TryGetValue("safe", out var v)
                            || v is not bool s
                            || !s);
                });

            _context.RegisterHelper(
                "format",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (first is null)
                    {
                        return;
                    }
                    var format = arguments.Length > 1
                        ? arguments.At<string>(1)
                        : null;
                    if (long.TryParse(first, out var intValue))
                    {
                        try
                        {
                            output.Write(intValue.ToString(format ?? "N0"));
                            return;
                        }
                        catch
                        {
                            if (format is null)
                            {
                                output.Write(intValue.ToString("N0"));
                                return;
                            }
                        }
                    }
                    else if (double.TryParse(first, out var floatValue))
                    {
                        try
                        {
                            output.Write(floatValue.ToString(format ?? "N"));
                            return;
                        }
                        catch
                        {
                            if (format is null)
                            {
                                output.Write(floatValue.ToString("N"));
                                return;
                            }
                        }
                    }
                    else if (DateTimeOffset.TryParse(first, out var timestamp))
                    {
                        try
                        {
                            output.Write(timestamp.ToString(format ?? "g"));
                            return;
                        }
                        catch
                        {
                            if (format is null)
                            {
                                output.Write(timestamp.ToString("g"));
                                return;
                            }
                        }
                    }
                    output.Write(first);
                });

            _context.RegisterHelper(
                "fullpagename",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (context["title"] is not PageTitle title)
                    {
                        return;
                    }
                    if (options.Data["options"] is WikiOptions wikiOptions)
                    {
                        output.Write(title.ToString(wikiOptions));
                    }
                    else
                    {
                        output.Write(title.ToString());
                    }
                });

            _context.RegisterHelper(
                "ifequal",
                (
                    EncodedTextWriter output,
                    BlockHelperOptions options,
                    Context context,
                    Arguments arguments
                ) =>
                {
                    if (arguments.Length < 2)
                    {
                        return;
                    }

                    if ((arguments[0] is bool bool1
                        || bool.TryParse(arguments[0].ToString(), out bool1))
                        && (arguments[1] is bool bool2
                        || bool.TryParse(arguments[1].ToString(), out bool2)))
                    {
                        if (bool1 == bool2)
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                    }
                    else if ((arguments[0] is long long1
                        || long.TryParse(arguments[0].ToString(), out long1))
                        && (arguments[1] is long long2
                        || long.TryParse(arguments[1].ToString(), out long2)))
                    {
                        if (long1 == long2)
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                    }
                    else if ((arguments[0] is double double1
                        || double.TryParse(arguments[0].ToString(), out double1))
                        && (arguments[1] is double double2
                        || double.TryParse(arguments[1].ToString(), out double2)))
                    {
                        if (double1 == double2)
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            var diff = Math.Abs(double1 - double2);
                            if (diff < double1 * 1e-15)
                            {
                                options.Template(output, context);
                            }
                            else
                            {
                                options.Inverse(output, context);
                            }
                        }
                    }
                    else if (arguments.At<string>(0) is string str1
                        && arguments.At<string>(1) is string str2
                        && string.CompareOrdinal(str1, str2) == 0)
                    {
                        options.Template(output, context);
                    }
                    else
                    {
                        options.Inverse(output, context);
                    }
                });

            _context.RegisterHelper(
                "lowercase",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (string.IsNullOrEmpty(first))
                    {
                        return;
                    }
                    output.Write(CultureInfo.CurrentCulture.TextInfo.ToLower(first));
                });

            _context.RegisterHelper(
                "namespace",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (context["title"] is PageTitle title)
                    {
                        output.Write(title.Namespace);
                    }
                });

            _context.RegisterHelper(
                "notoc",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) => output.WriteSafeString("<!-- NOTOC -->"));

            _context.RegisterHelper(
                helperName: "padleft",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (string.IsNullOrEmpty(first))
                    {
                        return;
                    }
                    if (arguments.Length < 2)
                    {
                        output.Write(first);
                        return;
                    }
                    var number = arguments.At<int>(1);
                    if (number == 0)
                    {
                        output.Write(first);
                    }
                    else if (arguments.Length > 2)
                    {
                        output.Write(first.PadLeft(number, arguments.At<string>(2)?[0] ?? ' '));
                    }
                    else
                    {
                        output.Write(first.PadLeft(number));
                    }
                });

            _context.RegisterHelper(
                helperName: "padright",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (string.IsNullOrEmpty(first))
                    {
                        return;
                    }
                    if (arguments.Length < 2)
                    {
                        output.Write(first);
                        return;
                    }
                    var number = arguments.At<int>(1);
                    if (number == 0)
                    {
                        output.Write(first);
                    }
                    else if (arguments.Length > 2)
                    {
                        output.Write(first.PadRight(number, arguments.At<string>(2)?[0] ?? ' '));
                    }
                    else
                    {
                        output.Write(first.PadRight(number));
                    }
                });

            _context.RegisterHelper(
                "pagename",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (context["title"] is PageTitle title)
                    {
                        output.Write(title.Title);
                    }
                    else if (options.Data["options"] is WikiOptions wikiOptions)
                    {
                        output.Write(wikiOptions.MainPageTitle);
                    }
                });

            _context.RegisterHelper(
                "preview",
                (
                    EncodedTextWriter output,
                    BlockHelperOptions options,
                    Context context,
                    Arguments arguments
                ) =>
                {
                    if (context["isPreview"] is not bool isPreview || !isPreview)
                    {
                        return;
                    }

                    output.Write(":::");
                    output.Write(PreviewClass);
                    output.Write(Environment.NewLine);
                    options.Template(output, context);
                    output.Write(Environment.NewLine);
                    output.Write(":::");
                });

            _context.RegisterHelper(
                "toc",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    var depth = arguments.Length > 0
                        ? arguments.At<string>(0)
                        : "*";
                    if (string.IsNullOrWhiteSpace(depth))
                    {
                        depth = "*";
                    }

                    var startingLevel = arguments.Length > 1
                        ? arguments.At<string>(1)
                        : "*";
                    if (string.IsNullOrWhiteSpace(startingLevel))
                    {
                        startingLevel = "*";
                    }

                    output.WriteSafeString(string.Format(
                        TableOfContentsExtension.ToCFormat,
                        depth,
                        startingLevel,
                        arguments.Length > 2
                            ? arguments.At<string>(2)
                            : null));
                });

            _context.RegisterHelper(
                "titlecase",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (string.IsNullOrEmpty(first))
                    {
                        return;
                    }
                    var second = arguments.Length > 1
                        ? arguments.At<string>(1)
                        : null;
                    if (second is null)
                    {
                        output.Write(first.ToWikiTitleCase());
                        return;
                    }

                    if (bool.TryParse(second, out var b))
                    {
                        if (b)
                        {
                            output.Write(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(first));
                        }
                    }
                    else if (double.TryParse(second, out var n)
                        && n > 0)
                    {
                        output.Write(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(first));
                    }
                });

            _context.RegisterHelper(
                "uppercase",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) =>
                {
                    if (arguments.Length == 0)
                    {
                        return;
                    }
                    var first = arguments.At<string>(0);
                    if (string.IsNullOrEmpty(first))
                    {
                        return;
                    }
                    output.Write(CultureInfo.CurrentCulture.TextInfo.ToUpper(first));
                });

            _context.RegisterHelper(
                "helperMissing",
                (
                    in EncodedTextWriter output,
                    in HelperOptions options,
                    in Context context,
                    in Arguments arguments
                ) => output.Write($"{{{{{options.Name}}}}}")); // Return the original transclusion as written
        }
        _context = _context.CreateSharedEnvironment();
    }

    /// <summary>
    /// Replaces all the non-page transclusions in the given <paramref name="markdown"/> with their
    /// contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="title">The title of the top-level article being generated.</param>
    /// <param name="markdown">A markdown string.</param>
    /// <param name="isTemplate">Whether the article is being rendered as a transclusion.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <param name="arguments">
    /// A collection of supplied arguments, if the markdown is itself a transclusion.
    /// </param>
    /// <param name="parameters">
    /// A collection of supplied parameter values, if the markdown is itself a transclusion.
    /// </param>
    /// <returns>
    /// The markdown with all transclusions substituted.
    /// </returns>
    public static string Transclude(
        WikiOptions options,
        PageTitle? title,
        string markdown,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        List<object>? arguments = null,
        Dictionary<string, object>? parameters = null)
    {
        var context = new Dictionary<string, object>();
        if (parameters is not null)
        {
            foreach (var key in parameters.Keys)
            {
                context[key] = parameters[key];
            }
        }
        if (arguments is not null)
        {
            context["args"] = arguments;
        }
        context["isCategory"] = title?.Namespace?.Equals(options.CategoryNamespace) == true;
        context["isTemplate"] = isTemplate;
        context["isPreview"] = isPreview;
        context["isTalk"] = isTalk;
        context["title"] = title ?? new();

        var data = new { options };

        try
        {
            return _context.Compile(markdown)(context, data);
        }
        catch
        {
            return markdown;
        }
    }

    /// <summary>
    /// Replaces all the non-page transclusions in the given <paramref name="markdown"/> with their
    /// contents.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="title">The title of the top-level article being generated.</param>
    /// <param name="markdown">A <see cref="StringBuilder"/> containing markdown.</param>
    /// <param name="isTemplate">Whether the article is being rendered as a transclusion.</param>
    /// <param name="isPreview">Whether a preview is being rendered.</param>
    /// <param name="isTalk">Whether the content is a message.</param>
    /// <param name="arguments">
    /// A collection of supplied arguments, if the markdown is itself a transclusion.
    /// </param>
    /// <param name="parameters">
    /// A collection of supplied parameter values, if the markdown is itself a transclusion.
    /// </param>
    /// <returns>
    /// The markdown with all transclusions substituted.
    /// </returns>
    public static string Transclude(
        WikiOptions options,
        PageTitle? title,
        StringBuilder markdown,
        bool isTemplate = false,
        bool isPreview = false,
        bool isTalk = false,
        List<object>? arguments = null,
        Dictionary<string, object>? parameters = null)
    {
        var context = new Dictionary<string, object>();
        if (parameters is not null)
        {
            foreach (var key in parameters.Keys)
            {
                context[key] = parameters[key];
            }
        }
        if (arguments is not null)
        {
            context["args"] = arguments;
        }
        context["isCategory"] = title?.Namespace?.Equals(options.CategoryNamespace) == true;
        context["isTemplate"] = isTemplate;
        context["isPreview"] = isPreview;
        context["isTalk"] = isTalk;
        context["title"] = title ?? new();

        var data = new { options };

        try
        {
            using var writer = new StringWriter();
            using var reader = new StringBuilderReader(markdown);
            _context.Compile(reader)(writer, context, data);
            return writer.ToString();
        }
        catch
        {
            return markdown.ToString();
        }
    }

    private static Engine GetEngine() => new(options =>
    {
        options.LimitMemory(ScriptMemoryLimitInBytes);
        options.LimitRecursion(ScriptRecursionLimit);
        options.MaxStatements(ScriptStatementLimit);
        options.TimeoutInterval(TimeSpan.FromSeconds(ScriptExecutionTimeoutInSeconds));
    });

    private static string GetScriptResult(
        string code,
        bool autoReturn,
        in BlockHelperOptions options,
        in Context context,
        in Arguments arguments)
    {
        var scriptCode = new StringBuilder("(function(){")
            .AppendLine();

        var wikiOptions = options.Data["options"] is WikiOptions wikiOptions1 ? wikiOptions1 : null;
        if (wikiOptions is not null)
        {
            scriptCode.AppendLine("const options = {")
                .Append("AboutPageTitle: \"")
                .Append(wikiOptions.AboutPageTitle)
                .AppendLine("\",")
                .Append("AdminNamespaces: [");
            foreach (var wikiNamespace in wikiOptions.AdminNamespaces)
            {
                scriptCode
                    .Append('"')
                    .Append(wikiNamespace)
                    .Append("\",");
            }
            scriptCode
                .AppendLine("],")
                .Append("CategoriesTitle: \"")
                .Append(wikiOptions.CategoriesTitle)
                .AppendLine("\",")
                .Append("CategoryNamespace: \"")
                .Append(wikiOptions.CategoryNamespace)
                .AppendLine("\",")
                .Append("ContactPageTitle: \"")
                .Append(wikiOptions.ContactPageTitle)
                .AppendLine("\",")
                .Append("ContentsPageTitle: \"")
                .Append(wikiOptions.ContentsPageTitle)
                .AppendLine("\",")
                .Append("CopyrightPageTitle: \"")
                .Append(wikiOptions.CopyrightPageTitle)
                .AppendLine("\",");
            if (wikiOptions.CustomAdminNamespaces is not null)
            {
                scriptCode
                    .Append("CustomAdminNamespaces: [");
                foreach (var wikiNamespace in wikiOptions.CustomAdminNamespaces)
                {
                    scriptCode
                        .Append('"')
                        .Append(wikiNamespace)
                        .Append("\",");
                }
                scriptCode
                    .AppendLine("],");
            }
            if (wikiOptions.CustomReservedNamespaces is not null)
            {
                scriptCode
                    .Append("CustomReservedNamespaces: [");
                foreach (var wikiNamespace in wikiOptions.CustomReservedNamespaces)
                {
                    scriptCode
                        .Append('"')
                        .Append(wikiNamespace)
                        .Append("\",");
                }
                scriptCode
                    .AppendLine("],");
            }
            scriptCode
                .Append("DefaultAnonymousPermission: ")
                .Append((int)wikiOptions.DefaultAnonymousPermission)
                .AppendLine(",")
                .Append("DefaultRegisteredPermission: ")
                .Append((int)wikiOptions.DefaultRegisteredPermission)
                .AppendLine(",")
                .Append("DefaultTableOfContentsDepth: ")
                .Append(wikiOptions.DefaultTableOfContentsDepth)
                .AppendLine(",")
                .Append("DefaultTableOfContentsTitle: \"")
                .Append(wikiOptions.DefaultTableOfContentsTitle)
                .AppendLine("\",")
                .Append("FileNamespace: \"")
                .Append(wikiOptions.FileNamespace)
                .AppendLine("\",")
                .Append("GroupNamespace: \"")
                .Append(wikiOptions.GroupNamespace)
                .AppendLine("\",")
                .Append("HelpPageTitle: \"")
                .Append(wikiOptions.HelpPageTitle)
                .AppendLine("\",")
                .Append("LinkTemplate: \"")
                .Append(wikiOptions.LinkTemplate)
                .AppendLine("\",")
                .Append("MainPageTitle: \"")
                .Append(wikiOptions.MainPageTitle)
                .AppendLine("\",")
                .Append("MaxFileSize: ")
                .Append(wikiOptions.MaxFileSize)
                .AppendLine(",")
                .Append("MaxFileSizeString: \"")
                .Append(wikiOptions.MaxFileSizeString)
                .AppendLine("\",")
                .Append("MinimumTableOfContentsHeadings: ")
                .Append(wikiOptions.MinimumTableOfContentsHeadings)
                .AppendLine(",")
                .Append("PolicyPageTitle: \"")
                .Append(wikiOptions.PolicyPageTitle)
                .AppendLine("\",")
                .Append("ReservedNamespaces: [");
            foreach (var wikiNamespace in wikiOptions.ReservedNamespaces)
            {
                scriptCode
                    .Append('"')
                    .Append(wikiNamespace)
                    .Append("\",");
            }
            scriptCode
                .AppendLine("],")
                .Append("ScriptNamespace: \"")
                .Append(wikiOptions.ScriptNamespace)
                .AppendLine("\",")
                .Append("SiteName: \"")
                .Append(wikiOptions.SiteName)
                .AppendLine("\",")
                .Append("SystemNamespace: \"")
                .Append(wikiOptions.SystemNamespace)
                .AppendLine("\",")
                .Append("TransclusionNamespace: \"")
                .Append(wikiOptions.TransclusionNamespace)
                .AppendLine("\",")
                .Append("UserNamespace: \"")
                .Append(wikiOptions.UserNamespace)
                .AppendLine("\",")
                .Append("UserDomains: ")
                .Append(wikiOptions.UserDomains.ToString().ToLowerInvariant())
                .AppendLine(",")
                .Append("WikiLinkPrefix: \"")
                .Append(wikiOptions.WikiLinkPrefix)
                .AppendLine("\",")
                .AppendLine("};");
        }

        var title = context["title"] is PageTitle pageTitle ? pageTitle : new();

        scriptCode.Append("const title = \"")
            .Append(title.Title)
            .AppendLine("\";")
            .Append("const pagename = \"")
            .Append(title.Title)
            .AppendLine("\";")
            .Append("const namespace = \"")
            .Append(title.Namespace)
            .AppendLine("\";")
            .Append("const domain = \"")
            .Append(title.Domain)
            .AppendLine("\";")
            .Append("const fullpagename = \"")
            .Append(wikiOptions is null ? title.ToString() : title.ToString(wikiOptions))
            .AppendLine("\";");

        scriptCode.AppendLine("const args = [];");
        var i = 0;
        foreach (var argument in arguments)
        {
            if (argument is KeyValuePair<string, object>)
            {
                continue;
            }

            scriptCode.Append("args[")
                .Append(i)
                .Append("] = ");
            switch (argument)
            {
                case bool b:
                    scriptCode
                        .Append(b.ToString().ToLowerInvariant())
                        .AppendLine(";");
                    break;
                case DateTimeOffset date:
                    scriptCode
                        .Append("new Date(")
                        .Append(date)
                        .AppendLine(");");
                    break;
                case long l:
                    scriptCode
                        .Append(l)
                        .AppendLine(";");
                    break;
                case double d:
                    scriptCode
                        .Append(d)
                        .AppendLine(";");
                    break;
                default:
                    scriptCode
                        .Append('"')
                        .Append(argument)
                        .AppendLine("\";");
                    break;
            }
        }
        if (context["args"] is List<object> args)
        {
            for (; i < args.Count; i++)
            {
                scriptCode.Append("args[")
                    .Append(i)
                    .Append("] = ");
                switch (args[i])
                {
                    case bool b:
                        scriptCode
                            .Append(b.ToString().ToLowerInvariant())
                            .AppendLine(";");
                        break;
                    case DateTimeOffset date:
                        scriptCode
                            .Append("new Date(")
                            .Append(date)
                            .AppendLine(");");
                        break;
                    case long l:
                        scriptCode
                            .Append(l)
                            .AppendLine(";");
                        break;
                    case double d:
                        scriptCode
                            .Append(d)
                            .AppendLine(";");
                        break;
                    default:
                        scriptCode
                            .Append('"')
                            .Append(args[i])
                            .AppendLine("\";");
                        break;
                }
            }
        }

        foreach (var property in context.Properties)
        {
            if (property.TrimmedValue is "title" or "args")
            {
                continue;
            }

            scriptCode.Append("const ")
                .Append(property.TrimmedValue)
                .Append(" = ");
            switch (context[property])
            {
                case bool b:
                    scriptCode
                        .Append(b.ToString().ToLowerInvariant())
                        .AppendLine(";");
                    break;
                case DateTimeOffset date:
                    scriptCode
                        .Append("new Date(")
                        .Append(date)
                        .AppendLine(");");
                    break;
                case long l:
                    scriptCode
                        .Append(l)
                        .AppendLine(";");
                    break;
                case double d:
                    scriptCode
                        .Append(d)
                        .AppendLine(";");
                    break;
                default:
                    scriptCode
                        .Append('"')
                        .Append(context[property])
                        .AppendLine("\";");
                    break;
            }
        }
        foreach (var (name, value) in arguments.Hash)
        {
            scriptCode.Append("const ")
                .Append(name)
                .Append(" = ");
            switch (value)
            {
                case bool b:
                    scriptCode
                        .Append(b.ToString().ToLowerInvariant())
                        .AppendLine(";");
                    break;
                case DateTimeOffset date:
                    scriptCode
                        .Append("new Date(")
                        .Append(date)
                        .AppendLine(");");
                    break;
                case long l:
                    scriptCode
                        .Append(l)
                        .AppendLine(";");
                    break;
                case double d:
                    scriptCode
                        .Append(d)
                        .AppendLine(";");
                    break;
                default:
                    scriptCode
                        .Append('"')
                        .Append(value)
                        .AppendLine("\";");
                    break;
            }
        }

        if (autoReturn)
        {
            scriptCode.Append("return ");
        }
        scriptCode.Append(code);
        if (autoReturn)
        {
            scriptCode.AppendLine(";");
        }
        else
        {
            scriptCode.AppendLine();
        }
        scriptCode.AppendLine("})();");
        try
        {
            return GetEngine()
                .Evaluate(scriptCode.ToString())
                .ToString()
                ?? string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return string.Empty;
        }
    }
}
