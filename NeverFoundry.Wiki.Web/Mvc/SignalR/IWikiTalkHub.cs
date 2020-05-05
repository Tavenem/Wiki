using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A SignalR hub for wiki talk messages.
    /// </summary>
    public interface IWikiTalkHub
    {
        /// <summary>
        /// Begin listening for messages sent to the given topic.
        /// </summary>
        /// <param name="topicId">A topic ID.</param>
        Task JoinTopic(string topicId);

        /// <summary>
        /// Stop listening for messages sent to the given topic.
        /// </summary>
        /// <param name="topicId">A topic ID.</param>
        Task LeaveTopic(string topicId);

        /// <summary>
        /// Notify clients who are viewing the relevant topic about a new reaction to a message, and
        /// save the reaction to the persistent data source.
        /// </summary>
        /// <param name="reaction">
        /// <para>
        /// The reaction that has been sent.
        /// </para>
        /// <para>
        /// Note: reactions to messages with unknown IDs are ignored.
        /// </para>
        /// </param>
        Task SendReaction(IReactionRequest reaction);

        /// <summary>
        /// Notify clients who are viewing the relevant topic about a new message, and save the
        /// message to the persistent data source.
        /// </summary>
        /// <param name="reply">
        /// <para>
        /// The message that has been sent.
        /// </para>
        /// <para>
        /// Note: messages with empty content are neither saved to the data source, nor forwarded to
        /// clients. Messages with missing topic IDs are also ignored.
        /// </para>
        /// </param>
        Task Send(IReplyRequest reply);
    }
}