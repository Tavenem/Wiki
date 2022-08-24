﻿using System.Text.Json.Serialization;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A JSON serializer context for <see cref="Queries.LinkInfo"/>.
/// </summary>
[JsonSerializable(typeof(LinkInfo))]
public partial class LinkInfoContext : JsonSerializerContext { }

/// <summary>
/// Information about a link to or from a wiki item.
/// </summary>
/// <param name="Title">The title of the item.</param>
/// <param name="WikiNamespace">The namespace of the item.</param>
/// <param name="ChildCount">
/// The number of child items if the link is a <see cref="Category"/>.
/// </param>
/// <param name="FileSize">
/// The file size if the link is a <see cref="WikiFile"/>.
/// </param>
/// <param name="FileType">
/// The file type if the link is a <see cref="WikiFile"/>.
/// </param>
public record LinkInfo(
    string Title,
    string WikiNamespace,
    int ChildCount,
    int FileSize,
    string? FileType);
