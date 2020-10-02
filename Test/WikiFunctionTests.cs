using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeverFoundry.DataStorage;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Test
{
    [TestClass]
    public class WikiFunctionTests
    {
        private const string Editor = "Tester";
        private const string InnerNestedTitle = "InnerNested";
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

        private static readonly IWikiOptions _Options = new WikiOptions();
        private static readonly IDataStore _DataStore = new InMemoryDataStore();

        private static Article? _Article;
        private static Article? _InnerNestedArticle;
        private static Article? _NestedArticle;

        [TestMethod]
        public void ExecTest() => TestTemplate("{{exec|Math.Pow(x, 3)|x=2}}", "8");

        [TestMethod]
        public void AnchorLinkTest()
        {
            TestTemplate("[[Title#Anchor|]]", "<a href=\"/Wiki/Title#anchor\" class=\"wiki-link-exists\">Title § Anchor</a>");
            TestTemplate("[[Title#Anchor||]]", "<a href=\"/Wiki/Title#anchor\" class=\"wiki-link-exists\">title § anchor</a>");
            TestTemplate("[[#Local Anchor|]]", "<a href=\"#Local%20Anchor\" class=\"wiki-link-exists\">Local Anchor</a>");
            TestTemplate("[[#Local Anchor||]]", "<a href=\"#Local%20Anchor\" class=\"wiki-link-exists\">local anchor</a>");
        }

        [TestMethod]
        public void FormatTest()
        {
            TestTemplate("{{format|52}}", "52");
            TestTemplate("{{format|52|D3}}", "052");
            TestTemplate("{{format|54321}}", "54,321");

            TestTemplate("{{format|5.2}}", "5.20");
            TestTemplate("{{format|5.2|C2}}", "$5.20");
            TestTemplate("{{format|54321|e5}}", "5.43210e+004");

            TestTemplate("{{format|03/10/2020 11:37 AM}}", "3/10/2020 11:37 AM");
            TestTemplate("{{format|03/10/2020 11:37 AM|d}}", "3/10/2020");
            TestTemplate("{{format|03/10/2020 11:37 AM +0:00|u}}", "2020-03-10 11:37:00Z");
        }

        [TestMethod]
        public void FullPageNameTest()
        {
            TestTemplate("{{fullpagename}}", "Title");

            var nested = GetNestedArticle("{{fullpagename}}");
            Assert.AreEqual($"<p>{_Options.TransclusionNamespace}:Nested</p>\n", nested.Html);
            TestTemplate("{{Nested}}", "Title");
        }

        [TestMethod]
        public void IfTest()
        {
            TestTemplate("{{if|true|success}}", "success");
            TestTemplate("{{if|true|success|other}}", "success");

            TestTemplate("{{if|false|other|success}}", "success");
            TestTemplate("{{if|false|other}}", string.Empty, paragraph: false);

            TestTemplate("{{if|1|success}}", "success");
            TestTemplate("{{if|1|success|other}}", "success");

            TestTemplate("{{if|0|other|success}}", "success");
            TestTemplate("{{if|0|other}}", string.Empty, paragraph: false);
        }

        [TestMethod]
        public void IfCategoryTest()
        {
            const string Markdown = "{{ifcategory|yes|no}}";
            TestTemplate(Markdown, "no");

            var category = Category.NewAsync(
                _Options,
                _DataStore,
                Title,
                Editor,
                Markdown)
                .GetAwaiter()
                .GetResult();
            Assert.AreEqual("<p>yes</p>\n", category.Html);
        }

        [TestMethod]
        public void IfEqTest()
        {
            TestTemplate("{{ifeq|one|one|success}}", "success");
            TestTemplate("{{ifeq|one|one|success|other}}", "success");

            TestTemplate("{{ifeq|one|two|other|success}}", "success");
            TestTemplate("{{ifeq|one|two|other}}", string.Empty, paragraph: false);

            TestTemplate("{{ifeq|true|TRUE|success}}", "success");
            TestTemplate("{{ifeq|true|TRUE|success|other}}", "success");

            TestTemplate("{{ifeq|1,234|1234|success}}", "success");
            TestTemplate("{{ifeq|1,234|1234|success|other}}", "success");

            TestTemplate("{{ifeq|1234.0|1234|success}}", "success");
            TestTemplate("{{ifeq|1234.0|1234|success|other}}", "success");
        }

        [TestMethod]
        public void IfNotTemplateTest() => TestTemplate("{{ifnottemplate|success}}", "success");

        [TestMethod]
        public void IfTalkTest() => TestTemplate("{{iftalk|fail|success}}", "success");

        [TestMethod]
        public void IfTemplateTest()
        {
            _ = GetNestedArticle("{{iftemplate|success}}");
            TestTemplate("{{Nested}}", "success");
        }

        [TestMethod]
        public void LinkTest()
        {
            TestTemplate("[[Title|Alt <strong>title</strong>]]", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\">Alt <strong>title</strong></a>");
            TestTemplate("[[title]]", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>");
            TestTemplate("[[Title]]s", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span>s</a>");
            TestTemplate("[[Wiki:Title]]", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>");
            TestTemplate("[[Wiki:Title|]]", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\">Title</a>");
            TestTemplate("[[Wiki:Title||]]", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\">title</a>");
            TestTemplate("[[Wiki:Title|]]s", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\">Titles</a>");
            TestTemplate("[[Wiki:Title||]]s", "<a href=\"/Wiki/Title\" class=\"wiki-link-exists\">titles</a>");
        }

        [TestMethod]
        public void NamespaceTest() => TestTemplate("{{namespace}}", _Options.DefaultNamespace);

        [TestMethod]
        public void NoTableOfContentsTest()
        {
            TestTemplate(string.Format(LongArticle, $"{{{{notoc}}}}{Environment.NewLine}{Environment.NewLine}"), string.Format(LongArticleExpected, string.Empty), paragraph: false);
            TestTemplate(string.Format(LongArticle, $"<!-- NOTOC -->{Environment.NewLine}{Environment.NewLine}"), string.Format(LongArticleExpected, string.Empty), paragraph: false);
        }

        [TestMethod]
        public void PadLeftTest()
        {
            TestTemplate("{{padleft|1|3}}", "001");
            TestTemplate("{{padleft|1|3|:}}", "::1");
            TestTemplate("{{padleft|1|3|:m}}", "::1");

            TestTemplate("{{padleft|1|m}}", "1");
        }

        [TestMethod]
        public void PadRightTest()
        {
            TestTemplate("{{padright|1|3}}", "100");
            TestTemplate("{{padright|1|3|:}}", "1::");
            TestTemplate("{{padright|1|3|:m}}", "1::");

            TestTemplate("{{padright|1|m}}", "1");
        }

        [TestMethod]
        public void PageNameTest()
        {
            TestTemplate("{{pagename}}", "Title");

            _ = GetNestedArticle("{{pagename}}");
            TestTemplate("{{Nested}}", "Title");
        }

        [TestMethod]
        public void PreviewTest()
        {
            var article = GetArticle("content");
            Assert.AreEqual("<p>content</p>\n", article.Html);
            Assert.AreEqual("<p>content</p>\n", article.Preview);

            article = GetArticle(
@"content

{{preview|hidden}}");
            Assert.AreEqual("<p>content</p>\n", article.Html);
            Assert.AreEqual("<p><span class=\"wiki-preview\">hidden</span></p>\n", article.Preview);
        }

        [TestMethod]
        public async Task RevisionTest()
        {
            var article = GetArticle("Test content");
            var timestamp = article.Timestamp;
            var revision = await article.GetHtmlAsync(_Options, _DataStore, timestamp).ConfigureAwait(false);
            Assert.AreEqual("<p>Test content</p>\n", revision);
        }

        [TestMethod]
        public void SerializeNewtonsoftTest()
        {
            var article = GetArticle("Content with a [[WikiLink]].");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(article, new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            Console.WriteLine(json);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Article>(json);
            Assert.AreEqual(article.MarkdownContent, result.MarkdownContent);
        }

        [TestMethod]
        public void SerializeSystemTextJsonTest()
        {
            var article = GetArticle("Content with a [[WikiLink]].");
            var json = System.Text.Json.JsonSerializer.Serialize(article);
            Console.WriteLine(json);

            var result = System.Text.Json.JsonSerializer.Deserialize<Article>(json);
            Assert.AreEqual(article.MarkdownContent, result?.MarkdownContent);
        }

        [TestMethod]
        public void TableOfContentsTest()
        {
            TestTemplate(
                string.Format(LongArticle, $"{{{{toc}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);
            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|1}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC 1 -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|*|2}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#nested-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC * 2 -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"#nested-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|*|*|Headings}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC * * Headings -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"#first-heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"#nested-heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"#second-heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"#third-heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);
        }

        [TestMethod]
        public void ToLowerTest() => TestTemplate("{{tolower|miXed}}", "mixed");

        [TestMethod]
        public void ToTitleCaseTest() => TestTemplate("{{totitlecase|miXed}}", "MiXed");

        [TestMethod]
        public void ToUpperTest() => TestTemplate("{{toupper|miXed}}", "MIXED");

        [TestMethod]
        public void ComplexTest()
        {
            const string InnerTemplate =
@"var p = new List<string>();
if (!string.IsNullOrEmpty(_1))
{
    p.Add(_1);
}
if (!string.IsNullOrEmpty(_2))
{
    p.Add(_2);
}
if (!string.IsNullOrEmpty(_3))
{
    p.Add(_3);
}
if (!string.IsNullOrEmpty(_4))
{
    p.Add(_4);
}
if (p.Count == 0)
{
    return string.Empty;
}
var s = new StringBuilder();
for (var i = 0; i < p.Count - 1; i++)
{
  if (i > 0)
  {
    s.Append("", "");
  }
  s.Append(""[["").Append(p[i]).Append(""]]"");
}
if (p.Count > 2)
{
  s.Append(',');
}
if (p.Count > 1)
{
    s.Append("" and "");
}
s.Append(""[["").Append(p[p.Count - 1]).Append(""]]"");
return s.ToString();";
            _ = GetInnerNestedArticle(InnerTemplate);

            const string Template =
@":::wiki-article-ref
For {{if|((1))|((1)), see {{if|((2))|{{exec|code = {{" + InnerNestedTitle + @"}}|((2))|((3))|((4))|((5))}}|[[{{fullpagename}} (disambiguation)|]]}}|other uses, see [[{{fullpagename}} (disambiguation)|]]}}
:::";
            _ = GetNestedArticle(Template);

            TestTemplate(
                $"{{{{{NestedTitle}}}}}",
                "<div class=\"wiki-article-ref\"><p>For other uses, see <a href=\"/Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>\n",
                false);
            TestTemplate(
                $"{{{{{NestedTitle}|stuff}}}}",
                "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"/Wiki/Title%20(disambiguation)\" class=\"wiki-link-missing\">Title</a></p>\n</div>\n",
                false);
            TestTemplate(
                $"{{{{{NestedTitle}|stuff|Title}}}}",
                "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a></p>\n</div>\n",
                false);
            TestTemplate(
                $"{{{{{NestedTitle}|stuff|Title|Other}}}}",
                "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a> and <a href=\"/Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a></p>\n</div>\n",
                false);
            TestTemplate(
                $"{{{{{NestedTitle}|stuff|Title|Other|Misc}}}}",
                "<div class=\"wiki-article-ref\"><p>For stuff, see <a href=\"/Wiki/Title\" class=\"wiki-link-exists\"><span class=\"wiki-link-title\">Title</span></a>, <a href=\"/Wiki/Other\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Other</span></a>, and <a href=\"/Wiki/Misc\" class=\"wiki-link-missing\"><span class=\"wiki-link-title\">Misc</span></a></p>\n</div>\n",
                false);
        }

        private static Article GetArticle(string markdown)
        {
            if (_Article is null)
            {
                _Article = Article.NewAsync(
                    _Options,
                    _DataStore,
                    Title,
                    Editor,
                    markdown)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                _Article.ReviseAsync(_Options, _DataStore, Editor, markdown: markdown).GetAwaiter().GetResult();
            }
            return _Article;
        }

        private static Article GetInnerNestedArticle(string markdown)
        {
            if (_InnerNestedArticle is null)
            {
                _InnerNestedArticle = Article.NewAsync(
                    _Options,
                    _DataStore,
                    InnerNestedTitle,
                    Editor,
                    markdown,
                    _Options.TransclusionNamespace)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                _InnerNestedArticle.ReviseAsync(_Options, _DataStore, Editor, markdown: markdown).GetAwaiter().GetResult();
            }
            return _InnerNestedArticle;
        }

        private static Article GetNestedArticle(string markdown)
        {
            if (_NestedArticle is null)
            {
                _NestedArticle = Article.NewAsync(
                    _Options,
                    _DataStore,
                    NestedTitle,
                    Editor,
                    markdown,
                    _Options.TransclusionNamespace)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                _NestedArticle.ReviseAsync(_Options, _DataStore, Editor, markdown: markdown).GetAwaiter().GetResult();
            }
            return _NestedArticle;
        }

        private static void TestTemplate(string markdown, string expected, bool paragraph = true)
        {
            var article = GetArticle(markdown);
            Assert.AreEqual(paragraph ? $"<p>{expected}</p>\n" : expected, article.Html);
        }
    }
}
