namespace NeverFoundry.Wiki.Messaging
{
    /// <summary>
    /// A type of reaction to a message.
    /// </summary>
    public enum ReactionType
    {
        /// <summary>
        /// A positive reaction.
        /// </summary>
        Positive = 0,

        /// <summary>
        /// A negative reaction.
        /// </summary>
        Negative = 1,

        /// <summary>
        /// An amused reaction.
        /// </summary>
        Funny = 2,

        /// <summary>
        /// A sad reaction.
        /// </summary>
        Sad = 3,

        /// <summary>
        /// A surprised reaction.
        /// </summary>
        Surprise = 4,
    }
}
