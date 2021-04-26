using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Test
{
    [TestClass]
    public class SerializationTests
    {
        private static readonly IWikiOptions _Options = new WikiOptions();

        [TestMethod]
        public void ArticleTest()
        {
            var value = new Article(
                "TEST_ID",
                Article.ArticleIdItemTypeName,
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
                null);

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Article>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = new Article(
                "TEST_ID",
                Article.ArticleIdItemTypeName,
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

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<Article>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void CategoryTest()
        {
            var value = new Category(
                "TEST_ID",
                Category.CategoryIdItemTypeName,
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
                null);

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Category>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void MarkdownItemTest()
        {
            var value = new MarkdownItemTestSubclass(
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new[] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }));

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = MarkdownItemTestSubclass.New(_Options, new InMemoryDataStore(), "Test markdown");

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<MarkdownItemTestSubclass>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void MessageTest()
        {
            var value = new Message(
                "TEST_ID",
                Message.MessageIdItemTypeName,
                "Test markdown",
                "Test markdown",
                "Test markdown",
                new ReadOnlyCollection<WikiLink>(new [] { new WikiLink(false, false, false, false, "Test Title", "Test Namespace") }),
                "TEST_TOPIC_ID",
                "TEST_SENDER_ID",
                false,
                "Test Sender Name",
                0,
                null);

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Message>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void MissingPageTest()
        {
            var value = new MissingPage(
                MissingPage.GetId("Test Title", "Test Namespace"),
                MissingPage.MissingPageIdItemTypeName,
                "Test Title",
                "Test Namespace",
                new List<string> { "Test_ID_2" }.AsReadOnly());

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<MissingPage>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void MixedArticleTypesTest()
        {
            var value = new List<Article>
            {
                new Article(
                    "TEST_ID",
                    Article.ArticleIdItemTypeName,
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
                    Category.CategoryIdItemTypeName,
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
                    WikiFile.WikiFileIdItemTypeName,
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

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<List<Article>>(json);
            Assert.IsNotNull(deserialized);
            Assert.IsTrue(value.OrderBy(x => x.Id).SequenceEqual(deserialized!.OrderBy(x => x.Id)));
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void TransclusionTest()
        {
            var value = new Transclusion("Test Title", "Test Namespace");

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Transclusion>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void WikiFileTest()
        {
            var value = new WikiFile(
                "TEST_ID",
                WikiFile.WikiFileIdItemTypeName,
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
                null);

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiFile>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void WikiLinkTest()
        {
            var value = new WikiLink(false, false, false, false, "Test Title", "Test Namespace");

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiLink>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = new WikiLink(false, true, false, false, "Test Title", "Test Namespace");

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiLink>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = new WikiLink(true, false, false, false, "Test Title", _Options.CategoryNamespace);

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiLink>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = new WikiLink(true, true, false, false, "Test Title", _Options.CategoryNamespace);

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiLink>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));

            value = new WikiLink(false, false, true, false, "Test Title", "Test Namespace");

            json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            deserialized = System.Text.Json.JsonSerializer.Deserialize<WikiLink>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }

        [TestMethod]
        public void WikiRevisionTest()
        {
            var value = new Revision(
                "TEST_ID",
                Revision.RevisionIdItemTypeName,
                "TEST_WIKI_ID",
                "Test Editor",
                "Test Title",
                "Test Namespace",
                "Test Revision",
                false,
                true,
                "Test comment",
                0);

            var json = System.Text.Json.JsonSerializer.Serialize(value);
            Console.WriteLine();
            Console.WriteLine(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<Revision>(json);
            Assert.AreEqual(value, deserialized);
            Assert.AreEqual(json, System.Text.Json.JsonSerializer.Serialize(deserialized));
        }
    }
}
