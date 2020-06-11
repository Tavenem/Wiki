using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A message sent from a user to an audience.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    [Serializable]
    public sealed class Message : MarkdownItem
    {
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
        /// The name of the sender of this message.
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// The timestamp when this message was sent, in UTC.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DateTimeOffset Timestamp => new DateTimeOffset(TimestampTicks, TimeSpan.Zero);

        /// <summary>
        /// The timestamp when this message was sent, in UTC Ticks.
        /// </summary>
        public long TimestampTicks { get; }

        /// <summary>
        /// The ID of the topic to which this message has been addressed.
        /// </summary>
        public string TopicId { get; }

        internal Message(
            string topicId,
            string senderId,
            string senderName,
            string? markdown,
            long timestampTicks,
            string? replyMessageId = null) : base(markdown)
        {
            ReplyMessageId = replyMessageId;
            SenderId = senderId;
            SenderName = senderName;
            TimestampTicks = timestampTicks;
            TopicId = topicId;
        }

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        private Message(
            string id,
            string markdownContent,
            IList<WikiLink> wikiLinks,
            string topicId,
            string senderId,
            string senderName,
            long timestampTicks,
            string? replyMessageId = null) : base(id, markdownContent, wikiLinks)
        {
            ReplyMessageId = replyMessageId;
            SenderId = senderId;
            SenderName = senderName;
            TimestampTicks = timestampTicks;
            TopicId = topicId;
        }

        private Message(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (ReadOnlyCollection<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(ReadOnlyCollection<WikiLink>)) ?? new WikiLink[0] as IList<WikiLink>,
            (string?)info.GetValue(nameof(TopicId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(SenderId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(SenderName), typeof(string)) ?? string.Empty,
            (long?)info.GetValue(nameof(TimestampTicks), typeof(long)) ?? default,
            (string?)info.GetValue(nameof(ReplyMessageId), typeof(string)))
        { }

        /// <summary>
        /// Creates a new reply.
        /// </summary>
        /// <param name="topicId">The ID of the topipc to which the reply is being addressed.</param>
        /// <param name="senderId">The ID of the sender of this message.</param>
        /// <param name="senderName">The name of the sender of this message.</param>
        /// <param name="markdown">The raw markdown content.</param>
        /// <param name="replyMessageId">
        /// The ID of the message to which this reply is addressed (<see langword="null"/> for
        /// messages addressed directly to a topic).
        /// </param>
        public static async Task<Message> ReplyAsync(
            string topicId,
            string senderId,
            string senderName,
            string markdown,
            string? replyMessageId = null)
        {
            if (!string.IsNullOrEmpty(markdown))
            {
                markdown = TransclusionParser.Transclude(
                    null,
                    null,
                    markdown,
                    out _);
            }

            var message = new Message(
                topicId,
                senderId,
                senderName,
                markdown,
                DateTimeOffset.UtcNow.Ticks,
                replyMessageId);
            await WikiConfig.DataStore.StoreItemAsync(message).ConfigureAwait(false);
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
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(MarkdownContent), MarkdownContent);
            info.AddValue(nameof(WikiLinks), WikiLinks);
            info.AddValue(nameof(TopicId), TopicId);
            info.AddValue(nameof(SenderId), SenderId);
            info.AddValue(nameof(SenderName), SenderName);
            info.AddValue(nameof(TimestampTicks), TimestampTicks);
            info.AddValue(nameof(ReplyMessageId), ReplyMessageId);
        }
    }
}
