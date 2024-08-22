namespace Tavenem.Wiki.MarkdownExtensions.TableOfContents;

/// <summary>
/// Options for the <see cref="TableOfContentsExtension"/>.
/// </summary>
/// <param name="DefaultDepth">
/// <para>
/// The number of levels of hierarchy which will be shown, when not specified.
/// </para>
/// <para>
/// Default is 3.
/// </para>
/// </param>
/// <param name="DefaultStartingLevel">
/// <para>
/// The first level of hierarchy (1-based) which will be shown, when not specified.
/// </para>
/// <para>
/// Default is 1.
/// </para>
/// </param>
/// <param name="DefaultTitle">
/// <para>
/// The default title for any table of contents which does not specify one.
/// </para>
/// <para>
/// Default is "Contents"
/// </para>
/// </param>
/// <param name="MinimumTopLevel">
/// <para>
/// The minimum number of top-level headings (as defined by <see
/// cref="DefaultStartingLevel"/>) required to display a default table of contents.
/// </para>
/// <para>
/// An explicitly located table of contents is always displayed, regardless of this value or
/// the number of headings.
/// </para>
/// </param>
public readonly record struct TableOfContentsOptions(
    int DefaultDepth = 3,
    int DefaultStartingLevel = 1,
    string DefaultTitle = "Contents",
    int MinimumTopLevel = 3);
