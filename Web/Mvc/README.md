# NeverFoundry.Wiki.Mvc
This is the "reference" implementation of `NeverFoundry.Wiki` for the web. It is a [Razor class
library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be included in
an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to turn it into a wiki.

In order to use it in a project, the following steps should be taken:

1. Add [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction) by calling
   `AddSignalR()` in the `ConfigureServices` method of your `Startup` class. This is necessary for
   discussion pages to function.
1. Call one of the overloads of `AddWiki` in the `ConfigureServices` method of your `Startup` class.
   `AddWiki()` requires two parameters, and you will usually want to provide three.
   
   The first parameter is either an instance of `IWikiUserManager` or a function which provides one.
   This interface allows the wiki to get information about users. Typically this will be a wrapper
   around your actual user persistence mechanism (e.g. [ASP.NET Core
   Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)). See
   the "complete" sample for an example implementation.

   The second parameter is either an instance of `IWikiGroupManager` or a function which provides
   one. This interface allows the wiki to get information about user groups. Typically this will be
   a wrapper around your actual user group persistence mechanism. See the "complete" sample for an
   example implementation.

   The third parameter is optional, but you will nearly always want to provide it: it is either an
   instance of `IWikiOptions` or a function which provides one. This interface allows you to
   configure the wiki, and provides the following properties:
   * `CompactLayoutPath`: Sets the property of the same name in `WikiWebConfig`.
   * `DataStore`: Sets the property of the same name in `WikiConfig`.
   
     Note: this property may be left `null`, which leaves the value in `WikiConfig` unchanged.
   * `LoginPath`: Sets the property of the same name in `WikiWebConfig`.
   * `MainLayoutPath`: Sets the property of the same name in `WikiWebConfig`.
   * `SearchClient`: Can be set to an implementation of `ISearchClient`. If omitted, an instance of
     `DefaultSearchClient` will be used.
     
     Note: the `DefaultSearchClient` is not recommended for production use. It is provided only to
     ensure that basic search functionality operates when an implementation of `ISearchClient` is
     not available (e.g. during debugging if the production client cannot be used during
     development).
   * `TenorAPIKey`: Sets the property of the same name in `WikiWebConfig`.
1. Call `MapWiki()` in the configuration function of your `UseEndpoints()`
   call in the `Configure` method of your `Startup` class.

   For example:
   ```c#
   app.UseEndpoints(endpoints =>
   {
       endpoints.MapWiki();
       endpoints.MapDefaultControllerRoute();
       endpoints.MapRazorPages();
   });
   ```
1. Add references to the [Razor class
   library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class)'s stylesheets in the
   `<head>` tag of your main layout:
   ```html
   <link href="~/_content/NeverFoundry.Wiki.Mvc/libstyles.css" rel="stylesheet" />
   <link href="~/_content/NeverFoundry.Wiki.Mvc/styles.css" rel="stylesheet" />
   ```

   (Optionally, you may import the [Sass](https://sass-lang.com) stylesheet "`styles.scss`" located in the `wwwroot`
   folder in your own `scss` file if you prefer to extend or bundle the main stylesheet. You must
   still include the "`libstyles.css`" file, however.)
1. Add references to the Razor class library's scripts at the bottom of the `<body>` tag of your
   main layout:
   ```html
   <script src="~/_content/NeverFoundry.Wiki.Mvc/libs.js"></script>
   <script src="~/_content/NeverFoundry.Wiki.Mvc/script.js"></script>
   ```