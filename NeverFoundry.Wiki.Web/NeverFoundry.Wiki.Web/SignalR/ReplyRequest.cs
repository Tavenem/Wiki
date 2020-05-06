using System;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A reply made to an <see cref="IWikiTalkHub"/>
    /// </summary>
    public class ReplyRequest : IReplyRequest
    {
        /// <summary>
        /// The markdown content of this reply.
        /// </summary>
        public string Markdown { get; set; }

        /// <summary>
        /// The ID of the topic to which this reply has been addressed.
        /// </summary>
        public string TopicId { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ReplyRequest"/>.
        /// </summary>
        /// <param name="topicId">
        /// The ID of the topic to which this reply has been addressed.
        /// </param>
        /// <param name="markdown">
        /// The markdown content of this reply.
        /// </param>
        public ReplyRequest(string topicId, string markdown)
        {
            if (string.IsNullOrWhiteSpace(topicId))
            {
                throw new ArgumentNullException($"{nameof(topicId)} cannot be empty", nameof(topicId));
            }
            TopicId = topicId;
            Markdown = markdown;
        }
    }
}
