using NeverFoundry.Wiki.Web;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The revision DTO.
    /// </summary>
    public record RevisionViewModel(Revision Revision, bool EditorExists, string EditorName, bool EditorPageExists)
    {
        /// <summary>
        /// Get a new <see cref="RevisionViewModel"/>.
        /// </summary>
        public static async Task<RevisionViewModel> NewAsync(IWikiUserManager userManager, Revision revision)
        {
            var editor = await userManager.FindByIdAsync(revision.Editor).ConfigureAwait(false);
            var userExists = editor is not null;
            var userPageExists = userExists && !(Article.GetArticle(revision.Editor, WikiWebConfig.UserNamespace) is null);
            return new RevisionViewModel(revision, userExists, editor?.UserName ?? revision.Editor, userPageExists);
        }
    }
}
