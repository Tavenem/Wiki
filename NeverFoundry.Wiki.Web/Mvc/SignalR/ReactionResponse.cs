using NeverFoundry.Wiki.Messaging;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A compact form of <see cref="Reaction"/> suitable for SignalR transport.
    /// </summary>
    public class ReactionResponse : IReactionResponse
    {
        /// <summary>
        /// The ID of the message to which this reaction is addressed.
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// Whether the sender of this reaction exists.
        /// </summary>
        public bool SenderExists { get; set; }

        /// <summary>
        /// The ID of the sender of this reaction.
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// The name of the sender of this reaction.
        /// </summary>
        public string? SenderName { get; set; }

        /// <summary>
        /// The timestamp when this reaction was sent.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The type of reaction.
        /// </summary>
        public ReactionType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ReactionResponse"/>.
        /// </summary>
        /// <param name="reaction">A <see cref="Reaction"/>.</param>
        public ReactionResponse(Reaction reaction, bool senderExists)
        {
            SenderExists = senderExists;
            SenderId = reaction.SenderId;
            SenderName = reaction.SenderName;
            Timestamp = reaction.Timestamp.ToUniversalTime().Ticks;
            Type = reaction.Type;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ReactionResponse"/>.
        /// </summary>
        public ReactionResponse(string messageId, string senderId, string senderName, bool senderExists, ReactionType type)
        {
            MessageId = messageId;
            SenderExists = senderExists;
            SenderId = senderId;
            SenderName = senderName;
            Timestamp = System.DateTimeOffset.UtcNow.Ticks;
            Type = type;
        }
    }
}
