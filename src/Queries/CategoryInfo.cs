namespace Tavenem.Wiki.Queries;

/// <summary>
/// Information about a category.
/// </summary>
/// <param name="Item">
/// <para>
/// The <see cref="Category"/>.
/// </para>
/// <para>
/// This may be set to <see langword="null"/> even if the item exists, when <paramref
/// name="Permission"/> does not include <see cref="WikiPermission.Read"/>.
/// </para>
/// </param>
/// <param name="Permission">
/// The permission(s) the requesting user has for this item.
/// </param>
/// <param name="Articles">The articles contained directly in this category.</param>
/// <param name="Files">The files contained directly in this category.</param>
/// <param name="Subcategories">The child categories contained directly in this one.</param>
public record CategoryInfo(
    Category? Item,
    WikiPermission Permission,
    Dictionary<string, List<CategoryPage>>? Articles,
    Dictionary<string, List<CategoryFile>>? Files,
    Dictionary<string, List<Subcategory>>? Subcategories);
