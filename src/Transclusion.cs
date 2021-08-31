using System.Text.Json.Serialization;

namespace Tavenem.Wiki;

/// <summary>
/// A transclusion of one page within another page.
/// </summary>
public class Transclusion : IEquatable<Transclusion>
{
    /// <summary>
    /// The title for the linked article.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// The namespace for the linked article.
    /// </summary>
    public string WikiNamespace { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Transclusion"/>.
    /// </summary>
    /// <param name="title">The title for the linked article.</param>
    /// <param name="wikiNamespace">The namespace for the linked article.</param>
    [JsonConstructor]
    public Transclusion(string title, string wikiNamespace)
    {
        Title = title;
        WikiNamespace = wikiNamespace;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" />
    /// parameter; otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(Transclusion? other) => !(other is null)
        && Title == other.Title
        && WikiNamespace == other.WikiNamespace;

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj) => obj is Transclusion other && Equals(other);

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => HashCode.Combine(Title, WikiNamespace);

    /// <summary>
    /// Determines whether this transclusion corresponds to the given article.
    /// </summary>
    /// <param name="item">The <see cref="Article"/> to match.</param>
    /// <returns><see langword="true"/> if this transclusion corresponds to the given article;
    /// otherwise <see langword="false"/>.</returns>
    public bool IsMatch(Article item) => string.CompareOrdinal(item.Title, Title) == 0
        && string.CompareOrdinal(item.WikiNamespace, WikiNamespace) == 0;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => string.IsNullOrEmpty(WikiNamespace)
        ? Title
        : $"{WikiNamespace}:{Title}";

    /// <summary>
    /// Determines equality.
    /// </summary>
    public static bool operator ==(Transclusion? left, Transclusion? right) => EqualityComparer<Transclusion?>.Default.Equals(left, right);

    /// <summary>
    /// Determines inequality.
    /// </summary>
    public static bool operator !=(Transclusion? left, Transclusion? right) => !(left == right);
}
