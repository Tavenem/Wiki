using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A request record for a page of revision information for a wiki item.
/// </summary>
/// <param name="Title">The title of the wiki page.</param>
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
    PageTitle Title,
    int PageNumber = 1,
    int PageSize = 50,
    string? EditorId = null,
    long? Start = null,
    long? End = null);