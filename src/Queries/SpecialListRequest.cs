using System.Text.Json.Serialization;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A JSON serializer context for <see cref="Queries.SpecialListRequest"/>.
/// </summary>
[JsonSerializable(typeof(SpecialListRequest))]
public partial class SpecialListRequestContext : JsonSerializerContext { }

/// <summary>
/// A request record for a page of wiki items which fit one of the special conditions in the <see
/// cref="SpecialListType"/> enumeration.
/// </summary>
/// <param name="Type">The type of special list requested.</param>
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
public record SpecialListRequest(
    SpecialListType Type,
    int PageNumber = 0,
    int PageSize = 50,
    bool Descending = false,
    string? Sort = null,
    string? Filter = null);