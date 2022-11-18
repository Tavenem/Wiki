namespace Tavenem.Wiki;

/// <summary>
/// A type of special wiki page list.
/// </summary>
public enum SpecialListType
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// All category pages.
    /// </summary>
    All_Categories = 1,

    /// <summary>
    /// All files.
    /// </summary>
    All_Files = 2,

    /// <summary>
    /// All articles.
    /// </summary>
    All_Articles = 3,

    /// <summary>
    /// All redirected pages.
    /// </summary>
    All_Redirects = 4,

    /// <summary>
    /// All redirects which lead to nonexistent pages.
    /// </summary>
    Broken_Redirects = 5,

    /// <summary>
    /// All redirects which lead to other redirects.
    /// </summary>
    Double_Redirects = 6,

    /// <summary>
    /// All pages referred to in a link which do not exist.
    /// </summary>
    Missing_Pages = 7,

    /// <summary>
    /// All articles with no assigned category.
    /// </summary>
    Uncategorized_Articles = 8,

    /// <summary>
    /// All categories with no assigned category.
    /// </summary>
    Uncategorized_Categories = 9,

    /// <summary>
    /// All files with no assigned category.
    /// </summary>
    Uncategorized_Files = 10,

    /// <summary>
    /// All categories with no pages assigned to them.
    /// </summary>
    Unused_Categories = 11,

    /// <summary>
    /// The list of pages which link to a page.
    /// </summary>
    What_Links_Here = 12
}
