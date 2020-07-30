using Microsoft.AspNetCore.SignalR;
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
        internal const string PreviewNamespaceTemplate = "<span class=\"wiki-main-heading-namespace\">{0}</span><span class=\"wiki-main-heading-namespace-separator\">:</span>";
        internal const string PreviewTemplate = "<div class=\"wiki compact preview\"><div><main class=\"wiki-content\" role=\"main\"><div class=\"wiki-heading\" role=\"heading\"><h1 class=\"wiki-main-heading\">{0}<span class=\"wiki-main-heading-title\">{1}</span></h1></div><div class=\"wiki-body\"><div class=\"wiki-parser-output\">{2}</div></div></main></div></div>";

        private readonly IWikiUserManager _userManager;

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
        /// An <see cref="IWikiUserManager"/> instance.
        /// </para>
        /// <para>
        /// Note: this is expected to be provided by dependency injection.
        /// </para>
        /// </param>
        public WikiTalkHub(IWikiUserManager userManager) => _userManager = userManager;

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
            var preview = false;
            if (message.WikiLinks.Count == 1)
            {
                var link = message.WikiLinks.First();
                if (!link.IsCategory
                    && !link.IsTalk
                    && !link.Missing
                    && !string.IsNullOrEmpty(link.WikiNamespace))
                {
                    var article = Article.GetArticle(link.Title, link.WikiNamespace);
                    if (article is not null && !article.IsDeleted)
                    {
                        preview = true;
                        var previewHtml = string.Empty;
                        await Task.Run(() => previewHtml = article.GetPreview()).ConfigureAwait(false);
                        var namespaceStr = article.WikiNamespace == WikiConfig.DefaultNamespace
                            ? string.Empty
                            : string.Format(PreviewNamespaceTemplate, article.WikiNamespace);
                        html = string.Format(PreviewTemplate, namespaceStr, article.Title, previewHtml);
                    }
                }
            }
            if (!preview)
            {
                await Task.Run(() => html = message.GetHtml()).ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(html))
            {
                var senderPage = Article.GetArticle(user.Id, WikiWebConfig.UserNamespace);

                await Clients.Group(reply.TopicId).Receive(new MessageResponse(message, html, true, senderPage is not null)).ConfigureAwait(false);
            }
        }

        private ValueTask<bool> GetTopicEditPermissionAsync(string topicId, IWikiUser user)
            => GetTopicPermissionAsync(topicId, user, edit: true);

        private async ValueTask<bool> GetTopicPermissionAsync(string topicId, IWikiUser? user, bool edit)
        {
            if (string.IsNullOrWhiteSpace(topicId)
                || string.IsNullOrEmpty(topicId))
            {
                return false;
            }

            var article = await WikiConfig.DataStore.GetItemAsync<Article>(topicId).ConfigureAwait(false);
            if (article is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(article.Owner)
                || article.Owner == user?.Id)
            {
                return true;
            }

            if (edit)
            {
                if (user is null)
                {
                    return false;
                }
                else if (article.AllowedEditors?.Contains(user.Id) == true)
                {
                    return true;
                }
                else if (user.Groups is null)
                {
                    return false;
                }
                else
                {
                    return user.Groups.Contains(article.Owner)
                        || article.AllowedEditors?.Intersect(user.Groups).Any() != false;
                }
            }
            else if (article.AllowedViewers is null)
            {
                return true;
            }
            else if (user is null)
            {
                return false;
            }
            else if (article.AllowedViewers.Contains(user.Id))
            {
                return true;
            }
            else if (user.Groups is null)
            {
                return false;
            }
            else
            {
                return user.Groups.Contains(article.Owner)
                    || article.AllowedViewers?.Intersect(user.Groups).Any() != false;
            }
        }

        private ValueTask<bool> GetTopicViewPermissionAsync(string topicId, IWikiUser? user)
            => GetTopicPermissionAsync(topicId, user, edit: false);
    }
}
