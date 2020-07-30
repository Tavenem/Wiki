using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.MvcSample.Providers
{
    public class Fido2TokenProvider : IUserTwoFactorTokenProvider<WikiUser>
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<WikiUser> manager, WikiUser user)
            => Task.FromResult(true);

        public Task<string> GenerateAsync(string purpose, UserManager<WikiUser> manager, WikiUser user)
            => Task.FromResult("fido2");

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<WikiUser> manager, WikiUser user)
            => Task.FromResult(true);
    }
}
