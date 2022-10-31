using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A reference from a normalized (case-insensitive) full wiki page title to the current page
/// IDs assigned to that title.
/// </summary>
public class NormalizedPageReference : IdItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string NormalizedPageReferenceIdItemTypeName = ":NormalizedPageReference:";
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
        get => NormalizedPageReferenceIdItemTypeName;
        set { }
    }

    /// <summary>
    /// The IDs of the wiki pages which are currently assigned to the referenced full title.
    /// </summary>
    public IReadOnlyList<string> References { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="NormalizedPageReference"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="references">
    /// The IDs of the wiki pages which are currently assigned to the referenced full title.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers. The static <see
    /// cref="NewAsync(IDataStore, string, string, string, string?)"/> method is expected to be used
    /// otherwise, as it persists instances to the <see cref="IDataStore"/> and assigns the ID
    /// dynamically.
    /// </remarks>
    public NormalizedPageReference(
        string id,
        IReadOnlyList<string> references) : base(id)
        => References = references;

    /// <summary>
    /// Gets an ID for a <see cref="NormalizedPageReference"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The ID which should be used for a <see cref="NormalizedPageReference"/> given the
    /// parameters.
    /// </returns>
    public static string GetId(string title, string wikiNamespace, string? domain)
        => string.IsNullOrEmpty(domain)
        ? $"{wikiNamespace.ToLowerInvariant()}:{title.ToLowerInvariant()}:normalizedreference"
        : $"({domain}):{wikiNamespace.ToLowerInvariant()}:{title.ToLowerInvariant()}:normalizedreference";

    /// <summary>
    /// Gets the <see cref="NormalizedPageReference"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The <see cref="NormalizedPageReference"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    public static ValueTask<NormalizedPageReference?> GetNormalizedPageReferenceAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        string? domain)
        => dataStore.GetItemAsync<NormalizedPageReference>(GetId(title, wikiNamespace, domain));

    /// <summary>
    /// Get a new instance of <see cref="NormalizedPageReference"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which is currently assigned to the
    /// referenced full title.
    /// </param>
    /// <param name="title">
    /// The title of the wiki page which is currently assigned to the referenced full title.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace of the wiki page which is currently assigned to the referenced full title.
    /// </param>
    /// <param name="domain">
    /// The domain of the wiki page which is currently assigned to the referenced full title (if any).
    /// </param>
    public static async Task<NormalizedPageReference> NewAsync(
        IDataStore dataStore,
        string id,
        string title,
        string wikiNamespace,
        string? domain)
    {
        var result = new NormalizedPageReference(
            GetId(title, wikiNamespace, domain),
            new List<string> { id }.AsReadOnly());
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Adds a page to this reference.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which is to be assigned to the referenced
    /// full title.
    /// </param>
    public async Task AddReferenceAsync(IDataStore dataStore, string id)
    {
        if (References.Contains(id))
        {
            return;
        }

        var result = new NormalizedPageReference(
            Id,
            References.ToImmutableList().Add(id));
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a page from this reference.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which is to be removed from the referenced
    /// full title.
    /// </param>
    public async Task RemoveReferenceAsync(IDataStore dataStore, string id)
    {
        if (!References.Contains(id))
        {
            return;
        }

        if (References.Count == 1)
        {
            await dataStore.RemoveItemAsync<NormalizedPageReference>(Id).ConfigureAwait(false);
        }
        else
        {
            var result = new NormalizedPageReference(
                Id,
                References.ToImmutableList().Remove(id));
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        }
    }
}
