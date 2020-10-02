# NeverFoundry.Wiki.Web
This section contains two projects:
* `NeverFoundry.Wiki.Web`, which provides common logic for all
web-based implementations of `NeverFoundry.Wiki`.
* `NeverFoundry.Wiki.Mvc`, the "reference" implementation for `NeverFoundry.Wiki`, which is a [Razor
  class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) that can be
  included in an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core) project to turn it
  into a wiki.

## Configuration
The `WikiConfig` static class includes a number of settable properties which control the behavior of
the wiki.

Look in the `Mvc` folder for specific configuration information for `NeverFoundry.Wiki.Mvc`.