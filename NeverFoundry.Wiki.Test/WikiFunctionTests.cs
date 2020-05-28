﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Test
{
    [TestClass]
    public class WikiFunctionTests
    {
        private const string Editor = "Tester";
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
        private const string LongArticleExpected = "<p>First paragraph text</p>\n{0}<p>Second paragraph text</p>\n<h1>First heading</h1>\n<p>Second section text</p>\n<h2>Nested heading</h2>\n<p>Nested section text</p>\n<h1>Second heading</h1>\n<p>Third section text</p>\n<h1>Third heading</h1>\n<p>Fourth section text</p>\n";

        private static Article? _Article;
        private static Article? _NestedArticle;

        [TestMethod]
        public void ExecTest() => TestTemplate("{{exec|Math.Pow(x, 3)|x=2}}", "8");

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
            Assert.AreEqual($"<p>{WikiConfig.TransclusionNamespace}:Nested</p>\n", nested.GetHtml());
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
        public void IfTemplateTest()
        {
            _ = GetNestedArticle("{{iftemplate|success}}");
            TestTemplate("{{Nested}}", "success");
        }

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
            Assert.AreEqual("<p>content</p>\n", article.GetHtml());
            Assert.AreEqual("<p>content</p>\n", article.GetPreview());

            article = GetArticle(
@"content

{{preview|hidden}}");
            Assert.AreEqual("<p>content</p>\n", article.GetHtml());
            Assert.AreEqual("<p><span class=\"wiki-preview\">hidden</span></p>\n", article.GetPreview());
        }

        [TestMethod]
        public async Task RevisionTest()
        {
            var article = GetArticle("Test content");
            var timestamp = article.Timestamp;
            var revision = await article.GetHtmlAsync(timestamp).ConfigureAwait(false);
            Assert.AreEqual("<p>Test content</p>\n", revision);
        }

        [TestMethod]
        public void SiteNameTest() => TestTemplate("{{sitename}}", "A NeverFoundry Wiki Sample");

        [TestMethod]
        public void ServerUrlTest() => TestTemplate("{{serverurl}}", "http://localhost:5000/");

        [TestMethod]
        public void TableOfContentsTest()
        {
            TestTemplate(
                string.Format(LongArticle, $"{{{{toc}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);
            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|1}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC 1 -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|*|2}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC * 2 -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Contents</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"{{{{toc|*|*|Headings}}}}{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);

            TestTemplate(
                string.Format(LongArticle, $"<!-- TOC * * Headings -->{Environment.NewLine}{Environment.NewLine}"),
                string.Format(
                    LongArticleExpected,
                    "<div class=\"toc\" role=\"navigation\">\n   <h2 class=\"toc-title\">Headings</h2>\n   <ul>\n      <li><a href=\"http://localhost:5000/#First%20heading\"><span class=\"toc-number\">1</span><span class=\"toc-heading\">First heading</span></a>\n         <ul>\n            <li><a href=\"http://localhost:5000/#Nested%20heading\"><span class=\"toc-number\">1.1</span><span class=\"toc-heading\">Nested heading</span></a></li>\n         </ul>\n      </li>\n      <li><a href=\"http://localhost:5000/#Second%20heading\"><span class=\"toc-number\">2</span><span class=\"toc-heading\">Second heading</span></a></li>\n      <li><a href=\"http://localhost:5000/#Third%20heading\"><span class=\"toc-number\">3</span><span class=\"toc-heading\">Third heading</span></a></li>\n   </ul>\n</div>\n"),
                paragraph: false);
        }

        [TestMethod]
        public void ToLowerTest() => TestTemplate("{{tolower|miXed}}", "mixed");

        [TestMethod]
        public void ToTitleCaseTest() => TestTemplate("{{totitlecase|miXed}}", "MiXed");

        [TestMethod]
        public void ToUpperTest() => TestTemplate("{{toupper|miXed}}", "MIXED");

        private Article GetArticle(string markdown)
        {
            if (_Article is null)
            {
                _Article = Article.NewAsync(
                    Title,
                    Editor,
                    markdown)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                _Article.ReviseAsync(Editor, markdown: markdown).GetAwaiter().GetResult();
            }
            return _Article;
        }

        private Article GetNestedArticle(string markdown)
        {
            if (_NestedArticle is null)
            {
                _NestedArticle = Article.NewAsync(
                    NestedTitle,
                    Editor,
                    markdown,
                    WikiConfig.TransclusionNamespace)
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                _NestedArticle.ReviseAsync(Editor, markdown: markdown).GetAwaiter().GetResult();
            }
            return _NestedArticle;
        }

        private void TestTemplate(string markdown, string expected, bool paragraph = true)
        {
            var article = GetArticle(markdown);
            Assert.AreEqual(paragraph ? $"<p>{expected}</p>\n" : expected, article.GetHtml());
        }
    }
}