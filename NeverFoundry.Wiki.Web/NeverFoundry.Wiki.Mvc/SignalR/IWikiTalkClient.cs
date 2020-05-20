using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A client for receiving wiki discussion messages.
    /// </summary>
    public interface IWikiTalkClient : IAsyncDisposable
    {
        /// <summary>
        /// Whether the connection is active.
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Receive a new message.
        /// </summary>
        event EventHandler<IMessageResponse>? OnRecevied;

        /// <summary>
        /// <para>
        /// Receive a new message.
        /// </para>
        /// <para>
        /// Note: this method should only be invoked internally by an <see cref="IWikiTalkHub"/>.
        /// </para>
        /// </summary>
        /// <param name="message">
        /// An <see cref="IMessageResponse"/> with information about the message received.
        /// </param>
        void Receive(IMessageResponse message);

        /// <summary>
        /// Send a reply.
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
        Task SendAsync(IReplyRequest reply);

        /// <summary>
        /// <para>
        /// Starts a connection to the given topic. Re-tries once per second if necessary.
        /// </para>
        /// <para>
        /// Times out after 30 seconds.
        /// </para>
        /// </summary>
        /// <param name="topicId">The ID of the topic to join.</param>
        /// <returns>
        /// <see langword="true"/> if the connection was successfully established; otherwise <see
        /// langword="false"/>
        /// </returns>
        Task<bool> StartAsync(string topicId);
    }
}