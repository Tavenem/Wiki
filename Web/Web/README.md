# NeverFoundry.Wiki.Web
The `NeverFoundry.Wiki.Web` project provides common logic for all web-based implementations of
`NeverFoundry.Wiki`. It is included in the `NeverFoundry.Wiki.Mvc` project.

## Configuration
The `WikiWebConfig` static class includes a number of settable properties which control the behavior of
the wiki.
* `WikiTalkHubRoute`: A constant rather than a property, this provides a reference to the relative
  path to the [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction)
  [Hub](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs) used for discussion pages.
* `AdminGroupName`: The name of the admin user group. Default is "Wiki Admins".
* `AdminNamespaces`: An optional collection of namespaces which may not be assigned to pages by
  non-admin users. The namespace assigned to `SystemNamespace` is included automatically.

  Read-only. items can be added with the `AddAdminNamespace` method.
* `AboutPageTitle`*: The title of the main about page. Default is "About".
* `CompactLayoutPath`**: The layout used by wiki pages in compact view. Default is
  "`/Views/Wiki/_DefaultWikiMainLayout.cshtml`".
* `ContactPageTitle`*: The title of the main contact page. Default is "Contact".
* `ContentsPageTitle`*: The title of the main contents page. Default is "Contents".
* `CopyrightPageTitle`*: The title of the main copyright page. Default is "Copyright".
* `GroupNamespace`: The name of the user group namespace. Default is "Group".
* `HelpPageTitle`*: The title of the main help page. Default is "Help".
* `LoginPath`**: The path to the login page. Default is "`/Pages/Account/Login.cshtml`".
* `MainLayoutPath`**: The layout used by wiki pages. Default is
  "`/Views/Wiki/_DefaultWikiMainLayout.cshtml`".
* `MaxFileSize`: The maximum size (in bytes) of uploaded files. Default is 5,000,000 (5 MB).

  Setting this to a value less than or equal to zero effectively prevents file uploads.
* `MaxFileSizeString`: Read-only. Gets a string representing the `MaxFileSize` in a reasonable unit
  (GB for large sizes, down to bytes for small ones).
* `PolicyPageTitle`*: The title of the main policy page. Default is "Policies".
* `SystemNamespace`: The name of the system namespace. Default is "System".
* `TenorAPIKey`**: The API key to be used for [Tenor](https://tenor.com) GIF integration. Leave `null`
  (the default) to omit GIF functionality.
* `UserNamespace`: The name of the user namespace. Default is "User".

**This property may be set to `null` or an empty `string` to disable the associated wiki page.*

***Note that when using `NeverFoundry.Wiki.Mvc`, this property can be set via `IWikiOptions` in the
`AddWiki` extension method.*