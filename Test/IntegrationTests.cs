using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Test;

[TestClass]
public class IntegrationTests
{
    private const string ExpectedWelcome = "<p>Welcome to the <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> sample.</p>\n<p></p>\n";
    private const string ExpectedWelcomeTransclusion = @"Welcome to the <a href=""https://github.com/Tavenem/Wiki"">Tavenem.Wiki</a> sample.";

    private static readonly string _ExpectedAbout = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>The <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> package is a <a href=\"https://dotnet.microsoft.com\">.NET</a> <a href=\"https://wikipedia.org/wiki/Wiki\">wiki</a> library.</p>\n<p>Unlike many wiki implementations, the main package (<code>Tavenem.Wiki</code>) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.</p>\n<p>See the <a href=\"/Wiki/System:Help\" class=\"wiki-link-missing\">Help</a> page for usage information.</p>\n<p></p>\n";

    [TestMethod]
    public async Task ArchiveTest()
    {
        const string AdminId = "AdminId";

        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();

        _ = await GetDefaultWelcomeAsync(options, dataStore, AdminId);
        _ = await GetDefaultMainAsync(options, dataStore, AdminId);
        var category = await Category.GetCategoryAsync(options, dataStore, "System pages");
        await SetDefaultCategoryAsync(options, dataStore, category!, AdminId);
        _ = await GetDefaultAboutAsync(options, dataStore, AdminId);

        var pageCount = dataStore.Query<Article>().Count();
        var revisionCount = dataStore.Query<Revision>().Count();

        var archive = await dataStore.GetWikiArchiveAsync(options);

        Assert.IsNull(archive.Messages);

        Assert.IsNotNull(archive.Pages);
        Assert.AreEqual(pageCount, archive.Pages.Count);

        Assert.IsNotNull(archive.Revisions);
        Assert.AreEqual(revisionCount, archive.Revisions.Count);

        var json = System.Text.Json.JsonSerializer.Serialize(archive, WikiJsonSerializerContext.Default.Archive);
        Console.WriteLine(json);

        var deserialized = System.Text.Json.JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Archive);
        Assert.IsNotNull(deserialized);

        Assert.IsNull(deserialized.Messages);

        Assert.IsNotNull(deserialized.Pages);
        Assert.AreEqual(pageCount, deserialized.Pages.Count);

        Assert.IsNotNull(deserialized.Revisions);
        Assert.AreEqual(revisionCount, deserialized.Revisions.Count);

        dataStore = new InMemoryDataStore();

        await deserialized.RestoreAsync(dataStore, options);

        var newPageCount = dataStore.Query<Article>().Count();
        var newRevisionCount = dataStore.Query<Revision>().Count();

        Assert.AreEqual(pageCount, newPageCount);
        Assert.AreEqual(revisionCount, newRevisionCount);
    }

    [TestMethod]
    public async Task CreateWikiTest()
    {
        const string AdminId = "AdminId";

        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();

        var welcome = await GetDefaultWelcomeAsync(options, dataStore, AdminId);
        Assert.AreEqual(ExpectedWelcome, welcome.Html, ignoreCase: false);

        var main = await GetDefaultMainAsync(options, dataStore, AdminId);
        var missing = main.WikiLinks.FirstOrDefault(x => x.Missing);
        Assert.IsNotNull(missing);

        var category = await Category.GetCategoryAsync(options, dataStore, "System pages");
        Assert.IsNotNull(category);
        await SetDefaultCategoryAsync(options, dataStore, category!, AdminId);
        missing = category.WikiLinks.FirstOrDefault(x => x.Missing);
        Assert.IsNotNull(missing);

        var about = await GetDefaultAboutAsync(options, dataStore, AdminId);

        main = dataStore.GetItem<Article>(main.Id, TimeSpan.Zero);
        Assert.IsNotNull(main);
        missing = main.WikiLinks.FirstOrDefault(x => x.Missing);
        Assert.IsNull(missing);

        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(main);
        missing = main.WikiLinks.FirstOrDefault(x => x.Missing);
        Assert.IsNull(missing);

        Assert.AreEqual(_ExpectedAbout, about.Html, ignoreCase: false);
    }

    private static Task<Article> GetDefaultAboutAsync(WikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
        options,
        dataStore,
        "About",
        adminId,
@$"{{{{Welcome}}}}

The [Tavenem.Wiki](https://github.com/Tavenem/Wiki) package is a [.NET](https://dotnet.microsoft.com) [[w:Wiki||]] library.

Unlike many wiki implementations, the main package (`Tavenem.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

See the [[System:Help|]] page for usage information.

[[{options.CategoryNamespace}:System pages]]",
        "System",
        null,
        adminId,
        new[] { adminId });

    private static Task<Article> GetDefaultMainAsync(WikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
        options,
        dataStore,
        options.MainPageTitle,
        adminId,
@$"{{{{Welcome}}}}

See the [[System:About|]] page for more information.

[[{options.CategoryNamespace}:System pages]]",
        options.DefaultNamespace,
        null,
        adminId,
        new[] { adminId });

    private static Task<Article> GetDefaultWelcomeAsync(WikiOptions options, IDataStore dataStore, string adminId) => Article.NewAsync(
        options,
        dataStore,
        "Welcome",
        adminId,
@$"Welcome to the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample.

{{{{ifnottemplate|[[{options.CategoryNamespace}:System pages]]}}}}",
        options.TransclusionNamespace,
        null,
        adminId,
        new[] { adminId });

    private static Task SetDefaultCategoryAsync(WikiOptions options, IDataStore dataStore, Category category, string adminId) => category.ReviseAsync(
        options,
        dataStore,
        adminId,
        markdown: "These are system pages in the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample [[w:Wiki||]]. [[System:About|]]",
        revisionComment: "Provide a description",
        allowedEditors: new[] { adminId });
}
