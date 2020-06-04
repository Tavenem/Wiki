using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeverFoundry.Wiki.Mvc.Hubs;
using NeverFoundry.Wiki.Mvc.Services.Search;
using NeverFoundry.Wiki.Web;
using System;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// Contains the <see cref="MapWiki(IEndpointRouteBuilder)"/> extension method for <see
    /// cref="IEndpointRouteBuilder"/>.
    /// </summary>
    public static class WikiEndpointRouteBuilderExtensions
    {
        private const string LinkTemplate = "onmousemove=\"wikimvc.showPreview(event, '{LINK}');\" onmouseleave=\"wikimvc.hidePreview();\"";
        private const string WikiController = "Wiki";

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="options">
        /// The options used to configure the wiki system.
        /// </param>
        public static void AddWiki(this IServiceCollection services, IWikiOptions? options = null)
        {
            WikiWebConfig.CompactLayoutPath = options?.CompactLayoutPath;
            WikiWebConfig.LoginPath = options?.LoginPath;
            WikiWebConfig.MainLayoutPath = options?.MainLayoutPath;
            WikiWebConfig.TenorAPIKey = options?.TenorAPIKey;

            if (options?.SearchClient is null)
            {
                services.AddScoped<ISearchClient>(provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var userManager = provider.GetRequiredService<UserManager<WikiUser>>();
                    return new DefaultSearchClient(loggerFactory.CreateLogger<DefaultSearchClient>(), userManager);
                });
            }
            else
            {
                services.AddScoped(_ => options.SearchClient);
            }

            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="builder">
        /// A function which provides the options used to configure the wiki system.
        /// </param>
        public static void AddWiki(this IServiceCollection services, Func<IServiceProvider, IWikiOptions> builder)
        {
            services.AddScoped(provider =>
            {
                var options = builder.Invoke(provider);

                WikiWebConfig.CompactLayoutPath = options?.CompactLayoutPath;
                WikiWebConfig.LoginPath = options?.LoginPath;
                WikiWebConfig.MainLayoutPath = options?.MainLayoutPath;
                WikiWebConfig.TenorAPIKey = options?.TenorAPIKey;

                if (options?.SearchClient is null)
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var userManager = provider.GetRequiredService<UserManager<WikiUser>>();
                    return new DefaultSearchClient(loggerFactory.CreateLogger<DefaultSearchClient>(), userManager);
                }
                else
                {
                    return options.SearchClient;
                }
            });

            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// <para>
        /// Adds endpoints for the NeverFoundry.Wiki library.
        /// </para>
        /// <para>
        /// Should be added after setting <see cref="WikiConfig.MainPageTitle"/>, if a custom value
        /// is to be set.
        /// </para>
        /// <para>
        /// Should be added before all other endpoint mapping to ensure that wiki patterns are
        /// matched before falling back to default routing logic.
        /// </para>
        /// </summary>
        /// <param name="endpoints">An <see cref="IEndpointRouteBuilder"/> instance.</param>
        public static void MapWiki(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<WikiTalkHub>(WikiWebConfig.WikiTalkHubRoute);

            endpoints.MapControllerRoute(
                name: "wiki-ns",
                pattern: "Wiki/{wikiNamespace}:{title}/{action=Read}",
                defaults: new { controller = WikiController, isCompact = false });
            endpoints.MapControllerRoute(
                name: "wiki",
                pattern: $"Wiki/{{title={WikiConfig.MainPageTitle}}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = false, wikiNamespace = WikiConfig.DefaultNamespace });
            endpoints.MapControllerRoute(
                name: "wiki-ns-c",
                pattern: "Compact/Wiki/{wikiNamespace}:{title}/{action=Read}",
                defaults: new { controller = WikiController, isCompact = true });
            endpoints.MapControllerRoute(
                name: "wiki-c",
                pattern: $"Compact/Wiki/{{title={WikiConfig.MainPageTitle}}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = true, wikiNamespace = WikiConfig.DefaultNamespace });
        }
    }
}
