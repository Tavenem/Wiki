using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.Wiki;

/// <summary>
/// A discussion topic with a collection of messages.
/// </summary>
/// <remarks>
/// <para>
/// Most wiki discussion topics are expected to be wiki pages, and support for pages as topics is
/// built-in.
/// </para>
/// <para>
/// Other message topics are possible as well, by using custom IDs.
/// </para>
/// </remarks>
public class Topic : IdItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string TopicIdItemTypeName = ":Topic:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonPropertyName("$type"), JsonPropertyOrder(-2)]
    public override string IdItemTypeName
    {
        get => TopicIdItemTypeName;
        set { }
    }

    /// <summary>
    /// The collection of messages associated with this topic, in chronological order.
    /// </summary>
    public IReadOnlyCollection<Message>? Messages { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="Topic"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="messages">
    /// The collection of messages associated with this topic, in chronological order.
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers. The static <see
    /// cref="NewAsync(IDataStore, PageTitle, IReadOnlyCollection{Message}?)"/> method is expected
    /// to be used otherwise, as it persists instances to the <see cref="IDataStore"/> and assigns
    /// the ID dynamically.
    /// </remarks>
    [JsonConstructor]
    public Topic(
        string id,
        IReadOnlyCollection<Message>? messages) : base(id)
        => Messages = messages;

    /// <summary>
    /// Gets an ID for a <see cref="Topic"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <returns>
    /// The ID which should be used for a <see cref="Topic"/> given the parameters.
    /// </returns>
    public static string GetId(PageTitle title) => TopicIdItemTypeName + title.ToString();

    /// <summary>
    /// Gets an ID for a <see cref="Topic"/> given the parameters.
    /// </summary>
    /// <param name="title">The title of the wiki page.</param>
    /// <param name="wikiNamespace">The namespace of the wiki page.</param>
    /// <param name="domain">The domain of the wiki page (if any).</param>
    /// <returns>
    /// The ID which should be used for a <see cref="Topic"/> given the parameters.
    /// </returns>
    public static string GetId(string title, string wikiNamespace, string? domain)
        => string.IsNullOrEmpty(domain)
        ? $"{wikiNamespace}:{title}:messages"
        : $"({domain}):{wikiNamespace}:{title}:messages";

    /// <summary>
    /// Gets the title of the wiki page associated with this <see cref="Topic"/>.
    /// </summary>
    /// <returns>
    /// The title of the wiki page associated with this <see cref="Topic"/>, or an empty title if
    /// this topic's <see cref="IdItem.Id"/> does not appear to refer to a wiki page.
    /// </returns>
    public PageTitle GetTitle() => string.IsNullOrEmpty(Id)
        || Id.Length <= TopicIdItemTypeName.Length
        ? new()
        : PageTitle.Parse(Id[TopicIdItemTypeName.Length..]);

    /// <summary>
    /// Gets the <see cref="Topic"/> that fits the given parameters.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">The title of the wiki page associated with the topic.</param>
    /// <returns>
    /// The <see cref="Topic"/> that fits the given parameters; or <see langword="null"/>, if there
    /// is no such item.
    /// </returns>
    public static ValueTask<Topic?> GetTopicAsync(
        IDataStore dataStore,
        PageTitle title)
        => dataStore.GetItemAsync<Topic>(GetId(title));

    /// <summary>
    /// Get a new instance of <see cref="Topic"/>.
    /// </summary>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="title">
    /// The title of the wiki page associated with this topic.
    /// </param>
    /// <param name="messages">
    /// The collection of messages associated with this topic, in chronological order.
    /// </param>
    public static async Task<Topic> NewAsync(
        IDataStore dataStore,
        PageTitle title,
        IReadOnlyCollection<Message>? messages)
    {
        var result = new Topic(GetId(title), messages);
        await dataStore.StoreItemAsync(result)
            .ConfigureAwait(false);
        return result;
    }

    internal async Task RestoreAsync(IDataStore dataStore)
    {
        if (!Id.StartsWith(TopicIdItemTypeName))
        {
            await dataStore.StoreItemAsync(this)
                .ConfigureAwait(false);
            return;
        }

        var title = GetTitle();

        var existing = await GetTopicAsync(dataStore, title);
        if (existing is null)
        {
            _ = await NewAsync(dataStore, title, Messages)
                .ConfigureAwait(false);
        }
        else
        {
            existing.Messages = Messages;
            await dataStore.StoreItemAsync(existing)
                .ConfigureAwait(false);
        }
    }
}
