using Tavenem.Wiki.Queries;

namespace Tavenem.Wiki;

/// <summary>
/// The result of a wiki search.
/// </summary>
/// <param name="Similarity">The similarity of this result to the search query.</param>
/// <param name="Info">The <see cref="PageTitle"/> of the page containing the match.</param>
/// <param name="Excerpt">An optional excerpt indicating the search match.</param>
/// <param name="UniqueIndex">A unique value used for deterministic comparison.</param>
internal readonly record struct SearchScore(float Similarity, PageSearchInfo Info, string? Excerpt, long UniqueIndex)
{
    internal static readonly IComparer<SearchScore> Comparer = Comparer<SearchScore>.Create((SearchScore a, SearchScore b) =>
    {
        var num = b.Similarity.CompareTo(a.Similarity);
        return num == 0
            ? a.UniqueIndex.CompareTo(b.UniqueIndex)
            : num;
    });
}
