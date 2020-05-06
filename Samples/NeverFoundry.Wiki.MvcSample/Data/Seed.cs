using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeverFoundry.DataStorage;
using NeverFoundry.DataStorage.Marten;
using NeverFoundry.Wiki.Messaging;
using NeverFoundry.Wiki.Mvc;
using NeverFoundry.Wiki.Sample.Logging;
using NeverFoundry.Wiki.Web;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Sample.Data
{
    public static class Seed
    {
        private const string AdminUsername = "Admin";

        public static IDocumentStore GetDocumentStore(string connectionString, MartenLogger logger) => DocumentStore.For(config =>
        {
            config.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            config.Connection(connectionString);
            config.Logger(logger);
            config.Schema.For<Article>()
                .AddSubClass<Category>()
                .AddSubClass<WikiFile>()
                .SoftDeleted();
            config.Schema.For<WikiRevision>()
                .SoftDeleted();
            config.Schema.For<Message>()
                .SoftDeleted();
        });

        public static async Task InitializeDatabasesAsync(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();

            SeedUsers(serviceScope);

            DataStore.Instance = new MartenDataStore(serviceScope.ServiceProvider.GetRequiredService<IDocumentStore>());

            await AddDefaultWikiPagesAsync(serviceScope).ConfigureAwait(false);
        }

        private static async Task AddDefaultWikiPagesAsync(IServiceScope serviceScope)
        {
            var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<WikiUser>>();
            var admin = await userMgr.FindByNameAsync(AdminUsername).ConfigureAwait(false);
            var adminId = admin?.Id;

            if (string.IsNullOrEmpty(adminId))
            {
                throw new Exception("Admin not found");
            }

            if (Article.GetArticle("Welcome", WikiConfig.TransclusionNamespace) is null)
            {
                _ = await GetDefaultWelcomeAsync(adminId).ConfigureAwait(false);
            }

            if (Article.GetArticle(WikiConfig.MainPageTitle) is null)
            {
                _ = await GetDefaultMainAsync(adminId).ConfigureAwait(false);
            }

            if (Article.GetArticle(WikiWebConfig.AboutPageTitle, WikiWebConfig.SystemNamespace) is null)
            {
                _ = await GetDefaultAboutAsync(adminId).ConfigureAwait(false);
            }

            if (Article.GetArticle("Blazor") is null)
            {
                _ = await GetDefaultBlazorAsync(adminId).ConfigureAwait(false);
            }

            if (Article.GetArticle(WikiWebConfig.HelpPageTitle, WikiWebConfig.SystemNamespace) is null)
            {
                _ = await GetDefaultHelpAsync(adminId).ConfigureAwait(false);
            }

            if (Article.GetArticle("MVC") is null)
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

The source code for `NeverFoundry.Wiki` is available online, and also includes a complete sample implementation (which you are viewing now).

See the [[System:Help|]] page for usage information.

[[{WikiConfig.CategoriesTitle}:System pages]]",
            WikiWebConfig.SystemNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultBlazorAsync(string adminId) => Article.NewAsync(
            "Blazor",
            adminId,
@"{{Welcome}}

The `NeverFoundry.Wiki.Blazor` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with a [Blazor](http://blazor.net) site (which depends on [[MVC|NeverFoundry.Wiki.Mvc]] as a back-end). Note that this isn't a complete website, but rather a [Razor components class library](https://docs.microsoft.com/en-us/aspnet/core/blazor/class-libraries) which can be included in a Blazor project to enable wiki functionality.

In order to integrate it into a Blazor site, there are a number of steps to follow:

1. **MVC**

   See the steps in the [[MVC|MVC article]]. These should be applied to your server project, which will serve wiki pages.
2. **Routing**

   Replace the default `Router` component in your `App.razor` file with a `WikiRouter` component.

   For example:

   ```html
   <CascadingAuthenticationState>
       <WikiRouter AppAssembly=""@typeof(Program).Assembly"" OuterLayout=""@typeof(MainLayout)"">
           <Found Context=""routeData"">
               <AuthorizeRouteView RouteData=""@routeData"" DefaultLayout=""@typeof(MainLayout)"" />
           </Found>
       </WikiRouter>
   </CascadingAuthenticationState>
   ```

   The `WikiRouter` component overrides the default routing behavior of a Blazor site in order to treat all routes which begin with a predetermined relative path as valid wiki pages (whether or not they already exist). URLs which match a page with an `@page` route will still go where they belong, even if they start with the defined wiki prefix. All unmatched URLs relative to the wiki prefix will be interpreted as a request for a wiki page by that name, and will not fall back to the defined ""not found"" handling (the standard wiki content shown for articles which don't exists will be shown instead).

[[" + WikiConfig.CategoriesTitle + ":Help pages]]",
            WikiConfig.DefaultNamespace,
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

In addition NeverFoundry Wiki adds a few special syntax structures:

## Wiki Links
A wiki link is constructed using double square braces: `[[Example Link]]`. The content between the braces should be the exact title of a wiki page.

A link can include a namespace: `[[Namespace:Title]]`. If a page is in the default namespace, it may be omitted. The ""Talk"" pseudo-namespace can also be included to link directly to an article's discussion page: `[[Talk:Namespace:Title]]`.

In order to display an alternative title for a link, use a pipe character, followed by the display title: `[[Title|display]]` would be rendered as ""[[Title|display]]"" and link to the article ""Title."" If the pipe character is included, but nothing (ignoring whitespace) appears afterward, the display title is generated automatically by stripping any namespace prefix, as well as anything at the end of the title in parentheses. For example: `[[Namespace:Title (extra)|]]` would become ""[[Namespace:Title (extra)|]]"". If you use a second pipe character as the display title, the same stripping behavior is performed, and the result is also converted to lower case. For example: `[[Namespace:Title (extra)||]]` would become ""[[Namespace:Title (extra)||]]"".

If any characters appear after a link's closing braces, but before the next punctuation or whitespace character, they will automatically be added to the title to create a display value: `[[Title]]s` would appear as ""[[Title]]s"" and link to the article ""Title."" This doesn't work if the link already includes a display value: `[[Title|display]]s` would appear as ""[[Title|display]]s"" (note: the trailing ""s"" is not included in the link text).

Files uploaded to the wiki can be linked in the same way. Uploaded images can be prefixed with an exclamation point in order to display the image directly, similar to standard CommonMark image links. For example: `![[File: MyImage]]`. The image will also be a link to the file's wiki page.

A special format exists to link to [Wikipedia](http://wikipedia.org). If the title of a wiki link is prefixed with ""w:"" the link is transformed into an external link to the wikipedia page for the given title. Display titles continue to work as usual, but if no display title is explicitly given the ""w:"" is stripped to provide the default. For example: `[[w:Wiki]]` becomes ""[[w:Wiki]]"" with a link to the page ""http://wikipedia.org/wiki/Wiki."" Wikipedia links (like all external links) are not checked for validity. In other words, special formatting which may usually be applied for links to non-existing wiki pages does not get applied to links to Wikipedia.

A similar special format exists for Creative Commons links: links whose titles are prefixed by ""cc:"" are automatically translated to an external link to the [WikiMedia Commons](http://commons.wikimedia.org), and the display (if not provided explicitly) has the ""cc:"" and any extension stripped. Images can also be directly linked, and when doing so they become a link to the page on the Creative Commons (in order to fulfill the license terms of Creative Commons content, which usually requires attribution). For example: `![[cc:Example.jpg]]` becomes ![[cc:Example.jpg]] with a link to ""https://commons.wikimedia.org/wiki/File:Example.jpg.""

## Transclusions
Any article can be transcluded into any other article by enclosing its link in double curly braces rather than square braces. For example: `{{Title}}` within the body of an article would cause the entire contents of the article ""Title"" to be rendered in place of the transclusion link when the page is displayed. The namespace of a page should also be included if it isn't the default. For example: `{{Namespace:Title}}` would transclude an article named ""Title"" in the ""Namespace"" namespace. Note that the assumed namespace when one is not explicitly provided may not be the same as the one for wiki links. See `TransclusionNamespace` below for details.

Transclusions can be included within transclusions, but after 100 levels of recursion any further transclusions will no longer be processed. Instead they will appear in the final article as originally written, including the surrounding brackets. This is done (rather than omitting such overly deep transclusions entirely) to make it clear to the editor and/or reader that there is a problem with the markdown, whereas an omission might be overlooked.

If the content of a transclusion is not a valid function or article name, the transclusion will appear in the article as-is. For example, a tranclusion like `{{4x4=16}}` (where ""4x4=16"" is not the name of an article) would be rendered as ""`{{4x4=16}}`"" without alteration.

Transclusions cannot span over multiple lines. In other words, line breaks between brackets will cause them to be regarded as literal characters, rather than enclosing a transclusion. If you need to include multi-line content as a parameter of a transclusion, however, there is an easy workaround: put the multi-line content into its own article, and transclude it.

Discussion pages cannot be transcluded. In other words, a transclusion like `{{Talk:Title}}` would not transclude the contents of the ""Title"" article's discussion page. Instead, ""`{{Talk:Title}}`"" would be rendered as-is. The reason for this behavior is that discussion pages are not actually articles. They are instead constructed on demand from a tree-like collection of messages which requires special rendering logic, including interactive elements that do not appear on standard article pages.

### Parameters
Values can be passed to transcluded articles by adding a pipe character after the title, then a value with an optional name: `{{Title|x=hello}}` transcludes the article ""Title"" and passes it ""hello"" as a parameter with the name ""x."" Inside the transcluded article a parameter can be referred to by their name inside double parenthesis. In the previous example article ""Title"" could include `((x))`, which would be replaced when the article was transcluded with ""hello."" Double parenthesis containing anything which isn't a parameter name will be rendered as-is (which both allows the use of nested parenthesis for other purposes, and also makes parameter errors clear in the rendered text).

The name of a parameter can be omitted in a transclusion link: `{{Title|hello}}`. All unnamed parameters are assigned a default name which starts with ""1"" and increases for each unnamed parameter.For example: `{{Title|hello|x=there|friend}}` Includes parameters ""1"" with the value ""hello,"" ""x"" with the value ""there,"" and ""2"" with the value ""friend.""

The value of a parameter can be a transclusion, or another parameter.For example: `{{Title|x={{Other}}}}` transcludes the article named ""Title"" and passes it a parameter equal to the full content of the article named ""Other.""

# Functions
There are a number of built-in functions. Functions use the same syntax as transclusions (double curly braces) but instead of an article title the function name is used.

Functions take precedence over transclusions. That means that if there is an article with the same name as a transclusion (which is allowed), it cannot be transcluded, since any attempt to do so would invoke the function instead.

- **exec** - The most powerful function. The parameter named ""code"", or the first unnamed parameter if none are named ""code,"" is interpreted and executed as C# code.

   All other parameters are passed into the code to be executed as variables. Unnamed parameters are prefixed with an underscore (""_""), since variables cannot start with a digit in C#.

   Any parameter which can be parsed as a number is assigned to a numeric variable of the appropriate type (e.g. integral values become integers; floating point values become doubles). A parameter which can be parsed as a boolean becomes one. A parameter which can be parsed as a `DateTimeOffset` becomes a variable of that type. All other parameters become strings with their literal value.

   `ToString()` is called on the return value of the code (if any) and the result is displayed.

   For example: `{{exec|Math.Pow(x, 2)|x=3}}` is rendered as ""{{exec|Math.Pow(x, 2)|x=3}}"".
- **format** - The first parameter is parsed as either a number or a `DateTimeOffset`, then `ToString()` is called on the result.

   If a second parameter is passed, it is used as the argument for the `ToString()` method if it is a valid format string for the type. If not, ""N0"" is used for integral numbers, ""N"" for floating point numbers, and ""g"" for `DateTimeOffsets`.

   For example: `{{format|1234.567|N2}}` is rendered as ""{{format|1234.567|N2}}"".
- **fullpagename** - Displays the title of the current page. Even when this function is used in a transcluded article, the title displayed will be that of the main article currently being viewed.

   Includes the namespace only when it isn't the default.

   The name of the article is displayed in title case.

   For example: `{{fullpagename}}` is rendered as ""{{fullpagename}}"" on this page.
- **if** - Attempts to parse the first parameter as either a boolean value or a number. If the value is either true or greater than zero, the second parameter is displayed. If not, the third parameter is displayed (or nothing, if there is no third parameter).

   For example: `{{if|true|success}}` is rendered as ""{{if|true|success}}"".
- **ifeq** - If the first and second parameters are equal, displays the third parameter. Otherwise, displays the fourth parameter (or nothing, if there is no fourth parameter).

   For example: `{{ifeq|1|1|success}}` is rendered as ""{{ifeq|1|1|success}}"".
- **ifnottemplate** - Displays the first parameter only if the current article is not being transcluded. In other words, the parameter is rendered when the article is viewed directly.

   For example: `{{ifnottemplate|success}}` is rendered as ""{{ifnottemplate|success}}"" on this page.
- **iftemplate** - Displays the first parameter only if the current article is being transcluded.

   For example: `{{iftemplate|not shown}}` is rendered as ""{{iftemplate|not shown}}"" on this page (i.e., nothing).
- **notoc** - Suppresses the generation of the default table of contents.

   The default table of contents is placed above the first heading in an article, or below the first paragraph (whichever comes first). It is omitted if any explicit table of contents are placed (with the **toc** function or the equivalent HTML comment).

   If any explicit table of contents are placed, this function does not suppress those. Only the default table is omitted.

   The same effect can also be accomplished by placing the HTML comment `<!-- NOTOC -->` anywhere in a page. The comment is generally preferred to the function, since the function requires more processing by the wiki engine, which adds to saving and loading time.
- **padleft** - Prefixes the first parameter with a number of leading characters. The total minimum length of the result is determined by the second parameter. The character used is a ""0"" unless a third parameter is supplied, in which case the first character of that parameter will be used.

   For example: `{{padleft|1|3}}` renders as ""{{padleft|1|3}}"" and `{{padleft|1|3|:}}` renders as ""{{padleft|1|3|:}}"", while `{{padleft|4562|3}}` renders as ""{{padleft|4562|3}}"" (unchanged since it is already at least 3 characters long).

   If the second parameter is not a valid number, the first parameter will be displayed unchanged.
- **padright** - Works the same as **padleft** except that it adds the padding characters to the end of the first parameter, instead of the beginning.

   For example: `{{padright|1|3}}` renders as ""{{padright|1|3}}"" and `{{padright|1|3|:}}` renders as ""{{padright|1|3|:}}"".
- **pagename** - Displays the title of the current page. Even when this function is used in a transcluded article, the title displayed will be that of the main article currently being viewed.

   Unlike **fullpagename** this never includes the namespace.

   The name of the article is displayed in title case.

   For example: `{{pagename}}` is rendered as ""{{pagename}}"" on this page.
- **preview** - The first paramater is displayed only when the article is being viewed as a preview.

    Any article which has preview content will display all of its explicitly marked preview content as its preview. Articles with no explicitly marked preview content will display between 100 and 500 characters (the preview stops after the first paragraph break following the 100 character mark, or after 500 characters, whichever comes first).

   For example: `{{preview|not shown}}` is rendered as """" on this page (i.e., nothing).
 - **redirect** - A special function which must be at the very beginning of an article.The first parameter should be a wiki page title (with namespace, if non-default). When a wiki article which starts with this function in retrieved, the article specified in the parameter (even if it doesn't exist) is returned instead.

   A redirect may lead to another article which also redirects to another page, but this practice is discouraged since each step adds client time and database overhead. It is more efficient for the first redirecting article to point directly to the final target (i.e.the real article). A special page exists which lists double redirects, so that they may be discovered and eliminated eliminate them (by setting the first in a redirection chain to point directly to the final target).

   Redirection sequences abort if a cycle is detected (including a redirection which points to itself). In this situation the final redirecting article in the chain is displayed as-is (i.e. the wiki markup showing the redirect will be visible to the requesting client).

   A redirection chain also aborts if there are more than 100 links before reaching a non-redirect target.
- **sitename** - Displays the value of `WikiConfig.SiteName` (see below).

   For example: `{{sitename}}` is rendered as ""{{sitename}}"" on this wiki.
- **serverurl** - Displays the value of `WikiConfig.ServerUrl` (see below).

   For example: `{{serverurl}}` is rendered as ""{{serverurl}}"" on this site.

   The address is not prerendered as a link, in order to be suitable for combination with additional path values to form a complete URL. In order to format the value as a link by itself, use the standard CommonMark link format: e.g. `[{{serverurl}}]({{serverurl}})` renders as [{{serverurl}}]({{serverurl}}).
- **toc** - Displays a table of contents at the position of the function. In order to determine which headings to include, the table of contents looks for the closest heading which precedes itself (contents in between the heading and the table of contents are allowed). If none are found, all headings are included. If there is a preceding heading, all headings which follow the table of contents marker are included, up until the first heading of a lower level in the hierarchy than the preceding heading. For example: if you place a table of contents after a level 3 heading, it will include all following headings until it reaches a level 2 or 1 heading (which will not be included). This allows you to place tables of contents for subsections of an article.

   A depth (number of levels of hierarchy to display) can be specified with the first parameter, which overrides the default depth for the table of contents. Depth does not refer to the absolute level of a heading; it is relative to the starting level (see below). For example, if a table of contents has a starting level of 3, a depth of 2 would indicate that levels 3 and 4 should be displayed. A single '*' chartacter can be used in place of this parameter, which indicates that the default value should be used.

   A starting level can be specified with the second parameter, which overrides the default starting level for the table of contents. This number specifies the first level of the hierarchy which should be displayed in the table. It defaults to 1, which indicates that the top level of heading included should be displayed. For example: setting this to 2 would cause all headings with the lowest level to be omitted from the table. Note that this value does not refer to the absolute level of the headings. If a table of contents is placed after a level 3 heading, a starting level of 1 would indicate that headings of level 4 and down should be displayed. A single '*' chartacter can be used in place of this parameter, which indicates that the default value should be used.

   A title can be specified with the third parameter. This will be displayed as a heading above the table of contents itself. The default is ""Contents"". Note that a '\*' character does *not* indicate the default value for this parameter (simply omit a third parameter for that effect). If a '\*' character is used, it will appear as the actual title of the table of contents.

   The default table of contents is placed above the first heading in an article, or below the first paragraph (whichever comes first). It is omitted if any explicit table of contents are placed. To restore the default table after placing one explicitly, you can place another table of contents marker in the same position as the default would normally appear.

   The same effect can also be accomplished by placing the HTML comment `<!-- TOC -->` at the desired position, optionally with a specified depth, starting level, and title (e.g. `<!-- TOC * 2 Topics -->`). The comment is generally preferred to the function, since the function requires more processing by the wiki engine, which adds to saving and loading time.
- **tolower** - Displays the first parameter in lower case.

   For example: `{{tolower|exAmple teXt}}` is rendered as ""{{tolower|exAmple teXt}}"".
- **totitlecase** - Displays the first parameter in title case.

   This uses the same casing logic which is applied to wiki article titles: the first character is capitalized, and all other characters are left unchanged. This preserves special casing for terms like [YouTube](https://www.youtube.com)).

   For example: `{{totitlecase|exAmple teXt}}` is rendered as ""{{totitlecase|exAmple teXt}}"".

   To instead capitalize the first letter of each word and force all other characters to lowercase, pass a second parameter to the function which evaluates to true or a number greater than zero. For example: `{{totitlecase|exAmple teXt|true}}` renders as ""{{totitlecase|exAmple teXt|true}}"".
- **toupper** - Displays the first parameter in upper case.

   For example: `{{toupper|exAmple teXt}}` is rendered as ""{{toupper|exAmple teXt}}"".

# Customization

The `WikiConfig` static class contains a number of properties which can be set to custom values.

## Site identity
- **`SiteName`** - The name of your wiki.

   Default is ""A NeverFoundry Wiki Sample""; this is clearly not suitable for production, and should always be replaced.

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`ServerUrl`** - The primary URL of your wiki.

   Default is ""http://localhost:5000/""; this is clearly not suitable for production, and should always be replaced.

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.

##  Site pages
- **`CategoriesTitle`** - The title of the categories namespace, and the article on categories in the main wiki.

   Default is ""Categories""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`DefaultNamespace`** - The name of the default namespace.

   Default is ""Wiki""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`FileNamespace`** - The name of the file namespace.

   Default is ""File""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`MainPageTitle`** - The title of the main page (shown when no article title is given).

   Default is ""Main""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`TalkNamespace`** - The name of the talk pseudo-namespace.

   Default is ""Talk""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`TransclusionNamespace`** - For transclusions this namespace is assumed instead of `DefaultNamespace` when no namespace is explicitly specified.

   When this property is set to a non-empty value, all transclusions which do not explicitly specify a namespace are presumed to be in the namespace with the assigned value.

   Default is ""Transclusion""

   Unlike most of the other namespace properties, this may be assigned a `null` or empty value. If so, transclusions follow the default behavior of assuming `DefaultNamespace` when omitted. This is not recommended, however, as this avoids confusion with general articles. For example: an article which formats calendar events for display might be intended to be transcluded as `{{Event|1-5-2020}}`. A user looking for a general article about events would likely be confused to see the formatting and parameter code in that article. Placing all such articles into their own namespace, and keeping them out of the default namespace, helps to avoid this confusion. It also avoids any trouble with giving transcluded articles titles that do not conflict with general-topic articles.

## Site behavior
- **`DefaultTableOfContentsDepth`** - The default number of levels of nesting shown in an article's table of contents.

   Default is 3

   Can be overridden by specifying the level for a given article.
- **`DefaultTableOfContentsTitle`** - The default title of tables of contents.

   Default is ""Contents""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`LinkTemplate`** - The template used to format wiki links.

   The default is `[{TITLE}]({LINK}){.wiki-link wiki-link-{EXISTS}}`, where `{LINK}` is replaced by the relative URI, `{TITLE}` is replaced by the page name (or explicit display value), and `{EXISTS}` is replaced by either ""exists"" if the article exists or ""missing"" if the article does not exist. (Note that the `NeverFoundry.Wiki.Mvc` project overrides this to enable link previews.)

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`MinimumTableOfContentsHeadings`** - The minimum number of headings required in an article to display a table of contents by default.

   Default is 3

   Can be overridden by specifying the location of a table of contents explicitly for a given article.
- **`Postprocessors`** - May be set to a `IList<IArticleProcessor>` (which implements a `Func<string, string> Process` property). Each processor in the list is passed the content of an article *after* it is parsed into HTML but *before* it is sanitized. Processors are run in the order they are added to the collection, and each is passed the output of the last.

   Note that no processors are run if the initial content is empty (i.e. you cannot use a processor to add content to an empty article).

   Note also that postprocessors can significantly slow down article saving and loading. It is recommended to use them sparingly, if at all, and to optimize them as far as is practical.

# MVC
The `NeverFoundry.Wiki.Mvc` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with an [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) site. This implementation can be used as-is, or you can use the source as the starting point to build your own implementation. See [[MVC|the MVC page]] for more information.

# Blazor
The `NeverFoundry.Wiki.Blazor` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with a [Blazor](http://blazor.net) site (which depends on NeverFoundry.Wiki.Mvc as a back-end). This implementation can be used as-is, or you can use the source as the starting point to build your own implementation. See [[Blazor|the Blazor page]] for more information.

[[" + WikiConfig.CategoriesTitle + @":System pages]]
[[" + WikiConfig.CategoriesTitle + ":Help pages]]",
            WikiWebConfig.SystemNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultMainAsync(string adminId) => Article.NewAsync(
            WikiConfig.MainPageTitle,
            adminId,
@$"{{{{Welcome}}}}

See the [[System:About|]] page or the [[System:Help|]] page for more information.

[[{WikiConfig.CategoriesTitle}:System pages]]",
            WikiConfig.DefaultNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultMVCAsync(string adminId) => Article.NewAsync(
            "MVC",
            adminId,
@"{{Welcome}}

The `NeverFoundry.Wiki.Mvc` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with an [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview) site. Note that this isn't a complete website, but rather a [Razor class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be included in an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to enable wiki functionality.

In order to integrate it into an ASP.NET Core site, there are a number of steps to follow:

1. **Scripts**

   A script reference is required in your main layout file:

   ```html
    <script src=""~/_content/NeverFoundry.Wiki.Mvc/script.js""></script>
   ```
2. **Style**

   The following style reference is required in your main layout file:

   ```html
    <link href=""~/_content/NeverFoundry.Wiki.Mvc/libstyles.css"" rel=""stylesheet"" />
   ```

   The site's HTML has been extensively decorated with ids and classes, to the point that every element should be able to be uniquely referenced in CSS. You may define your own custom stylesheets, or use the included `styles.css` file, which reproduces a style similar to [MediaWiki](https://www.mediawiki.org).

   To include the default styles, add the following reference (in addition to the one above):

   ```html
    <link href=""~/_content/NeverFoundry.Wiki.Mvc/styles.css"" rel=""stylesheet"" />
   ```

   Instead of using the precompiled `styles.css` file, you can instead compile from the Sass source. The default Sass file is located at `/wwwroot/styles.scss` in the source and NuGet distribution. The path `_content/NeverFoundry.Wiki.Blazor/styles.scss` will work at runtime if you have installed the NuGet package, but this is of limited use since most Sass build processes require access to the file during build, not runtime. For access to the file during build steps (e.g. in a webpack or gulp process, or a CI/CD pipeline) it is recommended to download the scss file from source.

   The Sass file contains a number of variables used to define the colors of the site. Each one uses `!default` to allow overriding with your own colors. Be sure to include the `styles.scss` file after your own variable definitions.

   There is also a `/styles/styles-bootstrap.scss` file. If your site will use [Bootstrap](https://getbootstrap.com) this file can be used in place of `/styles/styles.scss`. It includes that file, but also sets Bootstrap's background and foreground variables to match those used for the wiki. This will only work if you are also compiling Bootstrap from Sass source. Bootstrap classes have also been used in certain places throughout the HTML so that it will more closely align with the Bootstrap style. This does not require compiling Bootstrap styles from the Sass source, and should work out of the box. It should not affect the site's appearance if Bootstrap is not used, although if you use any other style libraries, there is a chance that class names may overlap, which might have unintended effects. If that happens, custom CSS rules might be required in your own stylesheets.

   The `background-image` property of the `#wiki-sidebar-logo-link` element should be set to your site's logo in your CSS. A 160px x 160px image will display best.

   If this style property is left unset, a blank space will appear at the top-left corder of wiki pages where the logo is expected to be. If you intend to omit a logo and want to collapse this empty space, you can set the `height` property of `#wiki-sidebar-logo` and `#wiki-sidebar-logo a` to 0 to avoid this empty space.
3. **References**

   Add the following reference to your main `_ViewImports.cshtml` file:

   ```csharp
   @using NeverFoundry.Wiki.Mvc
   ```
6. **SignalR**

   The wiki uses [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction) to enable real-time updates on discussion pages.

   Add a call to `services.AddSignalR()` in your `Startup.ConfigureServices(IServiceCollection services)` method.

   The default ID claim used by SignalR isn't appropriate for use with `NeverFoundry.Wiki` (or much of anything, as it's rarely guaranteed to be unique). Instead, you should use a custom ID provider supplied in the `NeverFoundry.Wiki.Blazor.Messaging` namespace: call `services.AddSingleton<IUserIdProvider, WikiUserIdProvider>()` in your server project's `Startup.ConfigureServices(IServiceCollection services)` method (after adding a `using` statement with the namespace just mentioned). This will force SignalR to use the unique `Id` property as its identifier.

   To add the (pre-defined) talk Hub to your server project, add a call to `endpoints.MapHub<WikiTalkHub>(BlazorConfig.WikiTalkHubRoute)` inside your `app.UseEndpoints` call in the `Startup.Configure` method. Make sure to add this *before* the call to `MapFallbackToClientSideBlazor`.
6. **Users**

   The wiki has a concept of users. Anonymous interaction is possible, but some operations can only be performed by signed-in users, such as editing pages, uploading files, or participating in discussions. Any content item can also have view and/or edit permissions restricted to only certain users or groups.

   Any identity system may be used to manage your users, provided the user class is (or inherits from) the `WikiUser` class (in the `NeverFoundry.Wiki` namespace). If your user class is inaccessible (i.e. you cannot choose which class to inherit) you can employ a shim class which implements this interface to manage wiki users. The shim class can either inherit from your ""real"" user class, or contains it as a child property if the base is sealed. For example:

   ```csharp
   public class MyWikiUser : WikiUser
   {
        public MyOriginalUser User { get; set; }

        ...
   }
   ```
7. **The Wiki**
   Add a call to `services.AddWiki()` in your `Startup.ConfigureServices(IServiceCollection services)` method.
8. **Things to avoid**

   The `LinkTemplate` property of `WikiConfig` should not be manually adjusted in a `NeverFoundry.Wiki.Mvc` project. It is automatically customized to enable link previews on hover.

### Customization

The `WikiWebConfig` static class in the `NeverFoundry.Wiki.Web` namespace contains a number of properties which can be set to custom values.

###  Site pages
- **`AdminNamespaces`** - An optional collection of namespaces which may not be assigned to pages by non-admin users.

   The namespace assigned to **SystemNamespace** is included automatically.
- **`AboutPageTitle`** - The title of the main about page.

   Default is ""About""

   May be set to `null` or an empty string, which disables the about page (i.e. hides the link).
- **`ContactPageTitle`** - The title of the main contact page.

   Default is ""Contact""

   May be set to `null` or an empty string, which disables the contact page (i.e. hides the link).
- **`ContentsPageTitle`** - The title of the main contents page.

   Default is ""Contents""

   May be set to `null` or an empty string, which disables the contents page (i.e. hides the link).
- **`CopyrightPageTitle`** - The title of the main copyright page.

   Default is ""Copyright""

   May be set to `null` or an empty string, which disables the copyright page (i.e. hides the link).

   Consider carefully before omitting this special page, unless you supply an alternate copyright notice on your wiki.
- **`HelpPageTitle`** - The title of the main help page.

   Default is ""Help""

   May be set to `null` or an empty string, which disables the help page (i.e. hides the link).
- **`PolicyPageTitle`** - The title of the main policy page.

   Default is ""Policies""

   May be set to `null` or an empty string, which disables the policy page (i.e. hides the link).
- **`SystemNamespace`** - The name of the system namespace.

   Default is ""System""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`UserNamespace`** - The name of the user namespace.

   Default is ""Users""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.

### Site behavior
- **`MaxFileSize`** - The maximum size (in bytes) of uploaded files.

   Setting this to zero effectively prevents file uploads.

   This limit is observed regardless of the implementation assigned to `SaveFile`, but if you wish to employ more complex logic to determine the allowed size of uploads (e.g. based on file type or individual user permissions), you can set this to the largest limit you wish to allow, then provide more fine-grained limits in your `SaveFile` implementation.

[[" + WikiConfig.CategoriesTitle + ":Help pages]]",
            WikiConfig.DefaultNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultWelcomeAsync(string adminId) => Article.NewAsync(
            "Welcome",
            adminId,
@$"Welcome to the [NeverFoundry](http://neverfoundry.com).Wiki sample.

{{{{ifnottemplate|[[{WikiConfig.CategoriesTitle}:System pages]]}}}}",
            WikiConfig.TransclusionNamespace,
            adminId,
            new[] { adminId });

        private static void SeedUsers(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<WikiUser>>();
            const string AdminEmail = "admin@neverfoundry.com";
            var admin = userMgr.FindByEmailAsync(AdminEmail).GetAwaiter().GetResult();
            if (admin is null)
            {
                admin = new WikiUser
                {
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    HasUploadPermission = true,
                    UserName = AdminUsername,
                };
                var result = userMgr.CreateAsync(admin, "Admin1!").GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
                result = userMgr.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(WikiClaims.Claim_SiteAdmin, "true", ClaimValueTypes.Boolean),
                    new Claim(WikiClaims.Claim_WikiAdmin, "true", ClaimValueTypes.Boolean),
                }).GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
            }

            var exampleUser = userMgr.FindByNameAsync("example").GetAwaiter().GetResult();
            if (exampleUser is null)
            {
                exampleUser = new WikiUser
                {
                    Email = "example@example.com",
                    EmailConfirmed = true,
                    UserName = "example",
                };
                var result = userMgr.CreateAsync(exampleUser, "E#amp1e").GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new AggregateException(result.Errors.Select(x => new Exception(x.Description)));
                }
            }
        }

        private static Task SetDefaultCategoryAsync(Category category, string adminId) => category.ReviseAsync(
            adminId,
            markdown: "These are system pages in the [NeverFoundry](http://neverfoundry.com).Wiki sample [[w:Wiki||]].",
            revisionComment: "Provide a description",
            allowedEditors: new[] { adminId });
    }
}
