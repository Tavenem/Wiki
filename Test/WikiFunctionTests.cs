﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavenem.DataStorage;

namespace Tavenem.Wiki.Test;

[TestClass]
public class WikiFunctionTests
{
    private const string TEST_OWNER_ID = "Tester";
    private const string InnerNestedTitle = "InnerNested";
    private const string Namespace = "Namespace";
    private const string NestedTitle = "Nested";
    private const string Title = "Title";

    private const string LongArticle =
@"First paragraph text

{0}Second paragraph text

# First heading

Second section text

## Nested heading

Nested section text

# Second heading

Third section text

# Third heading

Fourth section text";
    private const string LongArticleExpected = "<p>First paragraph text</p>\n{0}<p>Second paragraph text</p>\n<h1 id=\"first-heading\">First heading</h1>\n<p>Second section text</p>\n<h2 id=\"nested-heading\">Nested heading</h2>\n<p>Nested section text</p>\n<h1 id=\"second-heading\">Second heading</h1>\n<p>Third section text</p>\n<h1 id=\"third-heading\">Third heading</h1>\n<p>Fourth section text</p>\n";

    private static readonly WikiOptions _Options = new();

    [TestMethod]
    public Task EvalTest() => TestTemplateAsync(new InMemoryDataStore(), "{{eval|Math.pow(x, 3)|x=2}}", "8");

    [TestMethod]
    public async Task ExecTest()
    {
        var dataStore = new InMemoryDataStore();
        _ = await GetArticleAsync(dataStore, "return Math.pow(x, 3);", "NestedExec", _Options.ScriptNamespace);
        await TestTemplateAsync(dataStore, "{{exec|NestedExec|x=2}}", "8");
    }

    [TestMethod]
    public async Task AnchorLinkTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "[[Title#Anchor|]]", "<a href=\"./Wiki/Title#anchor\" class=\"wiki-link-exists\">Title § Anchor</a>");
        await TestTemplateAsync(dataStore, "[[Title#Anchor||]]", "<a href=\"./Wiki/Title#anchor\" class=\"wiki-link-exists\">title § anchor</a>");
        await TestTemplateAsync(dataStore, "[[#Local Anchor|]]", "<a href=\"#local-anchor\" class=\"wiki-link-exists\">Local Anchor</a>");
        await TestTemplateAsync(dataStore, "[[#Local Anchor||]]", "<a href=\"#local-anchor\" class=\"wiki-link-exists\">local anchor</a>");
    }

    [TestMethod]
    public async Task FormatTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{format|52}}", "52");
        await TestTemplateAsync(dataStore, "{{format|52|D3}}", "052");
        await TestTemplateAsync(dataStore, "{{format|54321}}", "54,321");

        await TestTemplateAsync(dataStore, "{{format|5.2}}", "5.20");
        await TestTemplateAsync(dataStore, "{{format|5.2|C2}}", "$5.20");
        await TestTemplateAsync(dataStore, "{{format|54321|e5}}", "5.43210e+004");

        await TestTemplateAsync(dataStore, "{{format|03/10/2020 11:37 AM}}", "3/10/2020 11:37 AM");
        await TestTemplateAsync(dataStore, "{{format|03/10/2020 11:37 AM|d}}", "3/10/2020");
        await TestTemplateAsync(dataStore, "{{format|03/10/2020 11:37 AM +0:00|u}}", "2020-03-10 11:37:00Z");
    }

    [TestMethod]
    public async Task FullPageNameTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{fullpagename}}", Title);

        var nested = await GetArticleAsync(dataStore, "{{fullpagename}}", NestedTitle, _Options.TransclusionNamespace);
        Assert.AreEqual($"<p>{_Options.TransclusionNamespace}:{NestedTitle}</p>\n", nested.Html);
        await TestTemplateAsync(dataStore, $$$"""{{{{{NestedTitle}}}}}""", Title);
    }

    [TestMethod]
    public async Task IfTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{if|true|success}}", "success");
        await TestTemplateAsync(dataStore, "{{if|true|success|other}}", "success");

        await TestTemplateAsync(dataStore, "{{if|false|other|success}}", "success");
        await TestTemplateAsync(dataStore, "{{if|false|other}}", string.Empty, paragraph: false);

        await TestTemplateAsync(dataStore, "{{if|1|success}}", "success");
        await TestTemplateAsync(dataStore, "{{if|1|success|other}}", "success");

        await TestTemplateAsync(dataStore, "{{if|0|other|success}}", "success");
        await TestTemplateAsync(dataStore, "{{if|0|other}}", string.Empty, paragraph: false);
    }

    [TestMethod]
    public async Task IfCategoryTest()
    {
        var dataStore = new InMemoryDataStore();
        const string Markdown = "{{ifcategory|yes|no}}";
        await TestTemplateAsync(dataStore, Markdown, "no");

        var page = Category.Empty(new(Title, _Options.CategoryNamespace));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            Markdown);
        var category = dataStore.GetItem<Category>(page.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        Assert.AreEqual("<p>yes</p>\n", category.Html);
    }

    [TestMethod]
    public async Task IfEqTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{ifeq|one|one|success}}", "success");
        await TestTemplateAsync(dataStore, "{{ifeq|one|one|success|other}}", "success");

        await TestTemplateAsync(dataStore, "{{ifeq|one|two|other|success}}", "success");
        await TestTemplateAsync(dataStore, "{{ifeq|one|two|other}}", string.Empty, paragraph: false);

        await TestTemplateAsync(dataStore, "{{ifeq|true|TRUE|success}}", "success");
        await TestTemplateAsync(dataStore, "{{ifeq|true|TRUE|success|other}}", "success");

        await TestTemplateAsync(dataStore, "{{ifeq|1,234|1234|success}}", "success");
        await TestTemplateAsync(dataStore, "{{ifeq|1,234|1234|success|other}}", "success");

        await TestTemplateAsync(dataStore, "{{ifeq|1234.0|1234|success}}", "success");
        await TestTemplateAsync(dataStore, "{{ifeq|1234.0|1234|success|other}}", "success");
    }

    [TestMethod]
    public Task IfNotTemplateTest() => TestTemplateAsync(new InMemoryDataStore(), "{{ifnottemplate|success}}", "success");

    [TestMethod]
    public Task IfTalkTest() => TestTemplateAsync(new InMemoryDataStore(), "{{iftalk|fail|success}}", "success");

    [TestMethod]
    public async Task IfTemplateTest()
    {
        var dataStore = new InMemoryDataStore();
        _ = await GetArticleAsync(dataStore, "{{iftemplate|success}}", NestedTitle, _Options.TransclusionNamespace);
        await TestTemplateAsync(dataStore, $$$"""{{{{{NestedTitle}}}}}""", "success");
    }

    [TestMethod]
    public async Task LinkTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "[[Title|Alt <strong>title</strong>]]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Alt <strong>title</strong></a>");
        await TestTemplateAsync(dataStore, "[[title]]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>");
        await TestTemplateAsync(dataStore, "[[Title]]s", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span>s</a>");
        await TestTemplateAsync(dataStore, "[[Namespace:Title]]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-namespace\">Namespace</span><span class=\"wiki-link-title\">Title</span></a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[[Namespace:Title]]s", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-namespace\">Namespace</span><span class=\"wiki-link-title\">Title</span>s</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[[Namespace:Title|]]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">Title</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[[Namespace:Title||]]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">title</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[[Namespace:Title|]]s", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">Titles</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[[Namespace:Title||]]s", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">titles</a>", @namespace: Namespace);
    }

    [TestMethod]
    public Task NamespaceTest() => TestTemplateAsync(new InMemoryDataStore(), "{{namespace}}", Namespace, @namespace: Namespace);

    [TestMethod]
    public async Task NoTableOfContentsTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, string.Format(LongArticle, $"{{{{notoc}}}}{Environment.NewLine}{Environment.NewLine}"), string.Format(LongArticleExpected, string.Empty), paragraph: false);
        await TestTemplateAsync(dataStore, string.Format(LongArticle, $"<!-- NOTOC -->{Environment.NewLine}{Environment.NewLine}"), string.Format(LongArticleExpected, string.Empty), paragraph: false);
    }

    [TestMethod]
    public async Task PadLeftTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{padleft|1|3}}", "001");
        await TestTemplateAsync(dataStore, "{{padleft|1|3|:}}", "::1");
        await TestTemplateAsync(dataStore, "{{padleft|1|3|:m}}", "::1");
        await TestTemplateAsync(dataStore, "{{padleft|1|m}}", "1");
    }

    [TestMethod]
    public async Task PadRightTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{padright|1|3}}", "100");
        await TestTemplateAsync(dataStore, "{{padright|1|3|:}}", "1::");
        await TestTemplateAsync(dataStore, "{{padright|1|3|:m}}", "1::");
        await TestTemplateAsync(dataStore, "{{padright|1|m}}", "1");
    }

    [TestMethod]
    public async Task PageNameTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{pagename}}", Title);

        _ = await GetArticleAsync(dataStore, "{{pagename}}", NestedTitle, _Options.TransclusionNamespace);
        await TestTemplateAsync(dataStore, $$$"""{{{{{NestedTitle}}}}}""", Title);
    }

    [TestMethod]
    public async Task PreviewTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "content");
        Assert.AreEqual("<p>content</p>\n", article.Html);
        Assert.AreEqual("<p>content</p>\n", article.Preview);

        article = await GetArticleAsync(
            dataStore,
@"content

{{preview|hidden}}");
        Assert.AreEqual("<p>content</p>\n", article.Html);
        Assert.AreEqual("<p><span class=\"wiki-preview\">hidden</span></p>\n", article.Preview);
    }

    [TestMethod]
    public async Task ReviseTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "Article with [[link]]");
        Assert.AreEqual("<p>Article with <a href=\"./Wiki/Link\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Link</span></a></p>\n", article.Html);
        article = await GetArticleAsync(dataStore, "Revised article with [[link]]");
        Assert.AreEqual("<p>Revised article with <a href=\"./Wiki/Link\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Link</span></a></p>\n", article.Html);
    }

    [TestMethod]
    public async Task RevisionTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "Test content");
        var timestamp = article.Timestamp;
        var revision = await article
            .GetHtmlAsync(_Options, dataStore, timestamp)
            .ConfigureAwait(false);
        Assert.AreEqual("<p>Test content</p>\n", revision);
    }

    [TestMethod]
    public async Task SerializeTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "Content with a [[WikiLink]].");
        var json = System.Text.Json.JsonSerializer.Serialize(article);
        Console.WriteLine(json);

        var result = System.Text.Json.JsonSerializer.Deserialize<Article>(json);
        Assert.AreEqual(article.MarkdownContent, result?.MarkdownContent);
    }

    [TestMethod]
    public async Task TableOfContentsTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"{{{{toc}}}}{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);
        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"<!-- TOC -->{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"{{{{toc|1}}}}{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"<!-- TOC 1 -->{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"{{{{toc|*|2}}}}{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#nested-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"<!-- TOC * 2 -->{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#nested-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"{{{{toc|*|*|Headings}}}}{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);

        await TestTemplateAsync(
            dataStore,
            string.Format(LongArticle, $"<!-- TOC * * Headings -->{Environment.NewLine}{Environment.NewLine}"),
            string.Format(
                LongArticleExpected,
                "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
            paragraph: false);
    }

    [TestMethod]
    public Task ToLowerTest() => TestTemplateAsync(new InMemoryDataStore(), "{{tolower|miXed}}", "mixed");

    [TestMethod]
    public Task ToTitleCaseTest() => TestTemplateAsync(new InMemoryDataStore(), "{{totitlecase|miXed}}", "MiXed");

    [TestMethod]
    public Task ToUpperTest() => TestTemplateAsync(new InMemoryDataStore(), "{{toupper|miXed}}", "MIXED");

    [TestMethod]
    public async Task ComplexTest()
    {
        var dataStore = new InMemoryDataStore();

        const string InnerTemplate = """
            let s;
            if (args.length && args[0] && args[0].length) {
                s = args[0] + ', ';
            } else {
                s = 'For other uses, ';
            }
            if (args.length <= 1) {
                if (fullTitle == null) {
                    return s + 'try searching for this topic.';
                } else {
                    return s + 'see [[' + fullTitle + ' (disambiguation)|]]';
                }
            } else {
                s += 'see ';
            }
            for (let i = 1; i < args.length - 1; i++) {
                if (i > 1) {
                    s += ', ';
                }
                s += '[[' + args[i] + ']]';
            }
            if (args.length > 3) {
                s += ',';
            }
            if (args.length > 2) {
                s += ' and ';
            }
            s += '[[' + args[args.length - 1] + ']]';
            return s;
            """;
        _ = await GetArticleAsync(dataStore, InnerTemplate, InnerNestedTitle, _Options.ScriptNamespace);

        const string Template = $$$$$"""
            :::wiki-article-ref
            {{iftemplate|{{exec|{{{{{InnerNestedTitle}}}}}}}}}{{ifnottemplate|This is the ""For"" template}}
            :::
            """;
        _ = await GetArticleAsync(dataStore, Template, NestedTitle, _Options.TransclusionNamespace);

        await TestTemplateAsync(
            dataStore,
            $"{{{{{NestedTitle}}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>\n",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{{NestedTitle}|For stuff}}}}",
            "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"./Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>\n",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{{NestedTitle}||Title}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a></p>\n</div>\n",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{{NestedTitle}||Title|Other}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a> and <a href=\"./Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a></p>\n</div>\n",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{{NestedTitle}||Title|Other|Misc}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>, <a href=\"./Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a>, and <a href=\"./Wiki/Misc\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Misc</span></a></p>\n</div>\n",
            false);
    }

    private static async Task<Article> GetArticleAsync(IDataStore dataStore, string markdown, string? title = null, string? @namespace = null)
    {
        var pageTitle = new PageTitle(title ?? Title, @namespace);
        var id = IPage<Page>.GetId(pageTitle);
        var article = dataStore.GetItem<Article>(id, TimeSpan.Zero);
        if (article is null)
        {
            article = Article.Empty(pageTitle);
            await article.UpdateAsync(
                _Options,
                dataStore,
                TEST_OWNER_ID,
                markdown);
        }
        else
        {
            await article.UpdateAsync(
                _Options,
                dataStore,
                TEST_OWNER_ID,
                markdown);
        }
        article = dataStore.GetItem<Article>(id, TimeSpan.Zero);
        Assert.IsNotNull(article);
        return article;
    }

    private static async Task TestTemplateAsync(
        IDataStore dataStore,
        string markdown,
        string expected,
        bool paragraph = true,
        string? @namespace = null)
    {
        var article = await GetArticleAsync(dataStore, markdown, null, @namespace);
        Assert.AreEqual(paragraph ? $"<p>{expected}</p>\n" : expected, article.Html);
    }
}
