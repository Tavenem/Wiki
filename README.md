# NeverFoundry.Wiki
The [NeverFoundry](http://neverfoundry.com).Wiki package is a [.NET](https://dotnet.microsoft.com)
[wiki](http://wikipedia.com/wiki/Wiki) library.

Unlike many wiki software packages, the main package (`NeverFoundry.Wiki`) is
implementation-agnostic. It provides a set of core features which can be used to build a web-based
wiki, a desktop application, a distributed cloud app with native clients, or any other architecture
desired.

The "reference" implementation included out-of-the-box (`NeverFoundry.Wiki.Mvc`) is a [Razor class
library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) which can be included in
an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to turn it into a wiki.

The source code for `NeverFoundry.Wiki` is available online (presumably you are viewing it now), and
also includes multiple sample implementations, one of which is a reasonably complete example of an
MVC application, complete with database and search client.

## Configuration
The `WikiOptions` class is used to control the behavior of the wiki. It is expected as a parameter
to various methods, and in most implementations is expected to be provided by dependency injection
after being configured during initialization.

Most of the properties of this class are not expected to change once a wiki has gone into operation.
Doing so can cause existing wiki pages to become inaccessible, or to be formatted incorrectly.

* `CategoriesTitle`: The name of the article on categories in the main wiki. Default is
  "Categories".
* `CategoryNamespace`: The name of the categories namespace. Default is "Category".
* `DefaultNamespace`: The name of the default namespace. Default is "Wiki".
* `DefaultTableOfContentsDepth`: The default number of levels of nesting shown in an article's table
  of contents. Can be overridden by specifying the level for a given article. Default is 3.
* `DefaultTableOfContentsTitle`: The default title of tables of contents. Default is "Contents".
* `FileNamespace`: The name of the file namespace. Default is "File".
* `LinkTemplate`: A string added to all wiki links, if non-empty. The string '\{LINK\}', if included,
  will be replaced by the full article title being linked.
* `MainPageTitle`: The title of the main page (shown when no article title is given). Default is
  "Main".
* `MinimumTableOfContentsHeadings`: The minimum number of headings required in an article to display
  a table of contents by default. Can be overridden by specifying the location of a table of
  contents explicitly for a given article. Default is 3.
* `Postprocessors`: A collection of preprocessors which transform the HTML of an article *after* it
  is parsed from markdown but *before* it is sanitized.

  Processors are run in the order they are added to the collection.
* `ReservedNamespaces`: An optional collection of namespaces which may not be assigned to pages by
  users. The namespaces assigned to `CategoryNamespace`, `FileNamespace`, and `TalkNamespace` are
  included automatically.

  Read-only. Values can be added with the `AddReservedNamespace` method.
* `ScriptNamespace`: The name of the script namespace. Default is "Script".
* `SiteName`: The name of the wiki. Displayed as a subheading below each article title. Default is
  "a NeverFoundry wiki".
* `TalkNamespace`: The name of the talk pseudo-namespace. Default is "Talk".
* `TransclusionNamespace`: The name of the transclusion namespace. Default is "Transclusion".
* `WikiLinkPrefix`: The prefix added before wiki links (to distinguish them from other pages on the
  same server). Default is "Wiki".

## Building the Samples

Once you've cloned the source code the solution should be opened in [Visual
Studio](https://visualstudio.microsoft.com) ([Code](https://code.visualstudio.com)). You may instead
use another IDE of your choice, or use CLI only, but no guidance is provided for alternatives. Your
environment should be configured to work with ASP.NET 5.0. To build the "complete" sample, your
environment should also be configured to work with [Docker](https://www.docker.com) containers.
[Docker Desktop](https://www.docker.com/products/docker-desktop) is required if you are working on
Windows or Mac, and should be configured for Linux containers. If you are developing on Linux you
will need to use the [Docker Engine](https://docs.docker.com/install/) itself.

### Customization

The values in `appsettings.json` and `appsettings.Development.json` should be replaced as needed.
The `wwwroot` folder also contains multiple files which should be replaced or customized, including
the icons in the `images` folder, and the `index.html` file.

Also see the README files in the `Web` folder for the many customization options
available for the wiki itself, including some essential basics such as site identity and branding.

The "complete" sample includes [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
migrations for [ASP.NET Core
Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity), and seed
code for the identity database. The shared sample project also includes seed data for the wiki
database. The samples have been designed to get their data dependencies up and running from a cold
start, so that no manual configuration or database knowledge is required simply to view the sample
wiki. If you are using the sample as the basis for a production project, however, you may want to
alter or remove this code to avoid accidentally altering your schemas at runtime.

It is also worth mentioning that the "complete" sample's Docker Compose project hard-codes
connection strings, passwords, and other sensitive information in plain text. This is obviously not
suitable for a production scenario. Your own sensitive information should be stored and applied to
the build process as environment variables in a more secure manner (e.g. [User
Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) in development, [Azure
Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) in production, etc.).
