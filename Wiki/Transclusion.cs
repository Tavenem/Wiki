using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// A transclusion of one page within another page.
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject]
    public class Transclusion : ISerializable, IEquatable<Transclusion>
    {
        /// <summary>
        /// Gets the full title of this transclusion (including namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public virtual string FullTitle => string.CompareOrdinal(WikiNamespace, WikiConfig.DefaultNamespace) == 0
            ? Title
            : $"{WikiNamespace}:{Title}";

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
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public Transclusion(string title, string? wikiNamespace)
        {
            Title = title;
            WikiNamespace = wikiNamespace ?? WikiConfig.DefaultNamespace;
        }

        private Transclusion(SerializationInfo info, StreamingContext context) : this(
            (string?)info.GetValue(nameof(Title), typeof(string)) ?? string.Empty,
            (string?)info.GetValue(nameof(WikiNamespace), typeof(string)) ?? string.Empty)
        { }

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

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
        }

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
}
