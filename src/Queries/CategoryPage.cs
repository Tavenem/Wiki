namespace Tavenem.Wiki.Queries;

/// <summary>
/// A page in a category.
/// </summary>
/// <param name="Title">The title of the represented <see cref="Article"/>.</param>
/// <param name="WikiNamespace">The namespace of the represented <see cref="Article"/>.</param>
public record CategoryPage(string Title, string WikiNamespace);
