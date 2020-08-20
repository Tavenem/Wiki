using NeverFoundry.Wiki.MarkdownExtensions.Transclusions;
using System.ComponentModel.DataAnnotations;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class UploadViewModel
    {
        [Display(Name = "Allowed editors (optional)")]
        public string? AllowedEditors { get; set; }

        [Display(Name = "Allowed viewers (optional)")]
        public string? AllowedViewers { get; set; }

        [Display(Name = "Revision comment (e.g. briefly describe your changes)")]
        public string? Comment { get; set; }

        public WikiRouteData Data { get; }

        [Required]
        public string? File { get; set; }

        public string? Markdown { get; set; }

        public bool OverwriteConfirm { get; set; }

        public bool OverwritePermission { get; set; }

        public string? Owner { get; set; }

        [Display(Name = "Make me the owner")]
        public bool OwnerSelf { get; set; }

        public string? Preview { get; }

        public bool ShowPreview { get; set; }

        [Required]
        public string? Title { get; set; }

        public UploadViewModel() => Data = new WikiRouteData();

        public UploadViewModel(
            WikiRouteData data,
            string? markdown = null,
            string? previewTitle = null)
        {
            Data = data;

            Markdown = markdown;

            if (!string.IsNullOrWhiteSpace(previewTitle))
            {
                var (wikiNamespace, title, _, _) = Article.GetTitleParts(previewTitle);
                var fullTitle = Article.GetFullTitle(title, wikiNamespace);
                Preview = string.IsNullOrWhiteSpace(markdown)
                    ? null
                    : MarkdownItem.RenderHtml(TransclusionParser.Transclude(title, fullTitle, markdown, out _));
            }

            Title = previewTitle;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
