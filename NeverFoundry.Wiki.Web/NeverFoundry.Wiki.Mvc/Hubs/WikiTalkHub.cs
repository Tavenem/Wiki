using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using NeverFoundry.Wiki.Web.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Hubs
{
    /// <summary>
    /// A SignalR hub for sending wiki discussion messages.
    /// </summary>
    public class WikiTalkHub : Hub<IWikiTalkClient>, IWikiTalkHub
    {
        private readonly UserManager<WikiUser> _userManager;

        /// <summary>
        /// <para>
        /// Initializes a new instance of <see cref="WikiTalkHub"/>.
        /// </para>
        /// <para>
        /// Note: this class is expected to be used in a <c>MapHub{T}</c> call, not instantiated
        /// directly.
        /// </para>
        /// </summary>
        /// <param name="userManager">
        /// <para>
        /// A <see cref="UserManager{T}"/> of <see cref="WikiUser"/> instance.
        /// </para>
        /// <para>
        /// Note: this is expected to be provided by dependency injection.
        /// </para>
        /// </param>
        public WikiTalkHub(UserManager<WikiUser> userManager) => _userManager = userManager;

        /// <summary>
        /// Begin listening for messages sent to the given topic.
        /// </summary>
        /// <param name="topicId">A topic ID.</param>
        public async Task JoinTopic(string topicId)
        {
            var user = await _userManager.GetUserAsync(Context.User).ConfigureAwait(false);
            var viewPermission = await GetTopicViewPermissionAsync(topicId, user).ConfigureAwait(false);
            if (viewPermission)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, topicId).ConfigureAwait(false);
            }
            else
            {
                throw new HubException("You do not have permission to view this topic.");
            }
        }

        /// <summary>
        /// Stop listening for messages sent to the given topic.
        /// </summary>
        /// <param name="topicId">A topic ID.</param>
        public Task LeaveTopic(string topicId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, topicId);

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
        public async Task Send(ReplyRequest reply)
        {
            if (string.IsNullOrWhiteSpace(reply.TopicId)
                || string.IsNullOrWhiteSpace(reply.Markdown))
            {
                return;
            }

            var user = await _userManager.GetUserAsync(Context.User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                throw new HubException("You do not have permission to reply to this topic.");
            }

            var editPermission = await GetTopicEditPermissionAsync(reply.TopicId, user).ConfigureAwait(false);
            if (!editPermission)
            {
                throw new HubException("You do not have permission to reply to this topic.");
            }

            var message = await Message.ReplyAsync(reply.TopicId, user.Id, user.UserName, reply.Markdown, reply.MessageId).ConfigureAwait(false);
            var html = string.Empty;
            var preview = string.Empty;
            await Task.Run(() => html = message.GetHtml()).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(html))
            {
                var senderPage = Article.GetArticle(user.Id, WikiWebConfig.UserNamespace);

                await Clients.Group(reply.TopicId).Receive(new MessageResponse(message, html, true, !(senderPage is null))).ConfigureAwait(false);
            }
        }

        private Task<bool> GetTopicEditPermissionAsync(string topicId, WikiUser user)
            => GetTopicPermissionAsync(topicId, user, edit: true);

        private async Task<bool> GetTopicPermissionAsync(string topicId, WikiUser? user, bool edit)
        {
            if (string.IsNullOrWhiteSpace(topicId)
                || string.IsNullOrEmpty(topicId))
            {
                return false;
            }

            var article = await DataStore.GetItemAsync<Article>(topicId).ConfigureAwait(false);
            if (article is null)
            {
                return false;
            }

            return edit
                ? !(user is null) && article.AllowedEditors?.Contains(user.Id) == true
                : article.AllowedViewers is null
                    || (!(user is null) && article.AllowedViewers.Contains(user.Id));
        }

        private Task<bool> GetTopicViewPermissionAsync(string topicId, WikiUser? user)
            => GetTopicPermissionAsync(topicId, user, edit: false);
    }
}
