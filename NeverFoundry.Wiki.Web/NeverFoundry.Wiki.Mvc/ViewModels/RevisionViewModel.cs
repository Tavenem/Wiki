using NeverFoundry.Wiki.Mvc.Services;
using NeverFoundry.Wiki.Web;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class RevisionViewModel
    {
        public WikiRevision Revision { get; set; }

        public bool EditorExists { get; set; }

        public string EditorName { get; set; }

        public bool EditorPageExists { get; set; }

        public RevisionViewModel(WikiRevision revision, bool userExists, string userName, bool userPageExists)
        {
            Revision = revision;
            EditorExists = userExists;
            EditorName = userName;
            EditorPageExists = userPageExists;
        }

        public static async Task<RevisionViewModel> NewAsync(IUserManager userManager, WikiRevision revision)
        {
            var editor = await userManager.FindByIdAsync(revision.Editor).ConfigureAwait(false);
            var userExists = !(editor is null);
            var userPageExists = userExists && !(Article.GetArticle(revision.Editor, WikiWebConfig.UserNamespace) is null);
            return new RevisionViewModel(revision, userExists, editor?.UserName ?? revision.Editor, userPageExists);
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
