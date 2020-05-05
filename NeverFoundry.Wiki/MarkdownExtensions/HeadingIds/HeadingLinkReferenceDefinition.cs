using Markdig.Syntax;

namespace NeverFoundry.Wiki.MarkdownExtensions.HeadingIds
{
    internal class HeadingLinkReferenceDefinition : LinkReferenceDefinition
    {
        public HeadingBlock? HeadingBlock { get; set; }
    }
}
