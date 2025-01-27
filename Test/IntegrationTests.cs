using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavenem.DataStorage;
using Tavenem.Wiki.Queries;
using Tavenem.Wiki.Services;

namespace Tavenem.Wiki.Test;

[TestClass]
public class IntegrationTests
{
    private const string AdminId = "AdminId";
    private const string CategoryTitle = "System pages";
    private const string ExpectedWelcome = "<p>Welcome to the <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> sample.</p>\n<p></p>";
    private const string ExpectedWelcomeTransclusion = @"Welcome to the <a href=""https://github.com/Tavenem/Wiki"">Tavenem.Wiki</a> sample.";

    private const string WelcomeTitle = "Welcome";

    private static readonly WikiUser Admin = new()
    {
        DisplayName = "admin",
        Id = AdminId,
        IsWikiAdmin = true,
    };

    private static readonly string _ExpectedAbout = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>The <a href=\"https://github.com/Tavenem/Wiki\">Tavenem.Wiki</a> package is a <a href=\"https://dotnet.microsoft.com\">.NET</a> <a href=\"https://wikipedia.org/wiki/Wiki\">wiki</a> library.</p>\n<p>Unlike many wiki implementations, the main package (<code>Tavenem.Wiki</code>) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.</p>\n<p>See the <a href=\"./Wiki/System:Help\" class=\"wiki-link-missing\">Help</a> page for usage information.</p>\n<p></p>";

    private static readonly string _ExpectedMain = $"<p>{ExpectedWelcomeTransclusion}</p>\n<p>See the <a href=\"./Wiki/System:About\" class=\"wiki-link-exists\">About</a> page for more information.</p>\n<p></p>";

    [TestMethod]
    public async Task ArchiveTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);
        var permissionManager = new PermissionManager();

        _ = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await GetDefaultMainAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await UpdateCategoryAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager, permissionManager);

        var pageCount = dataStore.Query<Page>().Count();

        var archive = await dataStore.GetWikiArchiveAsync(options);

        Assert.IsNotNull(archive.Pages);
        Assert.AreEqual(pageCount, archive.Pages.Count);

        var json = System.Text.Json.JsonSerializer.Serialize(archive, WikiArchiveJsonSerializerContext.Default.Archive);
        Console.WriteLine(json);

        var deserialized = System.Text.Json.JsonSerializer.Deserialize(json, WikiArchiveJsonSerializerContext.Default.Archive);
        Assert.IsNotNull(deserialized);

        Assert.IsNotNull(deserialized.Pages);
        Assert.AreEqual(pageCount, deserialized.Pages.Count);

        var dataStore2 = new InMemoryDataStore();
        await deserialized.RestoreAsync(dataStore2, options, AdminId);

        var newPageCount = dataStore2.Query<Page>().Count();
        Assert.AreEqual(pageCount, newPageCount);

        var main = await dataStore.GetWikiPageAsync(
            options,
            new(),
            true);
        Assert.AreEqual(_ExpectedMain, main.Html);
    }

    [TestMethod]
    public async Task CreateWikiTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);
        var permissionManager = new PermissionManager();

        var success = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager, permissionManager);
        Assert.IsTrue(success);
        var welcome = await dataStore.GetWikiPageAsync(
            options,
            new(WelcomeTitle, options.TransclusionNamespace),
            true);
        Assert.AreEqual(ExpectedWelcome, welcome.Html, ignoreCase: false);

        success = await GetDefaultMainAsync(options, dataStore, userManager, groupManager, permissionManager);
        Assert.IsTrue(success);
        var main = await dataStore.GetWikiPageAsync(
            options,
            new(),
            true);

        var category = await IPage<Category>
            .GetExistingPageAsync(
            dataStore,
            new(CategoryTitle, options.CategoryNamespace),
            typeInfo: WikiJsonSerializerContext.Default.Category);
        Assert.IsNotNull(category);
        await UpdateCategoryAsync(options, dataStore, userManager, groupManager, permissionManager);
        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);

        success = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager, permissionManager);
        Assert.IsTrue(success);

        main = dataStore.GetItem<Article>(main.Id, TimeSpan.Zero);
        Assert.IsNotNull(main);

        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        Assert.AreEqual(3, category.Children?.Count);

        var about = await dataStore.GetWikiPageAsync(
            options,
            new(options.AboutPageTitle, options.SystemNamespace),
            true);
        Assert.AreEqual(_ExpectedAbout, about.Html, ignoreCase: false);
    }

    [TestMethod]
    public async Task RedirectTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);
        var permissionManager = new PermissionManager();

        _ = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager, permissionManager);

        var aboutTitle = new PageTitle(options.AboutPageTitle, options.SystemNamespace);
        var redirectTitle = new PageTitle("AboutRedirect", options.SystemNamespace);
        await dataStore.AddOrReviseWikiPageAsync(
            options,
            userManager,
            groupManager,
            permissionManager,
            Admin,
            redirectTitle,
            null,
            null,
            false,
            AdminId,
            [AdminId],
            redirectTitle: aboutTitle);
        var about = await dataStore.GetWikiPageAsync(
            options,
            redirectTitle);
        Assert.AreEqual(aboutTitle, about.Title);
    }

    [TestMethod]
    public async Task SearchTest()
    {
        var options = new WikiOptions();
        var dataStore = new InMemoryDataStore();
        var userManager = new WikiUserManager(dataStore);
        dataStore.StoreItem(Admin);
        var groupManager = new WikiGroupManager(dataStore, userManager);
        var permissionManager = new PermissionManager();

        _ = await GetDefaultWelcomeAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await GetDefaultMainAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await UpdateCategoryAsync(options, dataStore, userManager, groupManager, permissionManager);
        _ = await GetDefaultAboutAsync(options, dataStore, userManager, groupManager, permissionManager);

        using var cache = new MemoryCache(Options.Create<MemoryCacheOptions>(new()));

        var searchRequest = new SearchRequest("wiki");
        var searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(3, searchResults.TotalCount);
        Assert.IsTrue(searchResults.Any(x => x.Title == new PageTitle()));
        Assert.IsTrue(searchResults.Any(x => x.Title == new PageTitle(options.AboutPageTitle, options.SystemNamespace)));
        Assert.IsTrue(searchResults.Any(x => x.Title == new PageTitle(CategoryTitle, options.CategoryNamespace)));

        searchRequest = new SearchRequest("wiki", Namespace: string.Empty);
        searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(1, searchResults.TotalCount);
        Assert.IsNull(searchResults[0].Title.Title);

        searchRequest = new SearchRequest("wiki", Namespace: options.SystemNamespace);
        searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(1, searchResults.TotalCount);
        Assert.AreEqual(options.AboutPageTitle, searchResults[0].Title.Title);

        searchRequest = new SearchRequest("desire");
        searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(1, searchResults.TotalCount);
        Assert.AreEqual(options.AboutPageTitle, searchResults[0].Title.Title);

        searchRequest = new SearchRequest("\"native\"");
        searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(1, searchResults.TotalCount);
        Assert.AreEqual(options.AboutPageTitle, searchResults[0].Title.Title);

        searchRequest = new SearchRequest("wiki -native");
        searchResults = await dataStore.SearchWikiAsync(
            options,
            groupManager,
            permissionManager,
            searchRequest,
            Admin,
            cache);

        Assert.AreEqual(2, searchResults.TotalCount);
        Assert.IsTrue(searchResults.Any(x => x.Title == new PageTitle()));
        Assert.IsTrue(searchResults.Any(x => x.Title == new PageTitle(CategoryTitle, options.CategoryNamespace)));
    }

    private static Task<bool> GetDefaultAboutAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IPermissionManager permissionManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        permissionManager,
        Admin,
        new PageTitle(options.AboutPageTitle, options.SystemNamespace),
        $$$"""
        {{> {{{WelcomeTitle}}}}}

        The [Tavenem.Wiki](https://github.com/Tavenem/Wiki) package is a [.NET](https://dotnet.microsoft.com) [w:Wiki||] library.

        Unlike many wiki implementations, the main package (`Tavenem.Wiki`) is implementation-agnostic. It provides a set of core features which can be used to build a web-based wiki, a desktop application, a distributed cloud app with native clients, or any other architecture desired.

        See the [{{{options.SystemNamespace}}}:{{{options.HelpPageTitle}}}|] page for usage information.

        [{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]
        """,
        null,
        false,
        AdminId,
        [AdminId]);

    private static Task<bool> GetDefaultMainAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IPermissionManager permissionManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        permissionManager,
        Admin,
        new PageTitle(),
        $$$"""
        {{> {{{WelcomeTitle}}}}}

        See the [{{{options.SystemNamespace}}}:{{{options.AboutPageTitle}}}|] page for more information.

        [{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]
        """,
        null,
        false,
        AdminId,
        [AdminId]);

    private static Task<bool> GetDefaultWelcomeAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IPermissionManager permissionManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        permissionManager,
        Admin,
        new PageTitle(WelcomeTitle, options.TransclusionNamespace),
        $$$"""
        Welcome to the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample.

        {{#unless isTemplate}}[{{{options.CategoryNamespace}}}:{{{CategoryTitle}}}]{{/unless}}
        """,
        null,
        false,
        AdminId,
        [AdminId]);

    private static Task<bool> UpdateCategoryAsync(
        WikiOptions options,
        IDataStore dataStore,
        IWikiUserManager userManager,
        IWikiGroupManager groupManager,
        IPermissionManager permissionManager) => dataStore.AddOrReviseWikiPageAsync(
        options,
        userManager,
        groupManager,
        permissionManager,
        Admin,
        new PageTitle(CategoryTitle, options.CategoryNamespace),
        $"These are system pages in the [Tavenem.Wiki](https://github.com/Tavenem/Wiki) sample [w:Wiki||]. [{options.SystemNamespace}:{options.AboutPageTitle}|]",
        "Provide a description",
        false,
        AdminId,
        [AdminId]);
}
