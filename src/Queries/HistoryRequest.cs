using System.Text.Json.Serialization;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A JSON serializer context for <see cref="Queries.HistoryRequest"/>.
/// </summary>
[JsonSerializable(typeof(HistoryRequest))]
public partial class HistoryRequestContext : JsonSerializerContext { }

/// <summary>
/// A request record for a page of revision information for a wiki item.
/// </summary>
/// <param name="Title">
/// <para>
/// The title of the wiki page.
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
/// The namespace of the wiki page.
/// </para>
/// <para>
/// May be omitted, in which case <see cref="WikiOptions.DefaultNamespace"/> will be used.
/// </para>
/// </param>
/// <param name="PageNumber">The requested page number. The first page is 1.</param>
/// <param name="PageSize">The number of items to return per page.</param>
/// <param name="EditorId">
/// If provided, only revisions made by the editor with the given ID will be returned.
/// </param>
/// <param name="Start">
/// The number of ticks in the date/time of the first record to be retrieved.
/// </param>
/// <param name="End">
/// The number of ticks in the date/time of the last record to be retrieved.
/// </param>
public record HistoryRequest(
    string? Title = null,
    string? WikiNamespace = null,
    int PageNumber = 1,
    int PageSize = 50,
    string? EditorId = null,
    long? Start = null,
    long? End = null);