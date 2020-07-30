using Microsoft.AspNetCore.Mvc;
using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class EditViewModel
    {
        [Display(Name = "Allowed editors (optional)")]
        public string? AllowedEditors { get; set; }

        [Display(Name = "Allowed viewers (optional)")]
        public string? AllowedViewers { get; set; }

        [Display(Name = "Revision comment (e.g. briefly describe your changes)")]
        public string? Comment { get; set; }

        public WikiRouteData Data { get; }

        [HiddenInput]
        public string? Id => Data.WikiItem?.Id;

        public bool IsOutdated { get; }

        public string Markdown { get; set; }

        public string? Owner { get; set; }

        [Display(Name = "Make me the owner")]
        public bool OwnerSelf { get; set; }

        public string? Preview { get; }

        [Display(Name = "Leave a redirect behind")]
        public bool Redirect { get; set; } = true;

        [Required]
        public string? Title { get; set; }

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

        public static async Task<EditViewModel> NewAsync(
            IWikiUserManager userManager,
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
                        var group = await WikiConfig.DataStore.GetItemAsync<IWikiGroup>(editorId).ConfigureAwait(false);
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
                        var group = await WikiConfig.DataStore.GetItemAsync<IWikiGroup>(viewerId).ConfigureAwait(false);
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
#pragma warning restore CS1591 // No documentation for "internal" code
}
