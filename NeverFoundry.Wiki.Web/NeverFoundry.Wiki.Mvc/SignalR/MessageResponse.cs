namespace NeverFoundry.Wiki.Web.SignalR
{
    /// <summary>
    /// A compact form of <see cref="Message"/> suitable for SignalR transport.
    /// </summary>
    public class MessageResponse
    {
        /// <summary>
        /// The HTML content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The ID of this message.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The ID of the message to which this reply is addressed (<see langword="null"/> for
        /// messages addressed directly to a topic).
        /// </summary>
        public string? ReplyMessageId { get; }

        /// <summary>
        /// Whether the sender of this message exists.
        /// </summary>
        public bool SenderExists { get; set; }

        /// <summary>
        /// The ID of the sender of this message.
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// The name of the sender of this message.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// The timestamp when this message was sent.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// The ID of the topic to which this message was addressed.
        /// </summary>
        public string TopicId { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="MessageResponse"/>.
        /// </summary>
        /// <param name="message">The <see cref="Message"/>.</param>
        /// <param name="html">
        /// The HTML content of the message.
        /// </param>
        /// <param name="senderExists">
        /// <see langword="true"/> if the sender of the message still exists as a wiki user.
        /// </param>
        public MessageResponse(Message message, string html, bool senderExists)
        {
            Content = html;
            Id = message.Id;
            ReplyMessageId = message.ReplyMessageId;
            SenderExists = senderExists;
            SenderId = message.SenderId;
            SenderName = message.SenderName;
            Timestamp = message.Timestamp.ToUniversalTime().Ticks;
            TopicId = message.TopicId;
        }
    }
}
