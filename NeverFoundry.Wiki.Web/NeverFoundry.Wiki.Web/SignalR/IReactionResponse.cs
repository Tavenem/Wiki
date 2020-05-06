using NeverFoundry.Wiki.Messaging;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A compact form of <see cref="Reaction"/> suitable for SignalR transport.
    /// </summary>
    public interface IReactionResponse
    {
        /// <summary>
        /// The ID of the message to which this reaction is addressed.
        /// </summary>
        public string? MessageId { get; set; }

        /// <summary>
        /// Whether the sender of this reaction exists.
        /// </summary>
        bool SenderExists { get; set; }

        /// <summary>
        /// The ID of the sender of this reaction.
        /// </summary>
        string SenderId { get; set; }

        /// <summary>
        /// The name of the sender of this reaction.
        /// </summary>
        string? SenderName { get; set; }

        /// <summary>
        /// The timestamp when this reaction was sent.
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// The type of reaction.
        /// </summary>
        ReactionType Type { get; set; }
    }
}