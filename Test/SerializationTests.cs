using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Text.Json;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Test;

[TestClass]
public class SerializationTests
{
    private static readonly WikiOptions _Options = new WikiOptions();

    [TestMethod]
    public void ArticleTest()
    {
        var value = new Article(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            "Test Namespace",
            false,
            "TEST_OWNER_ID",
            null,
            null,
            null,
            null,
            false,
            false,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Article>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new Article(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            "Test Namespace",
            false,
            "TEST_OWNER_ID",
            new ReadOnlyCollection<string>(new string[] { "TEST_EDITOR_ID" }),
            new ReadOnlyCollection<string>(new string[] { "TEST_VIEWER_ID" }),
            null,
            null,
            false,
            false,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<Article>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void ArticleTest_Context()
    {
        var value = new Article(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            "Test Namespace",
            false,
            "TEST_OWNER_ID",
            null,
            null,
            null,
            null,
            false,
            false,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.Article);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.Article);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.Article));

        value = new Article(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            "Test Namespace",
            false,
            "TEST_OWNER_ID",
            new ReadOnlyCollection<string>(new string[] { "TEST_EDITOR_ID" }),
            new ReadOnlyCollection<string>(new string[] { "TEST_VIEWER_ID" }),
            null,
            null,
            false,
            false,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.Article);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.Article);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.Article));
    }

    [TestMethod]
    public void CategoryTest()
    {
        var value = new Category(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            new List<string> { "TEST_CHILD_ID" },
            0,
            _Options.CategoryNamespace,
            false,
            "TEST_OWNER_ID",
            null,
            null,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Category>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void CategoryTest_Context()
    {
        var value = new Category(
            "TEST_ID",
            "Test title",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            new List<string> { "TEST_CHILD_ID" },
            0,
            _Options.CategoryNamespace,
            false,
            "TEST_OWNER_ID",
            null,
            null,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.Category);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.Category);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.Category));
    }

    [TestMethod]
    public void MarkdownItemTest()
    {
        var value = new MarkdownItemTestSubclass(
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }));

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = MarkdownItemTestSubclass.New(_Options, new InMemoryDataStore(), "Test markdown");

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void MarkdownItemTest_Context()
    {
        var value = new MarkdownItemTestSubclass(
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }));

        var json = JsonSerializer.Serialize(
            value,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() });
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(
            json,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() });
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(
            deserialized,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() }));

        value = MarkdownItemTestSubclass.New(_Options, new InMemoryDataStore(), "Test markdown");

        json = JsonSerializer.Serialize(
            value,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() });
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<MarkdownItemTestSubclass>(
            json,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() });
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(
            deserialized,
            new JsonSerializerOptions() { TypeInfoResolver = new MarkdownItemPolymorphicTypeResolver() }));
    }

    [TestMethod]
    public void MessageTest()
    {
        var value = new Message(
            "TEST_ID",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            "TEST_TOPIC_ID",
            "TEST_SENDER_ID",
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
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            "TEST_TOPIC_ID",
            "TEST_SENDER_ID",
            false,
            "Test Sender Name",
            0,
            null);

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.Message);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.Message);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.Message));
    }

    [TestMethod]
    public void MissingPageTest()
    {
        var value = new MissingPage(
            MissingPage.GetId("Test Title", "Test Namespace"),
            "Test Title",
            "Test Namespace",
            new List<string> { "Test_ID_2" }.AsReadOnly());

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<MissingPage>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void MissingPageTest_Context()
    {
        var value = new MissingPage(
            MissingPage.GetId("Test Title", "Test Namespace"),
            "Test Title",
            "Test Namespace",
            new List<string> { "Test_ID_2" }.AsReadOnly());

        var json = JsonSerializer.Serialize(value, MissingPageContext.Default.MissingPage);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MissingPageContext.Default.MissingPage);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MissingPageContext.Default.MissingPage));
    }

    [TestMethod]
    public void MixedArticleTypesTest()
    {
        var value = new List<Article>
        {
            new Article(
                "TEST_ID",
                "Test title",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                0,
                "Test Namespace",
                false,
                "TEST_OWNER_ID",
                null,
                null,
                null,
                null,
                false,
                false,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
            new Category(
                "TEST_ID",
                "Test title",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                new List<string> { "TEST_CHILD_ID" },
                0,
                _Options.CategoryNamespace,
                false,
                "TEST_OWNER_ID",
                null,
                null,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
            new WikiFile(
                "TEST_ID",
                "Test title",
                "Test/Path",
                100,
                "test/type",
                "TEST_OWNER_ID",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                0,
                _Options.FileNamespace,
                false,
                "TEST_OWNER_ID",
                null,
                null,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
        };

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<List<Article>>(json);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(value.OrderBy(x => x.Id).SequenceEqual(deserialized!.OrderBy(x => x.Id)));
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void MixedArticleTypesTest_Context()
    {
        var value = new List<Article>
        {
            new Article(
                "TEST_ID",
                "Test title",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                0,
                "Test Namespace",
                false,
                "TEST_OWNER_ID",
                null,
                null,
                null,
                null,
                false,
                false,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
            new Category(
                "TEST_ID",
                "Test title",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                new List<string> { "TEST_CHILD_ID" },
                0,
                _Options.CategoryNamespace,
                false,
                "TEST_OWNER_ID",
                null,
                null,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
            new WikiFile(
                "TEST_ID",
                "Test title",
                "Test/Path",
                100,
                "test/type",
                "TEST_OWNER_ID",
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                0,
                _Options.FileNamespace,
                false,
                "TEST_OWNER_ID",
                null,
                null,
                new ReadOnlyCollection<string>(Array.Empty<string>()),
                null),
        };

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.ListArticle);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.ListArticle);
        Assert.IsNotNull(deserialized);
        Assert.IsTrue(value.OrderBy(x => x.Id).SequenceEqual(deserialized!.OrderBy(x => x.Id)));
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.ListArticle));
    }

    [TestMethod]
    public void TransclusionTest()
    {
        var value = new Transclusion("Test Title", "Test Namespace");

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<Transclusion>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void TransclusionTest_Context()
    {
        var value = new Transclusion("Test Title", "Test Namespace");

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.Transclusion);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.Transclusion);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.Transclusion));
    }

    [TestMethod]
    public void WikiFileTest()
    {
        var value = new WikiFile(
            "TEST_ID",
            "Test title",
            "Test/Path",
            100,
            "test/type",
            "TEST_OWNER_ID",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            _Options.FileNamespace,
            false,
            "TEST_OWNER_ID",
            null,
            null,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<WikiFile>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void WikiFileTest_Context()
    {
        var value = new WikiFile(
            "TEST_ID",
            "Test title",
            "Test/Path",
            100,
            "test/type",
            "TEST_OWNER_ID",
            "Test markdown",
            "Test markdown",
            "Test markdown",
            new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
            0,
            _Options.FileNamespace,
            false,
            "TEST_OWNER_ID",
            null,
            null,
            new ReadOnlyCollection<string>(Array.Empty<string>()),
            null);

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiFile);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiFile);
        Assert.AreEqual(value, deserialized);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiFile));
    }

    [TestMethod]
    public void WikiLinkTest()
    {
        var value = new WikiLink(false, false, false, false, "Test Title", "Test Namespace");

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(false, true, false, false, "Test Title", "Test Namespace");

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(true, false, false, false, "Test Title", _Options.CategoryNamespace);

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(true, true, false, false, "Test Title", _Options.CategoryNamespace);

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));

        value = new WikiLink(false, false, true, false, "Test Title", "Test Namespace");

        json = JsonSerializer.Serialize(value);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize<WikiLink>(json);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized));
    }

    [TestMethod]
    public void WikiLinkTest_Context()
    {
        var value = new WikiLink(false, false, false, false, "Test Title", "Test Namespace");

        var json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiLink);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiLink);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiLink));

        value = new WikiLink(false, true, false, false, "Test Title", "Test Namespace");

        json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiLink);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiLink);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiLink));

        value = new WikiLink(true, false, false, false, "Test Title", _Options.CategoryNamespace);

        json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiLink);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiLink);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiLink));

        value = new WikiLink(true, true, false, false, "Test Title", _Options.CategoryNamespace);

        json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiLink);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiLink);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiLink));

        value = new WikiLink(false, false, true, false, "Test Title", "Test Namespace");

        json = JsonSerializer.Serialize(value, MarkdownItemContext.Default.WikiLink);
        Console.WriteLine();
        Console.WriteLine(json);
        deserialized = JsonSerializer.Deserialize(json, MarkdownItemContext.Default.WikiLink);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, MarkdownItemContext.Default.WikiLink));
    }

    [TestMethod]
    public void WikiRevisionTest()
    {
        var value = new Revision(
            "TEST_ID",
            "TEST_WIKI_ID",
            "Test Editor",
            "Test Title",
            "Test Namespace",
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
            "TEST_ID",
            "TEST_WIKI_ID",
            "Test Editor",
            "Test Title",
            "Test Namespace",
            "Test Revision",
            false,
            true,
            "Test comment",
            0);

        var json = JsonSerializer.Serialize(value, RevisionContext.Default.Revision);
        Console.WriteLine();
        Console.WriteLine(json);
        var deserialized = JsonSerializer.Deserialize(json, RevisionContext.Default.Revision);
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(value, deserialized);
        Assert.AreEqual(json, JsonSerializer.Serialize(deserialized, RevisionContext.Default.Revision));
    }
}
