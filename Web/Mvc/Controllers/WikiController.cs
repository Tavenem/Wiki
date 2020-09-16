using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.Mvc.Hubs;
using NeverFoundry.Wiki.Mvc.Models;
using NeverFoundry.Wiki.Mvc.Services.Search;
using NeverFoundry.Wiki.Mvc.ViewModels;
using NeverFoundry.Wiki.Web;
using NeverFoundry.Wiki.Web.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Controllers
{
    /// <summary>
    /// The <see cref="Controller"/> for wiki operations.
    /// </summary>
    public class WikiController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFileManager _fileManager;
        private readonly IWikiGroupManager _groupManager;
        private readonly ILogger<WikiController> _logger;
        private readonly ISearchClient _searchClient;
        private readonly IWikiUserManager _userManager;
        private readonly IWikiOptions _wikiOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="WikiController"/>.
        /// </summary>
        public WikiController(
            IWebHostEnvironment environment,
            IFileManager fileManager,
            IWikiGroupManager groupManager,
            ILogger<WikiController> logger,
            ISearchClient searchClient,
            IWikiUserManager userManager,
            IWikiOptions wikiOptions)
        {
            _environment = environment;
            _fileManager = fileManager;
            _groupManager = groupManager;
            _logger = logger;
            _searchClient = searchClient;
            _userManager = userManager;
            _wikiOptions = wikiOptions;
        }

        /// <summary>
        /// The edit view.
        /// </summary>
        public async Task<IActionResult> EditAsync()
        {
            var data = GetWikiRouteData();

            data.IsEdit = true;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(_wikiOptions.LoginPath))
                {
                    var url = new StringBuilder(_wikiOptions.LoginPath)
                        .Append(_wikiOptions.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            var wikiItem = await GetWikiItemAsync(data, true).ConfigureAwait(false);
            data.WikiItem = wikiItem;
            data.CanEdit = VerifyPermission(data, user, edit: true);
            if (!data.CanEdit)
            {
                return View("NotAuthorized", data);
            }
            else if (wikiItem is null)
            {
                if (WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
                else if (!user.IsWikiAdmin
                    && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
            }

            var markdown = string.Empty;
            if (wikiItem is not null)
            {
                markdown = wikiItem.MarkdownContent;
                data.Categories = wikiItem.Categories;
            }

            var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user).ConfigureAwait(false);
            return View("Edit", vm);
        }

        /// <summary>
        /// The edit endpoint.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(EditModel model)
        {
            var data = GetWikiRouteData();

            data.IsEdit = true;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(_wikiOptions.LoginPath))
                {
                    var url = new StringBuilder(_wikiOptions.LoginPath)
                        .Append(_wikiOptions.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            var (wikiNamespace, title, _, _) = Article.GetTitleParts(model.Title);
            data.Title = title;
            data.WikiNamespace = wikiNamespace;

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            data.WikiItem = wikiItem;
            data.CanEdit = VerifyPermission(data, user, edit: true);

            string? owner = null;
            var ownerHasId = false;
            var ownerIsGroup = false;
            if (data.IsUserPage)
            {
                owner = user.Id;
                ownerHasId = true;
            }
            else if (data.IsGroupPage)
            {
                owner = model.Title;
                ownerHasId = true;
            }
            else
            {
                owner = model.OwnerSelf ? user.Id : GetEditorId(model.Owner, out ownerHasId, out ownerIsGroup);
            }
            if (ownerIsGroup)
            {
                ModelState.AddModelError("Model", "Groups cannot own items.");
                var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }

            if (!data.CanEdit)
            {
                return View("NotAuthorized", data);
            }
            else if (wikiItem is null)
            {
                if (WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
                else if (!user.IsWikiAdmin
                    && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
            }
            else if (wikiItem.Owner is not null)
            {
                if (model.OwnerSelf)
                {
                    if (user.Id != wikiItem.Owner
                        && user.Groups?.Contains(wikiItem.Owner) != true)
                    {
                        return View("NotAuthorized", data);
                    }
                }
                else if (owner is not null && owner != wikiItem.Owner)
                {
                    return View("NotAuthorized", data);
                }
            }
            if (!model.OwnerSelf && owner is not null)
            {
                var intendedOwner = await _userManager.FindByIdAsync(owner).ConfigureAwait(false);
                if (intendedOwner is null && !ownerHasId)
                {
                    intendedOwner = await _userManager.FindByNameAsync(owner).ConfigureAwait(false);
                }
                if (intendedOwner is null)
                {
                    ModelState.AddModelError("Model", "No such owner found.");
                    var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }
            }

            if (wikiItem is not null)
            {
                data.Categories = wikiItem.Categories;
            }

            if (!ModelState.IsValid)
            {
                var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }

            if (model.ShowPreview)
            {
                var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown, model.Title).ConfigureAwait(false);
                return View("Edit", vm);
            }

            List<string>? allowedEditors = null;
            if (model.AllowedEditors is not null
                && !data.IsUserPage
                && !data.IsGroupPage
                && (model.OwnerSelf || owner is not null))
            {
                foreach (var name in model.AllowedEditors.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var id = GetEditorId(name, out var hasId, out var isGroup);

                    if (isGroup)
                    {
                        var group = await _groupManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (group is not null)
                        {
                            (allowedEditors ??= new List<string>()).Add($"G:{group.Id}");
                        }
                    }
                    else
                    {
                        var editor = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (editor is null && !hasId)
                        {
                            editor = await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                        }
                        if (editor is not null)
                        {
                            (allowedEditors ??= new List<string>()).Add(editor.Id);
                        }
                        else if (!hasId)
                        {
                            var groupEditor = await _groupManager.FindByIdAsync(id).ConfigureAwait(false)
                                ?? await _groupManager.FindByNameAsync(id).ConfigureAwait(false);
                            if (groupEditor is not null)
                            {
                                (allowedEditors ??= new List<string>()).Add($"G:{groupEditor.Id}");
                            }
                        }
                    }
                }
            }
            List<string>? allowedViewers = null;
            if (model.AllowedViewers is not null
                && (model.OwnerSelf || owner is not null))
            {
                foreach (var name in model.AllowedViewers.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var id = GetEditorId(name, out var hasId, out var isGroup);

                    if (isGroup)
                    {
                        var group = await _groupManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (group is not null)
                        {
                            (allowedViewers ??= new List<string>()).Add($"G:{group.Id}");
                        }
                    }
                    else
                    {
                        var viewer = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (viewer is null && !hasId)
                        {
                            viewer = await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                        }
                        if (viewer is not null)
                        {
                            (allowedViewers ??= new List<string>()).Add(viewer.Id);
                        }
                        else if (!hasId)
                        {
                            var groupEditor = await _groupManager.FindByIdAsync(id).ConfigureAwait(false)
                                ?? await _groupManager.FindByNameAsync(id).ConfigureAwait(false);
                            if (groupEditor is not null)
                            {
                                (allowedViewers ??= new List<string>()).Add($"G:{groupEditor.Id}");
                            }
                        }
                    }
                }
            }

            if (model.Delete)
            {
                if (wikiItem is null)
                {
                    return NotFound();
                }

                try
                {
                    await wikiItem.ReviseAsync(
                        user.Id,
                        revisionComment: model.Comment,
                        isDeleted: true,
                        owner: owner,
                        allowedEditors: allowedEditors,
                        allowedViewers: allowedViewers)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Wiki item with ID {Id} could not be deleted by user with ID {UserId}.", wikiItem.Id, user.Id);
                    ModelState.AddModelError("Model", "The article was not deleted successfully.");
                    var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }

                if (wikiItem is WikiFile file)
                {
                    try
                    {
                        await _fileManager.DeleteFileAsync(file.FilePath).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to delete file {Path} during delete operation", file.FilePath);
                    }
                }

                _logger.LogInformation("Wiki item with ID {Id} was deleted by user with ID {UserId}.", wikiItem.Id, user.Id);
                return RedirectToAction("Read", new { rev = string.Empty });
            }

            if (wikiItem is null)
            {
                try
                {
                    var newArticle = await Article.NewAsync(
                        title,
                        user.Id,
                        model.Markdown,
                        wikiNamespace,
                        owner,
                        allowedEditors,
                        allowedViewers)
                        .ConfigureAwait(false);
                    return RedirectToAction("Read", new { title = newArticle.Title, wikiNamespace = newArticle.WikiNamespace });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User with ID {UserId} failed to add a new wiki item with title {Title} to namespace {WikiNamespace}.", user.Id, title, wikiNamespace);
                    ModelState.AddModelError("Model", "The new item could not be created.");
                    var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }
            }

            var oldTitle = wikiItem.Title;
            var newTitle = title.ToWikiTitleCase();
            var titleMatches = string.Equals(newTitle, wikiItem.Title, StringComparison.CurrentCulture);
            if (titleMatches)
            {
                newTitle = null;
            }

            var oldNamespace = wikiItem.WikiNamespace;
            var newNamespace = wikiNamespace.ToWikiTitleCase();
            var namespaceMatches = string.Equals(newNamespace, wikiItem.WikiNamespace, StringComparison.CurrentCulture);
            if (namespaceMatches)
            {
                newNamespace = null;
            }

            try
            {
                await wikiItem.ReviseAsync(
                    user.Id,
                    newTitle,
                    model.Markdown,
                    model.Comment,
                    newNamespace,
                    isDeleted: false,
                    owner,
                    allowedEditors,
                    allowedViewers)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User with ID {UserId} failed to edit wiki item with ID {Id}, new title {Title}, and new namespace {WikiNamespace}.", user.Id, wikiItem.Id, newTitle, newNamespace);
                ModelState.AddModelError("Model", "The edit could not be completed.");
                var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }

            if ((!titleMatches || !namespaceMatches) && model.Redirect)
            {
                try
                {
                    await Article.NewAsync(
                        oldTitle,
                        user.Id,
                        $"{{{{redirect|{Article.GetFullTitle(newTitle ?? oldTitle, newNamespace ?? oldNamespace)}}}}}",
                        oldNamespace,
                        owner,
                        allowedEditors,
                        allowedViewers)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add redirect to wiki item with ID {Id}, title {Title}, and namespace {WikiNamespace} for user with ID {UserId}.", wikiItem.Id, newTitle ?? oldTitle, newNamespace ?? oldNamespace, user.Id);
                    ModelState.AddModelError("Model", "The redirect could not be created automatically.");
                    var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }
            }

            return RedirectToAction("Read", new { title = newTitle ?? wikiItem.Title, wikiNamespace = newNamespace ?? wikiItem.WikiNamespace });
        }

        /// <summary>
        /// The preview endpoint.
        /// </summary>
        [HttpPost("wiki/api/preview")]
        public async Task<JsonResult> GetPreviewAsync([FromForm] string? link = null)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                return Json(string.Empty);
            }

            var (wikiNamespace, title, isTalk, _) = Article.GetTitleParts(link);
            if (isTalk)
            {
                return Json(string.Empty);
            }

            Article? article;
            if (string.Equals(wikiNamespace, WikiConfig.FileNamespace, StringComparison.OrdinalIgnoreCase))
            {
                article = WikiFile.GetFile(title);
            }
            else if (string.Equals(wikiNamespace, WikiConfig.CategoryNamespace, StringComparison.OrdinalIgnoreCase))
            {
                article = Category.GetCategory(title);
            }
            else
            {
                article = Article.GetArticle(title, wikiNamespace);
            }
            if (article is null)
            {
                return Json(string.Empty);
            }

            if (!string.IsNullOrEmpty(article.Owner) && !(article.AllowedViewers is null))
            {
                if (User is null)
                {
                    return Json(string.Empty);
                }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user?.IsDeleted != false
                    || user.IsDisabled)
                {
                    return Json(string.Empty);
                }
                if (article.Owner != user.Id
                    && !article.AllowedViewers.Contains(user.Id)
                    && article.AllowedEditors?.Contains(user.Id) != true
                    && (user.Groups is null
                    || (!user.Groups.Contains(article.Owner)
                    && !article.AllowedViewers.Intersect(user.Groups).Any()
                    && article.AllowedEditors?.Intersect(user.Groups).Any() != true)))
                {
                    return Json(string.Empty);
                }
            }

            return Json(article.Preview);
        }

        /// <summary>
        /// The history view.
        /// </summary>
        public async Task<IActionResult> HistoryAsync(
            int pageNumber = 1,
            int pageSize = 50,
            string? editor = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null)
        {
            var data = GetWikiRouteData();

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (data.IsSystem)
            {
                var special = await TryGettingSystemPage(data, user).ConfigureAwait(false);
                if (special is not null)
                {
                    return special;
                }
                data.IsSystem = false;
            }

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            if (wikiItem is null)
            {
                data.CanEdit = user is not null
                    && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase))
                    && (user.IsWikiAdmin
                    || !WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)));
                return View("NoContent", data);
            }

            data.WikiItem = wikiItem;
            if (!VerifyPermission(data, user))
            {
                return View("NotAuthorized", data);
            }
            data.CanEdit = VerifyPermission(data, user, edit: true);

            data.IsHistory = true;

            var vm = await HistoryViewModel.NewAsync(
                _userManager,
                data,
                pageNumber,
                pageSize,
                editor,
                start,
                end).ConfigureAwait(false);

            return View("History", vm);
        }

        /// <summary>
        /// The read view.
        /// </summary>
        public async Task<IActionResult> ReadAsync()
        {
            var data = GetWikiRouteData();

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (data.IsSystem)
            {
                var special = await TryGettingSystemPage(data, user).ConfigureAwait(false);
                if (special is not null)
                {
                    return special;
                }
                data.IsSystem = false;
            }

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            if (wikiItem?.IsDeleted != false)
            {
                data.CanEdit = user is not null
                    && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase))
                    && (user.IsWikiAdmin
                    || !WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)));
                return View("NoContent", data);
            }

            data.WikiItem = wikiItem;
            if (!VerifyPermission(data, user))
            {
                return View("NotAuthorized", data);
            }
            data.CanEdit = VerifyPermission(data, user, edit: true);

            if (data.IsTalk)
            {
                var vm = new TalkViewModel(
                    data,
                    _wikiOptions.TalkHubRoute ?? WikiOptions.DefaultTalkHubRoute,
                    _wikiOptions.TenorAPIKey,
                    wikiItem?.Id);
                if (wikiItem is not null)
                {
                    var replies = await WikiConfig.DataStore.Query<Message>().Where(x => x.TopicId == wikiItem.Id)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    var responses = new List<MessageResponse>();
                    var senders = new Dictionary<string, bool>();
                    var senderPages = new Dictionary<string, bool>();
                    foreach (var reply in replies)
                    {
                        var html = string.Empty;
                        var preview = false;
                        if (reply.WikiLinks.Count == 1)
                        {
                            var link = reply.WikiLinks.First();
                            if (!link.IsCategory
                                && !link.IsTalk
                                && !link.Missing
                                && !string.IsNullOrEmpty(link.WikiNamespace))
                            {
                                var article = Article.GetArticle(link.Title, link.WikiNamespace);
                                if (article is not null && !article.IsDeleted)
                                {
                                    preview = true;
                                    var namespaceStr = article.WikiNamespace == WikiConfig.DefaultNamespace
                                        ? string.Empty
                                        : string.Format(WikiTalkHub.PreviewNamespaceTemplate, article.WikiNamespace);
                                    html = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(string.Format(WikiTalkHub.PreviewTemplate, namespaceStr, article.Title, article.Preview));
                                }
                            }
                        }
                        if (!preview)
                        {
                            html = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(reply.Html);
                        }
                        IWikiUser? replyUser = null;
                        if (!senders.TryGetValue(reply.SenderId, out var exists))
                        {
                            replyUser = await _userManager.FindByIdAsync(reply.SenderId).ConfigureAwait(false);
                            exists = replyUser?.IsDeleted == false;
                            senders.Add(reply.SenderId, exists);
                        }
                        if (!senderPages.TryGetValue(reply.SenderId, out var pageExists))
                        {
                            if (!exists)
                            {
                                pageExists = false;
                            }
                            else
                            {
                                if (replyUser is null)
                                {
                                    replyUser = await _userManager.FindByIdAsync(reply.SenderId).ConfigureAwait(false);
                                }
                                pageExists = replyUser?.IsDeleted == false
                                    && !(Article.GetArticle(replyUser.Id, WikiWebConfig.UserNamespace) is null);
                            }
                            senderPages.Add(reply.SenderId, pageExists);
                        }
                        responses.Add(new MessageResponse(
                            reply,
                            html,
                            exists,
                            pageExists));
                    }
                    vm.Messages = responses.OrderBy(x => x.TimestampTicks).ToList();
                }
                return View("Talk", vm);
            }

            var model = await WikiItemViewModel.NewAsync(data).ConfigureAwait(false);

            if (data.IsCategory)
            {
                var categoryModel = await CategoryViewModel.NewAsync(data, model).ConfigureAwait(false);
                return View("Category", categoryModel);
            }
            else if (data.IsGroupPage)
            {
                var groupModel = await GroupViewModel.NewAsync(_groupManager, data, model).ConfigureAwait(false);
                return View("Group", groupModel);
            }

            if (data.IsFile)
            {
                return View("File", model);
            }

            return View("Article", model);
        }

        /// <summary>
        /// The search view.
        /// </summary>
        public async Task<IActionResult> SearchAsync(
            string? query = null,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? searchNamespace = null,
            string? owner = null)
        {
            var data = GetWikiRouteData();

            if (string.IsNullOrWhiteSpace(query))
            {
                if (data.IsSystem && string.Equals(data.Title, "Search", StringComparison.OrdinalIgnoreCase))
                {
                    return View(new SearchViewModel(new SearchResult()));
                }
                return RedirectToAction("Read");
            }

            if (!string.IsNullOrEmpty(owner))
            {
                var owners = owner.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var ownerIds = new List<string>();
                foreach (var name in owners)
                {
                    var excluded = name.Length > 0 && name[0] == '!';
                    var ownerName = excluded ? name[1..] : name;
                    var ownerId = GetEditorId(ownerName, out var hasId, out var isGroup);
                    if (isGroup) // Nothing is owned by a group.
                    {
                        continue;
                    }

                    if (hasId)
                    {
                        if (!string.IsNullOrEmpty(ownerId))
                        {
                            ownerIds.Add(excluded ? $"!{ownerId}" : ownerId);
                        }
                    }
                    else
                    {
                        var foundOwner = await _userManager.FindByNameAsync(ownerId).ConfigureAwait(false)
                            ?? await _userManager.FindByIdAsync(ownerId).ConfigureAwait(false);
                        if (foundOwner is not null)
                        {
                            ownerIds.Add(excluded ? $"!{foundOwner.Id}" : foundOwner.Id);
                        }
                    }
                }
                if (ownerIds.Count == 0)
                {
                    return View(new SearchViewModel(new SearchResult()));
                }
                owner = string.Join(';', ownerIds);
            }

            query = query.Trim();
            var original = query;

            query = query.Trim('"');
            var (queryNamespace, title, isTalk, _) = Article.GetTitleParts(query);
            var wikiItem = GetWikiItem(title, queryNamespace);

            if (!original.StartsWith("\"") && wikiItem is not null)
            {
                return RedirectToAction("Read", new { wikiNamespace = isTalk ? $"{WikiConfig.TalkNamespace}:{queryNamespace}" : queryNamespace, title });
            }

            if (!data.IsSystem || !string.Equals(data.Title, "Search", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Search", new
                {
                    isTalk = false,
                    wikiNamespace = WikiWebConfig.SystemNamespace,
                    title = "Search",
                    query = original,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    searchNamespace,
                    owner,
                });
            }

            data.IsSearch = true;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(searchNamespace))
            {
                var namespaces = searchNamespace.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var normalizedNamespaces = new List<string>();
                foreach (var name in namespaces)
                {
                    var excluded = name.Length > 0 && name[0] == '!';
                    var normalizedNamespace = excluded ? name[1..].ToWikiTitleCase() : name.ToWikiTitleCase();
                    if (!string.IsNullOrEmpty(normalizedNamespace))
                    {
                        normalizedNamespaces.Add(excluded ? $"!{normalizedNamespace}" : normalizedNamespace);
                    }
                }
                if (normalizedNamespaces.Count == 0)
                {
                    return View(new SearchViewModel(new SearchResult()));
                }
                owner = string.Join(';', normalizedNamespaces);
            }

            var result = await _searchClient.SearchAsync(new SearchRequest
            {
                Descending = descending,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Query = query,
                Sort = sort,
                WikiNamespace = searchNamespace?.ToWikiTitleCase(),
                Owner = owner,
            }, user).ConfigureAwait(false);

            return View(new SearchViewModel(result, wikiItem));
        }

        /// <summary>
        /// The search suggestion endpoint.
        /// </summary>
        [HttpPost("wiki/api/suggest")]
        public async Task<JsonResult> GetSearchSuggestionsAsync([FromForm] string? search = null)
        {
            var (wikiNamespace, title, isTalk, defaultNamespace) = Article.GetTitleParts(search);

            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(Array.Empty<string>());
            }

            IReadOnlyList<Article> items;
            if (defaultNamespace)
            {
                items = await WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase)
                        && x.WikiNamespace == WikiConfig.DefaultNamespace)
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (items.Count == 0)
                {
                    items = await WikiConfig.DataStore.Query<Article>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                if (items.Count == 0)
                {
                    items = await WikiConfig.DataStore.Query<Category>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                if (items.Count == 0)
                {
                    items = await WikiConfig.DataStore.Query<WikiFile>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
            }
            else if (wikiNamespace == WikiConfig.CategoryNamespace)
            {
                items = await WikiConfig.DataStore.Query<Category>()
                    .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            else if (wikiNamespace == WikiConfig.FileNamespace)
            {
                items = await WikiConfig.DataStore.Query<WikiFile>()
                    .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                items = await WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase)
                        && x.WikiNamespace == wikiNamespace)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            return Json(items.Select(x => x.FullTitle));
        }

        /// <summary>
        /// The page list endpoint.
        /// </summary>
        public async Task<IActionResult> GetSpecialListAsync(
            string? type = null,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
        {
            if (string.IsNullOrEmpty(type)
                || !Enum.TryParse<SpecialListType>(type, ignoreCase: true, out var t))
            {
                return NotFound();
            }

            return await GetSpecialListAsync(t, pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false);
        }

        /// <summary>
        /// The upload view.
        /// </summary>
        public async Task<IActionResult> ShowUploadAsync(UploadViewModel model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(_wikiOptions.LoginPath))
                {
                    var url = new StringBuilder(_wikiOptions.LoginPath)
                        .Append(_wikiOptions.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            if (user.IsDeleted || user.IsDisabled)
            {
                return View("NotAuthorizedToUpload");
            }

            var limit = await _groupManager.UserMaxUploadLimit(user).ConfigureAwait(false);
            if (limit == 0)
            {
                return View("NotAuthorizedToUpload");
            }

            return View("Upload", model);
        }

        /// <summary>
        /// The upload endpoint.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAsync(UploadViewModel model)
        {
            var data = GetWikiRouteData();

            data.IsEdit = true;

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.File))
            {
                return View("Upload", model);
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(_wikiOptions.LoginPath))
                {
                    var url = new StringBuilder(_wikiOptions.LoginPath)
                        .Append(_wikiOptions.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            if (user.IsDeleted || user.IsDisabled)
            {
                return View("NotAuthorizedToUpload");
            }

            var limit = await _groupManager.UserMaxUploadLimit(user).ConfigureAwait(false);
            if (limit == 0)
            {
                return View("NotAuthorizedToUpload");
            }

            var (wikiNamespace, title, _, defaultNamespace) = Article.GetTitleParts(model.Title);
            if (!defaultNamespace && !string.Equals(wikiNamespace, WikiConfig.FileNamespace, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(UploadViewModel.Title), "Files cannot be given a namespace");
                return View("Upload", model);
            }

            var wikiItem = WikiFile.GetFile(title);
            data.WikiItem = wikiItem;
            data.CanEdit = VerifyPermission(data, user, edit: true);

            var ownerHasId = false;
            var ownerIsGroup = false;
            var owner = model.OwnerSelf ? user.Id : GetEditorId(model.Owner, out ownerHasId, out ownerIsGroup);
            if (ownerIsGroup)
            {
                ModelState.AddModelError("Model", "Groups cannot own items.");
                var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }

            if (!data.CanEdit)
            {
                return View("NotAuthorized", data);
            }
            else if (wikiItem is null)
            {
                if (WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
                else if (!user.IsWikiAdmin
                    && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return View("NotAuthorized", data);
                }
            }
            else if (wikiItem.Owner is not null)
            {
                if (model.OwnerSelf)
                {
                    if (user.Id != wikiItem.Owner
                        && user.Groups?.Contains(wikiItem.Owner) != true)
                    {
                        return View("NotAuthorized", data);
                    }
                }
                else if (owner is not null && owner != wikiItem.Owner)
                {
                    return View("NotAuthorized", data);
                }
            }
            if (!model.OwnerSelf && owner is not null)
            {
                var intendedOwner = await _userManager.FindByIdAsync(owner).ConfigureAwait(false);
                if (intendedOwner is null && !ownerHasId)
                {
                    intendedOwner = await _userManager.FindByNameAsync(owner).ConfigureAwait(false);
                }
                if (intendedOwner is null)
                {
                    ModelState.AddModelError("Model", "No such owner found.");
                    var vm = await EditViewModel.NewAsync(_userManager, _groupManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }
            }

            if (wikiItem is not null)
            {
                model.OverwritePermission = VerifyPermission(wikiItem, user, userPage: false, groupPage: false, edit: true);
                if (!model.OverwriteConfirm || !model.OverwritePermission)
                {
                    return View("OverwriteFileConfirm", model);
                }
            }

            if (model.ShowPreview)
            {
                var vm = new UploadViewModel(data, model.Markdown, model.Title);
                return View("Upload", vm);
            }

            var tempPath = Path.Combine(_environment.WebRootPath, "files", "temp", model.File);
            FileInfo fileInfo;
            try
            {
                if (!Directory.Exists(tempPath))
                {
                    model.File = null;
                    return View("Upload", model);
                }
                var files = Directory.GetFiles(tempPath);
                if (files.Length == 0)
                {
                    Directory.Delete(tempPath);
                    model.File = null;
                    return View("Upload", model);
                }
                fileInfo = new FileInfo(files[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file copy for temp file with id {ID}", model.File);
                return new StatusCodeResult(500);
            }

            var size = fileInfo.Length / 1000;
            if (size > WikiWebConfig.MaxFileSize || (limit > 0 && size > limit))
            {
                return View("NotAuthorizedForUploadSize");
            }

            if (limit > 0)
            {
                var nextPage = false;
                var totalSize = size;
                do
                {
                    var result = await _searchClient.SearchAsync(new SearchRequest
                    {
                        WikiNamespace = WikiConfig.FileNamespace,
                        Uploader = user.Id,
                    }, null).ConfigureAwait(false);

                    totalSize += result.SearchHits.OfType<WikiFile>().Sum(x => (long)(x.FileSize / 1000));

                    nextPage = result.SearchHits.HasNextPage;
                } while (nextPage && totalSize < limit);
                if (totalSize > limit)
                {
                    return View("NotAuthorizedForUploadSize");
                }
            }

            var fileName = fileInfo.Name;
            string? storagePath = null;
            try
            {
                storagePath = await _fileManager.SaveFileAsync(fileInfo.OpenRead(), fileInfo.Name, owner).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file upload for file at path {Path}", fileInfo.FullName);
                ModelState.AddModelError(nameof(UploadViewModel.File), "File could not be uploaded");
                return View("Upload", model);
            }
            if (string.IsNullOrWhiteSpace(storagePath))
            {
                ModelState.AddModelError(nameof(UploadViewModel.File), "File could not be uploaded");
                return View("Upload", model);
            }
            try
            {
                fileInfo.Directory?.Delete(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during temp file delete for file at path {Path}", fileInfo.FullName);
            }

            if (wikiItem is not null)
            {
                var oldPath = Path.Combine(_environment.WebRootPath, wikiItem.FilePath[1..]);
                try
                {
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to delete old file {Path} during overwrite operation", oldPath);
                }
            }

            List<string>? allowedEditors = null;
            if (model.AllowedEditors is not null
                && (model.OwnerSelf || owner is not null))
            {
                foreach (var name in model.AllowedEditors.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var id = GetEditorId(name, out var hasId, out var isGroup);

                    if (isGroup)
                    {
                        var group = await _groupManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (group is not null)
                        {
                            (allowedEditors ??= new List<string>()).Add($"G:{group.Id}");
                        }
                    }
                    else
                    {
                        var editor = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (editor is null && !hasId)
                        {
                            editor = await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                        }
                        if (editor is not null)
                        {
                            (allowedEditors ??= new List<string>()).Add(editor.Id);
                        }
                        else if (!hasId)
                        {
                            var groupEditor = await _groupManager.FindByIdAsync(id).ConfigureAwait(false)
                                ?? await _groupManager.FindByNameAsync(id).ConfigureAwait(false);
                            if (groupEditor is not null)
                            {
                                (allowedEditors ??= new List<string>()).Add($"G:{groupEditor.Id}");
                            }
                        }
                    }
                }
            }
            List<string>? allowedViewers = null;
            if (model.AllowedViewers is not null
                && (model.OwnerSelf || owner is not null))
            {
                foreach (var name in model.AllowedViewers.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var id = GetEditorId(name, out var hasId, out var isGroup);

                    if (isGroup)
                    {
                        var group = await _groupManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (group is not null)
                        {
                            (allowedViewers ??= new List<string>()).Add($"G:{group.Id}");
                        }
                    }
                    else
                    {
                        var viewer = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
                        if (viewer is null && !hasId)
                        {
                            viewer = await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                        }
                        if (viewer is not null)
                        {
                            (allowedViewers ??= new List<string>()).Add(viewer.Id);
                        }
                        else if (!hasId)
                        {
                            var groupEditor = await _groupManager.FindByIdAsync(id).ConfigureAwait(false)
                                ?? await _groupManager.FindByNameAsync(id).ConfigureAwait(false);
                            if (groupEditor is not null)
                            {
                                (allowedViewers ??= new List<string>()).Add($"G:{groupEditor.Id}");
                            }
                        }
                    }
                }
            }

            if (wikiItem is null)
            {
                try
                {
                    var newArticle = await WikiFile.NewAsync(
                        title,
                        user.Id,
                        storagePath,
                        (int)size,
                        new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var type) ? type : "application/octet-stream",
                        model.Markdown,
                        model.Comment,
                        owner,
                        allowedEditors,
                        allowedViewers)
                        .ConfigureAwait(false);
                    return RedirectToAction("Read", new { title = newArticle.Title, wikiNamespace = WikiConfig.FileNamespace });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User with ID {UserId} failed to upload a new file with title {Title} of size {Length}.", user.Id, title, size);
                    ModelState.AddModelError("Model", "The file page could not be created.");
                    return View("Upload", model);
                }
            }

            var newTitle = string.Equals(title.ToWikiTitleCase(), wikiItem.Title, StringComparison.CurrentCulture)
                ? null
                : title.ToWikiTitleCase();

            try
            {
                await wikiItem.ReviseAsync(
                    user.Id,
                    newTitle,
                    storagePath,
                    (int)size,
                    new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var type) ? type : "application/octet-stream",
                    model.Markdown,
                    model.Comment,
                    isDeleted: false,
                    owner,
                    allowedEditors,
                    allowedViewers)
                    .ConfigureAwait(false);
                return RedirectToAction("Read", new { title = newTitle ?? wikiItem.Title, wikiNamespace = WikiConfig.FileNamespace });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User with ID {UserId} failed to upload new file for wiki item with ID {Id}, new title {Title}, and new length {Length}.", user.Id, wikiItem.Id, newTitle, size);
                ModelState.AddModelError("Model", "The file page edit could not be completed.");
                return View("Upload", model);
            }
        }

        /// <summary>
        /// The file upload api endpoint.
        /// </summary>
        [HttpPost("wiki/api/fileupload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (user?.IsDeleted != false || user.IsDisabled)
            {
                return Unauthorized();
            }

            var limit = await _groupManager.UserMaxUploadLimit(user).ConfigureAwait(false);
            if (limit == 0)
            {
                return Unauthorized();
            }

            if (file.Length == 0 || file.Length > WikiWebConfig.MaxFileSize)
            {
                return BadRequest();
            }

            if (file.Length > WikiWebConfig.MaxFileSize
                || (limit > 0 && file.Length / 1000 > limit))
            {
                return Unauthorized();
            }

            if (!IsValidContentType(file.ContentType))
            {
                return new UnsupportedMediaTypeResult();
            }

            var id = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var path = Path.Combine(_environment.WebRootPath, "files", "temp", id, file.FileName);
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using var stream = System.IO.File.Create(path);
                await file.CopyToAsync(stream).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file upload for file at path {Path} with size {Size}", path, file.Length);
                return new StatusCodeResult(500);
            }

            return Ok(id);
        }

        /// <summary>
        /// The delete temp upload file api endpoint.
        /// </summary>
        [HttpDelete("wiki/api/fileupload")]
        public async Task<IActionResult> UploadFileDeleteTempAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false || user.IsDisabled)
            {
                return Unauthorized();
            }

            var path = Path.Combine(_environment.WebRootPath, "files", "temp", id);
            try
            {
                if (!Directory.Exists(path))
                {
                    return Ok();
                }
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file delete for temp file with id {ID}", id);
                return new StatusCodeResult(500);
            }

            return Ok();
        }

        /// <summary>
        /// The fetch remote upload file api endpoint.
        /// </summary>
        [HttpGet("wiki/api/fileupload/fetch/{url}")]
        public async Task<IActionResult> UploadFileFetchRemoteAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)
                || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false || user.IsDisabled)
            {
                return Unauthorized();
            }

            var fileName = uri.Segments[^1];
            if (fileName.EndsWith('/'))
            {
                return BadRequest();
            }

            byte[] bytes;
            try
            {
                using var wc = new WebClient();
                bytes = wc.DownloadData(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file fetch for remote file at URL {URL}", url);
                return new StatusCodeResult(500);
            }

            Response.Headers.Add("Content-Disposition", $"inline;filename=\"{fileName}\"");
            return File(bytes, new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var type) ? type : "application/octet-stream");
        }

        /// <summary>
        /// The restore upload file api endpoint.
        /// </summary>
        [HttpGet("wiki/api/fileupload/{id}")]
        public async Task<IActionResult> UploadFileRestoreAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false || user.IsDisabled)
            {
                return Unauthorized();
            }

            var path = Path.Combine(_environment.WebRootPath, "files");
            string? fileName;
            byte[] bytes;
            try
            {
                if (!Directory.Exists(path))
                {
                    return BadRequest();
                }
                var files = Directory.GetFiles(path);
                if (files.Length == 0)
                {
                    return BadRequest();
                }
                var index = Array.FindIndex(files, x => Path.GetFileNameWithoutExtension(x).Equals(id));
                if (index == -1)
                {
                    return BadRequest();
                }
                fileName = Path.GetFileName(files[index]);
                bytes = await System.IO.File.ReadAllBytesAsync(files[index]).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file restore for temp file with id {ID}", id);
                return new StatusCodeResult(500);
            }

            Response.Headers.Add("Content-Disposition", $"inline;filename=\"{fileName}\"");
            return File(bytes, new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var type) ? type : "application/octet-stream");
        }

        /// <summary>
        /// The restore temp upload file api endpoint.
        /// </summary>
        [HttpGet("wiki/api/fileupload/temp/{id}")]
        public async Task<IActionResult> UploadFileRestoreTempAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false || user.IsDisabled)
            {
                return Unauthorized();
            }

            var path = Path.Combine(_environment.WebRootPath, "files", "temp", id);
            string? fileName;
            byte[] bytes;
            try
            {
                if (!Directory.Exists(path))
                {
                    return BadRequest();
                }
                var files = Directory.GetFiles(path);
                if (files.Length == 0)
                {
                    Directory.Delete(path);
                    return BadRequest();
                }
                bytes = await System.IO.File.ReadAllBytesAsync(files[0]).ConfigureAwait(false);
                fileName = Path.GetFileName(files[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file restore for temp file with id {ID}", id);
                return new StatusCodeResult(500);
            }

            Response.Headers.Add("Content-Disposition", $"inline;filename=\"{fileName}\"");
            return File(bytes, new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var type) ? type : "application/octet-stream");
        }

        /// <summary>
        /// The "what links here" list view.
        /// </summary>
        public Task<IActionResult> WhatLinksHereAsync(
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
            => GetSpecialListAsync(SpecialListType.What_Links_Here, pageNumber, pageSize, sort, descending, filter);

        private static string? GetEditorId(string? name, out bool hasId, out bool isGroup)
        {
            hasId = false;
            isGroup = false;
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var bracketIndex = name.IndexOf('[');
            if (bracketIndex != -1 && name[^1] == ']')
            {
                hasId = true;
                if (name.AsSpan().Slice(bracketIndex + 1, 7).Equals("Group: ", StringComparison.OrdinalIgnoreCase))
                {
                    isGroup = true;
                    return name[(bracketIndex + 8)..^1];
                }
                else
                {
                    return name[(bracketIndex + 1)..^1];
                }
            }

            return name;
        }

        private static Article? GetWikiItem(string title, string wikiNamespace, bool noRedirect = false)
        {
            if (string.Equals(wikiNamespace, WikiConfig.CategoryNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return Category.GetCategory(title);
            }
            else if (string.Equals(wikiNamespace, WikiConfig.FileNamespace, StringComparison.OrdinalIgnoreCase))
            {
                return WikiFile.GetFile(title);
            }
            else
            {
                return Article.GetArticle(title, wikiNamespace, noRedirect);
            }
        }

        private static bool IsValidContentType(string type)
            => type.StartsWith("image/")
            || type.StartsWith("audio/")
            || type.StartsWith("video/")
            || type.Equals("application/pdf");

        private static bool VerifyPermission(WikiRouteData data, IWikiUser? user, bool edit = false)
            => VerifyPermission(data.WikiItem, user, data.IsUserPage, data.IsGroupPage, edit);

        private static bool VerifyPermission(Article? item, IWikiUser? user, bool userPage = false, bool groupPage = false, bool edit = false)
        {
            if (user?.IsDeleted == true || user?.IsDisabled == true)
            {
                return false;
            }

            if (item is null || user?.IsWikiAdmin == true)
            {
                return true;
            }

            if (edit)
            {
                if (user is null)
                {
                    return false;
                }

                if (userPage)
                {
                    return string.Equals(item.Title, user!.Id);
                }

                if (groupPage)
                {
                    if (item.Title == WikiWebConfig.AdminGroupName)
                    {
                        return user.IsWikiAdmin;
                    }
                    else if (user.Groups is null)
                    {
                        return false;
                    }
                    else
                    {
                        return user.Groups.Contains(item.Title);
                    }
                }
            }

            if (item.Owner is null)
            {
                return true;
            }

            if (user is null)
            {
                return edit
                    ? item.AllowedEditors is null
                    : item.AllowedViewers is null;
            }

            if (edit)
            {
                if (item.AllowedEditors is null)
                {
                    return true;
                }
            }
            else if (item.AllowedViewers is null)
            {
                return true;
            }

            if (string.Equals(item.Owner, user.Id))
            {
                return true;
            }

            if (edit
                ? item.AllowedEditors!.Contains(user.Id)
                : item.AllowedViewers!.Contains(user.Id))
            {
                return true;
            }

            if (user.Groups is null)
            {
                return false;
            }

            if (user.Groups.Contains(item.Owner))
            {
                return true;
            }

            var formattedGroups = user.Groups.Select(x => $"G:{x}").ToList();
            return edit
                ? item.AllowedEditors!.Intersect(formattedGroups).Any()
                : item.AllowedViewers!.Intersect(formattedGroups).Any();
        }

        private async Task<IActionResult> GetSpecialListAsync(
            SpecialListType type,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
        {
            var data = GetWikiRouteData();

            if (type == SpecialListType.What_Links_Here)
            {
                data.IsSpecialList = true;

                if (!ControllerContext.RouteData.Values.TryGetValue(WikiRouteData.RouteTitle, out var ti)
                    || ti is not string wT
                    || string.IsNullOrWhiteSpace(wT))
                {
                    return RedirectToAction("Read");
                }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
                data.WikiItem = wikiItem;
                if (wikiItem is null)
                {
                    data.CanEdit = user is not null
                        && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase))
                        && (user.IsWikiAdmin
                        || !WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)));
                }
                else
                {
                    if (!VerifyPermission(data, user))
                    {
                        return View("NotAuthorized", data);
                    }
                    data.CanEdit = VerifyPermission(data, user, edit: true);
                }
            }
            else
            {
                data.IsSystem = true;
                data.Title = data.Title.Replace('_', ' ');
                ViewData["Title"] = type.ToString().Replace('_', ' ');
            }
            var vm = await SpecialListViewModel.NewAsync(data, type, pageNumber, pageSize, sort, descending, filter).ConfigureAwait(false);
            return View("WikiItemList", vm);
        }

        private async Task<Article?> GetWikiItemAsync(WikiRouteData data, bool noRedirect = false)
        {
            var article = GetWikiItem(data.Title, data.WikiNamespace, noRedirect || data.NoRedirect);
            if (article is not null)
            {
                data.DisplayNamespace = article.WikiNamespace;
                data.DisplayTitle = article.Title;
                ViewData["Title"] = Article.GetFullTitle(data.DisplayTitle, data.DisplayNamespace, data.IsTalk);
            }

            if (data.IsUserPage)
            {
                var user = await _userManager.FindByIdAsync(article?.Title ?? data.Title).ConfigureAwait(false)
                    ?? await _userManager.FindByNameAsync(article?.Title ?? data.Title).ConfigureAwait(false);
                if (user is not null)
                {
                    data.DisplayTitle = user.UserName;
                    ViewData["Title"] = Article.GetFullTitle(data.DisplayTitle, data.WikiNamespace, data.IsTalk);

                    if (article is null && user.Id != data.Title)
                    {
                        article = GetWikiItem(user.Id, data.WikiNamespace, noRedirect || data.NoRedirect);
                        if (article is not null)
                        {
                            data.Title = article.Title;
                        }
                    }
                }
            }
            else if (data.IsGroupPage)
            {
                var group = await _groupManager.FindByIdAsync(article?.Title ?? data.Title).ConfigureAwait(false)
                    ?? await _groupManager.FindByNameAsync(article?.Title ?? data.Title).ConfigureAwait(false);
                if (group is not null)
                {
                    data.Group = group;
                    data.DisplayTitle = group.GroupName;
                    ViewData["Title"] = Article.GetFullTitle(data.DisplayTitle, data.WikiNamespace, data.IsTalk);

                    if (article is null && group.Id != data.Title)
                    {
                        article = GetWikiItem(group.Id, data.WikiNamespace, noRedirect || data.NoRedirect);
                        if (article is not null)
                        {
                            data.Title = article.Title;
                        }
                    }
                }
            }

            return article;
        }

        private WikiRouteData GetWikiRouteData()
        {
            var data = new WikiRouteData(_wikiOptions, ControllerContext.RouteData, HttpContext.Request.Query);
            if (data.IsCompact)
            {
                if (!WikiConfig.WikiLinkPrefix.EndsWith("/Compact"))
                {
                    WikiConfig.WikiLinkPrefix += "/Compact";
                }
            }
            else if (WikiConfig.WikiLinkPrefix.EndsWith("/Compact"))
            {
                WikiConfig.WikiLinkPrefix = WikiConfig.WikiLinkPrefix[..^8];
            }
            ViewData[nameof(WikiRouteData)] = data;
            ViewData["Title"] = Article.GetFullTitle(data.Title, data.WikiNamespace, data.IsTalk);
            return data;
        }

        private async Task<IActionResult?> TryGettingSystemPage(WikiRouteData data, IWikiUser? user)
        {
            if (string.Equals(data.Title, "Search", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Search");
            }
            else if (string.Equals(data.Title, "Special", StringComparison.OrdinalIgnoreCase))
            {
                return View("Special");
            }
            else if (string.Equals(data.Title, "Upload", StringComparison.OrdinalIgnoreCase))
            {
                if (user is null)
                {
                    if (!string.IsNullOrEmpty(_wikiOptions.LoginPath))
                    {
                        var url = new StringBuilder(_wikiOptions.LoginPath)
                            .Append(_wikiOptions.LoginPath.Contains('?') ? '&' : '?')
                            .Append("returnUrl=")
                            .Append(HttpContext.Request.GetEncodedUrl())
                            .ToString();
                        return LocalRedirect(url);
                    }
                    return View("NotAuthenticated");
                }

                if (user.IsDeleted || user.IsDisabled)
                {
                    return View("NotAuthorizedToUpload");
                }

                var limit = await _groupManager.UserMaxUploadLimit(user).ConfigureAwait(false);
                if (limit == 0)
                {
                    return View("NotAuthorizedToUpload");
                }

                return View("Upload", new UploadViewModel(data));
            }
            else if (Enum.TryParse<SpecialListType>(data.Title, ignoreCase: true, out var type))
            {
                var pageNumber = HttpContext.Request.Query.TryGetValue("pageNumber", out var n)
                    && n.Count >= 1
                    && int.TryParse(n[0], out var pN)
                    ? pN
                    : 1;
                var pageSize = HttpContext.Request.Query.TryGetValue("pageSize", out var p)
                    && p.Count >= 1
                    && int.TryParse(p[0], out var pS)
                    ? pS
                    : 50;
                var sort = HttpContext.Request.Query.TryGetValue("sort", out var s)
                    && s.Count >= 1
                    ? s[0]
                    : null;
                var descending = HttpContext.Request.Query.TryGetValue("descending", out var d)
                    && d.Count >= 1
                    && bool.TryParse(d[0], out var ds)
                    && ds;
                var filter = HttpContext.Request.Query.TryGetValue("filter", out var f)
                    && f.Count >= 1
                    ? f[0]
                    : null;
                return await GetSpecialListAsync(
                    type,
                    pageNumber,
                    pageSize,
                    sort,
                    descending,
                    filter).ConfigureAwait(false);
            }

            return null;
        }
    }
}
