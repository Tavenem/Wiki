![build](https://img.shields.io/github/workflow/status/Tavenem/Wiki/publish/main) [![NuGet downloads](https://img.shields.io/nuget/dt/Tavenem.Wiki)](https://www.nuget.org/packages/Tavenem.Wiki/)

Tavenem.Wiki
==

Tavenem.Wiki is a [.NET](https://dotnet.microsoft.com) [wiki](http://wikipedia.com/wiki/Wiki)
library.

Unlike many wiki software packages, this main library is implementation-agnostic. It provides a set
of core features which can be used to build a web-based wiki, a desktop application, a distributed
cloud app with native clients, or any other architecture desired.

The "reference" implementation ([Tavenem.Wiki.Mvc](https://github.com/Tavenem/Wiki.Mvc)) is a [Razor
class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be
included in an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to turn it into
a wiki.

## Installation

Tavenem.Wiki is available as a [NuGet package](https://www.nuget.org/packages/Tavenem.Wiki/).

## Configuration
The `IWikiOptions` interface is used to control the behavior of the wiki. It is expected as a
parameter to various methods, and in most implementations is expected to be provided by dependency
injection after being configured during initialization.

Most of the properties of this class are not expected to change once a wiki has gone into operation.
Doing so can cause existing wiki pages to become inaccessible, or to be formatted incorrectly.

- `CategoriesTitle`: The name of the article on categories in the main wiki. Default is
  "Categories".
- `CategoryNamespace`: The name of the categories namespace. Default is "Category".
- `DefaultNamespace`: The name of the default namespace. Default is "Wiki".
- `DefaultTableOfContentsDepth`: The default number of levels of nesting shown in an article's table
  of contents. Can be overridden by specifying the level for a given article. Default is 3.
- `DefaultTableOfContentsTitle`: The default title of tables of contents. Default is "Contents".
- `FileNamespace`: The name of the file namespace. Default is "File".
- `LinkTemplate`: A string added to all wiki links, if non-empty. The string '\{LINK\}', if included,
  will be replaced by the full article title being linked.
- `MainPageTitle`: The title of the main page (shown when no article title is given). Default is
  "Main".
- `MinimumTableOfContentsHeadings`: The minimum number of headings required in an article to display
  a table of contents by default. Can be overridden by specifying the location of a table of
  contents explicitly for a given article. Default is 3.
- `Postprocessors`: A collection of preprocessors which transform the HTML of an article *after* it
  is parsed from markdown but *before* it is sanitized.

  Processors are run in the order they are added to the collection.
- `ReservedNamespaces`: An optional collection of namespaces which may not be assigned to pages by
  users. The namespaces assigned to `CategoryNamespace`, `FileNamespace`, and `TalkNamespace` are
  included automatically.

  Read-only. Values can be added with the `AddReservedNamespace` method.
- `ScriptNamespace`: The name of the script namespace. Default is "Script".
- `SiteName`: The name of the wiki. Displayed as a subheading below each article title. Default is
  "a NeverFoundry wiki".
- `TalkNamespace`: The name of the talk pseudo-namespace. Default is "Talk".
- `TransclusionNamespace`: The name of the transclusion namespace. Default is "Transclusion".
- `WikiLinkPrefix`: The prefix added before wiki links (to distinguish them from other pages on the
  same server). Default is "Wiki".

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
