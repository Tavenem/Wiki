using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using Tavenem.DataStorage;
using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Test;

[TestClass]
public class SerializationTests
{
    private const string TEST_OWNER_ID = "TEST_OWNER_ID";
    private static readonly WikiOptions _Options = new();

    [TestMethod]
    public async Task ArticleTest()
    {
        var dataStore = new InMemoryDataStore();
        var page = Article.Empty(new("Test title"));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var json = JsonSerializer.Serialize(page);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Article>(json);
        Assert.AreEqual(page, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public async Task ArticleTest_Context()
    {
        var dataStore = new InMemoryDataStore();
        var page = Article.Empty(new("Test title"));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var json = JsonSerializer.Serialize(page, WikiJsonSerializerContext.Default.Article);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Article);
        Assert.AreEqual(page, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.Article));
    }

    [TestMethod]
    public async Task CategoryTest()
    {
        var dataStore = new InMemoryDataStore();
        var category = Category.Empty(new("Test title", _Options.CategoryNamespace));
        await category.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);
        Assert.AreEqual(0, category.Children?.Count ?? 0);
        var child = Article.Empty(new("Test child"));
        await child.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            $"[[{_Options.CategoryNamespace}:Test title]]",
            "Test comment",
            TEST_OWNER_ID);
        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        Assert.AreEqual(1, category.Children?.Count ?? 0);

        var json = JsonSerializer.Serialize(category);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Category>(json);
        Assert.AreEqual(category, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public async Task CategoryTest_Context()
    {
        var dataStore = new InMemoryDataStore();
        var category = Category.Empty(new("Test title", _Options.CategoryNamespace));
        await category.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);
        Assert.AreEqual(0, category.Children?.Count ?? 0);
        var child = Article.Empty(new("Test child"));
        await child.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            $"[[{_Options.CategoryNamespace}:Test title]]",
            "Test comment",
            TEST_OWNER_ID);
        category = dataStore.GetItem<Category>(category.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        Assert.AreEqual(1, category.Children?.Count ?? 0);

        var json = JsonSerializer.Serialize(category, WikiJsonSerializerContext.Default.Category);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Category);
        Assert.AreEqual(category, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.Category));
    }

    [TestMethod]
    public async Task MarkdownItemTest()
    {
        var value = new MarkdownItemTestSubclass(
            "Test markdown",
            "Test markdown",
            "Test markdown",
            "Test markdown");

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = await MarkdownItemTestSubclass.NewAsync(_Options, new InMemoryDataStore(), "Test markdown");

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public async Task MarkdownItemTest_Context()
    {
        var value = new MarkdownItemTestSubclass(
            "Test markdown",
            "Test markdown",
            "Test markdown",
            "Test markdown");

        var serializerOptions = new JsonSerializerOptions()
        {
            TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver(),
        };
        var json = JsonSerializer.Serialize(
            value,
            serializerOptions);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(
            json,
            serializerOptions);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize<MarkdownItemTestSubclass>(
            deserialized,
            serializerOptions));

        value = await MarkdownItemTestSubclass.NewAsync(_Options, new InMemoryDataStore(), "Test markdown");

        json = JsonSerializer.Serialize(
            value,
            serializerOptions);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(
            json,
            serializerOptions);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize<MarkdownItemTestSubclass>(
            deserialized,
            serializerOptions));
    }

    [TestMethod]
    public void MessageTest()
    {
        var value = new Message(
            "TEST_ID",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            TEST_OWNER_ID,
            false,
            "Test Sender Name",
            0,
            null);

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Message>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void MessageTest_Context()
    {
        var value = new Message(
            "TEST_ID",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            TEST_OWNER_ID,
            false,
            "Test Sender Name",
            0,
            null);

        var json = JsonSerializer.Serialize(value, WikiJsonSerializerContext.Default.Message);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Message);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.Message));
    }

    [TestMethod]
    public async Task MixedPageTypesTest()
    {
        var dataStore = new InMemoryDataStore();

        var article = Article.Empty(new("Test title"));
        await article.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var category = Category.Empty(new("Test title"));
        await category.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var file = WikiFile.Empty(new("Test title"));
        await file.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test/Path",
            100,
            "test/type",
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var value = new List<Page>
        {
            article,
            category,
            file,
        };

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<List<Page>>(json);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(value.OrderBy(x => x.Id).SequenceEqual(deserialized!.OrderBy(x => x.Id)));
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
        Assert.AreEqual(3, deserialized.Count);
        Assert.IsTrue(deserialized[0] is Article);
        Assert.IsTrue(deserialized[1] is Category);
        Assert.IsTrue(deserialized[2] is WikiFile);
    }

    [TestMethod]
    public async Task MixedArticleTypesTest_Context()
    {
        var dataStore = new InMemoryDataStore();

        var article = Article.Empty(new("Test title"));
        await article.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var category = Category.Empty(new("Test title"));
        await category.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var file = WikiFile.Empty(new("Test title"));
        await file.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test/Path",
            100,
            "test/type",
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var value = new List<Page>
        {
            article,
            category,
            file,
        };

        var json = JsonSerializer.Serialize(value, WikiJsonSerializerContext.Default.ListPage);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.ListPage);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(value.OrderBy(x => x.Id).SequenceEqual(deserialized!.OrderBy(x => x.Id)));
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.ListPage));
        Assert.AreEqual(3, deserialized.Count);
        Assert.IsTrue(deserialized[0] is Article);
        Assert.IsTrue(deserialized[1] is Category);
        Assert.IsTrue(deserialized[2] is WikiFile);
    }

    [TestMethod]
    public async Task WikiFileTest()
    {
        var dataStore = new InMemoryDataStore();
        var page = WikiFile.Empty(new("Test title"));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test/Path",
            100,
            "test/type",
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var json = JsonSerializer.Serialize(page);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<WikiFile>(json);
        Assert.AreEqual(page, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public async Task WikiFileTest_Context()
    {
        var dataStore = new InMemoryDataStore();
        var page = WikiFile.Empty(new("Test title"));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            "Test/Path",
            100,
            "test/type",
            "Test markdown",
            "Test comment",
            TEST_OWNER_ID);

        var json = JsonSerializer.Serialize(page, WikiJsonSerializerContext.Default.WikiFile);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.WikiFile);
        Assert.AreEqual(page, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.WikiFile));
    }

    [TestMethod]
    public void WikiLinkTest()
    {
        var value = new WikiLink(
            null,
            null,
            false,
            false,
            false,
            new("Test Title", "Test Namespace", "Test Domain"));
        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(
            null,
            null,
            false,
            false,
            true,
            new("Test Title", "Test Namespace", "Test Domain"));
        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(
            null,
            null,
            true,
            false,
            false,
            new("Test Title", _Options.CategoryNamespace, "Test Domain"));
        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(
            null,
            null,
            true,
            true,
            false,
            new("Test Title", _Options.CategoryNamespace, "Test Domain"));
        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(
            null,
            "talk",
            false,
            false,
            false,
            new("Test Title", "Test Namespace", "Test Domain"));
        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void WikiRevisionTest()
    {
        var value = new Revision(
            TEST_OWNER_ID,
            "Test Revision",
            false,
            true,
            "Test comment",
            0);
        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Revision>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void WikiRevisionTest_Context()
    {
        var value = new Revision(
            TEST_OWNER_ID,
            "Test Revision",
            false,
            true,
            "Test comment",
            0);
        var json = JsonSerializer.Serialize(value, WikiJsonSerializerContext.Default.Revision);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, WikiJsonSerializerContext.Default.Revision);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, WikiJsonSerializerContext.Default.Revision));
    }
}
