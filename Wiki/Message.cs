using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A message sent from a user to an audience.
    /// </summary>
    [Serializable]
    public class Message : MarkdownItem
    {
        /// <summary>
        /// The type discriminator for this type.
        /// </summary>
        public const string MessageIdItemTypeName = ":Message:";
        /// <summary>
        /// A built-in, read-only type discriminator.
        /// </summary>
        public virtual string IdItemTypeName => MessageIdItemTypeName;

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
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTimeOffset Timestamp => new(TimestampTicks, TimeSpan.Zero);

        /// <summary>
        /// The timestamp when this message was sent, in UTC Ticks.
        /// </summary>
        public long TimestampTicks { get; }

        /// <summary>
        /// The ID of the topic to which this message has been addressed.
        /// </summary>
        public string TopicId { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Message"/>.
        /// </summary>
        /// <param name="id">The item's <see cref="IdItem.Id"/>.</param>
        /// <param name="idItemTypeName">The type discriminator.</param>
        /// <param name="markdownContent">The raw markdown.</param>
        /// <param name="html">The rendered HTML content.</param>
        /// <param name="preview">A preview of this item's rendered HTML.</param>
        /// <param name="wikiLinks">The included <see cref="WikiLink"/> objects.</param>
        /// <param name="topicId">The ID of the topipc to which the reply is being addressed.</param>
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
        [System.Text.Json.Serialization.JsonConstructor]
        public Message(
            string id,
#pragma warning disable IDE0060 // Remove unused parameter: Used by deserializers.
            string idItemTypeName,
#pragma warning restore IDE0060 // Remove unused parameter
            string markdownContent,
            string html,
            string preview,
            IReadOnlyCollection<WikiLink> wikiLinks,
            string topicId,
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
            TopicId = topicId;
        }

        internal Message(
            string topicId,
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
            TopicId = topicId;
        }

        private Message(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            MessageIdItemTypeName,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Html), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(Preview), typeof(string)) ?? string.Empty,
            (IReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IReadOnlyCollection<WikiLink>)) ?? new ReadOnlyCollection<WikiLink>(Array.Empty<WikiLink>()),
            (string?)info.GetValue(nameof(TopicId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(SenderId), typeof(string)) ?? string.Empty,
            (bool?)info.GetValue(nameof(SenderIsAdmin), typeof(bool)) ?? default,
            (string?)info.GetValue(nameof(SenderName), typeof(string)) ?? string.Empty,
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (string?)info.GetValue(nameof(ReplyMessageId), typeof(string)))
        { }

        /// <summary>
        /// Creates a new reply.
        /// </summary>
        /// <param name="options">An <see cref="IWikiOptions"/> instance.</param>
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
            IWikiOptions options,
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
                markdown = TransclusionParser.Transclude(
                    options,
                    dataStore,
                    null,
                    null,
                    markdown,
                    out _);
            }

            var message = new Message(
                topicId,
                senderId,
                senderIsAdmin,
                senderName,
                markdown,
                RenderHtml(options, dataStore, markdown),
                RenderPreview(options, dataStore, PostprocessMessageMarkdown(options, dataStore, markdown, true)),
                new ReadOnlyCollection<WikiLink>(GetWikiLinks(options, dataStore, markdown)),
                DateTimeOffset.UtcNow.Ticks,
                replyMessageId);
            await dataStore.StoreItemAsync(message).ConfigureAwait(false);
            return message;
        }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(Html), Html);
            info.AddValue(nameof(Preview), Preview);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(TopicId), TopicId);
            info.AddValue(nameof(SenderId), SenderId);
            info.AddValue(nameof(SenderIsAdmin), SenderIsAdmin);
            info.AddValue(nameof(SenderName), SenderName);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
            info.AddValue(nameof(ReplyMessageId), ReplyMessageId);
        }

        private static string PostprocessMessageMarkdown(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            bool isPreview = false)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            return TransclusionParser.Transclude(
                options,
                dataStore,
                null,
                null,
                markdown,
                out _,
                isPreview: isPreview);
        }

        private protected override string PostprocessMarkdown(
            IWikiOptions options,
            IDataStore dataStore,
            string? markdown,
            bool isPreview = false) => PostprocessMessageMarkdown(
                options,
                dataStore,
                markdown,
                isPreview);
    }
}
