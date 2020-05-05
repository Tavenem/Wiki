namespace NeverFoundry.Wiki.MarkdownExtensions.Transclusions
{
    /// <summary>
    /// A transclusion.
    /// </summary>
    internal class Transclusion
    {
        /// <summary>
        /// The transcluded content (<see langword="null"/> until fully rendered).
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// The ending index (exclusive) within the overall string.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// The starting index within the overall string.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Transclusion"/>.
        /// </summary>
        /// <param name="start">
        /// The starting index within the overall string.
        /// </param>
        /// <param name="end">
        /// The ending index (exclusive) within the overall string.
        /// </param>
        public Transclusion(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
