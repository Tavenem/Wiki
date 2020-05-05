using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NeverFoundry.Wiki.Messaging
{
    /// <summary>
    /// A reaction to a message.
    /// </summary>
    [Serializable]
    public class Reaction : ISerializable
    {
        /// <summary>
        /// The ID of the sender of this reaction.
        /// </summary>
        public string SenderId { get; }

        /// <summary>
        /// The name of the sender of this reaction.
        /// </summary>
        public string SenderName { get; }

        /// <summary>
        /// The timestamp when this reaction was sent.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// The type of reaction.
        /// </summary>
        public ReactionType Type { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Reaction"/>.
        /// </summary>
        /// <param name="senderId">The ID of the sender of this reaction.</param>
        /// <param name="senderName">The name of the sender of this reaction.</param>
        /// <param name="type">The type of reaction.</param>
        /// <param name="timestamp">The timestamp when this reaction was sent.</param>
        public Reaction(string senderId, string senderName, ReactionType type, DateTimeOffset timestamp)
        {
            SenderId = senderId;
            SenderName = senderName;
            Timestamp = timestamp;
            Type = type;
        }

        private Reaction(SerializationInfo info, StreamingContext context) : this(
            (string)info.GetValue(nameof(SenderId), typeof(string)),
            (string)info.GetValue(nameof(SenderName), typeof(string)),
            (ReactionType)info.GetValue(nameof(Type), typeof(ReactionType)),
            (DateTimeOffset)info.GetValue(nameof(Timestamp), typeof(DateTimeOffset)))
        { }

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(SenderId), SenderId);
            info.AddValue(nameof(SenderName), SenderName);
            info.AddValue(nameof(Type), Type);
            info.AddValue(nameof(Timestamp), Timestamp);
        }
    }
}
