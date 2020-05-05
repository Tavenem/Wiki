using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Messaging
{
    /// <summary>
    /// A message sent from a user to an audience.
    /// </summary>
    [Serializable]
    public sealed class Message : MarkdownItem
    {
        /// <summary>
        /// Any reactions to this message.
        /// </summary>
        /// <remarks>
        /// A message can have at most one reaction per sender. A new reaction from a sender
        /// replaces any old reaction that sender may have made.
        /// </remarks>
        public IReadOnlyList<Reaction>? Reactions { get; private set; }

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
            DateTimeOffset timestamp) : base(markdown)
        {
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
            IEnumerable<Reaction>? reactions) : base(id, markdown, wikiLinks)
        {
            SenderId = senderId;
            SenderName = senderName;
            Timestamp = timestamp;
            TopicId = topicId;
            Reactions = reactions?.ToList();
        }

        private Message(SerializationInfo info, StreamingContext context) : this(
            (string)info.GetValue(nameof(Id), typeof(string)),
            (string)info.GetValue(nameof(MarkdownContent), typeof(string)),
            (IList<WikiLink>)info.GetValue(nameof(WikiLinks), typeof(IList<WikiLink>)),
            (string)info.GetValue(nameof(TopicId), typeof(string)),
            (string)info.GetValue(nameof(SenderId), typeof(string)),
            (string)info.GetValue(nameof(SenderName), typeof(string)),
            (DateTimeOffset)info.GetValue(nameof(Timestamp), typeof(DateTimeOffset)),
            (IList<Reaction>?)info.GetValue(nameof(Reactions), typeof(IList<Reaction>)))
        { }

        /// <summary>
        /// Creates a new reply.
        /// </summary>
        /// <param name="topicId">The ID of the topipc to which the reply is being addressed.</param>
        /// <param name="senderId">The ID of the sender of this message.</param>
        /// <param name="senderName">The name of the sender of this message.</param>
        /// <param name="markdown">The raw markdown content.</param>
        public static async Task<Message> ReplyAsync(
            string topicId,
            string senderId,
            string senderName,
            string markdown)
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
                DateTimeOffset.UtcNow);
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
            info.AddValue(nameof(Reactions), Reactions);
        }

        /// <summary>
        /// React to this message.
        /// </summary>
        /// <param name="reaction">The <see cref="Reaction"/>.</param>
        public async Task ReactAsync(Reaction reaction)
        {
            var reactions = Reactions?.ToList() ?? new List<Reaction>();
            reactions.RemoveAll(x => x.SenderId == reaction.SenderId);
            reactions.Add(reaction);
            Reactions = reactions;
            await SaveAsync().ConfigureAwait(false);
        }
    }
}
