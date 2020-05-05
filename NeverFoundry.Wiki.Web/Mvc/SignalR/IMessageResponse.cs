using System.Collections.Generic;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A compact form of <see cref="Message"/> suitable for SignalR transport.
    /// </summary>
    public interface IMessageResponse
    {
        /// <summary>
        /// The HTML content of the message.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// The ID of this message.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Any reactions to this message.
        /// </summary>
        /// <remarks>
        /// A message can have at most one reaction per sender. A new reaction from a sender
        /// replaces any old reaction that sender may have made.
        /// </remarks>
        IEnumerable<IReactionResponse>? Reactions { get; set; }

        /// <summary>
        /// Whether the sender of this message exists.
        /// </summary>
        bool SenderExists { get; set; }

        /// <summary>
        /// The ID of the sender of this message.
        /// </summary>
        string SenderId { get; set; }

        /// <summary>
        /// The name of the sender of this message.
        /// </summary>
        string SenderName { get; set; }

        /// <summary>
        /// The timestamp when this message was sent.
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// The ID of the topic to which this message was addressed.
        /// </summary>
        string TopicId { get; set; }
    }
}
