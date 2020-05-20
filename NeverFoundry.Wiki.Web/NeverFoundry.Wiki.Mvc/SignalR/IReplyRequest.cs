namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A reply made to an <see cref="IWikiTalkHub"/>
    /// </summary>
    public interface IReplyRequest
    {
        /// <summary>
        /// The markdown content of this reply.
        /// </summary>
        string Markdown { get; set; }

        /// <summary>
        /// The ID of the message to which this reply is addressed (<see langword="null"/> for
        /// messages addressed directly to a topic).
        /// </summary>
        string? MessageId { get; }

        /// <summary>
        /// The ID of the topic to which this reply has been addressed.
        /// </summary>
        string TopicId { get; set; }
    }
}
