using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A persisted reference to a missing page on the wiki. Used for efficient enumeration of
/// broken links.
/// </summary>
public class MissingPage : IdItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string MissingPageIdItemTypeName = ":MissingPage:";
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
        get => MissingPageIdItemTypeName;
        set { }
    }

    /// <summary>
    /// The IDs of pages which reference this missing page.
    /// </summary>
    public IReadOnlyList<string> References { get; } = new List<string>().AsReadOnly();

    /// <summary>
    /// The title of this missing page. Must be unique within its namespace, and non-empty.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// The namespace to which this page should belong.
    /// </summary>
    public string WikiNamespace { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="MissingPage"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="title">
    /// The title of this missing page. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace to which this page should belong.
    /// </param>
    /// <param name="references">
    /// The IDs of pages which reference this missing page.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers. The static <see
    /// cref="NewAsync(IDataStore, string, string?, string[])"/> method is expected to be used
    /// otherwise, as it persists instances to the <see cref="IDataStore"/> and builds the
    /// reference list dynamically.
    /// </remarks>
    public MissingPage(
        string id,
        string title,
        string wikiNamespace,
        IReadOnlyList<string> references) : base(id)
    {
        Title = title;
        WikiNamespace = wikiNamespace;
        References = references;
    }

    /// <summary>
    /// Gets an ID for a <see cref="MissingPage"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <returns>
    /// The ID which should be used for a <see cref="MissingPage"/> given the parameters.
    /// </returns>
    public static string GetId(string title, string wikiNamespace)
        => $"{wikiNamespace}:{title}:missing";

    /// <summary>
    /// Gets the <see cref="MissingPage"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <returns>
    /// The <see cref="MissingPage"/> that fits the given parameters; or <see langword="null"/>,
    /// if there is no such item.
    /// </returns>
    public static ValueTask<MissingPage?> GetMissingPageAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace)
        => dataStore.GetItemAsync<MissingPage>(GetId(title, wikiNamespace));

    /// <summary>
    /// Get a new instance of <see cref="MissingPage"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of this missing page. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace to which this page should belong.
    /// </param>
    /// <param name="referenceIds">The IDs of the initial pages which references this missing page.</param>
    public static async Task<MissingPage> NewAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        params string[] referenceIds)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"{nameof(title)} cannot be empty.", nameof(title));
        }
        var result = new MissingPage(
            GetId(title, wikiNamespace),
            title,
            wikiNamespace,
            referenceIds);
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Get a new instance of <see cref="MissingPage"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of this missing page. Must be unique within its namespace, and non-empty.
    /// </param>
    /// <param name="wikiNamespace">
    /// The namespace to which this page should belong.
    /// </param>
    /// <param name="referenceIds">The IDs of the initial pages which references this missing page.</param>
    public static Task<MissingPage> NewAsync(
        IDataStore dataStore,
        string title,
        string wikiNamespace,
        IEnumerable<string> referenceIds)
        => NewAsync(dataStore, title, wikiNamespace, referenceIds.ToArray());

    /// <summary>
    /// Adds a page to this collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which links to the referenced wiki page.
    /// </param>
    public async Task AddReferenceAsync(IDataStore dataStore, string id)
    {
        if (References.Contains(id))
        {
            return;
        }

        var result = new MissingPage(
            Id,
            Title,
            WikiNamespace,
            References.ToImmutableList().Add(id));
        await dataStore.StoreItemAsync(result).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a page from this collection.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the wiki page which no longer links to the referenced
    /// wiki page.
    /// </param>
    public async Task RemoveReferenceAsync(IDataStore dataStore, string id)
    {
        if (!References.Contains(id))
        {
            return;
        }

        if (References.Count == 1)
        {
            await dataStore.RemoveItemAsync<MissingPage>(Id).ConfigureAwait(false);
        }
        else
        {
            var result = new MissingPage(
                Id,
                Title,
                WikiNamespace,
                References.ToImmutableList().Remove(id));
            await dataStore.StoreItemAsync(result).ConfigureAwait(false);
        }
    }
}
