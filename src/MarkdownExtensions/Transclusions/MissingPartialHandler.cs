using HandlebarsDotNet;

namespace Tavenem.Wiki.MarkdownExtensions.Transclusions;

internal class MissingPartialHandler : IMissingPartialTemplateHandler
{
    public void Handle(ICompiledHandlebarsConfiguration configuration, string partialName, in EncodedTextWriter textWriter)
        => textWriter.WriteSafeString($"{{{{#> {partialName}}}}}");
}