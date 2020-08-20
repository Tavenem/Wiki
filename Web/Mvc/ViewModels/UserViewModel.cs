using NeverFoundry.Wiki.Web;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The user DTO.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether there is a wiki user page for the user.
        /// </summary>
        public bool PageExists { get; set; }

        /// <summary>
        /// The user's user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="user"></param>
        public UserViewModel(IWikiUser user)
        {
            Id = user.Id;
            PageExists = !(Article.GetArticle(user.Id, WikiWebConfig.UserNamespace) is null);
            UserName = user.UserName;
        }
    }
}
