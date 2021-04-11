namespace Tavenem.Wiki.MarkdownExtensions.TableOfContents
{
    /// <summary>
    /// Options for the <see cref="TableOfContentsExtension"/>.
    /// </summary>
    public class TableOfContentsOptions
    {
        /// <summary>
        /// <para>
        /// The number of levels of hierarchy which will be shown, when not specified.
        /// </para>
        /// <para>
        /// Default is 3.
        /// </para>
        /// </summary>
        public int DefaultDepth { get; set; } = 3;

        /// <summary>
        /// <para>
        /// The first level of hierarchy (1-based) which will be shown, when not specified.
        /// </para>
        /// <para>
        /// Default is 1.
        /// </para>
        /// </summary>
        public int DefaultStartingLevel { get; set; } = 1;

        /// <summary>
        /// <para>
        /// The default title for any table of contents which does not specify one.
        /// </para>
        /// <para>
        /// Default is "Contents"
        /// </para>
        /// </summary>
        public string DefaultTitle { get; set; } = "Contents";

        /// <summary>
        /// <para>
        /// The minimum number of top-level headings (as defined by <see
        /// cref="DefaultStartingLevel"/>) required to display a default table of contents.
        /// </para>
        /// <para>
        /// An explicitly located table of contents is always displayed, regardless of this value or
        /// the number of headings.
        /// </para>
        /// </summary>
        public int MinimumTopLevel { get; set; } = 3;
    }
}
