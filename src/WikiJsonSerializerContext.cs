﻿using System.Text.Json.Serialization;
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
