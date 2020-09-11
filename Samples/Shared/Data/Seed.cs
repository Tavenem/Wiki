using NeverFoundry.Wiki.Web;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Samples.Data
{
    public static class Seed
    {
        public static async Task AddDefaultWikiPagesAsync(string adminId)
        {
            var welcomeReference = await PageReference
                .GetPageReferenceAsync("Welcome", WikiConfig.TransclusionNamespace)
                .ConfigureAwait(false);
            if (welcomeReference is null)
            {
                _ = await GetDefaultWelcomeAsync(adminId).ConfigureAwait(false);
            }

            var mainReference = await PageReference
                .GetPageReferenceAsync(WikiConfig.MainPageTitle)
                .ConfigureAwait(false);
            if (mainReference is null)
            {
                _ = await GetDefaultMainAsync(adminId).ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(WikiWebConfig.AboutPageTitle))
            {
                var aboutReference = await PageReference
                    .GetPageReferenceAsync(WikiWebConfig.AboutPageTitle, WikiWebConfig.SystemNamespace)
                    .ConfigureAwait(false);
                if (aboutReference is null)
                {
                    _ = await GetDefaultAboutAsync(adminId).ConfigureAwait(false);
                }
            }

            if (!string.IsNullOrEmpty(WikiWebConfig.HelpPageTitle))
            {
                var helpReference = await PageReference
                    .GetPageReferenceAsync(WikiWebConfig.HelpPageTitle, WikiWebConfig.SystemNamespace)
                    .ConfigureAwait(false);
                if (helpReference is null)
                {
                    _ = await GetDefaultHelpAsync(adminId).ConfigureAwait(false);
                }
            }

            var mvcReference = await PageReference
                .GetPageReferenceAsync("MVC")
                .ConfigureAwait(false);
            if (mvcReference is null)
            {
                _ = await GetDefaultMVCAsync(adminId).ConfigureAwait(false);
            }

            var category = Category.GetCategory("System pages");
            if (category is null)
            {
                throw new Exception("Failed to create category during article creation");
            }
            if (!category.MarkdownContent.StartsWith("These are system pages", StringComparison.Ordinal))
            {
                await SetDefaultCategoryAsync(category, adminId).ConfigureAwait(false);
            }
        }

        private static Task<Article> GetDefaultAboutAsync(string adminId) => Article.NewAsync(
            WikiWebConfig.AboutPageTitle ?? "About",
            adminId,
@$"{{{{Welcome}}}}

The [NeverFoundry](http://neverfoundry.com).Wiki package is a [.NET](https://dotnet.microsoft.com) [[w:Wiki||]] library.

Unlike many wiki implementations, the main package (`NeverFoundry.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

The ""reference"" implementation included out-of-the-box (`NeverFoundry.Wiki.Mvc`) is a [Razor class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be included in an [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) project to turn it into a wiki.

The source code for `NeverFoundry.Wiki` is available online, and also includes a variety of sample implementations (which you are viewing now).

See the [[System:Help|]] page for usage information.

[[{WikiConfig.CategoryNamespace}:System pages]]",
            WikiWebConfig.SystemNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultHelpAsync(string adminId) => Article.NewAsync(
            WikiWebConfig.HelpPageTitle ?? "Help",
            adminId,
@"{{Welcome}}

This page includes various information which will help you to get a [NeverFoundry](http://neverfoundry.com).Wiki instance up and running.

For information about the `NeverFoundry.Wiki` project, see the [[System:About|]] page.

# Markup
The NeverFoundry Wiki syntax is a custom flavor of markdown. It implements all the features of [CommonMark](http://commonmark.org), as well as many others. The implementation uses [Markdig](https://github.com/lunet-io/markdig), and details of most extensions to standard CommonMark can be found on [its GitHub page](https://github.com/lunet-io/markdig).

# MVC
The `NeverFoundry.Wiki.Mvc` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with an [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) site. This implementation can be used as-is, or you can use the source as the starting point to build your own implementation. See [[MVC|the MVC page]] for more information.

[[" + WikiConfig.CategoryNamespace + @":System pages]]
[[" + WikiConfig.CategoryNamespace + ":Help pages]]",
            WikiWebConfig.SystemNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultMainAsync(string adminId) => Article.NewAsync(
            WikiConfig.MainPageTitle,
            adminId,
@$"{{{{Welcome}}}}

See the [[System:About|]] page or the [[System:Help|]] page for more information.

[[{WikiConfig.CategoryNamespace}:System pages]]",
            WikiConfig.DefaultNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultMVCAsync(string adminId) => Article.NewAsync(
            "MVC",
            adminId,
@"{{Welcome}}

The `NeverFoundry.Wiki.Mvc` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with an [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) site. Note that this isn't a complete website, but rather a [Razor class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be included in an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to enable wiki functionality.

[[" + WikiConfig.CategoryNamespace + ":Help pages]]",
            WikiConfig.DefaultNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultWelcomeAsync(string adminId) => Article.NewAsync(
            "Welcome",
            adminId,
@$"Welcome to the [NeverFoundry](http://neverfoundry.com).Wiki sample.

{{{{ifnottemplate|[[{WikiConfig.CategoryNamespace}:System pages]]}}}}",
            WikiConfig.TransclusionNamespace,
            adminId,
            new[] { adminId });

        private static Task SetDefaultCategoryAsync(Category category, string adminId) => category.ReviseAsync(
            adminId,
            markdown: "These are system pages in the [NeverFoundry](http://neverfoundry.com).Wiki sample [[w:Wiki||]].",
            revisionComment: "Provide a description",
            allowedEditors: new[] { adminId });
    }
}
