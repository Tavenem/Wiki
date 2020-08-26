using NeverFoundry.Wiki.Web;

namespace NeverFoundry.Wiki.Mvc.ViewModels
{
    /// <summary>
    /// The user DTO.
    /// </summary>
    public record UserViewModel(string Id, bool PageExists, string UserName)
    {
        /// <summary>
        /// Initialize a new instance of <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="user"></param>
        public UserViewModel(IWikiUser user) : this(user.Id, Article.GetArticle(user.Id, WikiWebConfig.UserNamespace) is not null, user.UserName) { }
    }
}
