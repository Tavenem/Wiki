using NeverFoundry.Wiki.Web.SignalR;
using System.Collections.Generic;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class TalkViewModel
    {
        public WikiRouteData Data { get; }

        public IList<MessageResponse>? Messages { get; set; }

        public string? TopicId { get; set; }

        public TalkViewModel(WikiRouteData data, string? topicId)
        {
            Data = data;
            TopicId = topicId;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
