using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Tavenem.Wiki.MarkdownExtensions;

namespace Tavenem.Wiki
{
    /// <summary>
    /// Various customization and configuration options for the wiki system.
    /// </summary>
    public class WikiOptions : IWikiOptions
    {
        private const string CategoriesTitleDefault = "Categories";
        private string _categoriesTitle = CategoriesTitleDefault;
        /// <summary>
        /// <para>
        /// The name of the article on categories in the main wiki.
        /// </para>
        /// <para>
        /// If omitted "Categories" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? CategoriesTitle
        {
            get => _categoriesTitle;
            set => _categoriesTitle = string.IsNullOrWhiteSpace(value)
                ? CategoriesTitleDefault
                : value;
        }

        private const string CategoryNamespaceDefault = "Category";
        private string _categoryNamespace = CategoryNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the categories namespace.
        /// </para>
        /// <para>
        /// If omitted "Category" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? CategoryNamespace
        {
            get => _categoryNamespace;
            set => _categoryNamespace = string.IsNullOrWhiteSpace(value)
                ? CategoryNamespaceDefault
                : value;
        }

        private const string DefaultNamespaceDefault = "Wiki";
        private string _defaultNamespace = DefaultNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the default namespace.
        /// </para>
        /// <para>
        /// If omitted "Wiki" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? DefaultNamespace
        {
            get => _defaultNamespace;
            set => _defaultNamespace = string.IsNullOrWhiteSpace(value)
                ? DefaultNamespaceDefault
                : value;
        }

        /// <summary>
        /// <para>
        /// The default number of levels of nesting shown in an article's table of contents.
        /// </para>
        /// <para>
        /// Can be overridden by specifying the level for a given article.
        /// </para>
        /// </summary>
        public int DefaultTableOfContentsDepth { get; set; } = 3;

        private const string DefaultTableOfContentsTitleDefault = "Contents";
        private string _defaultTableOfContentsTitle = DefaultTableOfContentsTitleDefault;
        /// <summary>
        /// <para>
        /// The default title of tables of contents.
        /// </para>
        /// <para>
        /// If omitted "Contents" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? DefaultTableOfContentsTitle
        {
            get => _defaultTableOfContentsTitle;
            set => _defaultTableOfContentsTitle = string.IsNullOrWhiteSpace(value)
                ? DefaultTableOfContentsTitleDefault
                : value;
        }

        private const string FileNamespaceDefault = "File";
        private string _fileNamespace = FileNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the file namespace.
        /// </para>
        /// <para>
        /// If omitted "File" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? FileNamespace
        {
            get => _fileNamespace;
            set => _fileNamespace = string.IsNullOrWhiteSpace(value)
                ? FileNamespaceDefault
                : value;
        }

        /// <summary>
        /// <para>
        /// A string added to all wiki links, if non-empty.
        /// </para>
        /// <para>
        /// The string '{LINK}', if included, will be replaced by the full article title being
        /// linked.
        /// </para>
        /// </summary>
        public string? LinkTemplate { get; set; }

        private const string MainPageTitleDefault = "Main";
        private string _mainPageTitle = MainPageTitleDefault;
        /// <summary>
        /// <para>
        /// The title of the main page (shown when no article title is given).
        /// </para>
        /// <para>
        /// If omitted "Main" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? MainPageTitle
        {
            get => _mainPageTitle;
            set => _mainPageTitle = string.IsNullOrWhiteSpace(value)
                ? MainPageTitleDefault
                : value;
        }

        /// <summary>
        /// <para>
        /// The minimum number of headings required in an article to display a table of contents by
        /// default.
        /// </para>
        /// <para>
        /// Can be overridden by specifying the location of a table of contents explicitly for a given article.
        /// </para>
        /// </summary>
        public int MinimumTableOfContentsHeadings { get; set; } = 3;

        /// <summary>
        /// <para>
        /// An optional callback invoked when a new <see cref="Article"/> (including <see
        /// cref="Category"/> and <see cref="WikiFile"/>) is created.
        /// </para>
        /// <para>
        /// Receives the new <see cref="Article"/> as a parameter.
        /// </para>
        /// </summary>
        public OnCreatedFunc? OnCreated { get; }

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
        public OnDeletedFunc? OnDeleted { get; }

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
        public OnEditedFunc? OnEdited { get; }

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
        public IList<IArticleProcessor>? Postprocessors { get; set; } = new List<IArticleProcessor>();

        private List<string>? _reservedNamespaces;
        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by users.
        /// </para>
        /// <para>
        /// The namespaces assigned to <see cref="FileNamespace"/> and <see cref="TalkNamespace"/>
        /// are included automatically.
        /// </para>
        /// </summary>
        public IEnumerable<string> ReservedNamespaces => (_reservedNamespaces ?? Enumerable.Empty<string>())
            .Concat(new[] { FileNamespace, TalkNamespace });

        private const string ScriptNamespaceDefault = "Script";
        private string _scriptNamespace = ScriptNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the script namespace.
        /// </para>
        /// <para>
        /// If omitted "Script" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string ScriptNamespace
        {
            get => _scriptNamespace;
            set => _scriptNamespace = string.IsNullOrWhiteSpace(value)
                ? ScriptNamespaceDefault
                : value;
        }

        private const string SiteNameDefault = "a NeverFoundry wiki";
        private string _siteName = SiteNameDefault;
        /// <summary>
        /// <para>
        /// The name of the wiki. Displayed as a subheading below each article title.
        /// </para>
        /// <para>
        /// If omitted "a NeverFoundry wiki" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? SiteName
        {
            get => _siteName;
            set => _siteName = string.IsNullOrWhiteSpace(value)
                ? SiteNameDefault
                : value;
        }

        private const string TalkNamespaceDefault = "Talk";
        private string _talkNamespace = TalkNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the talk pseudo-namespace.
        /// </para>
        /// <para>
        /// If omitted "Talk" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? TalkNamespace
        {
            get => _talkNamespace;
            set => _talkNamespace = string.IsNullOrWhiteSpace(value)
                ? TalkNamespaceDefault
                : value;
        }

        private const string TransclusionNamespaceDefault = "Transclusion";
        private string _transclusionNamespace = TransclusionNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the transclusion namespace.
        /// </para>
        /// <para>
        /// If omitted "Transclusion" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? TransclusionNamespace
        {
            get => _transclusionNamespace;
            set => _transclusionNamespace = string.IsNullOrWhiteSpace(value)
                ? TransclusionNamespaceDefault
                : value;
        }

        private const string WikiLinkPrefixDefault = "Wiki";
        private string _wikiLinkPrefix = WikiLinkPrefixDefault;
        /// <summary>
        /// <para>
        /// The prefix added before wiki links (to distinguish them from other pages on the same
        /// server).
        /// </para>
        /// <para>
        /// If omitted "Wiki" will be used.
        /// </para>
        /// </summary>
        [NotNull]
        public string? WikiLinkPrefix
        {
            get => _wikiLinkPrefix;
            set => _wikiLinkPrefix = string.IsNullOrWhiteSpace(value)
                ? WikiLinkPrefixDefault
                : value;
        }

        /// <summary>
        /// <para>
        /// Adds one or more namespaces to the list of reserved names which may not be assigned to
        /// pages by users.
        /// </para>
        /// <para>
        /// The namespaces assigned to <see cref="CategoryNamespace"/>, <see cref="FileNamespace"/>,
        /// and <see cref="TalkNamespace"/> are included automatically.
        /// </para>
        /// </summary>
        /// <param name="namespaces">
        /// The namespace(s) to add.
        /// </param>
        /// <returns>This instance.</returns>
        public WikiOptions AddReservedNamespace(params string[] namespaces)
        {
            for (var i = 0; i < namespaces.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(namespaces[i]))
                {
                    (_reservedNamespaces ??= new List<string>()).Add(namespaces[i]);
                }
            }
            return this;
        }
    }
}
