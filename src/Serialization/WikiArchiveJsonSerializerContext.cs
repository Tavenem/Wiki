using System.Text.Json.Serialization;

namespace Tavenem.Wiki;

/// <summary>
/// A source generated serializer context for <see cref="Wiki.Archive"/> which minimizes the size of the payload.
/// </summary>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    IgnoreReadOnlyFields = true,
    IgnoreReadOnlyProperties = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Archive))]
public partial class WikiArchiveJsonSerializerContext : JsonSerializerContext;
