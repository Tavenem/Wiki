using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.Mvc.Hubs;
using NeverFoundry.Wiki.Mvc.Models;
using NeverFoundry.Wiki.Mvc.Services;
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
#pragma warning disable CS1591 // No documentation for "internal" code
    public class WikiController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<WikiController> _logger;
        private readonly ISearchClient _searchClient;
        private readonly IUserManager _userManager;

        public WikiController(
            IWebHostEnvironment environment,
            ILogger<WikiController> logger,
            ISearchClient searchClient,
            IUserManager userManager)
        {
            _environment = environment;
            _logger = logger;
            _searchClient = searchClient;
            _userManager = userManager;
        }

        public async Task<IActionResult> EditAsync()
        {
            var data = GetWikiRouteData();

            data.IsEdit = true;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(WikiWebConfig.LoginPath))
                {
                    var url = new StringBuilder(WikiWebConfig.LoginPath)
                        .Append(WikiWebConfig.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            data.WikiItem = wikiItem;
            data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);
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
                else if (WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                    {
                        return View("NotAuthorized", data);
                    }
                }
            }

            var markdown = string.Empty;
            if (!(wikiItem is null))
            {
                markdown = wikiItem.MarkdownContent;
                var html = wikiItem.GetHtml();
                data.Categories = wikiItem.Categories;
            }

            var vm = await EditViewModel.NewAsync(_userManager, data, user).ConfigureAwait(false);
            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(EditModel model)
        {
            var data = GetWikiRouteData();

            data.IsEdit = true;

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(WikiWebConfig.LoginPath))
                {
                    var url = new StringBuilder(WikiWebConfig.LoginPath)
                        .Append(WikiWebConfig.LoginPath.Contains('?') ? '&' : '?')
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
            data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);
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
                else if (WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                    {
                        return View("NotAuthorized", data);
                    }
                }
            }

            if (!(wikiItem is null))
            {
                var html = wikiItem.GetHtml();
                data.Categories = wikiItem.Categories;
            }

            if (!ModelState.IsValid)
            {
                var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }

            if (model.ShowPreview)
            {
                var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown, model.Title).ConfigureAwait(false);
                return View("Edit", vm);
            }

            List<string>? allowedEditors = null;
            if (!(model.AllowedEditors is null))
            {
                allowedEditors = new List<string>();
                foreach (var id in model.AllowedEditors.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    var editor = await _userManager.FindByIdAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByEmailAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                    if (!(editor is null))
                    {
                        allowedEditors.Add(editor.Id);
                    }
                }
            }
            List<string>? allowedViewers = null;
            if (!(model.AllowedViewers is null))
            {
                allowedViewers = new List<string>();
                foreach (var id in model.AllowedViewers.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    var viewer = await _userManager.FindByIdAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByEmailAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                    if (!(viewer is null))
                    {
                        allowedViewers.Add(viewer.Id);
                    }
                }
            }

            var owner = model.OwnerSelf ? user.Id : model.Owner;

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
                    var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }

                if (wikiItem is WikiFile file)
                {
                    var path = Path.Combine(_environment.WebRootPath, file.FilePath);
                    try
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unable to delete file {Path} during delete operation", path);
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
                    var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown).ConfigureAwait(false);
                    return View("Edit", vm);
                }
            }

            string? newTitle = null;
            string? newNamespace = null;

            var titleMatches = string.Equals(title.ToWikiTitleCase(), wikiItem.Title, StringComparison.CurrentCulture);
            var namespaceMatches = string.Equals(wikiNamespace.ToWikiTitleCase(), wikiItem.WikiNamespace, StringComparison.CurrentCulture);
            if (!titleMatches || !namespaceMatches)
            {
                if (!titleMatches)
                {
                    newTitle = title.ToWikiTitleCase();
                }
                if (!namespaceMatches)
                {
                    newNamespace = wikiNamespace.ToWikiTitleCase();
                }

                if (model.Redirect)
                {
                    try
                    {
                        await Article.NewAsync(
                            title,
                            user.Id,
                            $"{{{{redirect|{Article.GetFullTitle(newTitle ?? wikiItem.Title, newNamespace ?? wikiItem.WikiNamespace)}}}}}",
                            wikiNamespace,
                            owner,
                            allowedEditors,
                            allowedViewers)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to add redirect to wiki item with ID {Id}, title {Title}, and namespace {WikiNamespace} for user with ID {UserId}.", wikiItem.Id, title, wikiNamespace, user.Id);
                        ModelState.AddModelError("Model", "The redirect could not be created automatically.");
                        var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown).ConfigureAwait(false);
                        return View("Edit", vm);
                    }
                }
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
                return RedirectToAction("Read", new { title = newTitle ?? wikiItem.Title, wikiNamespace = newNamespace ?? wikiItem.WikiNamespace });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User with ID {UserId} failed to edit wiki item with ID {Id}, new title {Title}, and new namespace {WikiNamespace}.", user.Id, wikiItem.Id, newTitle, newNamespace);
                ModelState.AddModelError("Model", "The edit could not be completed.");
                var vm = await EditViewModel.NewAsync(_userManager, data, user, model.Markdown).ConfigureAwait(false);
                return View("Edit", vm);
            }
        }

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
                    && article.AllowedEditors?.Contains(user.Id) != true)
                {
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    var groupIds = claims
                        .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                        .Select(x => x.Value)
                        .ToList();
                    if (!groupIds.Contains(article.Owner)
                        && !article.AllowedViewers.Intersect(groupIds).Any()
                        && article.AllowedEditors?.Intersect(groupIds).Any() != true)
                    {
                        return Json(string.Empty);
                    }
                }
            }

            var content = article.GetPreview();
            return Json(content);
        }

        public async Task<IActionResult> HistoryAsync(
            int pageNumber = 1,
            int pageSize = 50,
            string? editor = null,
            DateTimeOffset? start = null,
            DateTimeOffset? end = null)
        {
            var data = GetWikiRouteData();

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            if (wikiItem is null)
            {
                data.CanEdit = !(user is null)
                    && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase));
                if (data.CanEdit && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var claims = await _userManager.GetClaimsAsync(user!).ConfigureAwait(false);
                    if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                    {
                        data.CanEdit = false;
                    }
                }
                return View("NoContent", data);
            }

            data.WikiItem = wikiItem;
            if (!await VerifyPermission(data, user).ConfigureAwait(false))
            {
                return View("NotAuthorized", data);
            }
            data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);

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

        public async Task<IActionResult> ReadAsync()
        {
            var data = GetWikiRouteData();

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            if (data.IsSystem)
            {
                var special = await TryGettingSystemPage(data, user).ConfigureAwait(false);
                if (!(special is null))
                {
                    return special;
                }
                data.IsSystem = false;
            }

            var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
            if (wikiItem?.IsDeleted != false)
            {
                data.CanEdit = !(user is null)
                    && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase));
                if (data.CanEdit && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var claims = await _userManager.GetClaimsAsync(user!).ConfigureAwait(false);
                    if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                    {
                        data.CanEdit = false;
                    }
                }
                return View("NoContent", data);
            }

            data.WikiItem = wikiItem;
            if (!await VerifyPermission(data, user).ConfigureAwait(false))
            {
                return View("NotAuthorized", data);
            }
            data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);

            if (data.IsTalk)
            {
                var vm = new TalkViewModel(data, wikiItem?.Id);
                if (!(wikiItem is null))
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
                                    var previewHtml = article.GetPreview();
                                    var namespaceStr = article.WikiNamespace == WikiConfig.DefaultNamespace
                                        ? string.Empty
                                        : string.Format(WikiTalkHub.PreviewNamespaceTemplate, article.WikiNamespace);
                                    html = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(string.Format(WikiTalkHub.PreviewTemplate, namespaceStr, article.Title, previewHtml));
                                }
                            }
                        }
                        if (!preview)
                        {
                            html = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(reply.GetHtml());
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
                var groupModel = await GroupViewModel.NewAsync(_userManager, data, model).ConfigureAwait(false);
                return View("Group", groupModel);
            }

            if (data.IsFile)
            {
                return View("File", model);
            }

            return View("Article", model);
        }

        public async Task<IActionResult> SearchAsync(
            string? query = null,
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false)
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

            query = query.Trim();
            var original = query;

            query = query.Trim('"');
            var (wikiNamespace, title, isTalk, _) = Article.GetTitleParts(query);
            var wikiItem = GetWikiItem(title, wikiNamespace);

            if (!original.StartsWith("\"") && !(wikiItem is null))
            {
                return RedirectToAction("Read", new { wikiNamespace = isTalk ? $"{WikiConfig.TalkNamespace}:{wikiNamespace}" : wikiNamespace, title });
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
                    descending
                });
            }

            data.IsSearch = true;

            var result = await _searchClient.SearchAsync(new SearchRequest
            {
                Descending = descending,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Query = query,
                Sort = sort,
            }, User).ConfigureAwait(false);

            return View(new SearchViewModel(result, wikiItem));
        }

        [HttpPost("wiki/api/suggest")]
        public async Task<JsonResult> GetSearchSuggestionsAsync(string? search = null)
        {
            search = WebUtility.UrlDecode(search);

            var (wikiNamespace, title, isTalk, defaultNamespace) = Article.GetTitleParts(search);

            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new string[0]);
            }

            IReadOnlyList<string> items;
            if (defaultNamespace)
            {
                if (wikiNamespace == WikiConfig.FileNamespace)
                {
                    items = await WikiConfig.DataStore.Query<WikiFile>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.FullTitle)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else if (wikiNamespace == WikiConfig.CategoryNamespace)
                {
                    items = await WikiConfig.DataStore.Query<Category>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.CurrentCultureIgnoreCase))
                        .Select(x => x.FullTitle)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    items = await WikiConfig.DataStore.Query<Article>()
                        .Where(x => x.Title.StartsWith(title, StringComparison.CurrentCultureIgnoreCase)
                            && x.WikiNamespace == WikiConfig.DefaultNamespace)
                        .Select(x => x.FullTitle)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
            }
            else
            {
                items = await WikiConfig.DataStore.Query<Article>()
                    .Where(x => x.Title.StartsWith(title, StringComparison.CurrentCultureIgnoreCase)
                        && x.WikiNamespace == wikiNamespace)
                    .Select(x => x.FullTitle)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            return Json(items);
        }

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

        public async Task<IActionResult> ShowUploadAsync(UploadViewModel model)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user is null)
            {
                if (!string.IsNullOrEmpty(WikiWebConfig.LoginPath))
                {
                    var url = new StringBuilder(WikiWebConfig.LoginPath)
                        .Append(WikiWebConfig.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            if (user.IsDeleted
                || user.IsDisabled)
            {
                return View("NotAuthorizedToUpload");
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return View("NotAuthorizedToUpload");
                }
            }

            return View("Upload", model);
        }

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
                if (!string.IsNullOrEmpty(WikiWebConfig.LoginPath))
                {
                    var url = new StringBuilder(WikiWebConfig.LoginPath)
                        .Append(WikiWebConfig.LoginPath.Contains('?') ? '&' : '?')
                        .Append("returnUrl=")
                        .Append(HttpContext.Request.GetEncodedUrl())
                        .ToString();
                    return LocalRedirect(url);
                }
                return View("NotAuthenticated");
            }

            if (user.IsDeleted
                || user.IsDisabled)
            {
                return View("NotAuthorizedToUpload");
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return View("NotAuthorizedToUpload");
                }
            }

            var (wikiNamespace, title, _, defaultNamespace) = Article.GetTitleParts(model.Title);
            if (!defaultNamespace && !string.Equals(wikiNamespace, WikiConfig.FileNamespace, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(UploadViewModel.Title), "Files cannot be given a namespace");
                return View("Upload", model);
            }

            var wikiItem = WikiFile.GetFile(title);
            data.WikiItem = wikiItem;
            data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);
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
                else if (WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                    {
                        return View("NotAuthorized", data);
                    }
                }
            }
            if (!(wikiItem is null))
            {
                model.OverwritePermission = await VerifyPermissionAsync(wikiItem, user, userPage: false, groupPage: false, edit: true).ConfigureAwait(false);
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

            var size = (int)fileInfo.Length;
            var fileName = fileInfo.Name;
            var relativePath = Path.Combine("files", fileName);
            var path = Path.Combine(_environment.WebRootPath, relativePath);
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                System.IO.File.Copy(fileInfo.FullName, path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during file copy for file at path {Path} to destination {Destination}", fileInfo.FullName, path);
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

            if (!(wikiItem is null))
            {
                var oldPath = Path.Combine(_environment.WebRootPath, wikiItem.FilePath.Substring(1));
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
            if (!(model.AllowedEditors is null))
            {
                allowedEditors = new List<string>();
                foreach (var id in model.AllowedEditors.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    var editor = await _userManager.FindByIdAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByEmailAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                    if (!(editor is null))
                    {
                        allowedEditors.Add(editor.Id);
                    }
                }
            }
            List<string>? allowedViewers = null;
            if (!(model.AllowedViewers is null))
            {
                allowedViewers = new List<string>();
                foreach (var id in model.AllowedViewers.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    var viewer = await _userManager.FindByIdAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByEmailAsync(id).ConfigureAwait(false)
                        ?? await _userManager.FindByNameAsync(id).ConfigureAwait(false);
                    if (!(viewer is null))
                    {
                        allowedViewers.Add(viewer.Id);
                    }
                }
            }

            var owner = model.OwnerSelf ? user.Id : model.Owner;

            if (wikiItem is null)
            {
                try
                {
                    var newArticle = await WikiFile.NewAsync(
                        title,
                        user.Id,
                        "/" + relativePath.Replace('\\', '/'),
                        size,
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
                    "/" + relativePath.Replace('\\', '/'),
                    size,
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

        [HttpPost("wiki/api/fileupload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                return Unauthorized();
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return Unauthorized();
                }
            }

            if (file.Length == 0 || file.Length > WikiWebConfig.MaxFileSize)
            {
                return BadRequest();
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

        [HttpDelete("wiki/api/fileupload")]
        public async Task<IActionResult> UploadFileDeleteTempAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                return Unauthorized();
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return Unauthorized();
                }
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

        [HttpGet("wiki/api/fileupload/fetch/{url}")]
        public async Task<IActionResult> UploadFileFetchRemoteAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url)
                || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                return Unauthorized();
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return Unauthorized();
                }
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

        [HttpGet("wiki/api/fileupload/{id}")]
        public async Task<IActionResult> UploadFileRestoreAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                return Unauthorized();
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return Unauthorized();
                }
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

        [HttpGet("wiki/api/fileupload/temp/{id}")]
        public async Task<IActionResult> UploadFileRestoreTempAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user?.IsDeleted != false
                || user.IsDisabled)
            {
                return Unauthorized();
            }
            if (!user.HasUploadPermission)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                var groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value);
                var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                    .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                    .CountAsync()
                    .ConfigureAwait(false);
                if (uploadGroupCount == 0)
                {
                    return Unauthorized();
                }
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

        public Task<IActionResult> WhatLinksHereAsync(
            int pageNumber = 1,
            int pageSize = 50,
            string? sort = null,
            bool descending = false,
            string? filter = null)
            => GetSpecialListAsync(SpecialListType.What_Links_Here, pageNumber, pageSize, sort, descending, filter);

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
                    || !(ti is string wT)
                    || string.IsNullOrWhiteSpace(wT))
                {
                    return RedirectToAction("Read");
                }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var wikiItem = await GetWikiItemAsync(data).ConfigureAwait(false);
                data.WikiItem = wikiItem;
                if (wikiItem is null)
                {
                    data.CanEdit = !(user is null)
                        && !WikiConfig.ReservedNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase));
                    if (data.CanEdit && WikiWebConfig.AdminNamespaces.Any(x => string.Equals(x, data.WikiNamespace, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var claims = await _userManager.GetClaimsAsync(user!).ConfigureAwait(false);
                        if (!claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin))
                        {
                            data.CanEdit = false;
                        }
                    }
                }
                else
                {
                    if (!await VerifyPermission(data, user).ConfigureAwait(false))
                    {
                        return View("NotAuthorized", data);
                    }
                    data.CanEdit = await VerifyPermission(data, user, edit: true).ConfigureAwait(false);
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

        private Article? GetWikiItem(string title, string wikiNamespace, bool noRedirect = false)
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

        private async Task<Article?> GetWikiItemAsync(WikiRouteData data)
        {
            var article = GetWikiItem(data.Title, data.WikiNamespace, data.NoRedirect);

            if (data.IsUserPage)
            {
                var user = await _userManager.FindByIdAsync(article?.Title ?? data.Title).ConfigureAwait(false);
                if (!(user is null))
                {
                    data.DisplayTitle = user.UserName;
                    ViewData["Title"] = Article.GetFullTitle(data.DisplayTitle, data.WikiNamespace, data.IsTalk);

                    if (article is null && user.Id != data.Title)
                    {
                        article = GetWikiItem(user.Id, data.WikiNamespace, data.NoRedirect);
                        if (!(article is null))
                        {
                            data.Title = article.Title;
                        }
                    }
                }
            }
            else if (data.IsGroupPage)
            {
                var group = await WikiConfig.DataStore.GetItemAsync<IWikiGroup>(article?.Title ?? data.Title).ConfigureAwait(false);
                if (!(group is null))
                {
                    data.Group = group;
                    data.DisplayTitle = group.GroupName;
                    ViewData["Title"] = Article.GetFullTitle(data.DisplayTitle, data.WikiNamespace, data.IsTalk);

                    if (article is null && group.Id != data.Title)
                    {
                        article = GetWikiItem(group.Id, data.WikiNamespace, data.NoRedirect);
                        if (!(article is null))
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
            var data = new WikiRouteData(ControllerContext.RouteData, HttpContext.Request.Query);
            if (data.IsCompact)
            {
                if (!WikiConfig.ServerUrl.EndsWith("Compact/"))
                {
                    WikiConfig.ServerUrl += WikiConfig.ServerUrl.EndsWith("/")
                        ? "Compact/"
                        : "/Compact/";
                }
            }
            else if (WikiConfig.ServerUrl.EndsWith("Compact/"))
            {
                WikiConfig.ServerUrl = WikiConfig.ServerUrl[..^8];
            }
            ViewData[nameof(WikiRouteData)] = data;
            ViewData["Title"] = Article.GetFullTitle(data.Title, data.WikiNamespace, data.IsTalk);
            return data;
        }

        private bool IsValidContentType(string type)
            => type.StartsWith("image/")
            || type.StartsWith("audio/")
            || type.StartsWith("video/")
            || type.Equals("application/pdf");

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
                    if (!string.IsNullOrEmpty(WikiWebConfig.LoginPath))
                    {
                        var url = new StringBuilder(WikiWebConfig.LoginPath)
                            .Append(WikiWebConfig.LoginPath.Contains('?') ? '&' : '?')
                            .Append("returnUrl=")
                            .Append(HttpContext.Request.GetEncodedUrl())
                            .ToString();
                        return LocalRedirect(url);
                    }
                    return View("NotAuthenticated");
                }

                if (user.IsDeleted
                    || user.IsDisabled)
                {
                    return View("NotAuthorizedToUpload");
                }
                if (!user.HasUploadPermission)
                {
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    var groupIds = claims
                        .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                        .Select(x => x.Value);
                    var uploadGroupCount = await WikiConfig.DataStore.Query<IWikiGroup>()
                        .Where(x => groupIds.Contains(x.Id) && x.HasUploadPermission)
                        .CountAsync()
                        .ConfigureAwait(false);
                    if (uploadGroupCount == 0)
                    {
                        return View("NotAuthorizedToUpload");
                    }
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

        private ValueTask<bool> VerifyPermission(WikiRouteData data, IWikiUser? user, bool edit = false)
            => VerifyPermissionAsync(data.WikiItem, user, data.IsUserPage, data.IsGroupPage, edit);

        private async ValueTask<bool> VerifyPermissionAsync(Article? item, IWikiUser? user, bool userPage = false, bool groupPage = false, bool edit = false)
        {
            if (user?.IsDeleted == true || user?.IsDisabled == true)
            {
                return false;
            }

            if (item is null)
            {
                return true;
            }

            List<string>? groupIds = null;
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
                    var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                    if (item.Title == WikiClaims.Claim_WikiAdmin)
                    {
                        return claims.HasBoolClaim(WikiClaims.Claim_WikiAdmin);
                    }
                    else
                    {
                        groupIds = claims
                            .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                            .Select(x => x.Value)
                            .ToList();
                        return groupIds.Contains(item.Title);
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

            if (string.Equals(item.Owner, user.Id)
                || (edit
                ? item.AllowedEditors?.Contains(user.Id) != false
                : item.AllowedViewers?.Contains(user.Id) != false))
            {
                return true;
            }

            if (groupIds is null)
            {
                var claims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
                groupIds = claims
                    .Where(x => x.Type == WikiClaims.Claim_WikiGroup)
                    .Select(x => x.Value)
                    .ToList();
            }
            return groupIds.Contains(item.Owner)
                || (edit
                ? item.AllowedEditors?.Intersect(groupIds).Any() != false
                : item.AllowedViewers?.Intersect(groupIds).Any() != false);
        }
    }
#pragma warning restore CS1591
}
