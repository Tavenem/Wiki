using Tavenem.Wiki.Models;

namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a category.
/// </summary>
/// <param name="Category">
/// <para>
/// The <see cref="Wiki.Category"/>.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the item exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for this item.
/// </param>
/// <param name="Pages">The pages contained directly in this category.</param>
/// <param name="Files">The files contained directly in this category.</param>
/// <param name="Subcategories">The child categories contained directly in this one.</param>
public record CategoryInfo(
    Category? Category,
    WikiPermission Permission,
    Dictionary<string, List<PageTitle>>? Pages,
    Dictionary<string, List<CategoryFile>>? Files,
    Dictionary<string, List<Subcategory>>? Subcategories);
