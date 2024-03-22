using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Models;

/// <summary>
/// A reference from a normalized (case-insensitive) full wiki page title to the current page
/// IDs assigned to that title.
/// </summary>
/// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
/// <param name="references">
/// The IDs of the wiki pages which are currently assigned to the referenced full title.
/// </param>
public class NormalizedPageReference(
    string id,
    IReadOnlyList<string> references) : IdItem(id)
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
    public IReadOnlyList<string> References { get; set; } = references;

    /// <summary>
    /// Gets the ID for a <see cref="NormalizedPageReference"/> given its <paramref name="title"/>.
    /// </summary>
    /// <param name="title">
    /// The title of the page.
    /// </param>
    /// <returns>
    /// The ID which should be used for a <see cref="NormalizedPageReference"/> given the parameters.
    /// </returns>
    public static string GetId(PageTitle title) => NormalizedPageReferenceIdItemTypeName + title.ToString();

    /// <summary>
    /// Adds a page to the reference for the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the page.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the page which is to be assigned to the referenced title.
    /// </param>
    public static async Task AddReferenceAsync(
        IDataStore dataStore,
        PageTitle title,
        string id)
    {
        var reference = await dataStore
            .GetItemAsync(GetId(title), WikiJsonSerializerContext.Default.NormalizedPageReference)
            .ConfigureAwait(false);
        if (reference is null)
        {
            reference = new NormalizedPageReference(
                GetId(title),
                new List<string> { id }.AsReadOnly());
        }
        else if (reference.References.Contains(id))
        {
            return;
        }
        else
        {
            reference.References = reference.References.ToImmutableList().Add(id);
        }

        await dataStore.StoreItemAsync(reference, WikiJsonSerializerContext.Default.NormalizedPageReference)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the <see cref="NormalizedPageReference"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the page.</param>
    /// <returns>
    /// The <see cref="NormalizedPageReference"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    public static ValueTask<NormalizedPageReference?> GetNormalizedPageReferenceAsync(
        IDataStore dataStore,
        PageTitle title)
        => dataStore.GetItemAsync(GetId(title), WikiJsonSerializerContext.Default.NormalizedPageReference);

    /// <summary>
    /// Removes a page from the reference for the given <paramref name="title"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the page.</param>
    /// <param name="id">
    /// The <see cref="IdItem.Id"/> of the page which is to be removed from the referenced title.
    /// </param>
    public static async Task RemoveReferenceAsync(
        IDataStore dataStore,
        PageTitle title,
        string id)
    {
        var reference = await dataStore
            .GetItemAsync(GetId(title), WikiJsonSerializerContext.Default.NormalizedPageReference)
            .ConfigureAwait(false);
        if (reference?.References.Contains(id) != true)
        {
            return;
        }
        else if (reference.References.Count == 1)
        {
            await dataStore.RemoveItemAsync(reference)
                .ConfigureAwait(false);
        }
        else
        {
            reference.References = reference.References.ToImmutableList().Remove(id);
            await dataStore
                .StoreItemAsync(reference, WikiJsonSerializerContext.Default.NormalizedPageReference)
                .ConfigureAwait(false);
        }
    }
}
