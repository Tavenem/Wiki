using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// Represents the collection of other wiki pages which transclude a given wiki page.
/// </summary>
public class PageTransclusions : IdItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string PageTransclusionsIdItemTypeName = ":PageTransclusions:";
    /// <summary>
    /// <para>
    /// A built-in, read-only type discriminator.
    /// </para>
    /// <para>
    /// The set accessor performs no function.
    /// </para>
    /// </summary>
    [JsonPropertyName("$type"), JsonPropertyOrder(-2)]
    public override string IdItemTypeName
    {
        get => PageTransclusionsIdItemTypeName;
        set { }
    }

    /// <summary>
    /// <para>
    /// The IDs of other wiki pages which transclude the primary wiki page.
    /// </para>
    /// <para>
    /// Does not include transclusions in discussion messages.
    /// </para>
    /// </summary>
    public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

    /// <summary>
    /// Initializes a new instance of <see cref="PageTransclusions"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="references">
    /// The IDs of other wiki pages which transclude the primary wiki page.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers. The static <see
    /// cref="NewAsync(IDataStore, string, string, string?, string)"/> method is expected to be used
    /// otherwise, as it persists instances to the <see cref="IDataStore"/> and builds the
    /// reference list dynamically.
    /// </remarks>
    [JsonConstructor]
    public PageTransclusions(
        string id,
        IReadOnlyList<string> references) : base(id)
        => References = references;

    /// <summary>
    /// Gets an ID for a <see cref="PageTransclusions"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The ID which should be used for a <see cref="PageTransclusions"/> given the parameters.
    /// </returns>
    public static string GetId(string title, string wikiNamespace, string? domain)
        => string.IsNullOrEmpty(domain)
        ? $"{wikiNamespace}:{title}:transclusions"
        : $"({domain}):{wikiNamespace}:{title}:transclusions";

    /// <summary>
    /// Gets the <see cref="PageTransclusions"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The <see cref="PageTransclusions"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    public static ValueTask<PageTransclusions?> GetPageTransclusionsAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        string? domain = null)
        => dataStore.GetItemAsync<PageTransclusions>(GetId(title, wikiNamespace, domain));

    /// <summary>
    /// Get a new instance of <see cref="PageTransclusions"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of the transcluded wiki page.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace of the transcluded wiki page.
    /// </param>
    /// <param name="domain">
    /// The domain of the transcluded wiki page (if any).
    /// </param>
    /// <param name="referenceId">
    /// The initial <see cref="IdItem.Id"/> of a wiki page which transcludes the primary page.
    /// </param>
    public static async Task<PageTransclusions> NewAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        string? domain,
        string referenceId)
    {
        var result = new PageTransclusions(
            GetId(title, wikiNamespace, domain),
            new List<string> { referenceId }.AsReadOnly());
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Adds a page to this collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which transcludes the referenced wiki page.
    /// </param>
    public async Task AddReferenceAsync(IDataStore dataStore, string id)
    {
        if (References.Contains(id))
        {
            return;
        }

        var result = new PageTransclusions(
            Id,
            References.ToImmutableList().Add(id));
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a page from this collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which no longer transcludes to the
    /// referenced wiki page.
    /// </param>
    public async Task RemoveReferenceAsync(IDataStore dataStore, string id)
    {
        if (!References.Contains(id))
        {
            return;
        }

        if (References.Count == 1)
        {
            await dataStore.RemoveItemAsync<PageTransclusions>(Id).ConfigureAwait(false);
        }
        else
        {
            var result = new PageTransclusions(
                Id,
                References.ToImmutableList().Remove(id));
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        }
    }
}
