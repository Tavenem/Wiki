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
also includes a complete sample implementation. The sample is itself the documentation for this
library in wiki form, and can be viewed [here]() (or compiled from source to view offline, if you
wish).

## Building the Sample

Once you've cloned the source code the solution should be opened in [Visual
Studio](https://visualstudio.microsoft.com) ([Code](https://code.visualstudio.com)). You may instead
use another IDE of your choice, or use CLI only, but no guidance is provided for alternatives. Your
environment should be configured to work with ASP.NET 5.0 and [Docker](https://www.docker.com)
containers. [Docker Desktop](https://www.docker.com/products/docker-desktop) is required if you are
working on Windows or Mac, and should be configured for Linux containers. If you are developing on
Linux you will need to use the [Docker Engine](https://docs.docker.com/install/) itself.

### Customization

The `appsettings.json` and `appsettings.Development.json` should be replaced with your own values.
The `NeverFoundry.Wiki.MvcSample` project's `wwwroot` folder also contains multiple files which
should be replaced or customized, including the icons in the `images` folder, and the `index.html`
file.

Also see the `NeverFoundry.Wiki` and `NeverFoundry.Wiki.Mvc` [documentation]() for the many
customization options available for the wiki itself, including some essential basics such as site
identity and branding.

The sample includes [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) migrations
for [ASP.NET Core
Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity), and seed
code for the identity database as well as the wiki database. The sample has been designed to get its
data dependencies up and running from a cold start, so that no manual configuration or database
knowledge is required simply to view the sample wiki. If you are using the sample as the basis for a
production project, however, you may want to alter or remove this code to avoid accidentally
altering your schemas at runtime.

It is also worth mentioning that the sample Docker Compose project hard-codes connection strings,
passwords, and other sensitive information in plain text. This is obviously not suitable for a
production scenario. Your own sensitive information should be stored and applied to the build
process as environment variables in a more secure manner (e.g. [User
Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets), [Azure Key
Vault](https://azure.microsoft.com/en-us/services/key-vault/), etc.).
