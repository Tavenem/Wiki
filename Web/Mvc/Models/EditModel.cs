using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NeverFoundry.Wiki.Mvc.Models
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class EditModel
    {
        [Display(Name = "Allowed editors (optional)")]
        public string? AllowedEditors { get; set; }

        [Display(Name = "Allowed viewers (optional)")]
        public string? AllowedViewers { get; set; }

        [Display(Name = "Revision comment (e.g. briefly describe your changes)")]
        public string? Comment { get; set; }

        public bool Delete { get; set; }

        [HiddenInput]
        public string? Id { get; set; }

        public string? Markdown { get; set; }

        public string? Owner { get; set; }

        [Display(Name = "Make me the owner")]
        public bool OwnerSelf { get; set; }

        [Display(Name = "Leave a redirect behind")]
        public bool Redirect { get; set; } = true;

        public bool ShowPreview { get; set; }

        [Required]
        public string? Title { get; set; }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
