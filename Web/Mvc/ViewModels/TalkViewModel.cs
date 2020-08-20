using NeverFoundry.Wiki.Web.SignalR;
using System.Collections.Generic;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The talk DTO.
    /// </summary>
    public class TalkViewModel
    {
        /// <summary>
        /// The associated <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData Data { get; }

        /// <summary>
        /// The messages.
        /// </summary>
        public IList<MessageResponse>? Messages { get; set; }

        /// <summary>
        /// The topid ID.
        /// </summary>
        public string? TopicId { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="TalkViewModel"/>.
        /// </summary>
        public TalkViewModel(WikiRouteData data, string? topicId)
        {
            Data = data;
            TopicId = topicId;
        }
    }
}
