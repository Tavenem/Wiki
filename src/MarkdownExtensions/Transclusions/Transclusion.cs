namespace Tavenem.Wiki.MarkdownExtensions.Transclusions;

/// <summary>
/// A transclusion.
/// </summary>
internal class Transclusion
{
    /// <summary>
    /// If this is the opening of a transclusion block, refers to the end.
    /// </summary>
    public Transclusion? BlockEnd { get; set; }

    /// <summary>
    /// If this is the closing of a transclusion block, refers to the start.
    /// </summary>
    public Transclusion? BlockStart { get; set; }

    /// <summary>
    /// The ending index (exclusive) of the content (inside the transclusion markup characters)
    /// within the overall string.
    /// </summary>
    public int ContentEnd { get; }

    /// <summary>
    /// The starting index of the content (inside the transclusion markup characters) within the
    /// overall string.
    /// </summary>
    public int ContentStart { get; }

    /// <summary>
    /// The ending index (exclusive) within the overall string.
    /// </summary>
    public int End { get; }

    /// <summary>
    /// Whether this is the closing of a transclusion block.
    /// </summary>
    public bool IsBlockEnd { get; }

    /// <summary>
    /// Whether this is the opening of a transclusion block.
    /// </summary>
    public bool IsBlockStart { get; }

    /// <summary>
    /// The transcluded page.
    /// </summary>
    public Page? Page { get; set; }

    /// <summary>
    /// The range in which the transclusion parameters may be found.
    /// </summary>
    public Range? Parameters { get; set; }

    /// <summary>
    /// The starting index within the overall string.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// The title of the page to be transcluded.
    /// </summary>
    public PageTitle? Title { get; set; }

    /// <summary>
    /// Whether this transclusion has been successfully replaced with its content.
    /// </summary>
    public bool WasTranscluded { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="Transclusion"/>.
    /// </summary>
    /// <param name="start">The starting index within the overall string.</param>
    /// <param name="contentStart">
    /// The starting index of the content (inside the transclusion markup characters) within the
    /// overall string.
    /// </param>
    /// <param name="contentEnd">
    /// The ending index (exclusive) of the content (inside the transclusion markup characters)
    /// within the overall string.
    /// </param>
    /// <param name="end">The ending index (exclusive) within the overall string.</param>
    /// <param name="isBlockEnd">Whether this is the closing of a transclusion block.</param>
    /// <param name="isBlockStart">Whether this is the opening of a transclusion block.</param>
    public Transclusion(
        int start,
        int contentStart,
        int contentEnd,
        int end,
        bool isBlockEnd = false,
        bool isBlockStart = false)
    {
        Start = start;
        ContentStart = contentStart;
        ContentEnd = contentEnd;
        End = end;
        IsBlockEnd = isBlockEnd;
        IsBlockStart = isBlockStart;
    }
}
