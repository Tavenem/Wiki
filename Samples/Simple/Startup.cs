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

            services.AddWiki(
                typeof(WikiUserManager),
                typeof(WikiGroupManager),
                new WikiOptions
                {
                    LinkTemplate = WikiMvcOptions.DefaultLinkTemplate,
                },
                new WikiWebOptions
                {
                    ContactPageTitle = null,
                    ContentsPageTitle = null,
                    CopyrightPageTitle = null,
                    MaxFileSize = 0,
                    PolicyPageTitle = null,
                },
                new WikiMvcOptions
                {
                    CompactRoutePort = 5003,
                    CompactLayoutPath = "/Pages/Shared/_Layout.cshtml",
                    LoginPath = "/",
                    MainLayoutPath = "/Pages/Shared/_Layout.cshtml",
                    TenorAPIKey = "ZB1P1TN5PVFQ",
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            Seed.AddDefaultWikiPagesAsync(
                serviceProvider.GetRequiredService<IWikiOptions>(),
                serviceProvider.GetRequiredService<IWikiWebOptions>(),
                serviceProvider.GetRequiredService<IDataStore>(),
                WikiUserManager.UserId)
                .GetAwaiter()
                .GetResult();

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
