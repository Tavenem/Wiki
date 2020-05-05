using NeverFoundry.Wiki.Messaging;

namespace NeverFoundry.Wiki.Web.SignalR
{
    public class ReactionRequest : IReactionRequest
    {
        /// <summary>
        /// The ID of the message to which this reaction is addressed.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// The ID of the topic to which this reaction has been addressed.
        /// </summary>
        public string TopicId { get; set; }

        /// <summary>
        /// The type of reaction.
        /// </summary>
        public ReactionType Type { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ReactionResponse"/>.
        /// </summary>
        public ReactionRequest(string messageId, string topicId, ReactionType type)
        {
            MessageId = messageId;
            TopicId = topicId;
            Type = type;
        }
    }
}
