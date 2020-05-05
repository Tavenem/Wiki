namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A reply made to a <see cref="WikiTalkHub"/>
    /// </summary>
    public interface IReplyRequest
    {
        /// <summary>
        /// The markdown content of this reply.
        /// </summary>
        string Markdown { get; set; }

        /// <summary>
        /// The ID of the topic to which this reply has been addressed.
        /// </summary>
        string TopicId { get; set; }
    }
}
