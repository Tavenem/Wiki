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
        /// The relative URL of the <see cref="IWikiTalkHub"/>.
        /// </summary>
        public string TalkHubRoute { get; set; }

        /// <summary>
        /// The API key to be used for Tenor GIF integration.
        /// </summary>
        public string? TenorAPIKey { get; set; }

        /// <summary>
        /// The topid ID.
        /// </summary>
        public string? TopicId { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="TalkViewModel"/>.
        /// </summary>
        public TalkViewModel(WikiRouteData data, string talkHubRoute, string? tenorAPIKey, string? topicId)
        {
            Data = data;
            TalkHubRoute = talkHubRoute;
            TenorAPIKey = tenorAPIKey;
            TopicId = topicId;
        }
    }
}
