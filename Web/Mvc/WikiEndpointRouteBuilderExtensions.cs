using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using NeverFoundry.Wiki;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Mvc.Hubs;
using NeverFoundry.Wiki.Mvc.Services.FileManager;
using NeverFoundry.Wiki.Mvc.Services.Search;
using NeverFoundry.Wiki.Web;
using System;

namespace Microsoft.Extensions.DependencyInjection
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
        /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
        /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
        /// <param name="options">
        /// The options used to configure the wiki system.
        /// </param>
        /// <param name="fileManager">
        /// <para>
        /// An <see cref="IFileManager"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClient">
        /// <para>
        /// An <see cref="ISearchClient"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="DefaultSearchClient"/> will be used. Note: the
        /// default client is not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            IWikiUserManager userManager,
            IWikiGroupManager groupManager,
            IWikiOptions? options = null,
            IFileManager? fileManager = null,
            ISearchClient? searchClient = null)
        {
            if (options is not null)
            {
                services.AddScoped(_ => options);
            }
            else
            {
                services.AddScoped<IWikiOptions>(_ => new WikiOptions());
            }

            services.AddScoped(_ => userManager);
            services.AddScoped(_ => groupManager);

            if (fileManager is null || fileManager is LocalFileManager)
            {
                services.AddHttpContextAccessor();
            }
            if (fileManager is null)
            {
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(_ => fileManager);
            }

            if (searchClient is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(_ => searchClient);
            }

            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="userManagerType">
        /// The type of <see cref="IWikiUserManager"/> to register.
        /// </param>
        /// <param name="groupManagerType">
        /// The type of <see cref="IWikiGroupManager"/> to register.
        /// </param>
        /// <param name="options">
        /// The options used to configure the wiki system.
        /// </param>
        /// <param name="fileManagerType">
        /// <para>
        /// The type of <see cref="IFileManager"/> to register.
        /// </para>
        /// <para>
        /// If omitted, <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClientType">
        /// <para>
        /// The type of <see cref="ISearchClient"/> to register.
        /// </para>
        /// <para>
        /// If omitted, <see cref="DefaultSearchClient"/> will be used. Note: the default client is
        /// not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            Type userManagerType,
            Type groupManagerType,
            IWikiOptions? options = null,
            Type? fileManagerType = null,
            Type? searchClientType = null)
        {
            if (options is not null)
            {
                services.AddScoped(_ => options);
            }
            else
            {
                services.AddScoped<IWikiOptions>(_ => new WikiOptions());
            }

            services.AddScoped(typeof(IWikiUserManager), userManagerType);
            services.AddScoped(typeof(IWikiGroupManager), groupManagerType);

            if (fileManagerType is null)
            {
                services.AddHttpContextAccessor();
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(typeof(IFileManager), fileManagerType);
            }

            if (searchClientType is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(typeof(ISearchClient), searchClientType);
            }

            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="userManagerBuilder">
        /// A function which provides an <see cref="IWikiUserManager"/> instance.
        /// </param>
        /// <param name="groupManagerBuilder">
        /// A function which provides an <see cref="IWikiGroupManager"/> instance.
        /// </param>
        /// <param name="options">
        /// The options used to configure the wiki system.
        /// </param>
        /// <param name="fileManagerBuilder">
        /// <para>
        /// A function which provides an <see cref="IFileManager"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClientBuilder">
        /// <para>
        /// A function which provides an <see cref="ISearchClient"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="DefaultSearchClient"/> will be used. Note: the
        /// default client is not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            Func<IServiceProvider, IWikiUserManager> userManagerBuilder,
            Func<IServiceProvider, IWikiGroupManager> groupManagerBuilder,
            IWikiOptions? options = null,
            Func<IServiceProvider, IFileManager>? fileManagerBuilder = null,
            Func<IServiceProvider, ISearchClient>? searchClientBuilder = null)
        {
            if (options is not null)
            {
                services.AddScoped(_ => options);
            }
            else
            {
                services.AddScoped<IWikiOptions>(_ => new WikiOptions());
            }

            services.AddScoped(userManagerBuilder);
            services.AddScoped(groupManagerBuilder);

            if (fileManagerBuilder is null)
            {
                services.AddHttpContextAccessor();
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(fileManagerBuilder);
            }

            if (searchClientBuilder is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(searchClientBuilder);
            }

            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="userManager">An <see cref="IWikiUserManager"/> instance.</param>
        /// <param name="groupManager">An <see cref="IWikiGroupManager"/> instance.</param>
        /// <param name="optionsBuilder">
        /// <para>
        /// A function which provides the options used to configure the wiki system.
        /// </para>
        /// <para>
        /// Note: this function is used to configure a scoped instance of <see
        /// cref="IWikiOptions"/>, and this instance is then immediately retrieved to set the <see
        /// cref="WikiConfig.DataStore"/>, if that option was configured. Ensure that <c>AddWiki</c>
        /// is called <i>after</i> adding any service dependencies necessary for your <paramref
        /// name="optionsBuilder"/> function to operate, or this property will remain at its
        /// default.
        /// </para>
        /// </param>
        /// <param name="fileManager">
        /// <para>
        /// An <see cref="IFileManager"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClient">
        /// <para>
        /// An <see cref="ISearchClient"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="DefaultSearchClient"/> will be used. Note: the
        /// default client is not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            IWikiUserManager userManager,
            IWikiGroupManager groupManager,
            Func<IServiceProvider, IWikiOptions> optionsBuilder,
            IFileManager? fileManager = null,
            ISearchClient? searchClient = null)
        {
            services.AddScoped(optionsBuilder);
            services.AddScoped(_ => userManager);
            services.AddScoped(_ => groupManager);

            if (fileManager is null || fileManager is LocalFileManager)
            {
                services.AddHttpContextAccessor();
            }
            if (fileManager is null)
            {
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(_ => fileManager);
            }

            if (searchClient is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(_ => searchClient);
            }

            var options = services.BuildServiceProvider().GetRequiredService<IWikiOptions>();
            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="userManagerType">
        /// The type of <see cref="IWikiUserManager"/> to register.
        /// </param>
        /// <param name="groupManagerType">
        /// The type of <see cref="IWikiGroupManager"/> to register.
        /// </param>
        /// <param name="optionsBuilder">
        /// <para>
        /// A function which provides the options used to configure the wiki system.
        /// </para>
        /// <para>
        /// Note: this function is used to configure a scoped instance of <see
        /// cref="IWikiOptions"/>, and this instance is then immediately retrieved to set the <see
        /// cref="WikiConfig.DataStore"/>, if that option was configured. Ensure that <c>AddWiki</c>
        /// is called <i>after</i> adding any service dependencies necessary for your <paramref
        /// name="optionsBuilder"/> function to operate, or this property will remain at its
        /// default.
        /// </para>
        /// </param>
        /// <param name="fileManagerType">
        /// <para>
        /// The type of <see cref="IFileManager"/> to register.
        /// </para>
        /// <para>
        /// If omitted, <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClientType">
        /// <para>
        /// The type of <see cref="ISearchClient"/> to register.
        /// </para>
        /// <para>
        /// If omitted, <see cref="DefaultSearchClient"/> will be used. Note: the default client is
        /// not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            Type userManagerType,
            Type groupManagerType,
            Func<IServiceProvider, IWikiOptions> optionsBuilder,
            Type? fileManagerType = null,
            Type? searchClientType = null)
        {
            services.AddScoped(optionsBuilder);
            services.AddScoped(typeof(IWikiUserManager), userManagerType);
            services.AddScoped(typeof(IWikiGroupManager), groupManagerType);

            if (fileManagerType is null)
            {
                services.AddHttpContextAccessor();
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(typeof(IFileManager), fileManagerType);
            }

            if (searchClientType is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(typeof(ISearchClient), searchClientType);
            }

            var options = services.BuildServiceProvider().GetRequiredService<IWikiOptions>();
            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
            WikiConfig.LinkTemplate = LinkTemplate;
        }

        /// <summary>
        /// Adds support for the NeverFoundry.Wiki library.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection"/> instance.</param>
        /// <param name="userManagerBuilder">
        /// A function which provides an <see cref="IWikiUserManager"/> instance.
        /// </param>
        /// <param name="groupManagerBuilder">
        /// A function which provides an <see cref="IWikiGroupManager"/> instance.
        /// </param>
        /// <param name="optionsBuilder">
        /// <para>
        /// A function which provides the options used to configure the wiki system.
        /// </para>
        /// <para>
        /// Note: this function is used to configure a scoped instance of <see
        /// cref="IWikiOptions"/>, and this instance is then immediately retrieved to set the <see
        /// cref="WikiConfig.DataStore"/>, if that option was configured. Ensure that <c>AddWiki</c>
        /// is called <i>after</i> adding any service dependencies necessary for your <paramref
        /// name="optionsBuilder"/> function to operate, or this property will remain at its
        /// default.
        /// </para>
        /// </param>
        /// <param name="fileManagerBuilder">
        /// <para>
        /// A function which provides an <see cref="IFileManager"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="LocalFileManager"/> will be used.
        /// </para>
        /// </param>
        /// <param name="searchClientBuilder">
        /// <para>
        /// A function which provides an <see cref="ISearchClient"/> instance.
        /// </para>
        /// <para>
        /// If omitted, an instance of <see cref="DefaultSearchClient"/> will be used. Note: the
        /// default client is not recommended for production use.
        /// </para>
        /// </param>
        public static void AddWiki(
            this IServiceCollection services,
            Func<IServiceProvider, IWikiUserManager> userManagerBuilder,
            Func<IServiceProvider, IWikiGroupManager> groupManagerBuilder,
            Func<IServiceProvider, IWikiOptions> optionsBuilder,
            Func<IServiceProvider, IFileManager>? fileManagerBuilder = null,
            Func<IServiceProvider, ISearchClient>? searchClientBuilder = null)
        {
            services.AddScoped(optionsBuilder);
            services.AddScoped(userManagerBuilder);
            services.AddScoped(groupManagerBuilder);

            if (fileManagerBuilder is null)
            {
                services.AddHttpContextAccessor();
                services.AddScoped<IFileManager, LocalFileManager>();
            }
            else
            {
                services.AddScoped(fileManagerBuilder);
            }

            if (searchClientBuilder is null)
            {
                services.AddScoped<ISearchClient, DefaultSearchClient>();
            }
            else
            {
                services.AddScoped(searchClientBuilder);
            }

            var options = services.BuildServiceProvider().GetRequiredService<IWikiOptions>();
            if (options?.DataStore is not null)
            {
                WikiConfig.DataStore = options.DataStore;
            }
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
            var options = endpoints.ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<IWikiOptions>();
            var talkHubRoute = options?.TalkHubRoute ?? WikiOptions.DefaultTalkHubRoute;

            endpoints.MapHub<WikiTalkHub>(talkHubRoute);

            endpoints.MapControllerRoute(
                name: "wiki-ns",
                pattern: $"{WikiConfig.WikiLinkPrefix}/{{wikiNamespace}}:{{title}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = false });
            endpoints.MapControllerRoute(
                name: "wiki",
                pattern: $"{WikiConfig.WikiLinkPrefix}/{{title={WikiConfig.MainPageTitle}}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = false, wikiNamespace = WikiConfig.DefaultNamespace });
            endpoints.MapControllerRoute(
                name: "wiki-ns-c",
                pattern: $"Compact/{WikiConfig.WikiLinkPrefix}/{{wikiNamespace}}:{{title}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = true });
            endpoints.MapControllerRoute(
                name: "wiki-c",
                pattern: $"Compact/{WikiConfig.WikiLinkPrefix}/{{title={WikiConfig.MainPageTitle}}}/{{action=Read}}",
                defaults: new { controller = WikiController, isCompact = true, wikiNamespace = WikiConfig.DefaultNamespace });
        }
    }
}
