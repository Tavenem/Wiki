using Microsoft.AspNetCore.Mvc;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using NeverFoundry.Wiki.Mvc.Controllers;
using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The edit DTO.
    /// </summary>
    public record EditViewModel
    (
        [Display(Name = "Allowed editors (optional)")]
        string? AllowedEditors,

        [Display(Name = "Allowed viewers (optional)")]
        string? AllowedViewers,

        WikiRouteData Data,
        bool IsOutdated,
        string Markdown,
        string? Owner,

        [Display(Name = "Make me the owner")]
        bool OwnerSelf,

        string? Preview,

        [Required] string? Title,

        [Display(Name = "Revision comment (e.g. briefly describe your changes)")]
        string? Comment = null,

        [Display(Name = "Leave a redirect behind")]
        bool Redirect = true)
    {

        /// <summary>
        /// The ID of the item.
        /// </summary>
        [HiddenInput]
        public string? Id => Data.WikiItem?.Id;

        /// <summary>
        /// Initialize a new instance of <see cref="EditViewModel"/>.
        /// </summary>
        public EditViewModel(
                WikiRouteData data,
                IWikiUser user,
                string markdown,
                string? previewTitle = null,
                string? preview = null,
                bool isOutdated = false,
                string? allowedEditors = null,
                string? allowedViewers = null) : this(
                    allowedEditors,
                    allowedViewers,
                    data,
                    isOutdated,
                    markdown,
                    data.WikiItem?.Owner,
                    string.Equals(data.WikiItem?.Owner, user.Id, System.StringComparison.Ordinal),
                    preview,
                    previewTitle
                        ?? data.WikiItem?.FullTitle
                        ?? (string.IsNullOrEmpty(data.Title)
                            ? null
                            : Article.GetFullTitle(data.Title, data.WikiNamespace)))
        { }

        /// <summary>
        /// Get a new <see cref="EditViewModel"/>.
        /// </summary>
        public static async Task<EditViewModel> NewAsync(
            IWikiUserManager userManager,
            IWikiGroupManager groupManager,
            WikiRouteData data,
            IWikiUser user,
            string? markdown = null,
            string? previewTitle = null)
        {
            var isOutdated = false;
            if (data.WikiItem is not null)
            {
                isOutdated = data.RequestedTimestamp.HasValue;
                if (string.IsNullOrWhiteSpace(markdown)
                    && string.IsNullOrWhiteSpace(previewTitle))
                {
                    markdown = data.RequestedTimestamp.HasValue
                        ? await data.WikiItem.GetMarkdownAsync(data.RequestedTimestamp.Value).ConfigureAwait(false)
                        : data.WikiItem.MarkdownContent;
                }
            }

            string? preview = null;
            if (!string.IsNullOrWhiteSpace(previewTitle))
            {
                var (wikiNamespace, title, _, _) = Article.GetTitleParts(previewTitle);
                var fullTitle = Article.GetFullTitle(title, wikiNamespace);
                preview = string.IsNullOrWhiteSpace(markdown)
                    ? null
                    : MarkdownItem.RenderHtml(TransclusionParser.Transclude(title, fullTitle, markdown, out _));
            }

            string? allowedEditors = null;
            if (data.WikiItem?.AllowedEditors is not null)
            {
                var editors = new List<string>();
                foreach (var editorId in data.WikiItem.AllowedEditors)
                {
                    if (editorId.StartsWith("G:"))
                    {
                        var group = await groupManager.FindByIdAsync(editorId[2..]).ConfigureAwait(false);
                        if (group is null)
                        {
                            editors.Add(editorId);
                        }
                        else
                        {
                            editors.Add($"{group.GroupName} [Group: {group.Id}]");
                        }
                    }
                    else
                    {
                        var editor = await userManager.FindByIdAsync(editorId).ConfigureAwait(false);
                        if (editor is null)
                        {
                            editors.Add(editorId);
                        }
                        else
                        {
                            editors.Add($"{editor.UserName} [{editor.Id}]");
                        }
                    }
                }
                allowedEditors = string.Join("; ", editors);
            }

            string? allowedViewers = null;
            if (data.WikiItem?.AllowedViewers is not null)
            {
                var viewers = new List<string>();
                foreach (var viewerId in data.WikiItem.AllowedViewers)
                {
                    if (viewerId.StartsWith("G:"))
                    {
                        var group = await groupManager.FindByIdAsync(viewerId[2..]).ConfigureAwait(false);
                        if (group is null)
                        {
                            viewers.Add(viewerId);
                        }
                        else
                        {
                            viewers.Add($"{group.GroupName} [Group: {group.Id}]");
                        }
                    }
                    else
                    {
                        var viewer = await userManager.FindByIdAsync(viewerId).ConfigureAwait(false);
                        if (viewer is null)
                        {
                            viewers.Add(viewerId);
                        }
                        else
                        {
                            viewers.Add($"{viewer.UserName} [{viewer.Id}]");
                        }
                    }
                }
                allowedViewers = string.Join("; ", viewers);
            }

            return new EditViewModel(
                data,
                user,
                markdown ?? string.Empty,
                previewTitle,
                preview,
                isOutdated,
                allowedEditors,
                allowedViewers);
        }
    }
}
