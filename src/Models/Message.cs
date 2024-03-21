using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.Wiki.MarkdownExtensions.Transclusions;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki;

/// <summary>
/// A message sent from a user to an audience.
/// </summary>
public class Message : MarkdownItem
{
    /// <summary>
    /// The type discriminator for this type.
    /// </summary>
    public const string MessageIdItemTypeName = ":Message:";
    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    [JsonIgnore]
    public override string IdItemTypeName => MessageIdItemTypeName;

    /// <summary>
    /// The ID of the message to which this reply is addressed (<see langword="null"/> for
    /// messages addressed directly to a topic).
    /// </summary>
    public string? ReplyMessageId { get; }

    /// <summary>
    /// The ID of the sender of this message.
    /// </summary>
    public string SenderId { get; }

    /// <summary>
    /// Whether the sender of this message is an admin.
    /// </summary>
    public bool SenderIsAdmin { get; }

    /// <summary>
    /// The name of the sender of this message.
    /// </summary>
    public string SenderName { get; }

    /// <summary>
    /// The timestamp when this message was sent, in UTC.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset Timestamp => new(TimestampTicks, TimeSpan.Zero);

    /// <summary>
    /// The timestamp when this message was sent, in UTC Ticks.
    /// </summary>
    public long TimestampTicks { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Message"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
    /// <param name="markdownContent">The raw markdown.</param>
    /// <param name="html">The rendered HTML content.</param>
    /// <param name="preview">A preview of this item's rendered HTML.</param>
    /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
    /// <param name="senderId">The ID of the sender of this message.</param>
    /// <param name="senderIsAdmin">Whether the sender of this message is an admin.</param>
    /// <param name="senderName">The name of the sender of this message.</param>
    /// <param name="timestampTicks">
    /// The timestamp when this message was sent, in UTC Ticks.
    /// </param>
    /// <param name="replyMessageId">
    /// The ID of the message to which this reply is addressed (<see langword="null"/> for
    /// messages addressed directly to a topic).
    /// </param>
    /// <remarks>
    /// Note: this constructor is most useful for deserializers.
    /// </remarks>
    [JsonConstructor]
    public Message(
        string id,
        string markdownContent,
        string html,
        string preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        string senderId,
        bool senderIsAdmin,
        string senderName,
        long timestampTicks,
        string? replyMessageId = null) : base(id, markdownContent, html, preview, wikiLinks)
    {
        ReplyMessageId = replyMessageId;
        SenderId = senderId;
        SenderIsAdmin = senderIsAdmin;
        SenderName = senderName;
        TimestampTicks = timestampTicks;
    }

    internal Message(
        string senderId,
        bool senderIsAdmin,
        string senderName,
        string? markdown,
        string? html,
        string? preview,
        IReadOnlyCollection<WikiLink> wikiLinks,
        long timestampTicks,
        string? replyMessageId = null) : base(markdown, html, preview, wikiLinks)
    {
        ReplyMessageId = replyMessageId;
        SenderId = senderId;
        SenderIsAdmin = senderIsAdmin;
        SenderName = senderName;
        TimestampTicks = timestampTicks;
    }

    /// <summary>
    /// Creates a new reply.
    /// </summary>
    /// <param name="options">A <see cref="WikiOptions"/> instance.</param>
    /// <param name="dataStore">An <see cref="IDataStore"/> instance.</param>
    /// <param name="topicId">The ID of the topipc to which the reply is being addressed.</param>
    /// <param name="senderId">The ID of the sender of this message.</param>
    /// <param name="senderIsAdmin">Whether the sender of this message is an admin.</param>
    /// <param name="senderName">The name of the sender of this message.</param>
    /// <param name="markdown">The raw markdown content.</param>
    /// <param name="replyMessageId">
    /// The ID of the message to which this reply is addressed (<see langword="null"/> for
    /// messages addressed directly to a topic).
    /// </param>
    public static async Task<Message> ReplyAsync(
        WikiOptions options,
        IDataStore dataStore,
        string topicId,
        string senderId,
        bool senderIsAdmin,
        string senderName,
        string markdown,
        string? replyMessageId = null)
    {
        if (!string.IsNullOrEmpty(markdown))
        {
            markdown = await TransclusionParser.TranscludeAsync(
                options,
                dataStore,
                null,
                markdown);
        }

        var message = new Message(
            senderId,
            senderIsAdmin,
            senderName,
            markdown,
            RenderHtml(options, dataStore, markdown),
            RenderPreview(options, dataStore, await PostprocessMessageMarkdownAsync(options, dataStore, markdown, true)),
            new ReadOnlyCollection<WikiLink>(GetWikiLinks(options, dataStore, markdown)),
            DateTimeOffset.UtcNow.Ticks,
            replyMessageId);

        var topic = await dataStore.GetItemAsync<Topic>(topicId).ConfigureAwait(false);
        if (topic is null)
        {
            topic = new Topic(
                topicId,
                [message]);
        }
        else
        {
            var messages = topic.Messages?.ToList() ?? [];
            messages.Add(message);
            topic.Messages = messages;
        }
        await dataStore.StoreItemAsync(topic).ConfigureAwait(false);

        return message;
    }

    private static ValueTask<string> PostprocessMessageMarkdownAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? markdown,
        bool isPreview = false)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return ValueTask.FromResult(string.Empty);
        }

        return TransclusionParser.TranscludeAsync(
            options,
            dataStore,
            null,
            markdown,
            isPreview: isPreview);
    }

    private protected override ValueTask<string> PostprocessMarkdownAsync(
        WikiOptions options,
        IDataStore dataStore,
        string? markdown,
        bool isPreview = false) => PostprocessMessageMarkdownAsync(
            options,
            dataStore,
            markdown,
            isPreview);
}
