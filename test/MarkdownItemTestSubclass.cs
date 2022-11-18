using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Test;

public class MarkdownItemPolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Type == typeof(MarkdownItem))
        {
            jsonTypeInfo.PolymorphismOptions!.DerivedTypes.Add(
                new(typeof(MarkdownItemTestSubclass), ":TestSubclass:"));
        }
        return jsonTypeInfo;
    }
}

/// <summary>
/// Test subclass of <see cref="MarkdownItem"/>.
/// </summary>
public class MarkdownItemTestSubclass : MarkdownItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    public MarkdownItemTestSubclass() { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    public MarkdownItemTestSubclass(string id) : base(id) { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="markdownContent">The raw markdown.</param>
    /// <param name="html">The rendered HTML content.</param>
    /// <param name="preview">A preview of this item's rendered HTML.</param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers.
    /// </remarks>
    public MarkdownItemTestSubclass(string id, string? markdownContent, string html, string preview, IReadOnlyCollection<WikiLink> wikiLinks)
        : base(id, markdownContent, html, preview, wikiLinks)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="MarkdownItemTestSubclass"/>.
    /// </summary>
    /// <param name="markdown">The raw markdown.</param>
    /// <param name="html">
    /// The rendered HTML content.
    /// </param>
    /// <param name="preview">
    /// A preview of this item's rendered HTML.
    /// </param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    public MarkdownItemTestSubclass(string? markdown, string? html, string? preview, IReadOnlyCollection<WikiLink> wikiLinks)
        : base(markdown, html, preview, wikiLinks)
    { }

    public static async Task<MarkdownItemTestSubclass> NewAsync(WikiOptions options, IDataStore dataStore, string? markdown)
    {
        var md = string.IsNullOrEmpty(markdown)
            ? null
            : await TransclusionParser.TranscludeAsync(
                options,
                dataStore,
                null,
                markdown);
        var wikiLinks = GetWikiLinks(options, dataStore, md);
        return new MarkdownItemTestSubclass(
            md,
            RenderHtml(options, dataStore, md),
            RenderPreview(
                options,
                dataStore,
                string.IsNullOrEmpty(markdown)
                    ? string.Empty
                    : await TransclusionParser.TranscludeAsync(
                        options,
                        dataStore,
                        null,
                        markdown,
                        isPreview: true)),
            wikiLinks);
    }
}
