using NeverFoundry.Wiki.Messaging;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A reaction for SignalR transport.
    /// </summary>
    public interface IReactionRequest
    {
        /// <summary>
        /// The ID of the message to which this reaction is addressed.
        /// </summary>
        string MessageId { get; set; }

        /// <summary>
        /// The ID of the topic to which this reaction has been addressed.
        /// </summary>
        string TopicId { get; set; }

        /// <summary>
        /// The type of reaction.
        /// </summary>
        ReactionType Type { get; set; }
    }
}
