using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Test;

[TestClass]
public class IntegrationTests
{
    private const string AdminId = "AdminId";
    private const string CategoryTitle = "System pages";
    private const string ExpectedWelcome = "<p>Welcome to the <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> sample.</p>\n<p></p>\n";
    private const string ExpectedWelcomeTransclusion = @"Welcome to the <a href=""https://github.com/Tavenem/Wiki"">Tavenem.Wiki</a> sample.";

    private const string WelcomeTitle = "Welcome";

    private static readonly WikiUser Admin = new()
    {
        DisplayName = "admin",
        Id = AdminId,
        IsWikiAdmin = true,
    };

    private static readonly string _ExpectedAbout = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>The <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> package is a <a href=\"https://dotnet.microsoft.com\">.NET</a> <a href=\"https://wikipedia.org/wiki/Wiki\">wiki</a> library.</p>\n<p>Unlike many wiki implementations, the main package (<code>Tavenem.Wiki</code>) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.</p>\n<p>See the <a href=\"./Wiki/System:Help\" class=\"wiki-link-missing\">Help</a> page for usage information.</p>\n<p></p>\n";

    [TestMethod]
    public async Task ArchiveTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);

        _ = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager);
        _ = await GetDefaultMainAsync(options, dataStore, userManager, groupManager);
        _ = await UpdateCategoryAsync(options, dataStore, userManager, groupManager);
        _ = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager);

        var pageCount = dataStore.Query<Page>().Count();

        var archive = await dataStore.GetWikiArchiveAsync(options);

        Assert.IsNotNull(archive.Pages);
        Assert.AreEqual(pageCount, archive.Pages.Count);

        var json = System.Text.Json.JsonSerializer.Serialize(archive, WikiJsonSerializerContext.Default.Archive);
        Console.WriteLine(json);

        var deserialized = System.Text.Json.JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Archive);
        Assert.IsNotNull(deserialized);

        Assert.IsNotNull(deserialized.Pages);
        Assert.AreEqual(pageCount, deserialized.Pages.Count);

        var dataStore2 = new InMemoryDataStore();
        await deserialized.RestoreAsync(dataStore2, options, AdminId);

        var newPageCount = dataStore2.Query<Page>().Count();
        Assert.AreEqual(pageCount, newPageCount);
    }

    [TestMethod]
    public async Task CreateWikiTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);

        var success = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager);
        Assert.IsTrue(success);
        var welcome = await dataStore.GetWikiPageAsync(
            options,
            new(WelcomeTitle, options.TransclusionNamespace),
            true);
        Assert.AreEqual(ExpectedWelcome, welcome.Html, ignoreCase: false);

        success = await GetDefaultMainAsync(options, dataStore, userManager, groupManager);
        Assert.IsTrue(success);
        var main = await dataStore.GetWikiPageAsync(
            options,
            new(),
            true);
        var missing = main.WikiLinks.FirstOrDefault(x => x.IsMissing);
        Assert.IsNotNull(missing);

        var category = await IPage<Category>
            .GetExistingPageAsync<Category>(dataStore, new(CategoryTitle, options.CategoryNamespace));
        Assert.IsNotNull(category);
        await UpdateCategoryAsync(options, dataStore, userManager, groupManager);
        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        missing = category.WikiLinks.FirstOrDefault(x => x.IsMissing);
        Assert.IsNotNull(missing);

        success = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager);
        Assert.IsTrue(success);

        main = dataStore.GetItem<Article>(main.Id, TimeSpan.Zero);
        Assert.IsNotNull(main);
        missing = main.WikiLinks.FirstOrDefault(x => x.IsMissing);
        Assert.IsNull(missing);

        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        missing = category.WikiLinks.FirstOrDefault(x => x.IsMissing);
        Assert.IsNull(missing);

        var about = await dataStore.GetWikiPageAsync(
            options,
            new(options.AboutPageTitle, options.SystemNamespace),
            true);
        Assert.AreEqual(_ExpectedAbout, about.Html, ignoreCase: false);
    }

    private static Task<bool> GetDefaultAboutAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        Admin,
        new PageTitle(options.AboutPageTitle, options.SystemNamespace),
        $$$"""
        {{{{{WelcomeTitle}}}}}

        The [Tavenem.Wiki](https://github.com/Tavenem/Wiki) package is a [.NET](https://dotnet.microsoft.com) [[w:Wiki||]] library.

        Unlike many wiki implementations, the main package (`Tavenem.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

        See the [[{{{options.SystemNamespace}}}:{{{options.HelpPageTitle}}}|]] page for usage information.

        [[{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]]
        """,
        null,
        false,
        AdminId,
        new[] { AdminId });

    private static Task<bool> GetDefaultMainAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        Admin,
        new PageTitle(),
        $$$"""
        {{{{{WelcomeTitle}}}}}

        See the [[{{{options.SystemNamespace}}}:{{{options.AboutPageTitle}}}|]] page for more information.

        [[{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]]
        """,
        null,
        false,
        AdminId,
        new[] { AdminId });

    private static Task<bool> GetDefaultWelcomeAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        Admin,
        new PageTitle(WelcomeTitle, options.TransclusionNamespace),
        $$$"""
        Welcome to the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample.

        {{ifnottemplate|[[{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]]}}
        """,
        null,
        false,
        AdminId,
        new[] { AdminId });

    private static Task<bool> UpdateCategoryAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        Admin,
        new PageTitle(CategoryTitle, options.CategoryNamespace),
        $"These are system pages in the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample [[w:Wiki||]]. [[{options.SystemNamespace}:{options.AboutPageTitle}|]]",
        "Provide a description",
        false,
        AdminId,
        new[] { AdminId });
}
