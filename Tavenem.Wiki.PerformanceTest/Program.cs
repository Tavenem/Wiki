using Tavenem.DataStorage;
using Tavenem.Wiki;

const string TEST_OWNER_ID = "Tester";
const string InnerNestedTitle = "InnerNested";
const string NestedTitle = "Nested";
const string Title = "Title";

WikiOptions _Options = new();

var dataStore = new InMemoryDataStore();

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

const string Template = $$$"""
    :::wiki-article-ref
    {{#if isTemplate}}
    {{#exec}}{{> Script:{{{InnerNestedTitle}}} }}{{/exec}}
    {{else}}
    This is the "For" template
    {{/if}}
    :::
    """;
_ = await GetArticleAsync(dataStore, Template, NestedTitle, _Options.TransclusionNamespace);

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

async Task<Article> GetArticleAsync(IDataStore dataStore, string markdown, string? title = null, string? @namespace = null)
{
    var pageTitle = new PageTitle(title ?? Title, @namespace);
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
    return dataStore.GetItem(id, WikiJsonSerializerContext.Default.Article, TimeSpan.Zero)
        ?? throw new InvalidOperationException();
}

async Task TestTemplateAsync(
    IDataStore dataStore,
    string markdown,
    string expected,
    bool paragraph = true,
    string? @namespace = null)
{
    var article = await GetArticleAsync(dataStore, markdown, null, @namespace);
    if (!string.Equals(
        paragraph
            ? $"<p>{expected}</p>"
            : expected,
        article.Html,
        StringComparison.Ordinal))
    {
        throw new InvalidOperationException();
    }
}