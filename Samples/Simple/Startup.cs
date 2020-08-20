using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Samples.Data;
using NeverFoundry.Wiki.Samples.Simple.Services;
using NeverFoundry.Wiki.Web;

namespace NeverFoundry.Wiki.Samples.Simple
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddSignalR(options => options.EnableDetailedErrors = true);

            var dataStore = new InMemoryDataStore();
            services.AddSingleton<IDataStore>(dataStore);
            WikiConfig.DataStore = dataStore;

            WikiWebConfig.ContactPageTitle = null;
            WikiWebConfig.ContentsPageTitle = null;
            WikiWebConfig.CopyrightPageTitle = null;
            WikiWebConfig.PolicyPageTitle = null;
            WikiWebConfig.MaxFileSize = 0;
            services.AddWiki(
                provider => new WikiUserManager(),
                provider => new WikiGroupManager(),
                provider => new WikiOptions
                {
                    CompactLayoutPath = "/Pages/Shared/_Layout.cshtml",
                    LoginPath = "/",
                    MainLayoutPath = "/Pages/Shared/_Layout.cshtml",
                    TenorAPIKey = "ZB1P1TN5PVFQ",
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Seed.AddDefaultWikiPagesAsync(WikiUserManager.UserId).GetAwaiter().GetResult();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapWiki();
                endpoints.MapRazorPages();
            });
        }
    }
}
