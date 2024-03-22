namespace Tavenem.Wiki.Queries;

/// <summary>
/// A request record for a page of wiki search results.
/// </summary>
/// <param name="Query">
/// <para>
/// The search query.
/// </para>
/// <para>
/// A query can consist of one or more words, punctuation, and other characters.
/// </para>
/// <para>
/// Searches are performed using semantic matching. This means that similar words and phrases will
/// be considered when ranking matches.
/// </para>
/// <para>
/// A word or phrase inside double quotes must be present on a page exactly as it appears in the
/// query for the page to be considered a match. Quoted terms can be combined with standard search
/// terms. The entire query (both quoted and unquoted portions) is considered when calculating the
/// best overall match.
/// </para>
/// <para>
/// If any term in the query is preceded by a hyphen (e.g. -exclude), pages which include that term
/// will not be considered a match.
/// </para>
/// </param>
/// <param name="Domain">
/// <para>
/// If <see langword="null"/> or empty, pages with any <see cref="PageTitle.Domain"/> are returned.
/// </para>
/// <para>
/// If empty (but not <see langword="null"/>), only pages with no <see cref="PageTitle.Domain"/> are
/// returned.
/// </para>
/// <para>
/// Otherwise only pages with a matching <see cref="PageTitle.Domain"/> are returned.
/// </para>
/// </param>
/// <param name="Namespace">
/// <para>
/// If <see langword="null"/> or empty, pages with any <see cref="PageTitle.Namespace"/> are
/// returned.
/// </para>
/// <para>
/// If empty (but not <see langword="null"/>), only pages with no <see cref="PageTitle.Namespace"/>
/// are returned.
/// </para>
/// <para>
/// Otherwise only pages with a matching <see cref="PageTitle.Namespace"/> are returned.
/// </para>
/// </param>
/// <param name="PageNumber">The requested page number. The first page is 1.</param>
/// <param name="PageSize">The number of items to return per page.</param>
/// <param name="Owner">
/// <para>
/// An optional owner or owners for whom to restrict results, or to exclude.
/// </para>
/// <para>
/// Each entry should be a user or group ID. May be a semicolon-delimited list, and any entry may be
/// prefixed with an exclamation mark to indicate that it should be excluded.
/// </para>
/// </param>
/// <param name="TitleMatchOnly">
/// Whether the query string should only consider matches in the title, rather than in the content
/// of the article as well.
/// </param>
/// <param name="Uploader">
/// <para>
/// An optional uploader or uploaders for whom to restrict results, or to exclude.
/// </para>
/// <para>
/// Each entry should be a user ID. May be a semicolon-delimited list, and any entry may be prefixed
/// with an exclamation mark to indicate that it should be excluded.
/// </para>
/// <para>
/// Only applies to <see cref="WikiFile"/>s.
/// </para>
/// </param>
public record SearchRequest(
    string Query,
    string? Domain = null,
    string? Namespace = null,
    int PageNumber = 1,
    int PageSize = 50,
    string? Owner = null,
    bool TitleMatchOnly = false,
    string? Uploader = null);