using Ganss.XSS;
using Markdig;
using NeverFoundry.DataStorage;
using NeverFoundry.Wiki.MarkdownExtensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A static class containing various customization and configuration options for the wiki system.
    /// </summary>
    public static class WikiConfig
    {
        private const string CategoriesTitleDefault = "Categories";
        private static string _CategoriesTitle = CategoriesTitleDefault;
        /// <summary>
        /// <para>
        /// The name of the article on categories in the main wiki.
        /// </para>
        /// <para>
        /// Default is "Categories"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? CategoriesTitle
        {
            get => _CategoriesTitle;
            set
            {
                _CategoriesTitle = string.IsNullOrWhiteSpace(value)
                    ? CategoriesTitleDefault
                    : value;
            }
        }

        private const string CategoryNamespaceDefault = "Category";
        private static string _CategoryNamespace = CategoryNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the categories namespace.
        /// </para>
        /// <para>
        /// Default is "Category"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? CategoryNamespace
        {
            get => _CategoryNamespace;
            set
            {
                _CategoryNamespace = string.IsNullOrWhiteSpace(value)
                    ? CategoryNamespaceDefault
                    : value;
            }
        }

        private static IDataStore? _DataStore;
        /// <summary>
        /// <para>
        /// The <see cref="IDataStore"/> to be used by the wiki.
        /// </para>
        /// <para>
        /// If omitted, the static <see cref="DataStorage.DataStore.Instance"/> will be used.
        /// </para>
        /// </summary>
        public static IDataStore DataStore
        {
            get => _DataStore ??= DataStorage.DataStore.Instance;
            set => _DataStore = value;
        }

        private const string DefaultNamespaceDefault = "Wiki";
        private static string _DefaultNamespace = DefaultNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the default namespace.
        /// </para>
        /// <para>
        /// Default is "Wiki"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? DefaultNamespace
        {
            get => _DefaultNamespace;
            set
            {
                _DefaultNamespace = string.IsNullOrWhiteSpace(value)
                    ? DefaultNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// <para>
        /// The default number of levels of nesting shown in an article's table of contents.
        /// </para>
        /// <para>
        /// Can be overridden by specifying the level for a given article.
        /// </para>
        /// </summary>
        public static int DefaultTableOfContentsDepth { get; set; } = 3;

        private const string DefaultTableOfContentsTitleDefault = "Contents";
        private static string _DefaultTableOfContentsTitle = DefaultTableOfContentsTitleDefault;
        /// <summary>
        /// <para>
        /// The default title of tables of contents.
        /// </para>
        /// <para>
        /// Default is "Contents"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? DefaultTableOfContentsTitle
        {
            get => _DefaultTableOfContentsTitle;
            set
            {
                _DefaultTableOfContentsTitle = string.IsNullOrWhiteSpace(value)
                    ? DefaultTableOfContentsTitleDefault
                    : value;
            }
        }

        private const string FileNamespaceDefault = "File";
        private static string _FileNamespace = FileNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the file namespace.
        /// </para>
        /// <para>
        /// Default is "File"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? FileNamespace
        {
            get => _FileNamespace;
            set
            {
                _FileNamespace = string.IsNullOrWhiteSpace(value)
                    ? FileNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// <para>
        /// A string added to all wiki links, if non-empty.
        /// </para>
        /// <para>
        /// The string '{LINK}', if included, will be replaced by the full article title being
        /// linked.
        /// </para>
        /// <para>
        /// Default is <see langword="null"/>.
        /// </para>
        /// </summary>
        public static string? LinkTemplate { get; set; }

        private const string MainPageTitleDefault = "Main";
        private static string _MainPageTitle = MainPageTitleDefault;
        /// <summary>
        /// <para>
        /// The title of the main page (shown when no article title is given).
        /// </para>
        /// <para>
        /// Default is "Main"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? MainPageTitle
        {
            get => _MainPageTitle;
            set
            {
                _MainPageTitle = string.IsNullOrWhiteSpace(value)
                    ? MainPageTitleDefault
                    : value;
            }
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
        public static int MinimumTableOfContentsHeadings { get; set; } = 3;

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
        public static IList<IArticleProcessor>? Postprocessors { get; set; } = new List<IArticleProcessor>();

        private static List<string>? _ReservedNamespaces;
        /// <summary>
        /// <para>
        /// An optional collection of namespaces which may not be assigned to pages by users.
        /// </para>
        /// <para>
        /// The namespaces assigned to <see cref="CategoryNamespace"/>, <see cref="FileNamespace"/>,
        /// and <see cref="TalkNamespace"/> are included automatically.
        /// </para>
        /// </summary>
        public static IEnumerable<string> ReservedNamespaces => (_ReservedNamespaces ?? Enumerable.Empty<string>())
            .Concat(new[] { CategoryNamespace, FileNamespace, TalkNamespace });

        private const string SiteNameDefault = "A NeverFoundry Wiki Sample";
        private static string _SiteName = SiteNameDefault;
        /// <summary>
        /// <para>
        /// The name of this wiki.
        /// </para>
        /// <para>
        /// Default is "A NeverFoundry Wiki Sample"; unlike most defaults this is clearly not
        /// suitable for production, and should always be replaced.
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? SiteName
        {
            get => _SiteName;
            set
            {
                _SiteName = string.IsNullOrWhiteSpace(value)
                    ? SiteNameDefault
                    : value;
            }
        }

        private const string ServerUrlDefault = "http://localhost:5000/";
        private static string _ServerUrl = ServerUrlDefault;
        /// <summary>
        /// <para>
        /// The primary URL of this wiki.
        /// </para>
        /// <para>
        /// Default is "http://localhost:5000/"; unlike most defaults this is clearly not
        /// suitable for production, and should always be replaced.
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? ServerUrl
        {
            get => _ServerUrl;
            set
            {
                _ServerUrl = string.IsNullOrWhiteSpace(value)
                    ? ServerUrlDefault
                    : value;
            }
        }

        private const string TalkNamespaceDefault = "Talk";
        private static string _TalkNamespace = TalkNamespaceDefault;
        /// <summary>
        /// <para>
        /// The name of the talk pseudo-namespace.
        /// </para>
        /// <para>
        /// Default is "Talk"
        /// </para>
        /// <para>
        /// May not be <see langword="null"/> or empty <see cref="string"/>. Setting to an empty or
        /// all whitespace value resets it to the default.
        /// </para>
        /// </summary>
        [NotNull]
        public static string? TalkNamespace
        {
            get => _TalkNamespace;
            set
            {
                _TalkNamespace = string.IsNullOrWhiteSpace(value)
                    ? TalkNamespaceDefault
                    : value;
            }
        }

        /// <summary>
        /// <para>
        /// The name of the transclusion namespace.
        /// </para>
        /// <para>
        /// Default is "Transclusion"
        /// </para>
        /// <para>
        /// May be assigned a <see langword="null"/> or empty <see cref="string"/> value.
        /// </para>
        /// </summary>
        public static string? TransclusionNamespace { get; set; } = "Transclusion";

        private static IHtmlSanitizer? _HtmlSanitizer;
        internal static IHtmlSanitizer HtmlSanitizer
        {
            get
            {
                if (_HtmlSanitizer is null)
                {
                    _HtmlSanitizer = new HtmlSanitizer();
                    _HtmlSanitizer.AllowedAttributes.Add("class");
                    _HtmlSanitizer.AllowedAttributes.Add("role");

                    _HtmlSanitizer.RemovingAttribute += (_, e) =>
                    {
                        if (e.Tag.TagName == "A" && LinkTemplate?.Contains(e.Attribute.Name) == true)
                        {
                            e.Cancel = true;
                        }
                        e.Cancel |= e.Attribute.Name.StartsWith("data-");
                    };
                }
                return _HtmlSanitizer;
            }
        }

        private static IHtmlSanitizer? _HtmlSanitizerFull;
        internal static IHtmlSanitizer HtmlSanitizerFull
            => _HtmlSanitizerFull ??= new HtmlSanitizer(new string[0], new string[0], new string[0], new string[0], new string[0])
            {
                KeepChildNodes = true
            };

        private static MarkdownPipeline? _MarkdownPipeline;
        internal static MarkdownPipeline MarkdownPipeline =>
            _MarkdownPipeline ??= new MarkdownPipelineBuilder()
            .UseWikiLinks()
            .UseAbbreviations()
            .UseTableOfContents(new MarkdownExtensions.TableOfContents.TableOfContentsOptions
            {
                DefaultDepth = DefaultTableOfContentsDepth,
                DefaultStartingLevel = 1,
                MinimumTopLevel = MinimumTableOfContentsHeadings,
                DefaultTitle = DefaultTableOfContentsTitle,
            })
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseAutoLinks()
            .UseGenericAttributes()
            .UseSmartyPants()
            .Build();

        private static MarkdownPipeline? _MarkdownPipelinePlainText;
        internal static MarkdownPipeline MarkdownPipelinePlainText =>
            _MarkdownPipelinePlainText ??= new MarkdownPipelineBuilder()
            .UseWikiLinks()
            .UseAbbreviations()
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseGenericAttributes()
            .UseSmartyPants()
            .Build();

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
        /// <param name="namespaces"></param>
        public static void AddReservedNamespace(params string[] namespaces)
        {
            for (var i = 0; i < namespaces.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(namespaces[i]))
                {
                    (_ReservedNamespaces ??= new List<string>()).Add(namespaces[i]);
                }
            }
        }
    }
}
