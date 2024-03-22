using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// The collected history of a wiki page.
/// </summary>
/// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
/// <param name="revisions">
/// The history of revisions to the referenced wiki page, in reverse chronological order (most
/// recent first).
/// </param>
/// <remarks>
/// Note: this constructor is most useful for deserialization. The static <see
/// cref="NewAsync(IDataStore, PageTitle, IReadOnlyCollection{Revision}?)"/> method
/// is expected to be used otherwise, as it persists instances to the <see cref="IDataStore"/>
/// and assigns the ID dynamically.
/// </remarks>
[method: JsonConstructor]
public class PageHistory(
    string id,
    IReadOnlyCollection<Revision>? revisions) : IdItem(id)
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string PageHistoryIdItemTypeName = ":PageHistory:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonPropertyName("$type"), JsonPropertyOrder(-2)]
    public override string IdItemTypeName
    {
        get => PageHistoryIdItemTypeName;
        set { }
    }

    /// <summary>
    /// The history of revisions to the referenced wiki page, in reverse chronological order (most
    /// recent first).
    /// </summary>
    public IReadOnlyCollection<Revision>? Revisions { get; set; } = revisions;

    /// <summary>
    /// Gets the ID for a <see cref="PageHistory"/> given its <paramref name="title"/>.
    /// </summary>
    /// <param name="title">
    /// The title of the page.
    /// </param>
    /// <returns>
    /// The ID which should be used for a <see cref="PageHistory"/> given the parameters.
    /// </returns>
    public static string GetId(PageTitle title) => PageHistoryIdItemTypeName + title.ToString();

    /// <summary>
    /// Gets the <see cref="PageHistory"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page.</param>
    /// <returns>
    /// The <see cref="PageHistory"/> that fits the given parameters; or <see
    /// langword="null"/>, if there is no such item.
    /// </returns>
    public static ValueTask<PageHistory?> GetPageHistoryAsync(
        IDataStore dataStore,
        PageTitle title)
        => dataStore.GetItemAsync(GetId(title), WikiJsonSerializerContext.Default.PageHistory);

    /// <summary>
    /// Get a new instance of <see cref="PageHistory"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of the page.
    /// </param>
    /// <param name="revisions">
    /// The history of revisions to the referenced wiki page, in reverse chronological order (most
    /// recent first).
    /// </param>
    public static async Task<PageHistory> NewAsync(
        IDataStore dataStore,
        PageTitle title,
        IReadOnlyCollection<Revision>? revisions)
    {
        var result = new PageHistory(
            GetId(title),
            revisions);
        await dataStore
            .StoreItemAsync(result, WikiJsonSerializerContext.Default.PageHistory)
            .ConfigureAwait(false);
        return result;
    }

    internal async Task RestoreAsync(IDataStore dataStore)
    {
        var title = GetTitle();

        var existing = await GetPageHistoryAsync(dataStore, title);
        if (existing is null)
        {
            _ = await NewAsync(dataStore, title, Revisions);
        }
        else
        {
            existing.Revisions = Revisions;
            await dataStore
                .StoreItemAsync(existing, WikiJsonSerializerContext.Default.PageHistory)
                .ConfigureAwait(false);
        }
    }

    private PageTitle GetTitle() => string.IsNullOrEmpty(Id)
        ? new()
        : PageTitle.Parse(Id[PageHistoryIdItemTypeName.Length..]);
}