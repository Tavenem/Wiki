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
            var factory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            if (factory is null)
            {
                throw new Exception();
            }
            using var serviceScope = factory.CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();

            await SeedUsersAsync(serviceScope).ConfigureAwait(false);

            var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<WikiUser>>();
            var admin = await userMgr.FindByNameAsync(AdminUsername).ConfigureAwait(false);
            var adminId = admin?.Id;

            if (string.IsNullOrEmpty(adminId))
            {
                throw new Exception("Admin not found");
            }

            WikiConfig.DataStore = serviceScope.ServiceProvider.GetRequiredService<IDataStore>();

            await SharedSeed.AddDefaultWikiPagesAsync(adminId).ConfigureAwait(false);
        }

        private static async Task SeedUsersAsync(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            if (context is null)
            {
                throw new Exception();
            }
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<WikiUser>>();
            const string AdminEmail = "admin@neverfoundry.com";
            var admin = await userMgr.FindByEmailAsync(AdminEmail).ConfigureAwait(false);
            if (admin is null)
            {
                admin = new WikiUser
                {
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    HasUploadPermission = true,
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

            var adminGroup = await WikiConfig.DataStore
                .Query<WikiGroup>()
                .FirstOrDefaultAsync(x => x.GroupName == WikiWebConfig.AdminGroupName)
                .ConfigureAwait(false);
            if (adminGroup is null)
            {
                adminGroup = new WikiGroup(WikiWebConfig.AdminGroupName, admin.Id, true);
                await WikiConfig.DataStore.StoreItemAsync(adminGroup).ConfigureAwait(false);
            }
        }
    }
}
