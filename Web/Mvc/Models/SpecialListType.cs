namespace NeverFoundry.Wiki.Mvc.Models
{
    /// <summary>
    /// A type of wiki page list.
    /// </summary>
    public enum SpecialListType
    {
        /// <summary>
        /// All category pages.
        /// </summary>
        All_Categories,

        /// <summary>
        /// All files.
        /// </summary>
        All_Files,

        /// <summary>
        /// All pages.
        /// </summary>
        All_Pages,

        /// <summary>
        /// All redirected pages.
        /// </summary>
        All_Redirects,

        /// <summary>
        /// All redirects which lead to nonexistent pages.
        /// </summary>
        Broken_Redirects,

        /// <summary>
        /// All redirects which lead to other redirects.
        /// </summary>
        Double_Redirects,

        /// <summary>
        /// All pages referred to in a link which do not exist.
        /// </summary>
        Missing_Pages,

        /// <summary>
        /// All articles with no assigned category.
        /// </summary>
        Uncategorized_Articles,

        /// <summary>
        /// All categories with no assigned category.
        /// </summary>
        Uncategorized_Categories,

        /// <summary>
        /// All files with no assigned category.
        /// </summary>
        Uncategorized_Files,

        /// <summary>
        /// All categories with no pages assigned to them.
        /// </summary>
        Unused_Categories,

        /// <summary>
        /// The list of pages which link to a page.
        /// </summary>
        What_Links_Here
    }
}
