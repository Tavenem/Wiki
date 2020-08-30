﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NeverFoundry.Wiki
{
    /// <summary>
    /// Represents an intra-wiki link.
    /// </summary>
    [Serializable]
    [Newtonsoft.Json.JsonObject]
    public class WikiLink : ISerializable, IEquatable<WikiLink>
    {
        /// <summary>
        /// <para>
        /// The linked article (if any).
        /// </para>
        /// <para>
        /// Note: this property is not persisted to storage, and should only be considered valid
        /// immediately after parsing.
        /// </para>
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Article? Article { get; set; }

        /// <summary>
        /// Gets the full title of the linked article (including namespace if the namespace is not
        /// <see cref="WikiConfig.DefaultNamespace"/>, and the Talk pseudo-namespace if this is a
        /// discussion link).
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string FullTitle => Article.GetFullTitle(Title, WikiNamespace, IsTalk);

        /// <summary>
        /// Whether this is a link to an existing page.
        /// </summary>
        public bool Missing { get; set; }

        /// <summary>
        /// Whether this is a link to a category.
        /// </summary>
        public bool IsCategory { get; }

        /// <summary>
        /// Whether a leading ':' precedes the namespace.
        /// </summary>
        public bool IsNamespaceEscaped { get; }

        /// <summary>
        /// Whether this is a link to a discussion page.
        /// </summary>
        public bool IsTalk { get; }

        /// <summary>
        /// The title for the linked article.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The namespace for the linked article.
        /// </summary>
        public string WikiNamespace { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiLink"/>.
        /// </summary>
        /// <param name="article">
        /// The linked article (if any).
        /// </param>
        /// <param name="missing">
        /// Whether this is a link to a missing page.
        /// </param>
        /// <param name="isCategory">
        /// Whether this is a link to a category.
        /// </param>
        /// <param name="isNamespaceEscaped">
        /// Whether a leading ':' precedes the namespace.
        /// </param>
        /// <param name="isTalk">
        /// Whether this is a link to a discussion page.
        /// </param>
        /// <param name="title">
        /// The title for the linked article.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace for the linked article.
        /// </param>
        public WikiLink(
            Article? article,
            bool missing,
            bool isCategory,
            bool isNamespaceEscaped,
            bool isTalk,
            string title,
            string? wikiNamespace)
        {
            Article = article;
            Missing = missing;
            IsCategory = isCategory;
            IsNamespaceEscaped = isNamespaceEscaped;
            IsTalk = isTalk;
            Title = title;
            WikiNamespace = wikiNamespace ?? WikiConfig.DefaultNamespace;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="WikiLink"/>.
        /// </summary>
        /// <param name="isCategory">
        /// Whether this is a link to a category.
        /// </param>
        /// <param name="isNamespaceEscaped">
        /// Whether a leading ':' precedes the namespace.
        /// </param>
        /// <param name="isTalk">
        /// Whether this is a link to a discussion page.
        /// </param>
        /// <param name="missing">
        /// Whether this is a link to an existing page.
        /// </param>
        /// <param name="title">
        /// The title for the linked article.
        /// </param>
        /// <param name="wikiNamespace">
        /// The namespace for the linked article.
        /// </param>
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public WikiLink(
            bool isCategory,
            bool isNamespaceEscaped,
            bool isTalk,
            bool missing,
            string title,
            string? wikiNamespace)
        {
            IsCategory = isCategory;
            IsNamespaceEscaped = isNamespaceEscaped;
            IsTalk = isTalk;
            Missing = missing;
            Title = title;
            WikiNamespace = wikiNamespace ?? WikiConfig.DefaultNamespace;
        }

        private WikiLink(SerializationInfo info, StreamingContext context) : this(
            (bool?)info.GetValue(nameof(IsCategory), typeof(bool)) ?? default,
            (bool?)info.GetValue(nameof(IsNamespaceEscaped), typeof(bool)) ?? default,
            (bool?)info.GetValue(nameof(IsTalk), typeof(bool)) ?? default,
            (bool?)info.GetValue(nameof(Missing), typeof(bool)) ?? default,
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
        public bool Equals(WikiLink? other) => !(other is null)
            && Missing == other.Missing
            && IsNamespaceEscaped == other.IsNamespaceEscaped
            && IsTalk == other.IsTalk
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
        public override bool Equals(object? obj) => obj is WikiLink other && Equals(other);

        /// <summary>Populates a <see cref="SerializationInfo"></see> with the data needed to
        /// serialize the target object.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> to populate with
        /// data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"></see>) for this
        /// serialization.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the
        /// required permission.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(IsCategory), IsCategory);
            info.AddValue(nameof(IsNamespaceEscaped), IsNamespaceEscaped);
            info.AddValue(nameof(IsTalk), IsTalk);
            info.AddValue(nameof(Missing), Missing);
            info.AddValue(nameof(Title), Title);
            info.AddValue(nameof(WikiNamespace), WikiNamespace);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => HashCode.Combine(Missing, IsNamespaceEscaped, IsTalk, Title, WikiNamespace);

        /// <summary>
        /// Determines whether this link corresponds to the given article.
        /// </summary>
        /// <param name="item">The <see cref="Article"/> to match.</param>
        /// <returns><see langword="true"/> if this link corresponds to the given article; otherwise
        /// <see langword="false"/>.</returns>
        public bool IsLinkMatch(Article item) => string.CompareOrdinal(item.Title, Title) == 0
            && string.CompareOrdinal(item.WikiNamespace, WikiNamespace) == 0;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (IsTalk)
            {
                return string.IsNullOrEmpty(WikiNamespace)
                    ? $"{WikiConfig.TalkNamespace}:{Title}"
                    : $"{WikiConfig.TalkNamespace}:{WikiNamespace}:{Title}";
            }
            if (IsNamespaceEscaped)
            {
                return $":{WikiNamespace}:{Title}";
            }
            return string.IsNullOrEmpty(WikiNamespace)
                ? Title
                : $"{WikiNamespace}:{Title}";
        }

        /// <summary>
        /// Determines equality.
        /// </summary>
        public static bool operator ==(WikiLink? left, WikiLink? right) => EqualityComparer<WikiLink?>.Default.Equals(left, right);

        /// <summary>
        /// Determines inequality.
        /// </summary>
        public static bool operator !=(WikiLink? left, WikiLink? right) => !(left == right);
    }
}
