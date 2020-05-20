using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A message sent from a user to an audience.
    /// </summary>
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
        /// The timestamp when this message was sent.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// The ID of the topic to which this message has been addressed.
        /// </summary>
        public string TopicId { get; }

        internal Message(
            string topicId,
            string senderId,
            string senderName,
            string? markdown,
            DateTimeOffset timestamp,
            string? replyMessageId = null) : base(markdown)
        {
            ReplyMessageId = replyMessageId;
            SenderId = senderId;
            SenderName = senderName;
            Timestamp = timestamp;
            TopicId = topicId;
        }

        private Message(
            string id,
            string markdown,
            IList<WikiLink> wikiLinks,
            string topicId,
            string senderId,
            string senderName,
            DateTimeOffset timestamp,
            string? replyMessageId = null) : base(id, markdown, wikiLinks)
        {
            ReplyMessageId = replyMessageId;
            SenderId = senderId;
            SenderName = senderName;
            Timestamp = timestamp;
            TopicId = topicId;
        }

        private Message(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Id), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(MarkdownContent), typeof(string)) ?? string.Empty,
            (IList<WikiLink>?)info.GetValue(nameof(WikiLinks), typeof(IList<WikiLink>)) ?? new WikiLink[0],
            (string?)info.GetValue(nameof(TopicId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(SenderId), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(SenderName), typeof(string)) ?? string.Empty,
            (DateTimeOffset?)info.GetValue(nameof(Timestamp), typeof(DateTimeOffset)) ?? DateTimeOffset.MinValue,
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
                DateTimeOffset.UtcNow,
                replyMessageId);
            await message.SaveAsync().ConfigureAwait(false);
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
            info.AddValue(nameof(Timestamp), Timestamp);
            info.AddValue(nameof(ReplyMessageId), ReplyMessageId);
        }
    }
}
