using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tavenem.Wiki;

/// <summary>
/// Provides a singleton <see cref="JsonSerializerOptions"/> instance for (de)serialization of <see
/// cref="Archive"/> objects.
/// </summary>
public static class WikiArchiveJsonSerializerOptions
{
    /// <summary>
    /// A singleton <see cref="JsonSerializerOptions"/> instance for (de)serialization of <see
    /// cref="Archive"/> objects.
    /// </summary>
    public static JsonSerializerOptions Instance { get; } = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = WikiArchiveJsonSerializerContext.Default,
    };
}
