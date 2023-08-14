namespace Tavenem.Wiki.Queries;

/// <summary>
/// A request record for a page of wiki items which share given title parts.
/// </summary>
/// <param name="Title">
/// <para>
/// The title requested.
/// </para>
/// <para>
/// If <see cref="PageTitle.Domain"/> or <see cref="PageTitle.Namespace"/> are <see
/// langword="null"/> or empty, only pages with <see langword="null"/> or and empty value for that
/// part are returned (i.e. not pages with any value).
/// </para>
/// <para>
/// If <see cref="PageTitle.Title"/> is <see langword="null"/> or empty, any page which matches the
/// other title parts is returned (i.e. not just the home page). When it is non-empty, only pages
/// with that exact title are returned (i.e. it does not function in the same way as <see
/// cref="Filter"/>).
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
public record TitleRequest(
    PageTitle Title,
    int PageNumber = 0,
    int PageSize = 50,
    bool Descending = false,
    string? Sort = null,
    string? Filter = null);