using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    private const string LongArticleExpected = "<p>First paragraph text</p>\n{0}<p>Second paragraph text</p>\n<h1 id=\"first-heading\">First heading</h1>\n<p>Second section text</p>\n<h2 id=\"nested-heading\">Nested heading</h2>\n<p>Nested section text</p>\n<h1 id=\"second-heading\">Second heading</h1>\n<p>Third section text</p>\n<h1 id=\"third-heading\">Third heading</h1>\n<p>Fourth section text</p>";

    private static readonly WikiOptions _Options = new();

    [TestMethod]
    public Task DomainTest() => TestTemplateAsync(new InMemoryDataStore(), "{{domain}}", string.Empty, false);

    [TestMethod]
    public Task EvalTest() => TestTemplateAsync(new InMemoryDataStore(), "{{#eval x=2}}Math.pow(x, 3){{/eval}}", "8");

    [TestMethod]
    public async Task ExecTest()
    {
        var dataStore = new InMemoryDataStore();
        _ = await GetArticleAsync(dataStore, "return Math.pow(x, 3);", "NestedExec", _Options.ScriptNamespace);
        await TestTemplateAsync(dataStore, $"{{{{#exec x=2}}}}{{{{> {_Options.ScriptNamespace}:NestedExec }}}}{{{{/exec}}}}", "8");
    }

    [TestMethod]
    public async Task AnchorLinkTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "[Title#Anchor|]", "<a href=\"./Wiki/Title#anchor\" class=\"wiki-link-exists\">Title § Anchor</a>");
        await TestTemplateAsync(dataStore, "[Title#Anchor||]", "<a href=\"./Wiki/Title#anchor\" class=\"wiki-link-exists\">title § anchor</a>");
        await TestTemplateAsync(dataStore, "[#Local Anchor|]", "<a href=\"#local-anchor\" class=\"wiki-link-exists\">Local Anchor</a>");
        await TestTemplateAsync(dataStore, "[#Local Anchor||]", "<a href=\"#local-anchor\" class=\"wiki-link-exists\">local anchor</a>");
    }

    [TestMethod]
    public async Task FormatTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{format 52}}", "52");
        await TestTemplateAsync(dataStore, "{{format 52 'D3'}}", "052");
        await TestTemplateAsync(dataStore, "{{format 52 D3}}", "052");

        await TestTemplateAsync(dataStore, "{{format 54321}}", "54,321");

        await TestTemplateAsync(dataStore, "{{format 5.2}}", "5.20");
        await TestTemplateAsync(dataStore, "{{format 5.2 'C2'}}", "$5.20");
        await TestTemplateAsync(dataStore, "{{format 5.2 C2}}", "$5.20");

        await TestTemplateAsync(dataStore, "{{format 54321 'e5'}}", "5.43210e+004");
        await TestTemplateAsync(dataStore, "{{format 54321 e5}}", "5.43210e+004");

        await TestTemplateAsync(dataStore, "{{format '03/10/2020 11:37 AM'}}", "3/10/2020 11:37 AM");
        await TestTemplateAsync(dataStore, "{{format '03/10/2020 11:37 AM' 'd'}}", "3/10/2020");
        await TestTemplateAsync(dataStore, "{{format '03/10/2020 11:37 AM' d}}", "3/10/2020");
        await TestTemplateAsync(dataStore, "{{format '03/10/2020 11:37 AM +0:00' 'u'}}", "2020-03-10 11:37:00Z");
        await TestTemplateAsync(dataStore, "{{format '03/10/2020 11:37 AM +0:00' u}}", "2020-03-10 11:37:00Z");
    }

    [TestMethod]
    public async Task FullPageNameTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{fullpagename}}", Title);

        var nested = await GetArticleAsync(dataStore, "{{fullpagename}}", NestedTitle, _Options.TransclusionNamespace);
        Assert.AreEqual($"<p>{_Options.TransclusionNamespace}:{NestedTitle}</p>", nested.Html);
        await TestTemplateAsync(dataStore, $$$"""{{> {{{NestedTitle}}} }}""", Title);
    }

    [TestMethod]
    public async Task HeadingsTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, LongArticle);
        Assert.IsNotNull(article);
        Assert.IsNotNull(article.Headings);
        Assert.AreEqual(article.Headings.Count, 4);

        var headings = article.Headings.ToList();

        Assert.AreEqual(headings[0].Id, "first-heading");
        Assert.AreEqual(headings[0].Level, 1);
        Assert.AreEqual(headings[0].OffsetLevel, 1);
        Assert.AreEqual(headings[0].Text, "First heading");

        Assert.AreEqual(headings[1].Id, "nested-heading");
        Assert.AreEqual(headings[1].Level, 2);
        Assert.AreEqual(headings[1].OffsetLevel, 2);
        Assert.AreEqual(headings[1].Text, "Nested heading");

        Assert.AreEqual(headings[2].Id, "second-heading");
        Assert.AreEqual(headings[2].Level, 1);
        Assert.AreEqual(headings[2].OffsetLevel, 1);
        Assert.AreEqual(headings[2].Text, "Second heading");

        Assert.AreEqual(headings[3].Id, "third-heading");
        Assert.AreEqual(headings[3].Level, 1);
        Assert.AreEqual(headings[3].OffsetLevel, 1);
        Assert.AreEqual(headings[3].Text, "Third heading");
    }

    [TestMethod]
    public async Task IfTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{#if true}}success{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if true}}success{{else}}other{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if false}}other{{else}}success{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if false}}other{{/if}}", string.Empty, paragraph: false);
        await TestTemplateAsync(dataStore, "{{#if 1}}success{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if 1}}success{{else}}other{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if 0}}other{{else}}success{{/if}}", "success");
        await TestTemplateAsync(dataStore, "{{#if 0}}other{{/if}}", string.Empty, paragraph: false);
    }

    [TestMethod]
    public async Task IfCategoryTest()
    {
        var dataStore = new InMemoryDataStore();
        const string Markdown = "{{#if isCategory}}yes{{else}}no{{/if}}";
        await TestTemplateAsync(dataStore, Markdown, "no");

        var page = Category.Empty(new(Title, _Options.CategoryNamespace));
        await page.UpdateAsync(
            _Options,
            dataStore,
            TEST_OWNER_ID,
            Markdown);
        var category = dataStore.GetItem<Category>(page.Id, TimeSpan.Zero);
        Assert.IsNotNull(category);
        Assert.AreEqual("<p>yes</p>", category.Html);
    }

    [TestMethod]
    public async Task IfEqTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{#ifequal one one}}success{{/ifequal}}", "success");
        await TestTemplateAsync(dataStore, "{{#ifequal one one}}success{{else}}other{{/ifequal}}", "success");

        await TestTemplateAsync(dataStore, "{{#ifequal one two}}other{{else}}success{{/ifequal}}", "success");
        await TestTemplateAsync(dataStore, "{{#ifequal one two}}other{{/ifequal}}", string.Empty, paragraph: false);

        await TestTemplateAsync(dataStore, "{{#ifequal true TRUE}}success{{/ifequal}}", "success");
        await TestTemplateAsync(dataStore, "{{#ifequal true TRUE}}success{{else}}other{{/ifequal}}", "success");

        await TestTemplateAsync(dataStore, "{{#ifequal 1,234 1234}}success{{/ifequal}}", "success");
        await TestTemplateAsync(dataStore, "{{#ifequal 1,234 1234}}success{{else}}other{{/ifequal}}", "success");

        await TestTemplateAsync(dataStore, "{{#ifequal 1234.0 1234}}success{{/ifequal}}", "success");
        await TestTemplateAsync(dataStore, "{{#ifequal 1234.0 1234}}success{{else}}other{{/ifequal}}", "success");
    }

    [TestMethod]
    public Task IfNotTemplateTest() => TestTemplateAsync(new InMemoryDataStore(), "{{#unless isTemplate}}success{{/unless}}", "success");

    [TestMethod]
    public Task IfTalkTest() => TestTemplateAsync(new InMemoryDataStore(), "{{#if isTalk}}fail{{else}}success{{/if}}", "success");

    [TestMethod]
    public async Task IfTemplateTest()
    {
        var dataStore = new InMemoryDataStore();
        _ = await GetArticleAsync(dataStore, "{{#if isTemplate}}success{{/if}}", NestedTitle, _Options.TransclusionNamespace);
        await TestTemplateAsync(dataStore, $"{{{{> {NestedTitle} }}}}", "success");
    }

    [TestMethod]
    public async Task LinkTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "[Title]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>");
        await TestTemplateAsync(dataStore, "[Namespace:Title]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-namespace\">Namespace</span><span class=\"wiki-link-title\">Title</span></a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[title]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\">title</a>");
        await TestTemplateAsync(dataStore, "[Alt <strong>title</strong>][Title]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Alt <strong>title</strong></a>");
        await TestTemplateAsync(dataStore, "[Title|s]", "<a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Titles</a>");
        await TestTemplateAsync(dataStore, "[Namespace:Title|]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">Title</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[Namespace:Title||]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">title</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[Namespace:Title|s]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">Titles</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, "[Namespace:Title||s]", "<a href=\"./Wiki/Namespace:Title\" class=\"wiki-link-exists\">titles</a>", @namespace: Namespace);
        await TestTemplateAsync(dataStore, $"[:{_Options.CategoryNamespace}:Title]", "<a href=\"./Wiki/Category:Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-namespace\">Category</span><span class=\"wiki-link-title\">Title</span></a>", @namespace: _Options.CategoryNamespace);
        await TestTemplateAsync(dataStore, "[w:Wiki]", "<a href=\"https://wikipedia.org/wiki/Wiki\">Wiki</a>");
        await TestTemplateAsync(dataStore, "![cc:Example.jpg]", "<a href=\"https://commons.wikimedia.org/wiki/File:Example.jpg\" target=\"_blank\"><img src=\"https://commons.wikimedia.org/wiki/Special:Redirect/file/File:Example.jpg\" alt=\"cc:Example.jpg\"></a>");
        await TestTemplateAsync(dataStore, "[all pages][~System:All_Pages]", "<a href=\"./Wiki/System:All_Pages\" class=\"wiki-link-exists\">all pages</a>", @namespace: "System");
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
        await TestTemplateAsync(dataStore, "x{{padleft '1' 3}}x", "x  1x");
        await TestTemplateAsync(dataStore, "x{{padleft '1' 3 '0'}}x", "x001x");
        await TestTemplateAsync(dataStore, "x{{padleft '1' 3 ':'}}x", "x::1x");
        await TestTemplateAsync(dataStore, "x{{padleft 1 3}}x", "x  1x");
        await TestTemplateAsync(dataStore, "x{{padleft 1 3 '0'}}x", "x001x");
        await TestTemplateAsync(dataStore, "x{{padleft 1 3 ':'}}x", "x::1x");
    }

    [TestMethod]
    public async Task PadRightTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "x{{padright '1' 3}}x", "x1  x");
        await TestTemplateAsync(dataStore, "x{{padright '1' 3 '0'}}x", "x100x");
        await TestTemplateAsync(dataStore, "x{{padright '1' 3 ':'}}x", "x1::x");
        await TestTemplateAsync(dataStore, "x{{padright 1 3}}x", "x1  x");
        await TestTemplateAsync(dataStore, "x{{padright 1 3 '0'}}x", "x100x");
        await TestTemplateAsync(dataStore, "x{{padright 1 3 ':'}}x", "x1::x");
    }

    [TestMethod]
    public async Task PageNameTest()
    {
        var dataStore = new InMemoryDataStore();
        await TestTemplateAsync(dataStore, "{{pagename}}", Title);

        _ = await GetArticleAsync(dataStore, "{{pagename}}", NestedTitle, _Options.TransclusionNamespace);
        await TestTemplateAsync(dataStore, $$$"""{{> {{{NestedTitle}}} }}""", Title);
    }

    [TestMethod]
    public async Task PreviewTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "content");
        Assert.AreEqual("<p>content</p>", article.Html);
        Assert.AreEqual("<p>content</p>", article.Preview);

        article = await GetArticleAsync(
            dataStore,
            """
            content

            {{#preview}}hidden{{/preview}}
            """);
        Assert.AreEqual("<p>content</p>", article.Html);

        Assert.IsNotNull(article.Preview);
        Assert.AreEqual("<div class=\"wiki-preview\"><p>hidden</p>\n</div>", article.Preview);
    }

    [TestMethod]
    public async Task ReviseTest()
    {
        var dataStore = new InMemoryDataStore();
        var article = await GetArticleAsync(dataStore, "Article with [link]");
        Assert.AreEqual("<p>Article with <a href=\"./Wiki/Link\" class=\"wiki-link-missing\">link</a></p>", article.Html);
        article = await GetArticleAsync(dataStore, "Revised article with [link]");
        Assert.AreEqual("<p>Revised article with <a href=\"./Wiki/Link\" class=\"wiki-link-missing\">link</a></p>", article.Html);
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
        Assert.AreEqual("<p>Test content</p>", revision);
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
            string.Format(LongArticle, $"{{{{toc 1}}}}{Environment.NewLine}{Environment.NewLine}"),
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
            string.Format(LongArticle, $"{{{{toc * 2}}}}{Environment.NewLine}{Environment.NewLine}"),
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
            string.Format(LongArticle, $"{{{{toc * * 'Headings'}}}}{Environment.NewLine}{Environment.NewLine}"),
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
    public Task ToLowerTest() => TestTemplateAsync(new InMemoryDataStore(), "{{lowercase 'miXed'}}", "mixed");

    [TestMethod]
    public Task ToTitleCaseTest() => TestTemplateAsync(new InMemoryDataStore(), "{{titlecase 'miXed'}}", "MiXed");

    [TestMethod]
    public Task ToUpperTest() => TestTemplateAsync(new InMemoryDataStore(), "{{uppercase 'miXed'}}", "MIXED");

    [TestMethod]
    public async Task ComplexTest()
    {
        var dataStore = new InMemoryDataStore();

        const string Template = $$$"""
            {{#unless isTemplate}}This is the "For" template. It executes the "For" script.{{/unless}}
            :::wiki-article-ref
            {{#if isTemplate}}{{#exec}}{{> Script:{{{InnerNestedTitle}}}}}{{/exec}}{{/if}}
            :::
            {{#unless isTemplate}}[Category:Link transclusions]{{/unless}}
            """;
        _ = await GetArticleAsync(dataStore, Template, NestedTitle, _Options.TransclusionNamespace);

        const string InnerTemplate = """
            let s;
            if (args.length && args[0] && args[0].length) {
                s = args[0] + ', ';
            } else {
                s = 'For other uses, ';
            }
            if (args.length <= 1) {
                if (fullpagename == null) {
                    return s + 'try searching for this topic.';
                } else {
                    return s + 'see [' + fullpagename + ' (disambiguation)|]';
                }
            } else {
                s += 'see ';
            }
            for (let i = 1; i < args.length - 1; i++) {
                if (i > 1) {
                    s += ', ';
                }
                s += '[' + args[i] + ']';
            }
            if (args.length > 3) {
                s += ',';
            }
            if (args.length > 2) {
                s += ' and ';
            }
            s += '[' + args[args.length - 1] + ']';
            return s;
            """;
        _ = await GetArticleAsync(dataStore, InnerTemplate, InnerNestedTitle, _Options.ScriptNamespace);

        await TestTemplateAsync(
            dataStore,
            $"{{{{> {NestedTitle} }}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{> {NestedTitle} 'For stuff'}}}}",
            "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"./Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{> {NestedTitle} '' 'Title'}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{> {NestedTitle} '' 'Title' 'Other'}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a> and <a href=\"./Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            $"{{{{> {NestedTitle} '' 'Title' 'Other' 'Misc'}}}}",
            "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"./Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>, <a href=\"./Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a>, and <a href=\"./Wiki/Misc\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Misc</span></a></p>\n</div>",
            false);
    }

    [TestMethod]
    public async Task TransclusionTest()
    {
        var dataStore = new InMemoryDataStore();

        const string Template = ":::wiki-main-article-ref\r\n{{#if isCategory}}The main article for this category is{{else}}Main article{{/if}} {{#if args.[1]}}[{{args.[1]}}][{{args.[0]}}]{{else}}{{#if args.[0]}}[{{args.[0]}}][{{pagename}}]{{else}}[{{pagename}}|]{{/if}}{{/if}}\r\n:::\r\n{{#unless isTemplate}}[Category:Transclusions]{{/unless}}";
        _ = await GetArticleAsync(dataStore, Template, new PageTitle(null, _Options.TransclusionNamespace));

        await TestTemplateAsync(
            dataStore,
            "{{> Main }}",
            "<div class=\"wiki-main-article-ref\"><p>The main article for this category is <a href=\"./Wiki/Title\" class=\"wiki-link-missing\">Title</a></p>\n</div>",
            false,
            _Options.CategoryNamespace);
        await TestTemplateAsync(
            dataStore,
            "{{> Main }}",
            "<div class=\"wiki-main-article-ref\"><p>Main article <a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Title</a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            "{{> Main 'Title'}}",
            "<div class=\"wiki-main-article-ref\"><p>Main article <a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Title</a></p>\n</div>",
            false);
        await TestTemplateAsync(
            dataStore,
            "{{> Main 'Title' 'Other'}}",
            "<div class=\"wiki-main-article-ref\"><p>Main article <a href=\"./Wiki/Title\" class=\"wiki-link-exists\">Other</a></p>\n</div>",
            false);
    }

    private static async Task<Article> GetArticleAsync(IDataStore dataStore, string markdown, PageTitle pageTitle)
    {
        var id = IPage<Page>.GetId(pageTitle);
        var article = dataStore.GetItem(id, WikiJsonSerializerContext.Default.Article, TimeSpan.Zero);
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
        article = dataStore.GetItem(id, WikiJsonSerializerContext.Default.Article, TimeSpan.Zero);
        Assert.IsNotNull(article);
        return article;
    }

    private static Task<Article> GetArticleAsync(IDataStore dataStore, string markdown, string? title = null, string? @namespace = null)
        => GetArticleAsync(dataStore, markdown, new PageTitle(title ?? Title, @namespace));

    private static async Task TestTemplateAsync(
        IDataStore dataStore,
        string markdown,
        string expected,
        bool paragraph = true,
        string? @namespace = null)
    {
        var article = await GetArticleAsync(dataStore, markdown, null, @namespace);
        Assert.AreEqual(paragraph ? $"<p>{expected}</p>" : expected, article.Html);
    }
}
