# NeverFoundry.Wiki.Web
The `NeverFoundry.Wiki.Web` project provides common logic for all web-based implementations of
`NeverFoundry.Wiki`. It is included in the `NeverFoundry.Wiki.Mvc` project.

## Configuration
The `WikiWebConfig` static class includes a number of settable properties which control the behavior of
the wiki.
* `AdminGroupName`: The name of the admin user group. Default is "Wiki Admins".
* `AdminNamespaces`: An optional collection of namespaces which may not be assigned to pages by
  non-admin users. The namespace assigned to `SystemNamespace` is included automatically.

  Read-only. items can be added with the `AddAdminNamespace` method.
* `AboutPageTitle`*: The title of the main about page. Default is "About".
* `ContactPageTitle`*: The title of the main contact page. Default is "Contact".
* `ContentsPageTitle`*: The title of the main contents page. Default is "Contents".
* `CopyrightPageTitle`*: The title of the main copyright page. Default is "Copyright".
* `GroupNamespace`: The name of the user group namespace. Default is "Group".
* `HelpPageTitle`*: The title of the main help page. Default is "Help".
* `MaxFileSize`: The maximum size (in bytes) of uploaded files. Default is 5,000,000 (5 MB).

  Setting this to a value less than or equal to zero effectively prevents file uploads.
* `MaxFileSizeString`: Read-only. Gets a string representing the `MaxFileSize` in a reasonable unit
  (GB for large sizes, down to bytes for small ones).
* `PolicyPageTitle`*: The title of the main policy page. Default is "Policies".
* `SystemNamespace`: The name of the system namespace. Default is "System".
* `UserNamespace`: The name of the user namespace. Default is "User".

**This property may be set to `null` or an empty `string` to disable the associated wiki page.*