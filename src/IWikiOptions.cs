using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavenem.Wiki.MarkdownExtensions;

namespace Tavenem.Wiki
{
#pragma warning disable RCS1060 // Declare each type in separate file.
    /// <summary>
    /// The delegate signature used by <see cref="IWikiOptions.OnCreated"/>.
    /// </summary>
    /// <param name="article">The new article.</param>
    /// <param name="editor">The ID of the editor who created the article.</param>
    public delegate ValueTask OnCreatedFunc(Article article, string editor);

    /// <summary>
    /// The delegate signature used by <see cref="IWikiOptions.OnDeleted"/>.
    /// </summary>
    /// <param name="article">The deleted article.</param>
    /// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
    /// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
    public delegate ValueTask OnDeletedFunc(Article article, string? oldOwner, string? newOwner);

    /// <summary>
    /// The delegate signature used by <see cref="IWikiOptions.OnEdited"/>.
    /// </summary>
    /// <param name="article">The edited article.</param>
    /// <param name="revision">The revision applied.</param>
    /// <param name="oldOwner">The original <see cref="Article.Owner"/>.</param>
    /// <param name="newOwner">The new <see cref="Article.Owner"/>.</param>
    public delegate ValueTask OnEditedFunc(Article article, Revision revision, string? oldOwner, string? newOwner);

    /// <summary>
    /// Various customization and configuration options for the wiki system.
    /// </summary>
    public interface IWikiOptions
    {
        /// <summary>
        /// <para>
        /// The name of the article on categories in the main wiki.
        /// </para>
        /// <para>
        /// If omitted "Categories" will be used.
        /// </para>
        /// </summary>
        string CategoriesTitle { get; }

        /// <summary>
        /// <para>
        /// The name of the categories namespace.
        /// </para>
        /// <para>
        /// If omitted "Category" will be used.
        /// </para>
        /// </summary>
        string CategoryNamespace { get; }

        /// <summary>
        /// <para>
        /// The name of the default namespace.
        /// </para>
        /// <para>
        /// If omitted "Wiki" will be used.
        /// </para>
        /// </summary>
        string DefaultNamespace { get; }

        /// <summary>
        /// <para>
        /// The default number of levels of nesting shown in an article's table of contents.
        /// </para>
        /// <para>
        /// Can be overridden by specifying the level for a given article.
        /// </para>
        /// </summary>
        int DefaultTableOfContentsDepth { get; }

        /// <summary>
        /// <para>
        /// The default title of tables of contents.
        /// </para>
        /// <para>
        /// If omitted "Contents" will be used.
        /// </para>
        /// </summary>
        string DefaultTableOfContentsTitle { get; }

        /// <summary>
        /// <para>
        /// The name of the file namespace.
        /// </para>
        /// <para>
        /// If omitted "File" will be used.
        /// </para>
        /// </summary>
        string FileNamespace { get; }

        /// <summary>
        /// <para>
        /// A string added to all wiki links, if non-empty.
        /// </para>
        /// <para>
        /// The string '{LINK}', if included, will be replaced by the full article title being
        /// linked.
        /// </para>
        /// </summary>
        string? LinkTemplate { get; }

        /// <summary>
        /// <para>
        /// The title of the main page (shown when no article title is given).
        /// </para>
        /// <para>
        /// If omitted "Main" will be used.
        /// </para>
        /// </summary>
        string MainPageTitle { get; }

        /// <summary>
        /// <para>
        /// The minimum number of headings required in an article to display a table of contents by
        /// default.
        /// </para>
        /// <para>
        /// Can be overridden by specifying the location of a table of contents explicitly for a given article.
        /// </para>
        /// </summary>
        int MinimumTableOfContentsHeadings { get; }

        /// <summary>
        /// <para>
        /// An optional callback invoked when a new <see cref="Article"/> (including <see
        /// cref="Category"/> and <see cref="WikiFile"/>) is created.
        /// </para>
        /// <para>
        /// Receives the new <see cref="Article"/> as a parameter.
        /// </para>
        /// </summary>
        OnCreatedFunc? OnCreated { get; }

        /// <summary>
        /// <para>
        /// An optional callback invoked when a new <see cref="Article"/> (including <see
        /// cref="Category"/> and <see cref="WikiFile"/>) is deleted.
        /// </para>
        /// <para>
        /// Receives the deleted <see cref="Article"/>, the original <see cref="Article.Owner"/>,
        /// and the new <see cref="Article.Owner"/> as parameters.
        /// </para>
        /// </summary>
        OnDeletedFunc? OnDeleted { get; }

        /// <summary>
        /// <para>
        /// An optional callback invoked when a new <see cref="Article"/> (including <see
        /// cref="Category"/> and <see cref="WikiFile"/>) is edited (not including deletion if <see
        /// cref="OnDeleted"/> is provided).
        /// </para>
        /// <para>
        /// Receives the edited <see cref="Article"/>, the <see cref="Revision"/> which was applied,
        /// the original <see cref="Article.Owner"/>, and the new <see cref="Article.Owner"/> as
        /// parameters.
        /// </para>
        /// </summary>
        OnEditedFunc? OnEdited { get; }

        /// <summary>
        /// A collection of preprocessors which transform the HTML of an article
        /// <i>after</i> it is parsed from markdown but <i>before</i> it is sanitized.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Processors are run in the order they are added to the collection.
        /// </para>
        /// <para>
        /// Note that no processors are run if the initial content is empty.
        /// </para>
        /// </remarks>
        IList<IArticleProcessor>? Postprocessors { get; }

        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by users.
        /// </para>
        /// <para>
        /// The namespaces assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/>
        /// are included automatically.
        /// </para>
        /// </summary>
        IEnumerable<string> ReservedNamespaces { get; }

        /// <summary>
        /// <para>
        /// The name of the script namespace.
        /// </para>
        /// <para>
        /// If omitted "Script" will be used.
        /// </para>
        /// </summary>
        string ScriptNamespace { get; }

        /// <summary>
        /// <para>
        /// The name of the wiki. Displayed as a subheading below each article title.
        /// </para>
        /// <para>
        /// If omitted "a NeverFoundry wiki" will be used.
        /// </para>
        /// </summary>
        string SiteName { get; }

        /// <summary>
        /// <para>
        /// The name of the talk pseudo-namespace.
        /// </para>
        /// <para>
        /// If omitted "Talk" will be used.
        /// </para>
        /// </summary>
        string TalkNamespace { get; }

        /// <summary>
        /// <para>
        /// The name of the transclusion namespace.
        /// </para>
        /// <para>
        /// If omitted "Transclusion" will be used.
        /// </para>
        /// </summary>
        string TransclusionNamespace { get; }

        /// <summary>
        /// <para>
        /// The prefix added before wiki links (to distinguish them from other pages on the same
        /// server).
        /// </para>
        /// <para>
        /// If omitted "Wiki" will be used.
        /// </para>
        /// </summary>
        string WikiLinkPrefix { get; }
    }
#pragma warning restore RCS1060 // Declare each type in separate file.
}