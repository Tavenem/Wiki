using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeverFoundry.DataStorage;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private const string ExpectedWelcome = "<p>Welcome to the <a href=\"http://neverfoundry.com\">NeverFoundry</a>.Wiki sample.</p>\n<p></p>\n";
        private const string ExpectedWelcomeTransclusion = @"Welcome to the <a href=""http://neverfoundry.com"">NeverFoundry</a>.Wiki sample.";

        private static readonly string _ExpectedAbout = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>The <a href=\"http://neverfoundry.com\">NeverFoundry</a>.Wiki package is a <a href=\"https://dotnet.microsoft.com\">.NET</a> <a href=\"http://wikipedia.org/wiki/Wiki\">wiki</a> library.</p>\n<p>Unlike many wiki implementations, the main package (<code>NeverFoundry.Wiki</code>) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.</p>\n<p>See the <a href=\"/Wiki/System:Help\" class=\"wiki-link-missing\">Help</a> page for usage information.</p>\n<p></p>\n";

        [TestMethod]
        public void CreateWikiTest()
        {
            const string AdminId = "AdminId";

            var options = new WikiOptions();
            var dataStore = new InMemoryDataStore();

            var welcome = GetDefaultWelcomeAsync(options, dataStore, AdminId).GetAwaiter().GetResult();
            Assert.AreEqual(ExpectedWelcome, welcome.Html, ignoreCase: false);

            GetDefaultMainAsync(options, dataStore, AdminId).GetAwaiter().GetResult();

            var about = GetDefaultAboutAsync(options, dataStore, AdminId).GetAwaiter().GetResult();

            var category = Category.GetCategory(options, dataStore, "System pages");
            Assert.IsNotNull(category);
            SetDefaultCategoryAsync(options, dataStore, category!, AdminId).GetAwaiter().GetResult();

            Assert.AreEqual(_ExpectedAbout, about.Html, ignoreCase: false);
        }

        private static Task<Article> GetDefaultAboutAsync(IWikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
            options,
            dataStore,
            "About",
            adminId,
@$"{{{{Welcome}}}}

The [NeverFoundry](http://neverfoundry.com).Wiki package is a [.NET](https://dotnet.microsoft.com) [[w:Wiki||]] library.

Unlike many wiki implementations, the main package (`NeverFoundry.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

See the [[System:Help|]] page for usage information.

[[{options.CategoryNamespace}:System pages]]",
            "System",
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultMainAsync(IWikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
            options,
            dataStore,
            options.MainPageTitle,
            adminId,
@$"{{{{Welcome}}}}

See the [[System:About|]] page for more information.

[[{options.CategoryNamespace}:System pages]]",
            options.DefaultNamespace,
            adminId,
            new[] { adminId });

        private static Task<Article> GetDefaultWelcomeAsync(IWikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
            options,
            dataStore,
            "Welcome",
            adminId,
@$"Welcome to the [NeverFoundry](http://neverfoundry.com).Wiki sample.

{{{{ifnottemplate|[[{options.CategoryNamespace}:System pages]]}}}}",
            options.TransclusionNamespace,
            adminId,
            new[] { adminId });

        private static Task SetDefaultCategoryAsync(IWikiOptions options, IDataStore dataStore, Category category, string adminId) => category.ReviseAsync(
            options,
            dataStore,
            adminId,
            markdown: "These are system pages in the [NeverFoundry](http://neverfoundry.com).Wiki sample [[w:Wiki||]].",
            revisionComment: "Provide a description",
            allowedEditors: new[] { adminId });
    }
}
