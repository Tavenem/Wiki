![build](https://img.shields.io/github/workflow/status/Tavenem/Wiki/publish/main) [![NuGet downloads](https://img.shields.io/nuget/dt/Tavenem.Wiki)](https://www.nuget.org/packages/Tavenem.Wiki/)

Tavenem.Wiki
==

Tavenem.Wiki is a [.NET](https://dotnet.microsoft.com) [wiki](http://wikipedia.com/wiki/Wiki)
library.

Unlike many wiki software packages, this main library is implementation-agnostic. It provides a set
of core features which can be used to build a web-based wiki, a desktop application, a distributed
cloud app with native clients, or any other architecture desired.

## Installation

Tavenem.Wiki is available as a [NuGet package](https://www.nuget.org/packages/Tavenem.Wiki/).

## Configuration
The `WikiOptions` object is used to control the behavior of the wiki. It is expected as a parameter
to various methods, and in most implementations is expected to be provided by dependency injection
after being configured during initialization.

Most of the properties of this class are not expected to change once a wiki has gone into operation.
Doing so can cause existing wiki pages to become inaccessible, or to be formatted incorrectly.

- `AboutPageTitle`*: The title of the main about page. Default is "About".
- `CategoriesTitle`: The name of the article on categories in the main wiki. Default is
  "Categories".
- `CategoryNamespace`: The name of the categories namespace. Default is "Category".
- `ContactPageTitle`*: The title of the main contact page. Default is "Contact".
- `ContentsPageTitle`*: The title of the main contents page. Default is "Contents".
- `CopyrightPageTitle`*: The title of the main copyright page. Default is "Copyright".
- `CustomAdminNamespaces`: An optional collection of namespaces which may not be assigned to pages by non-admin users.

  The `AdminNamespaces` read-only property may be used to get the full list, which includes the namespace assigned to `SystemNamespace` automatically.
- `CustomReservedNamespaces`: An optional collection of namespaces which may not be assigned to pages by users.

  The `ReservedNamespaces` read-only property may be used to get the full list, which includes the namespace assigned to `FileNamespace` automatically.
- `DefaultAnonymousPermission`: The default permission granted to an anonymous user for wiki content with no configured access control.

  This defaults to `Read`, which allows anonymous users to view any content for which no specific
  access has been configured. It can be set to `None` to disable anonymous browsing, and require all
  users to sign in prior to viewing any content.

  Note that anonymous users cannot make any changes regardless of this setting. A specific editor is
  required for all content creation and revision.
- `DefaultRegisteredPermission`: The default permission granted to a registered user for wiki content with no configured access control.

  This defaults to `All`, which allows registered users full access when no specific access controls
  take precedence.
- `DefaultTableOfContentsDepth`: The default number of levels of nesting shown in an article's table
  of contents. Can be overridden by specifying the level for a given article. Default is 3.
- `DefaultTableOfContentsTitle`: The default title of tables of contents. Default is "Contents".
- `FileNamespace`: The name of the file namespace. Default is "File".
- `GetDomainPermission`: When a user attempts to interact with an article in a domain (including viewing, creating, editing, or deleting items), this function is invoked (if provided) to determine the permissions the user has for that domain.

  Receives the user's ID and the name of the domain as parameters, and should return a
  `WikiPermission` enum value.
- `GroupNamespace`: The name of the user group namespace. Default is "Group".
- `HelpPageTitle`*: The title of the main help page. Default is "Help".
- `LinkTemplate`: A string added to all wiki links, if non-empty. The string '\{LINK\}', if included,
  will be replaced by the full article title being linked.
- `MainPageTitle`: The title of the main page for any namespace (shown when no article title is
  explicitly requested). If omitted "Main" will be used.
- `MaxFileSize`: The maximum size (in bytes) of uploaded files. Default is 5,000,000 (5 MB).

  Setting this to a value less than or equal to zero effectively prevents file uploads.
- `MaxFileSizeString`: Read-only. Gets a string representing the `MaxFileSize` in a reasonable unit
  (GB for large sizes, down to bytes for small ones).
- `MinimumTableOfContentsHeadings`: The minimum number of headings required in an article to display
  a table of contents by default. Can be overridden by specifying the location of a table of
  contents explicitly for a given article. Default is 3.
- `OnCreated`: An optional callback invoked when a new article (including categories and files) is
  created.
  
  Receives the new article as a parameter.
- `OnDeleted`: An optional callback invoked when an article (including categories and files) is
  deleted.
  
  Receives the deleted article, the original owner, and the new owner as parameters.
- `OnEdited`: An optional callback invoked when an article (including categories and files) is
  edited (not including deletion if `OnDeleted` is provided).
  
  Receives the deleted article, the revision which was applied, the original owner, and the new
  owner as parameters.
- `PolicyPageTitle`*: The title of the main policy page. Default is "Policies".
- `Postprocessors`: A collection of preprocessors which transform the HTML of an article *after* it
  is parsed from markdown but *before* it is sanitized.

  Processors are run in the order they are added to the collection.
- `ScriptNamespace`: The name of the script namespace. Default is "Script".
- `SiteName`: The name of the wiki. Displayed as a subheading below each article title. Default is
  "a NeverFoundry wiki".
- `SystemNamespace`: The name of the system namespace. Default is "System".
- `TransclusionNamespace`: The name of the transclusion namespace. Default is "Transclusion".

  When a transclusion omits the namespace, this namespace is assumed. To transclude a page from the default (empty) namespace, a transclusion may use a single hyphen as the namespace. The hyphen will be replaced during transclusion by the empty namespace. If your wiki actually has a namespace that uses a single hyphen as its name, pages may be transcluded from it by escaping the hyphen with a backslash character: '`\-`'.
- `UserNamespace`: The name of the user namespace. Default is "User".
- `UserDomains`: If set to `true` each user (and only that user) is automatically granted full permission in a domain with the same name as their user ID. The `GetDomainPermission` function, the `WikiUser.AllowedViewDomains` property, and the `WikiGroup.AllowedViewDomains` property will still be checked for other users attempting to access content in such domains, but the user with the matching ID will always be granted all permissions automatically. A possible use for user domains is as a "scratch-pad" area where articles can be drafted and tested prior to publication.
- `WikiLinkPrefix`: A prefix added before wiki links (to distinguish them from other pages on the
  same server). Default is "Wiki".
  
  May be set to `null` or an empty `string`, which omits any prefix from generated URLs.

**This property may be set to `null` or an empty `string` to disable the associated wiki page.*

*The associated page is expected in the system namespace, and is not expected to be in a domain*

## Markup
The Tavenem Wiki syntax is a custom flavor of markdown. It implements all the features of
[CommonMark](http://commonmark.org), as well as many others. The implementation uses
[Markdig](https://github.com/lunet-io/markdig), and details of most extensions to standard
CommonMark can be found on [its GitHub page](https://github.com/lunet-io/markdig).

## Roadmap

Tavenem.Wiki is currently in a **prerelease** state. Development is ongoing, and breaking changes
are possible before the first production release.

No release date is currently set for v1.0 of Tavenem.Wiki. The project is currently in a "wait and
see" phase while [Tavenem.DataStore](https://github.com/Tavenem/DataStore) (a dependency of
Tavenem.Wiki) is in prerelease. When that project has a stable release, a production release of
Tavenem.Wiki will follow.

## Contributing

Contributions are always welcome. Please carefully read the [contributing](docs/CONTRIBUTING.md) document to learn more before submitting issues or pull requests.

## Code of conduct

Please read the [code of conduct](docs/CODE_OF_CONDUCT.md) before engaging with our community, including but not limited to submitting or replying to an issue or pull request.
