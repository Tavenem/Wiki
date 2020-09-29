using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Web;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SharedSeed = NeverFoundry.Wiki.Samples.Data.Seed;

namespace NeverFoundry.Wiki.Samples.Complete.Data
{
    public static class Seed
    {
        private const string AdminUsername = "Admin";

        public static async Task InitializeDatabasesAsync(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            serviceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();

            await SeedUsersAsync(serviceProvider).ConfigureAwait(false);

            var userMgr = serviceProvider.GetRequiredService<UserManager<WikiUser>>();
            var admin = await userMgr.FindByNameAsync(AdminUsername).ConfigureAwait(false);
            var adminId = admin?.Id;

            if (string.IsNullOrEmpty(adminId))
            {
                throw new Exception("Admin not found");
            }

            await SharedSeed.AddDefaultWikiPagesAsync(
                serviceProvider.GetRequiredService<IWikiOptions>(),
                serviceProvider.GetRequiredService<IWikiWebOptions>(),
                serviceProvider.GetRequiredService<IDataStore>(),
                adminId).ConfigureAwait(false);
        }

        private static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<IdentityDbContext>();
            if (context is null)
            {
                throw new Exception();
            }
            context.Database.Migrate();

            var userMgr = serviceProvider.GetRequiredService<UserManager<WikiUser>>();
            const string AdminEmail = "admin@neverfoundry.com";
            var admin = await userMgr.FindByEmailAsync(AdminEmail).ConfigureAwait(false);
            if (admin is null)
            {
                admin = new WikiUser
                {
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    UploadLimit = -1,
                    UserName = AdminUsername,
                };
                var result = await userMgr.CreateAsync(admin, "Admin1!").ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
                result = await userMgr.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(WikiClaims.Claim_SiteAdmin, "true", ClaimValueTypes.Boolean),
                    new Claim(WikiClaims.Claim_WikiAdmin, "true", ClaimValueTypes.Boolean),
                }).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
            }

            var exampleUser = await userMgr.FindByNameAsync("example").ConfigureAwait(false);
            if (exampleUser is null)
            {
                exampleUser = new WikiUser
                {
                    Email = "example@example.com",
                    EmailConfirmed = true,
                    UserName = "example",
                };
                var result = await userMgr.CreateAsync(exampleUser, "E#amp1e").ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
            }

            var wikiWebOptions = serviceProvider.GetRequiredService<IWikiWebOptions>();
            var dataStore = serviceProvider.GetRequiredService<IDataStore>();
            var adminGroup = await dataStore
                .Query<WikiGroup>()
                .FirstOrDefaultAsync(x => x.GroupName == wikiWebOptions.AdminGroupName)
                .ConfigureAwait(false);
            if (adminGroup is null)
            {
                adminGroup = new WikiGroup(wikiWebOptions.AdminGroupName, admin.Id, -1);
                await dataStore.StoreItemAsync(adminGroup).ConfigureAwait(false);
            }
        }
    }
}
