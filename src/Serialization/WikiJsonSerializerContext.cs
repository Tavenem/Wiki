using System.Text.Json.Serialization;
using Tavenem.Wiki.Models;
using Tavenem.Wiki.Queries;

namespace Tavenem.Wiki;

/// <summary>
/// A source generated serializer context for <c>Tavenem.Wiki</c> types.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<MarkdownItem>))]
[JsonSerializable(typeof(List<Page>))]
[JsonSerializable(typeof(List<Article>))]
[JsonSerializable(typeof(List<UserPage>))]
[JsonSerializable(typeof(List<GroupPage>))]
[JsonSerializable(typeof(List<Category>))]
[JsonSerializable(typeof(List<WikiFile>))]
[JsonSerializable(typeof(List<Message>))]
[JsonSerializable(typeof(List<Revision>))]
[JsonSerializable(typeof(List<Topic>))]
[JsonSerializable(typeof(NormalizedPageReference))]
[JsonSerializable(typeof(PageHistory))]
[JsonSerializable(typeof(List<IWikiOwner>))]
[JsonSerializable(typeof(List<IWikiGroup>))]
[JsonSerializable(typeof(List<WikiGroup>))]
[JsonSerializable(typeof(List<IWikiUser>))]
[JsonSerializable(typeof(List<WikiUser>))]
[JsonSerializable(typeof(HistoryRequest))]
[JsonSerializable(typeof(List<LinkInfo>))]
[JsonSerializable(typeof(PagedRevisionInfo))]
[JsonSerializable(typeof(SpecialListRequest))]
[JsonSerializable(typeof(TitleRequest))]
[JsonSerializable(typeof(SearchRequest))]
[JsonSerializable(typeof(List<SearchHit>))]
public partial class WikiJsonSerializerContext : JsonSerializerContext;
