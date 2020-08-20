using System;

namespace NeverFoundry.Wiki.MarkdownExtensions
{
    /// <summary>
    /// An object with functions for transforming article content.
    /// </summary>
    public interface IArticleProcessor
    {
        /// <summary>
        /// A function which accepts the article content and returns a (possibly) modified version.
        /// </summary>
        /// <remarks>
        /// Note that no processors are run if the initial content is empty.
        /// </remarks>
        public Func<string, string> Process { get; }
    }
}
