using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// A subcategory in a category.
/// </summary>
/// <param name="Title">The title of the represented <see cref="Category"/>.</param>
/// <param name="Count">The number of articles in the represented <see cref="Category"/>.</param>
public record Subcategory(PageTitle Title, long Count);
