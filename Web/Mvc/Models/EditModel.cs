using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NeverFoundry.Wiki.Mvc.Models
{
    /// <summary>
    /// The edit DTO.
    /// </summary>
    public class EditModel
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
        /// Whether this is a delete operation.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// The ID of the item.
        /// </summary>
        [HiddenInput]
        public string? Id { get; set; }

        /// <summary>
        /// The markdown content.
        /// </summary>
        public string? Markdown { get; set; }

        /// <summary>
        /// The original title of this item.
        /// </summary>
        [HiddenInput]
        public string? OriginalTitle { get; set; }

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
        /// Whether to make the owner the only allowed editor.
        /// </summary>
        [Display(Name = "Make me the only editor")]
        public bool EditorSelf { get; set; }

        /// <summary>
        /// Whether to make the owner the only allowed viewer.
        /// </summary>
        [Display(Name = "Make me the only viewer")]
        public bool ViewerSelf { get; set; }

        /// <summary>
        /// Whether to automatically create a redirect for a renamed article.
        /// </summary>
        [Display(Name = "Leave a redirect behind")]
        public bool Redirect { get; set; } = true;

        /// <summary>
        /// Whether this is a request to show a preview.
        /// </summary>
        public bool ShowPreview { get; set; }

        /// <summary>
        /// The title.
        /// </summary>
        [Required]
        public string? Title { get; set; }
    }
}
