using System.Text.Json.Serialization;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A JSON serializer context for <see cref="Queries.WhatLinksHereRequest"/>.
/// </summary>
[JsonSerializable(typeof(WhatLinksHereRequest))]
public partial class WhatLinksHereRequestContext : JsonSerializerContext { }

/// <summary>
/// A request record for a page of wiki items which link to a given wiki item.
/// </summary>
/// <param name="Title">
/// <para>
/// The title of a wiki page.
/// </para>
/// <para>
/// May be omitted if the <paramref name="WikiNamespace"/> is also omitted, or equal to the <see
/// cref="WikiOptions.DefaultNamespace"/>, in which case <see cref="WikiOptions.MainPageTitle"/>
/// will be used.
/// </para>
/// <para>
/// If this parameter is <see langword="null"/> or empty, but <paramref name="WikiNamespace"/> is
/// <em>not</em> either omitted or equal to the <see cref="WikiOptions.DefaultNamespace"/>, the
/// result will always be <see langword="null"/>.
/// </para>
/// </param>
/// <param name="WikiNamespace">
/// <para>
/// The namespace of a wiki page.
/// </para>
/// <para>
/// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
/// </para>
/// </param>
/// <param name="PageNumber">The requested page number. The first page is 1.</param>
/// <param name="PageSize">The number of items to return per page.</param>
/// <param name="Descending">Whether the list should be sorted in descending order.</param>
/// <param name="Sort">
/// <para>
/// A sort criteria.
/// </para>
/// <para>
/// Accepts "timestamp" to sort by last revision.
/// </para>
/// <para>
/// If <see langword="null"/> an unspecified default sort order is used.
/// </para>
/// </param>
/// <param name="Filter">
/// If present only items with titles containing this value will be returned.
/// </param>
public record WhatLinksHereRequest(
    string? Title = null,
    string? WikiNamespace = null,
    int PageNumber = 0,
    int PageSize = 50,
    bool Descending = false,
    string? Sort = null,
    string? Filter = null);