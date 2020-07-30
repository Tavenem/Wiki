using NeverFoundry.Wiki.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class GroupViewModel : WikiItemViewModel
    {
        public IList<UserViewModel> Users { get; set; }

        public GroupViewModel(
            WikiRouteData data,
            string html,
            bool isDiff,
            IEnumerable<UserViewModel> users) : base(data, html, isDiff) => Users = users.ToList();

        public static async Task<GroupViewModel> NewAsync(IWikiUserManager userManager, WikiRouteData data, WikiItemViewModel vm)
        {
            var users = new List<IWikiUser>();
            if (data.Group is not null)
            {
                users.AddRange(await userManager.GetUsersInGroupAsync(data.Group).ConfigureAwait(false));
            }

            return new GroupViewModel(
                data,
                vm.Html,
                vm.IsDiff,
                users.Select(x => new UserViewModel(x)));
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
