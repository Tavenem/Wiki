using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A request record for a page of wiki pages which link to a given wiki page.
/// </summary>
/// <param name="Title">The title of a page.</param>
/// <param name="PageNumber">The requested page number. The first page is 1.</param>
/// <param name="PageSize">The number of items to return per page.</param>
/// <param name="Descending">Whether the list should be sorted in descending order.</param>
/// <param name="Sort">
/// <para>
/// A sort criteria.
/// </para>
/// <para>
/// Accepts "name" to sort by title, or "timestamp" to sort by last revision.
/// </para>
/// <para>
/// If <see langword="null"/> an unspecified default sort order is used.
/// </para>
/// </param>
/// <param name="Filter">
/// If present only items with titles containing this value will be returned.
/// </param>
public record WhatLinksHereRequest(
    PageTitle Title,
    int PageNumber = 0,
    int PageSize = 50,
    bool Descending = false,
    string? Sort = null,
    string? Filter = null);