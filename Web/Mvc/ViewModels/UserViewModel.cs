using NeverFoundry.Wiki.Web;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
#pragma warning disable CS1591 // No documentation for "internal" code
    public class UserViewModel
    {
        public string Id { get; set; }

        public bool PageExists { get; set; }

        public string UserName { get; set; }

        public UserViewModel(IWikiUser user)
        {
            Id = user.Id;
            PageExists = !(Article.GetArticle(user.Id, WikiWebConfig.UserNamespace) is null);
            UserName = user.UserName;
        }
    }
#pragma warning restore CS1591 // No documentation for "internal" code
}
