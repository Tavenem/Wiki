using NeverFoundry.Wiki.Web;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The revision DTO.
    /// </summary>
    public class RevisionViewModel
    {
        /// <summary>
        /// The revision.
        /// </summary>
        public Revision Revision { get; set; }

        /// <summary>
        /// Whether the editor still esists as a wiki user.
        /// </summary>
        public bool EditorExists { get; set; }

        /// <summary>
        /// The editor's user name.
        /// </summary>
        public string EditorName { get; set; }

        /// <summary>
        /// Whether there is a user page associated with the editor.
        /// </summary>
        public bool EditorPageExists { get; set; }

        /// <summary>
        /// Initialize a new <see cref="RevisionViewModel"/>.
        /// </summary>
        public RevisionViewModel(Revision revision, bool userExists, string userName, bool userPageExists)
        {
            Revision = revision;
            EditorExists = userExists;
            EditorName = userName;
            EditorPageExists = userPageExists;
        }

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
