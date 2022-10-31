using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A reference from a full wiki page title to the current page ID assigned to that title.
/// </summary>
public class PageReference : IdItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string PageReferenceIdItemTypeName = ":PageReference:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonPropertyName("$type"), JsonPropertyOrder(-2)]
    public override string IdItemTypeName
    {
        get => PageReferenceIdItemTypeName;
        set { }
    }

    /// <summary>
    /// The ID of the wiki page which is currently assigned to the referenced full title.
    /// </summary>
    public string Reference { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="PageReference"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="reference">
    /// The ID of the wiki page which is currently assigned to the referenced full title.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers. The static <see
    /// cref="NewAsync(IDataStore, string, string, string, string?)"/> method is expected to be used
    /// otherwise, as it persists instances to the <see cref="IDataStore"/> and assigns the ID
    /// dynamically.
    /// </remarks>
    [JsonConstructor]
    public PageReference(
        string id,
        string reference) : base(id)
        => Reference = reference;

    /// <summary>
    /// Gets an ID for a <see cref="PageReference"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The ID which should be used for a <see cref="PageReference"/> given the parameters.
    /// </returns>
    public static string GetId(string title, string wikiNamespace, string? domain)
        => string.IsNullOrEmpty(domain)
        ? $"{wikiNamespace}:{title}:reference"
        : $"({domain}):{wikiNamespace}:{title}:reference";

    /// <summary>
    /// Gets the <see cref="PageReference"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The <see cref="PageReference"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    public static ValueTask<PageReference?> GetPageReferenceAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        string? domain)
        => dataStore.GetItemAsync<PageReference>(GetId(title, wikiNamespace, domain));

    /// <summary>
    /// Get a new instance of <see cref="PageReference"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which is currently assigned to the referenced
    /// full title.
    /// </param>
    /// <param name="title">
    /// The title of the wiki page which is currently assigned to the referenced full title.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace of the wiki page which is currently assigned to the referenced full title.
    /// </param>
    /// <param name="domain">
    /// The domain of the wiki page which is currently assigned to the referenced full title (if
    /// any).
    /// </param>
    public static async Task<PageReference> NewAsync(
        IDataStore dataStore,
        string id,
        string title,
        string wikiNamespace,
        string? domain)
    {
        var result = new PageReference(
            GetId(title, wikiNamespace, domain),
            id);
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Gets the <see cref="PageReference"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The <see cref="PageReference"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    internal static PageReference? GetPageReference(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        string? domain)
        => dataStore.GetItem<PageReference>(GetId(title, wikiNamespace, domain));
}
