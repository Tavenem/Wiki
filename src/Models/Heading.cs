namespace Tavenem.Wiki;

/// <summary>
/// A heading in a wiki item.
/// </summary>
public class Heading
{
    /// <summary>
    /// The HTML <c>id</c> attribute of this heading.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The level of this heading (1–6).
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// The offset level of this heading, relative to its parent table of contents.
    /// </summary>
    public int OffsetLevel { get; set; }

    /// <summary>
    /// The text of this heading.
    /// </summary>
    public string? Text { get; set; }
}
