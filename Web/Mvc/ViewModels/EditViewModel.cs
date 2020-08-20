using Microsoft.AspNetCore.Mvc;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The edit DTO.
    /// </summary>
    public class EditViewModel
    {
        /// <summary>
        /// The allowed editor IDs.
        /// </summary>
        [Display(Name = "Allowed editors (optional)")]
        public string? AllowedEditors { get; set; }

        /// <summary>
        /// The allowed viewer IDs.
        /// </summary>
        [Display(Name = "Allowed viewers (optional)")]
        public string? AllowedViewers { get; set; }

        /// <summary>
        /// The optional comment.
        /// </summary>
        [Display(Name = "Revision comment (e.g. briefly describe your changes)")]
        public string? Comment { get; set; }

        /// <summary>
        /// The associated <see cref="WikiRouteData"/>.
        /// </summary>
        public WikiRouteData Data { get; }

        /// <summary>
        /// The ID of the item.
        /// </summary>
        [HiddenInput]
        public string? Id => Data.WikiItem?.Id;

        /// <summary>
        /// Whether the item has been edited since starting this edit session.
        /// </summary>
        public bool IsOutdated { get; }

        /// <summary>
        /// The markdown content.
        /// </summary>
        public string Markdown { get; set; }

        /// <summary>
        /// The ID of the owner.
        /// </summary>
        public string? Owner { get; set; }

        /// <summary>
        /// Whether to make the owner the editor.
        /// </summary>
        [Display(Name = "Make me the owner")]
        public bool OwnerSelf { get; set; }

        /// <summary>
        /// The rendered preview.
        /// </summary>
        public string? Preview { get; }

        /// <summary>
        /// Whether to automatically create a redirect for a renamed article.
        /// </summary>
        [Display(Name = "Leave a redirect behind")]
        public bool Redirect { get; set; } = true;

        /// <summary>
        /// The title.
        /// </summary>
        [Required]
        public string? Title { get; set; }

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
            string? allowedViewers = null)
        {
            AllowedEditors = allowedEditors;
            AllowedViewers = allowedViewers;
            Data = data;
            IsOutdated = isOutdated;
            Markdown = markdown;
            Owner = data.WikiItem?.Owner;
            OwnerSelf = string.Equals(data.WikiItem?.Owner, user.Id, System.StringComparison.Ordinal);
            Preview = preview;
            Title = previewTitle
                ?? data.WikiItem?.FullTitle
                ?? (string.IsNullOrEmpty(data.Title)
                    ? null
                    : Article.GetFullTitle(data.Title, data.WikiNamespace));
        }

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
            if (!(data.WikiItem is null))
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
            if (!(data.WikiItem?.AllowedEditors is null))
            {
                var editors = new List<string>();
                foreach (var editorId in data.WikiItem.AllowedEditors)
                {
                    var editor = await userManager.FindByIdAsync(editorId).ConfigureAwait(false);
                    if (editor is null)
                    {
                        var group = await groupManager.FindByIdAsync(editorId).ConfigureAwait(false);
                        if (group is null)
                        {
                            editors.Add(editorId);
                        }
                        else
                        {
                            editors.Add(group.GroupName);
                        }
                    }
                    else
                    {
                        editors.Add(editor.UserName);
                    }
                }
                allowedEditors = string.Join("; ", editors);
            }

            string? allowedViewers = null;
            if (!(data.WikiItem?.AllowedViewers is null))
            {
                var viewers = new List<string>();
                foreach (var viewerId in data.WikiItem.AllowedViewers)
                {
                    var viewer = await userManager.FindByIdAsync(viewerId).ConfigureAwait(false);
                    if (viewer is null)
                    {
                        var group = await groupManager.FindByIdAsync(viewerId).ConfigureAwait(false);
                        if (group is null)
                        {
                            viewers.Add(viewerId);
                        }
                        else
                        {
                            viewers.Add(group.GroupName);
                        }
                    }
                    else
                    {
                        viewers.Add(viewer.UserName);
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
