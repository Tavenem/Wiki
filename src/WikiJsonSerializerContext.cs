using System.Text.Json.Serialization;
using Tavenem.Wiki.Models;
using Tavenem.Wiki.Queries;

namespace Tavenem.Wiki;

/// <summary>
/// A source generated serializer context for <c>Tavenem.Wiki</c> types.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(MarkdownItem))]
[JsonSerializable(typeof(List<MarkdownItem>))]
[JsonSerializable(typeof(List<Page>))]
[JsonSerializable(typeof(List<Article>))]
[JsonSerializable(typeof(List<Category>))]
[JsonSerializable(typeof(List<WikiFile>))]
[JsonSerializable(typeof(List<Message>))]
[JsonSerializable(typeof(Revision))]
[JsonSerializable(typeof(List<Revision>))]
[JsonSerializable(typeof(Topic))]
[JsonSerializable(typeof(List<Topic>))]
[JsonSerializable(typeof(IWikiOwner))]
[JsonSerializable(typeof(List<IWikiOwner>))]
[JsonSerializable(typeof(IWikiGroup))]
[JsonSerializable(typeof(List<IWikiGroup>))]
[JsonSerializable(typeof(WikiGroup))]
[JsonSerializable(typeof(List<WikiGroup>))]
[JsonSerializable(typeof(IWikiUser))]
[JsonSerializable(typeof(List<IWikiUser>))]
[JsonSerializable(typeof(WikiUser))]
[JsonSerializable(typeof(List<WikiUser>))]
[JsonSerializable(typeof(CategoryInfo))]
[JsonSerializable(typeof(GroupPageInfo))]
[JsonSerializable(typeof(HistoryRequest))]
[JsonSerializable(typeof(LinkInfo))]
[JsonSerializable(typeof(List<LinkInfo>))]
[JsonSerializable(typeof(PagedRevisionInfo))]
[JsonSerializable(typeof(SpecialListRequest))]
[JsonSerializable(typeof(UserPageInfo))]
[JsonSerializable(typeof(WhatLinksHereRequest))]
[JsonSerializable(typeof(WikiEditInfo))]
[JsonSerializable(typeof(WikiPageInfo))]
public partial class WikiJsonSerializerContext : JsonSerializerContext { }

/// <summary>
/// A source generated serializer context for <see cref="Wiki.Archive"/> which minimizes the size of the payload.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    IgnoreReadOnlyFields = true,
    IgnoreReadOnlyProperties = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Archive))]
public partial class WikiArchiveJsonSerializerContext : JsonSerializerContext { }
