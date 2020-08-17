using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private const string ExpectedWelcome = "<p>Welcome to the <a href=\"http://neverfoundry.com\">NeverFoundry</a>.Wiki sample.</p>\n<p></p>\n";
        private const string ExpectedWelcomeTransclusion = @"Welcome to the <a href=""http://neverfoundry.com"">NeverFoundry</a>.Wiki sample.";

        private static readonly string _ExpectedAbout = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>The <a href=\"http://neverfoundry.com\">NeverFoundry</a>.Wiki package is a <a href=\"https://dotnet.microsoft.com\">.NET</a> <a href=\"http://wikipedia.org/wiki/Wiki\">wiki</a> library.</p>\n<p>Unlike many wiki implementations, the main package (<code>NeverFoundry.Wiki</code>) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.</p>\n<p>The “reference” implementation included out-of-the-box (<code>NeverFoundry.Wiki.Blazor</code>) is a <a href=\"https://docs.microsoft.com/en-us/aspnet/core/blazor/class-libraries\">Razor components class library</a> which can be included in a <a href=\"http://blazor.net\">blazor</a> project to turn it into a wiki.</p>\n<p>The source code for <code>NeverFoundry.Wiki</code> is available online, and also includes a complete sample implementation (which you are viewing now).</p>\n<p>See the <a href=\"http://localhost:5000/Wiki/System:Help\" class=\"wiki-link-exists\">Help</a> page for usage information.</p>\n<p></p>\n";

        [TestMethod]
        public void CreateWikiTest()
        {
            const string AdminId = "AdminId";

            var welcome = GetDefaultWelcomeAsync(AdminId).GetAwaiter().GetResult();
            Assert.AreEqual(ExpectedWelcome, welcome.GetHtml(), ignoreCase: false);

            GetDefaultMainAsync(AdminId).GetAwaiter().GetResult();

            var about = GetDefaultAboutAsync(AdminId).GetAwaiter().GetResult();

            GetDefaultBlazorAsync(AdminId).GetAwaiter().GetResult();

            GetDefaultHelpAsync(AdminId).GetAwaiter().GetResult();

            var category = Category.GetCategory("System pages");
            Assert.IsNotNull(category);
            SetDefaultCategoryAsync(category!, AdminId).GetAwaiter().GetResult();

            Assert.AreEqual(_ExpectedAbout, about.GetHtml(), ignoreCase: false);
        }

        private static Task<Article> GetDefaultAboutAsync(string adminId) => Article.NewAsync(
            "About",
            adminId,
@$"{{{{Welcome}}}}

The [NeverFoundry](http://neverfoundry.com).Wiki package is a [.NET](https://dotnet.microsoft.com) [[w:Wiki||]] library.

Unlike many wiki implementations, the main package (`NeverFoundry.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

The ""reference"" implementation included out-of-the-box (`NeverFoundry.Wiki.Blazor`) is a [Razor components class library](https://docs.microsoft.com/en-us/aspnet/core/blazor/class-libraries) which can be included in a [blazor](http://blazor.net) project to turn it into a wiki.

The source code for `NeverFoundry.Wiki` is available online, and also includes a complete sample implementation (which you are viewing now).

See the [[System:Help|]] page for usage information.

[[{WikiConfig.CategoryNamespace}:System pages]]",
            "System",
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultBlazorAsync(string adminId) => Article.NewAsync(
            "Blazor",
            adminId,
@"{{Welcome}}

The `NeverFoundry.Wiki.Blazor` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with a [Blazor](http://blazor.net) WebAssembly site. Note that this isn't a complete website, but rather a [Razor components class library](https://docs.microsoft.com/en-us/aspnet/core/blazor/class-libraries) which can be included in a Blazor project to turn enable wiki functionality.

In order to integrate it into a Blazor WebAssembly site, there are a number of steps to follow:

1. **Scripts**

   Two script reference are required in your `index.html` file:

   ```html
    <script src=""_content/BlazorInputFile/inputfile.js""></script>
    <script src=""_content/NeverFoundry.Wiki.Blazor/script.js""></script>
   ```

   [BlazorInputFile](https://github.com/SteveSandersonMS/BlazorInputFile) by [Steve Sanderson](http://blog.stevensanderson.com) is used for the file upload dialog. It can be omitted if your wiki does not allow file uploads.
2. **Style**

   The site's HTML has been extensively decorated with ids and classes, to the point that every element should be able to be uniquely referenced in CSS. You may define your own custom stylesheets, or use the included `styles.css` file, which reproduces a style similar to [MediaWiki](https://www.mediawiki.org).

   Instead of using the precompiled `styles.css` file, you can instead compile from the Sass source. The default Sass file is located at `/wwwroot/styles.scss` in the source and NuGet distribution. The path `_content/NeverFoundry.Wiki.Blazor/styles.scss` will also work at runtime if you have installed the NuGet package, but this is of limited use since most Sass build processes require access to the file during build, not runtime.

   The Sass file contains a number of variables used to define the colors of the site. Each one uses `!default` to allow overriding with your own colors. Be sure to include the `styles.scss` file after your own variable definitions.

   There is also a `/styles/styles-bootstrap.scss` file. If your site will use [Bootstrap](https://getbootstrap.com) this file can be used in place of `/styles/styles.scss`. It includes that file, but also sets Bootstrap's background and foreground variables to match those used for the wiki. This will only work if you are also compiling Bootstrap from Sass source. Bootstrap classes have also been used in certain places throughout the HTML so that it will more closely align with the Bootstrap style. This does not require compiling Bootstrap styles from the Sass source, and should work out of the box. It should not affect the site's appearance if Bootstrap is not used, although if you use any other style libraries, there is a chance that class names may overlap, which might have unintended effects. If that happens, custom CSS rules might be required in your own stylesheets.

   The `background-image` property of the `#wiki-sidebar-logo-link` element should be set to your site's logo in your CSS. A 160px x 160px image will display best.

   If this style property is left unset, a blank space will appear at the top-left corder of wiki pages where the logo is expected to be. If you intend to omit a logo and want to collapse this empty space, you can instead set the `height` property of `#wiki-sidebar-logo` and `#wiki-sidebar-logo a` to 0.
3. **References**

   Add the following references to your main `_Imports.razor` file:

   ```csharp
   @using NeverFoundry.Wiki.Blazor
   @using NeverFoundry.Wiki.Blazor.Messaging;
   ```
4. **Routing**

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

   The `WikiRouter` component overrides the default routing behavior of a Blazor site in order to treat all routes relative to the base URL as valid. URLs which match a page with an `@page` route will still go where they belong. All unmatched URLs will be interpreted as a request for a wiki page by that name. A page inviting the user to create a new article by that name is shown if no such page exists yet.

   Additional routing customization is possible, including custom authorizing and not-authorized content. See the sample source for a more complete example. Note that because the `WikiRouter` treats all routes as valid, there is no `NotFound` child for a `WikiRouter` component, unlike the default `Router` component.
5. **gRPC Services**

   Two gRPC-Web services are defined in the `NeverFoundry.Wiki.Blazor.Data` namespace, and must be implemented by your site's server project: `FileTransfer.FileTransferBase` and `WikiData.WikiDataBase`. See [this Microsoft documentation page](https://docs.microsoft.com/en-us/aspnet/core/grpc/browser) to begin learning about gRPC-Web in .NET Core browser apps. Follow the guidance there to fully enable gRPC in your server project. Be sure to override all the API methods of each service in your implementations. See the sample cource for a complete implementation.

   If you are building a static, stand-alone Blazor WebAssembly app, these services should be implemented in whatever server project hosts your data layer.
6. **SignalR**

   The wiki uses [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction) to enable real-time updates on discussion pages.

   Add a call to `services.AddSignalR()` in your server project's `Startup.ConfigureServices(IServiceCollection services)` method.

   The default ID claim used by SignalR isn't appropriate for use with `NeverFoundry.Wiki` (or much of anything, as it's rarely guaranteed to be unique). Instead, you should use a custom ID provider supplied in the `NeverFoundry.Wiki.Blazor.Messaging` namespace: call `services.AddSingleton<IUserIdProvider, WikiUserIdProvider>()` in your server project's `Startup.ConfigureServices(IServiceCollection services)` method (after adding a `using` statement with the namespace just mentioned). This will force SignalR to use the unique `Id` property as its identifier.

   To add the (pre-defined) talk Hub to your server project, add a call to `endpoints.MapHub<WikiTalkHub>(BlazorConfig.WikiTalkHubRoute)` inside your `app.UseEndpoints` call in the `Startup.Configure` method. Make sure to add this *before* the call to `MapFallbackToClientSideBlazor`.
6. Users

   The wiki has a concept of users. Anonymous interaction is possible, but some operations can only be performed by signed-in users, such as editing pages, uploading files, or participating in discussions. Any content item can also have view and/or edit permissions restricted to only certain users or groups.

   Any identity system may be used to manage your users, provided the user class implements the `IWikiUser` interface (in the main `NeverFoundry.Wiki` namespace). If the user class is inaccessible (i.e. you cannot apply a new interface) you can employ a shim class which implements this interface to manage wiki users. The shim class can either inherit from your ""real"" user class, or contains it as a child property if the base is sealed. For example:

   ```csharp
   public class MyWikiUser : IWikiUser
   {
        public MyOriginalUser User { get; set; }

        ...
   }
   ```
7. **Things to avoid**

   The `ReadFile` and `SaveFile` properties of the `WikiConfig` static class normally allow customizing the file upload and retrieval process. These should not be used in a `NeverFoundry.Wiki.Blazor` project, which automatically configures these properties to use the `FileTransfer` gRPC service.

   The `LinkTemplate` property of `WikiConfig` should also not be manually adjusted in a `NeverFoundry.Wiki.Blazor` project. It is automatically customized to enable link previews on hover.

### Customization

The `BlazorConfig` static class in the `NeverFoundry.Wiki.Blazor` namespace contains a number of properties which can be set to custom values.

###  Site pages
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

[[" + WikiConfig.CategoryNamespace + ":System pages]]",
            "System",
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultHelpAsync(string adminId) => Article.NewAsync(
            "Help",
            adminId,
@"{{Welcome}}

This page includes various information which will help you to get a [NeverFoundry](http://neverfoundry.com).Wiki instance up and running.

For information about the `NeverFoundry.Wiki` project, see the [[System:About|]] page.

# Markup
The NeverFoundry Wiki syntax is a custom flavor of markdown. It implements all the features of[CommonMark](http://commonmark.org), as well as many others. The implementation uses [Markdig](https://github.com/lunet-io/markdig), and details of most extensions to standard CommonMark can be found on [its GitHub page](https://github.com/lunet-io/markdig).

In addition NeverFoundry Wiki adds a few special syntax structures:

## Wiki Links
A wiki link is constructed using double square braces: `[[Example Link]]`. The content between the braces should be the exact title of a wiki page.

A link can include a namespace: `[[Namespace:Title]]`. If a page is in the default namespace, it may be omitted. The ""Talk"" pseudo-namespace can also be included to link directly to an article's discussion page: `[[Talk:Namespace:Title]]`.

In order to display an alternative title for a link, use a pipe character, followed by the display title: `[[Title|display]]` would be rendered as ""[[Title|display]]"" and link to the article ""Title."" If the pipe character is included, but nothing (ignoring whitespace) appears afterward, the display title is generated automatically by stripping any namespace prefix, as well as anything at the end of the title in parentheses. For example: `[[Namespace: Title (extra)|]]` would become ""[[Namespace: Title (extra)|]]"". If you use a second pipe character as the display title, the same stripping behavior is performed, and the result is also converted to lower case. For example: `[[Namespace: Title(extra)||]]` would become ""[[Namespace: Title(extra)||]]"".

If any characters appear after a link's closing braces, but before the next punctuation or whitespace character, they will automatically be added to the title to create a display value: `[[Title]]s` would appear as ""[[Title]]s"" and link to the article ""Title."" This doesn't work if the link already includes a display value: `[[Title|display]]s` would appear as ""[[Title|display]]s"" (note: the trailing""s"" is not included in the link text).

Anchors (section links) are also supported, and are stripped out of automatically-generated display values. For example: `[[Title#Section]]` appears as ""[[Title#Section]]"". Note that headings are automatically assigned anchor links with their text as the URL fragment. If a heading appears more than once in an article, the second instance will have a number appended to the end of its URL fragment, like so: ""Section-1"". Note that the first instance of a heading title will not have a number, and the second instance will have the number ""-1"" added.

Files uploaded to the wiki can be linked normally. Images uploaded can be prefixed with an exclamation point in order to display the image directly rather than link to the wiki page for the file, in the same way as standard CommonMark image links. For example: `![[File: MyImage]]`.

A special format exists to link to [Wikipedia](http://wikipedia.org). If the title of a wiki link is prefixed with ""w:"" the link is transformed into an external link to the wikipedia page for the given title. Display titles continue to work as usual, but if no display title is explicitly given the ""w:"" is stripped to provide the default. For example: `[[w:Wiki]]` becomes ""[[w:Wiki]]"" with a link to the page ""http://wikipedia.org/wiki/Wiki."" Wikipedia links (like all external links) are not checked for validity. In other words, special formatting which may usually be applied for links to non-existing wiki pages does not get applied to links to Wikipedia.

A similar special format exists for Creative Commons links: links whose titles are prefixed by ""cc:"" are automatically translated to an external link to the [WikiMedia Commons](http://commons.wikimedia.org), and the display (if not provided explicitly) has the ""cc:"" and any extension stripped. Images can also be directly linked, and when doing so they become a link to the page on the Creative Commons (in order to fulfill the license terms of Creative Commons content, which usually requires attribution). For example: `![[cc:Example.jpg]]` becomes ![[cc:Example.jpg]] with a link to ""https://commons.wikimedia.org/wiki/File:Example.jpg.""

## Transclusions
Any article can be transcluded into any other article by enclosing its link in double curly braces rather than square braces. For example: `{{Title}}` within the body of an article would cause the entire contents of the article ""Title"" to be rendered in place of the transclusion link when the page is displayed. The namespace of a page should also be included if it isn't the default. For example: `{{Namespace:Title}}` would transclude an article named ""Title"" in the ""Namespace"" namespace. Note that the assumed namespace when one is not explicitly provided may not be the same as the one for wiki links. See `TransclusionNamespace` below for details.

Transclusions can be included within transclusions, but after 100 levels of recursion any further transclusions will no longer be processed. Instead they will appear in the final article as originally written, including the surrounding brackets. This is done (rather than omitting such overly deep transclusions entirely) to make it clear to the editor and/or reader that there is a problem with the markdown, whereas an omission might be overlooked.

If the content of a transclusion is not a valid function or article name, the transclusion will appear in the article as-is. For example, a tranclusion like `{{4x4=16}}` (where ""4x4=16"" is not the name of an article) would be rendered as ""`{{4x4=16}}`"" without alteration.

Transclusions cannot span over multiple lines. In other words, line breaks between brackets will cause them to be regarded as literal characters, rather than enclosing a transclusion. If you need to include multi-line content as a parameter of a transclusion, however, there is an easy workaround: put the multi-line content into its own article, and transclude it.

Discussion pages cannot be transcluded. In other words, a transclusion like `{{Talk:Title}}` would not transclude the contents of the ""Title"" article's discussion page. Instead, ""`{{Talk:Title}}`"" would be rendered as-is. The reason for this behavior is that discussion pages are not actually articles. They are instead constructed on demand from a tree-like collection of messages which requires special rendering logic, including interactive elements that do not appear on standard article pages.

### Parameters
Values can be passed to transcluded articles by adding a pipe character after the title, then a value with an optional name: `{{Title|x=hello}}` transcludes the article ""Title"" and passes it ""hello"" as a parameter with the name ""x."" Inside the transcluded article a parameter can be referred to by their name inside triple curly braces. In the previous example article ""Title"" could include `<<x>>`, which would be replaced when the article was transcluded with ""hello.""

The name of a parameter can be omitted in a transclusion link: `{{Title|hello}}`. All unnamed parameters are assigned a default name which starts with ""1"" and increases for each unnamed parameter.For example: `{{Title|hello|x=there|friend}}` Includes parameters ""1"" with the value ""hello,"" ""x"" with the value ""there,"" and ""2"" with the value ""friend.""

The value of a parameter can be a transclusion, or another parameter.For example: `{{Title|x={{Other}}}}` transcludes the article named ""Title"" and passes it a parameter equal to the full content of the article named ""Other.""

# Functions
There are a number of built-in functions. Functions use the same syntax as transclusions (double curly braces) but instead of an article title the function name is used.

Functions take precedence over transclusions. That means that if there is an article with the same name as a transclusion (which is allowed), it cannot be transcluded, since any attempt to do so would invoke the function instead.

- **exec** - The most powerful function. The parameter named ""code"", or the first unnamed parameter if none are named ""code,"" is interpreted and executed as C# code.

   All other parameters are passed into the code to be executed as variables. Unnamed parameters are prefixed with an underscore (""_""), since variables cannot start with a digit in C#.

   `ToString()` is called on the return value of the code(if any) and the result is displayed.
- **format** - The first parameter is parsed as either a number or a `DateTimeOffset`, then `ToString()` is called on the result.

   If a second parameter is passed, it is used as the argument for the `ToString()` method if it is a valid format string for the type. If not, ""N"" is used for numbers, and ""g"" for Instants.
- **fullpagename** - Displays the title of the current page. Even when this function is used in a transcluded article, the title displayed will be that of the main article currently being viewed.

   Includes the namespace only when it isn't the default.

   The name of the article is displayed in title case.
- **if** - Attempts to parse the first parameter as either a boolean value or a number.If the value is either true or greater than zero, the second parameter is displayed.If not, the third parameter is displayed (or nothing, if there is no third parameter).
- **ifeq** - If the first and second parameters are equal, displays the third parameter.Otherwise, displays the fourth parameter (or nothing, if there is no fourth parameter).
- **ifnottemplate** - Displays the first parameter only if the current article is not being transcluded. In other words, the parameter is rendered when the article is viewed directly.
- **iftemplate** - Displays the first parameter only if the current article is being transcluded.
- **notoc** - Suppresses the generation of the default table of contents.

   The default table of contents is placed above the first heading in an article, or below the first paragraph (whichever comes first). It is omitted if any explicit table of contents are placed (with the **toc** function or the equivalent HTML comment).

   If any explicit table of contents are placed, this function does not suppress those. Only the default table is omitted.

   The same effect can also be accomplished by placing the HTML comment `<!-- NOTOC -->` anywhere in a page. The comment is generally preferred to the function, since the function requires more processing by the wiki engine, which adds to saving and loading time.
- **padleft** - Prefixes the first parameter with a number of leading characters. The total minimum length of the result is determined by the second parameter.The character used is a ""0"" unless a third parameter is supplied, in which case the first character of that parameter will be used.

   For example: `{{padleft|1|3}}` will produce ""001"" and `{{padleft|1|3|:}}` would instead generate ""::1"" while `{{padleft|4562|3}}` will produce 4562 (unchanged since it is already 3 characters long)

   If the second parameter is not a valid number, the first parameter will be displayed unchanged.
- **padright** - Works the same as **padleft** except that it adds the padding characters to the end of the first parameter, instead of the beginning.
- **pagename** - Displays the title of the current page.Even when this function is used in a transcluded article, the title displayed will be that of the main article currently being viewed.

   Unlike** fullpagename** this never includes the namespace.

   The name of the article is displayed in title case.
- ** preview** - The first paramater is displayed only when the article is being viewed as a preview.

    Any article which has preview content will display all of its explicitly marked preview content as its preview. Articles with no explicitly marked preview content will display approximately the first 200 characters.
 - **redirect** - A special function which must be at the very beginning of an article.The first parameter should be a wiki page title (with namespace, if non-default). When a wiki article which starts with this function in retrieved, the article specified in the parameter (even if it doesn't exist) is returned instead.

   A redirect may lead to another article which also redirects to another page, but this practice is discouraged since each step adds client time and database overhead. It is more efficient for the first redirecting article to point directly to the final target (i.e.the real article). A special page exists which lists double redirects, so that they may be discovered and eliminated eliminate them (by setting the first in a redirection chain to point directly to the final target).

   Redirection sequences abort if a cycle is detected (including a redirection which points to itself). In this situation the final redirecting article in the chain is displayed as-is (i.e. the wiki markup showing the redirect will be visible to the requesting client).

   A redirection chain also aborts if there are more than 100 links before reaching a non-redirect target.
- **sitename** - Displays the value of `WikiConfig.SiteName` (see below).
- **serverurl** - Displays the value of `WikiConfig.ServerUrl` (see below).
- **toc** - Displays a table of contents at the position of the function. In order to determine which headings to include, the table of contents looks for the closest heading which precedes itself (contents in between the heading and the table of contents are allowed). If none are found, all headings are included. If there is a preceding heading, all headings which follow the table of contents marker are included, up until the first heading of a lower level in the hierarchy than the preceding heading. For example: if you place a table of contents after a level 3 heading, it will include all following headings until it reaches a level 2 or 1 heading (which will not be included). This allows you to place tables of contents for subsections of an article.

   A depth (number of levels of hierarchy to display) can be specified with the first parameter, which overrides the default depth for the table of contents. Depth does not refer to the absolute level of a heading; it is relative to the starting level (see below). For example, if a table of contents has a starting level of 3, a depth of 2 would indicate that levels 3 and 4 should be displayed. A single '*' chartacter can be used in place of this parameter, which indicates that the default value should be used.

   A starting level can be specified with the second parameter, which overrides the default starting level for the table of contents. This number specifies the first level of the hierarchy which should be displayed in the table. It defaults to 1, which indicates that the top level of heading included should be displayed. For example: setting this to 2 would cause all headings with the lowest level to be omitted from the table. Note that this value does not refer to the absolute level of the headings. If a table of contents is placed after a level 3 heading, a starting level of 1 would indicate that headings of level 4 and down should be displayed. A single '*' chartacter can be used in place of this parameter, which indicates that the default value should be used.

   A title can be specified with the third parameter. This will be displayed as a heading above the table of contents itself. The default is ""Contents"". Note that a '*' character does *not* indicate the default value for this parameter (simply omit a third parameter for that effect). If a '*' character is used, it will appear as the actual title of the table of contents.

   The default table of contents is placed above the first heading in an article, or below the first paragraph (whichever comes first). It is omitted if any explicit table of contents are placed. To restore the default table after placing one explicitly, you can place another table of contents marker in the same position as the default would normally appear.

   The same effect can also be accomplished by placing the HTML comment `<!-- TOC -->` at the desired position, optionally with a specified depth, starting level, and title (e.g. `<!-- TOC * 2 Topics -->`). The comment is generally preferred to the function, since the function requires more processing by the wiki engine, which adds to saving and loading time.
- **tolower** - Displays the first parameter in lower case.
- **totitlecase** - Displays the first parameter in title case.
- **toupper** - Displays the first parameter in upper case.

# Customization

The `WikiConfig` static class contains a number of properties which can be set to custom values.

## Site identity
- **`SiteName`** - The name of your wiki.

   Default is ""A NeverFoundry Wiki Sample""; this is clearly not suitable for production, and should always be replaced.

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`ServerUrl`** - The primary URL of your wiki.

   Default is ""http://localhost:5000/Wiki/wiki""; this is clearly not suitable for production, and should always be replaced.

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.

##  Site pages
- **`CategoryNamespace`** - The title of the categories namespace, and the article on categories in the main wiki.

   Default is ""Categories""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`DefaultNamespace`** - The name of the default namespace.

   Default is ""Wiki""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`FileNamespace`** - The name of the file namespace.

   Default is ""File""

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`FilePath`** - The relative path to uploaded files.

   Default is ""file""

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

   The default is `[{TITLE}]({LINK}){.wiki-link wiki-link-{EXISTS}}`, where `{LINK}` is replaced by the relative URI, `{TITLE}` is replaced by the page name(or explicit display value), and `{EXISTS}` is replaced by either ""exists"" if the article exists or ""missing"" if the article does not exist. (Note that the `NeverFoundry.Wiki.Blazor` project overrides this to enable link previews.)

   May not be `null` or empty. Setting to an empty or all-whitespace value resets it to the default.
- **`MinimumTableOfContentsHeadings`** - The minimum number of headings required in an article to display a table of contents by default.

   Default is 3

   Can be overridden by specifying the location of a table of contents explicitly for a given article.
- **`Postprocessors`** - May be set to a `IList<IArticleProcessor>` (which implements a `Func<string, string> Process` property). Each processor in the list is passed the content of an article *after* it is parsed into HTML but *before* it is sanitized. Processors are run in the order they are added to the collection, and each is passed the output of the last.

   Note that no processors are run if the initial content is empty (i.e. you cannot use a processor to add content to an empty article).

   Note also that postprocessors can significantly slow down article saving and loading. It is recommended to use them sparingly, if at all, and to optimize them as far as is practical.
- **`ReadFile`** - Should be assigned a function which accepts a file name (a `string`) and returns the corresponding file as a byte array (or `null` if there is no such file).

   The default attempts to read the file from the local filesystem at the current working directory, in a subfolder named according to `FilePath`.

   May not be `null`. Setting to `null` resets it to the default. Setting it to a function which always returns `null` is feasible if your wiki implementation does not allow uploads.
- **`SaveFile`** - Should be assigned a function which accepts a file name (a `string`) and a byte array, and saves the given file. Should return a `bool` which indicates whether the operation was successful.

   The default attempts to save the file to the local filesystem at the current working directory, in a subfolder named according to `FilePath`.

   May not be `null`. Setting to `null` resets it to the default. Setting it to a function which always returns `false` is feasible if your wiki implementation does not allow uploads.

# Blazor
The `NeverFoundry.Wiki.Blazor` package contains a sample/default implementation of `NeverFoundry.Wiki` for use with a [Blazor](http://blazor.net) site. This implementation can be used as-is, or you can use the source as the starting point to build your own implementation. See [[Blazor|the Blazor page]] for more information.

[[" + WikiConfig.CategoryNamespace + ":System pages]]",
            "System",
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
