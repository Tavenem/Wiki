using System.Text;
using System.Text.Json.Serialization;

namespace Tavenem.Wiki.Models;

/// <summary>
/// Represents an intra-wiki link.
/// </summary>
public class WikiLink : IEquatable<WikiLink>
{
    /// <summary>
    /// Any action segment which follows the link.
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Any fragment which follows the link.
    /// </summary>
    public string? Fragment { get; set; }

    /// <summary>
    /// Whether this is a link to a category.
    /// </summary>
    public bool IsCategory { get; }

    /// <summary>
    /// Whether a leading ':' precedes the link.
    /// </summary>
    public bool IsEscaped { get; }

    /// <summary>
    /// <para>
    /// Whether this link's missing status should be ignored.
    /// </para>
    /// <para>
    /// Note: this property is not persisted to storage, and should only be considered valid
    /// immediately after parsing.
    /// </para>
    /// </summary>
    [JsonIgnore]
    public bool IsMissingIgnored { get; set; }

    /// <summary>
    /// The title of the linked article.
    /// </summary>
    public PageTitle Title { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="WikiLink"/>.
    /// </summary>
    /// <param name="action">
    /// Any action segment which follows the link.
    /// </param>
    /// <param name="fragment">
    /// Any fragment which follows the link.
    /// </param>
    /// <param name="isCategory">
    /// Whether this is a link to a category.
    /// </param>
    /// <param name="isEscaped">
    /// Whether a leading ':' precedes the link.
    /// </param>
    /// <param name="isMissingIgnored">
    /// Whether this link's missing status should be ignored.
    /// </param>
    /// <param name="title">
    /// The title of the linked page.
    /// </param>
    public WikiLink(
        string? action,
        string? fragment,
        bool isCategory,
        bool isEscaped,
        bool isMissingIgnored,
        PageTitle title)
    {
        Action = action;
        Fragment = fragment;
        IsCategory = isCategory;
        IsEscaped = isEscaped;
        IsMissingIgnored = isMissingIgnored;
        Title = title;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="WikiLink"/>.
    /// </summary>
    /// <param name="action">
    /// Any action segment which follows the link.
    /// </param>
    /// <param name="fragment">
    /// Any fragment which follows the link.
    /// </param>
    /// <param name="isCategory">
    /// Whether this is a link to a category.
    /// </param>
    /// <param name="isEscaped">
    /// Whether a leading ':' precedes the namespace.
    /// </param>
    /// <param name="title">
    /// The title for the linked article.
    /// </param>
    [JsonConstructor]
    public WikiLink(
        string? action,
        string? fragment,
        bool isCategory,
        bool isEscaped,
        PageTitle title)
    {
        Action = action;
        Fragment = fragment;
        IsCategory = isCategory;
        IsEscaped = isEscaped;
        Title = title;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" />
    /// parameter; otherwise, <see langword="false" />.
    /// </returns>
    public bool Equals(WikiLink? other) => other is not null
        && IsEscaped == other.IsEscaped
        && string.Equals(Action, other.Action, StringComparison.OrdinalIgnoreCase)
        && string.CompareOrdinal(Fragment, other.Fragment) == 0
        && Title.Equals(other.Title);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object;
    /// otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object? obj) => obj is WikiLink other && Equals(other);

    /// <summary>Serves as the default hash function.</summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => HashCode.Combine(Action, Fragment, IsEscaped, Title);

    /// <summary>
    /// Determines whether this link corresponds to the given page.
    /// </summary>
    /// <param name="page">The <see cref="Wiki.Page"/> to match.</param>
    /// <returns>
    /// <see langword="true"/> if this link corresponds to the given page; otherwise <see
    /// langword="false"/>.
    /// </returns>
    public bool IsLinkMatch(Page page) => Title.Equals(page.Title);

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[[");
        if (IsEscaped)
        {
            sb.Append(':');
        }
        sb.Append(Title.ToString());
        if (!string.IsNullOrEmpty(Action))
        {
            sb.Append('/');
            sb.Append(Action);
        }
        if (!string.IsNullOrEmpty(Fragment))
        {
            sb.Append('#');
            sb.Append(Fragment);
        }
        sb.Append("]]");
        return sb.ToString();
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
