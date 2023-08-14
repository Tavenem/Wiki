using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace Tavenem.Wiki;

/// <summary>
/// The title of a wiki page.
/// </summary>
public struct PageTitle : IEquatable<PageTitle>, IParsable<PageTitle>
{
    /// <summary>
    /// The domain of the page (if any).
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public string? Domain { get; set; }

    /// <summary>
    /// Whether all the title's parts are empty.
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty { get; }

    /// <summary>
    /// The namespace of the page (if any).
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    [JsonPropertyName("@namespace")]
    public string? Namespace { get; set; }

    /// <summary>
    /// The normalized (lowercase) namespace of the page (if any).
    /// </summary>
    [JsonIgnore]
    public string? NormalizedNamespace { get; }

    /// <summary>
    /// <para>
    /// The title of the page.
    /// </para>
    /// <para>
    /// May be null when referring to the default (i.e. home) page for the domain+namespace.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This property has a public setter for serialization support, but should not be directly set
    /// by non-library code.
    /// </remarks>
    public string? Title { get; set; }

    /// <summary>
    /// The normalized (lowercase) title of the page (if any).
    /// </summary>
    [JsonIgnore]
    public string? NormalizedTitle { get; }

    /// <summary>
    /// Constructs a new instance of <see cref="PageTitle"/>.
    /// </summary>
    public PageTitle()
    {
        Title = null;
        NormalizedTitle = null;

        Namespace = null;
        NormalizedNamespace = null;

        Domain = null;

        IsEmpty = true;
    }

    /// <summary>
    /// Constructs a new instance of <see cref="PageTitle"/>.
    /// </summary>
    /// <param name="title">
    /// <para>
    /// The title of the page.
    /// </para>
    /// <para>
    /// May be null when referring to the default (i.e. home) page for the domain+namespace.
    /// </para>
    /// </param>
    /// <param name="namespace">
    /// The namespace of the page (if any).
    /// </param>
    /// <param name="domain">
    /// The domain of the page (if any).
    /// </param>
    [JsonConstructor]
    public PageTitle(string? title, string? @namespace = null, string? domain = null) : this()
    {
        Title = title?.ToWikiTitleCase();
        NormalizedTitle = title?.ToLowerInvariant();

        Namespace = @namespace?.ToWikiTitleCase();
        NormalizedNamespace = @namespace?.ToLowerInvariant();

        Domain = domain;

        IsEmpty = string.IsNullOrEmpty(Domain)
            && string.IsNullOrEmpty(Namespace)
            && string.IsNullOrEmpty(Title);
    }

    /// <summary>
    /// Parses a string into a value.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">
    /// An object that provides culture-specific formatting information about <paramref name="s"/>.
    /// </param>
    /// <returns>
    /// The result of parsing <paramref name="s"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A <see cref="PageTitle"/> has the following format:
    /// </para>
    /// <para>
    /// (<c>domain</c>):<c>namespace</c>:title
    /// </para>
    /// <para>
    /// If the domain or namespace are <see langword="null"/> the entire segment, up to and
    /// including the colon, may be omitted. They may also be included as an empty segment, with
    /// the delimiting colon (and parenthesis for domain) but no content, or only whitespace.
    /// </para>
    /// <para>
    /// If the title is <see langword="null"/> but not the domain or namespace, the string must
    /// terminate in a colon, to remove any ambiguity with cases where the last part could also
    /// be construed as a title.
    /// </para>
    /// <para>
    /// Only the first segment is considered a domain when bracketed by parenthesis. If a
    /// parenthesized segment occurs in the second or a later position, the parenthesis are
    /// considered part of the text, and not a domain indicator.
    /// </para>
    /// <para>
    /// Excess colons are interpreted as part of the title.
    /// </para>
    /// </remarks>
    public static PageTitle Parse(string? s, IFormatProvider? provider = null)
        => TryParse(s, provider, out var result)
        ? result
        : throw new InvalidOperationException();

    /// <summary>
    /// Tries to parse a string into a <see cref="PageTitle"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">
    /// An object that provides culture-specific formatting information about <paramref name="s"/>.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="s"/>
    /// or an undefined value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="s"/> was successfully parsed; otherwise, <see
    /// langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A <see cref="PageTitle"/> has the following format:
    /// </para>
    /// <para>
    /// (<c>domain</c>):<c>namespace</c>:title
    /// </para>
    /// <para>
    /// If the domain or namespace are <see langword="null"/> the entire segment, up to and
    /// including the colon, may be omitted. They may also be included as an empty segment, with
    /// the delimiting colon (and parenthesis for domain) but no content, or only whitespace.
    /// </para>
    /// <para>
    /// If the title is <see langword="null"/> but not the domain or namespace, the string must
    /// terminate in a colon, to remove any ambiguity with cases where the last part could also
    /// be construed as a title.
    /// </para>
    /// <para>
    /// Only the first segment is considered a domain when bracketed by parenthesis. If a
    /// parenthesized segment occurs in the second or a later position, the parenthesis are
    /// considered part of the text, and not a domain indicator.
    /// </para>
    /// <para>
    /// Excess colons are interpreted as part of the title.
    /// </para>
    /// <para>
    /// Note: this method always returns <see langword="true"/>; there is no string which cannot be
    /// parsed as a <see cref="PageTitle"/>.
    /// </para>
    /// </remarks>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out PageTitle result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return true;
        }

        var parts = s.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            result = default;
            return true;
        }

        string? domain = null;
        if (parts[0].StartsWith('(')
            && parts[0].EndsWith(')'))
        {
            domain = parts[0][1..^1];
            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = null;
            }
            parts = parts.Length == 1
                ? Array.Empty<string>()
                : parts[1..];
        }

        if (parts.Length > 1)
        {
            result = new(
                string
                    .Join(':', parts[1..])
                    .ToWikiTitleCase(),
                string.IsNullOrWhiteSpace(parts[0])
                    ? null
                    : parts[0].ToWikiTitleCase(),
                domain);
            return true;
        }

        result = new(
            parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0])
                ? parts[0].ToWikiTitleCase()
                : null,
            null,
            domain);
        return true;
    }

    /// <summary>
    /// Tries to parse a string into a <see cref="PageTitle"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">
    /// When this method returns, contains the result of successfully parsing <paramref name="s"/>
    /// or an undefined value on failure.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="s"/> was successfully parsed; otherwise, <see
    /// langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A <see cref="PageTitle"/> has the following format:
    /// </para>
    /// <para>
    /// (<c>domain</c>):<c>namespace</c>:title
    /// </para>
    /// <para>
    /// If the domain or namespace are <see langword="null"/> the entire segment, up to and
    /// including the colon, may be omitted. They may also be included as an empty segment, with
    /// the delimiting colon (and parenthesis for domain) but no content, or only whitespace.
    /// </para>
    /// <para>
    /// If the title is <see langword="null"/> but not the domain or namespace, the string must
    /// terminate in a colon, to remove any ambiguity with cases where the last part could also
    /// be construed as a title.
    /// </para>
    /// <para>
    /// Only the first segment is considered a domain when bracketed by parenthesis. If a
    /// parenthesized segment occurs in the second or a later position, the parenthesis are
    /// considered part of the text, and not a domain indicator.
    /// </para>
    /// <para>
    /// Excess colons are interpreted as part of the title.
    /// </para>
    /// <para>
    /// Note: this method always returns <see langword="true"/>; there is no string which cannot be
    /// parsed as a <see cref="PageTitle"/>.
    /// </para>
    /// </remarks>
    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        [MaybeNullWhen(false)] out PageTitle result)
        => TryParse(s, CultureInfo.CurrentCulture, out result);

    /// <summary>
    /// Deconstructs this instance of <see cref="PageTitle"/>.
    /// </summary>
    /// <param name="title">
    /// <para>
    /// The title of the page.
    /// </para>
    /// <para>
    /// May be null when referring to the default (i.e. home) page for the domain+namespace.
    /// </para>
    /// </param>
    /// <param name="namespace">
    /// The namespace of the page (if any).
    /// </param>
    /// <param name="domain">
    /// The domain of the page (if any).
    /// </param>
    public readonly void Deconstruct(out string? title, out string? @namespace, out string? domain)
    {
        title = Title;
        @namespace = Namespace;
        domain = Domain;
    }

    /// <inheritdoc/>
    public readonly bool Equals(PageTitle other) => string.CompareOrdinal(Domain, other.Domain) == 0
        && string.CompareOrdinal(Namespace, other.Namespace) == 0
        && string.CompareOrdinal(Title, other.Title) == 0;

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is PageTitle other && Equals(other);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(Domain, Namespace, Title);

    /// <summary>
    /// Gets a string representation of the current instance.
    /// </summary>
    /// <returns>
    /// A string representation of the current instance.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A <see cref="PageTitle"/> has the following format:
    /// </para>
    /// <para>
    /// (<c>domain</c>):<c>namespace</c>:title
    /// </para>
    /// <para>
    /// If the domain is <see langword="null"/> the entire segment, up to and including the
    /// colon, will be omitted.
    /// </para>
    /// <para>
    /// If both the domain and namespace are <see langword="null"/> only the title will be shown,
    /// with no colons.
    /// </para>
    /// <para>
    /// If all three parts are <see langword="null"/> an empty string will be returned.
    /// </para>
    /// <para>
    /// If the title is <see langword="null"/> but not the domain or namespace, the string will
    /// terminate in a colon, to remove any ambiguity with cases where the last part could also
    /// be construed as a title.
    /// </para>
    /// </remarks>
    public override readonly string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(Domain))
        {
            sb.Append('(')
                .Append(Domain)
                .Append("):");
        }
        if (!string.IsNullOrEmpty(Namespace))
        {
            sb.Append(Namespace)
                .Append(':');
        }
        if (!string.IsNullOrEmpty(Title))
        {
            sb.Append(Title);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets a copy of this instance with the specified default title, if <see cref="Title"/> is
    /// currently <see langword="null"/>.
    /// </summary>
    /// <param name="title">
    /// The default title to supply if the current title is <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A new <see cref="PageTitle"/> with <see cref="Title"/> set to the given default if it was
    /// previously <see langword="null"/>, and the same <see cref="Namespace"/> and <see
    /// cref="Domain"/> as this instance.
    /// </returns>
    public readonly PageTitle WithDefaultTitle(string title)
        => new(Title ?? title, Namespace, Domain);

    /// <summary>
    /// Gets a copy of this instance with the specified <paramref name="domain"/>.
    /// </summary>
    /// <param name="domain">
    /// The domain of the page (if any).
    /// </param>
    /// <returns>
    /// A new <see cref="PageTitle"/> with <see cref="Domain"/> set to the given <paramref
    /// name="domain"/>, and the same <see cref="Namespace"/> and <see cref="Title"/> as this
    /// instance.
    /// </returns>
    public readonly PageTitle WithDomain(string? domain) => new(Title, Namespace, domain);

    /// <summary>
    /// Gets a copy of this instance with the specified <paramref name="namespace"/>.
    /// </summary>
    /// <param name="namespace">
    /// The namespace of the page (if any).
    /// </param>
    /// <returns>
    /// A new <see cref="PageTitle"/> with <see cref="Namespace"/> set to the given <paramref
    /// name="namespace"/>, and the same <see cref="Domain"/> and <see cref="Title"/> as this
    /// instance.
    /// </returns>
    public readonly PageTitle WithNamespace(string? @namespace) => new(Title, @namespace, Domain);

    /// <summary>
    /// Gets a copy of this instance with the specified title.
    /// </summary>
    /// <param name="title">
    /// The title of the page (if any).
    /// </param>
    /// <returns>
    /// A new <see cref="PageTitle"/> with <see cref="Title"/> set to the given value, and the same
    /// <see cref="Namespace"/> and <see cref="Domain"/> as this instance.
    /// </returns>
    public readonly PageTitle WithTitle(string? title)
        => new(title, Namespace, Domain);

    internal readonly bool IsMatch(PageTitle other)
    {
        if (string.IsNullOrEmpty(Domain))
        {
            if (!string.IsNullOrEmpty(other.Domain))
            {
                return false;
            }
        }
        else if (string.IsNullOrEmpty(other.Domain))
        {
            return false;
        }
        else if (!string.Equals(
            Domain,
            other.Domain,
            StringComparison.Ordinal))
        {
            return false;
        }

        if (string.IsNullOrEmpty(NormalizedNamespace))
        {
            if (!string.IsNullOrEmpty(other.NormalizedNamespace))
            {
                return false;
            }
        }
        else if (string.IsNullOrEmpty(other.NormalizedNamespace))
        {
            return false;
        }
        else if (!string.Equals(
            NormalizedNamespace,
            other.NormalizedNamespace,
            StringComparison.Ordinal))
        {
            return false;
        }

        if (string.IsNullOrEmpty(other.NormalizedTitle))
        {
            return true;
        }

        if (string.IsNullOrEmpty(NormalizedTitle))
        {
            return false;
        }
        if (!string.Equals(
            NormalizedTitle,
            other.NormalizedTitle,
            StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    internal readonly void WriteUrl(StringWriter writer, WikiOptions wikiOptions)
    {
        if (!string.IsNullOrEmpty(Domain))
        {
            writer.Write('(');
            UrlEncoder.Default.Encode(writer, Domain);
            writer.Write("):");
        }
        if (!string.IsNullOrEmpty(Namespace))
        {
            UrlEncoder.Default.Encode(writer, Namespace);
            writer.Write(':');
        }
        if (!string.IsNullOrEmpty(Title)
            || string.IsNullOrEmpty(Namespace))
        {
            UrlEncoder.Default.Encode(writer, Title ?? wikiOptions.MainPageTitle);
        }
    }

    /// <summary>
    /// Indicates whether two <see cref="PageTitle"/> objects are equal.
    /// </summary>
    /// <param name="left">
    /// The first object to compare.
    /// </param>
    /// <param name="right">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(PageTitle left, PageTitle right) => left.Equals(right);

    /// <summary>
    /// Indicates whether two <see cref="PageTitle"/> objects are unequal.
    /// </summary>
    /// <param name="left">
    /// The first object to compare.
    /// </param>
    /// <param name="right">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is unequal to <paramref name="right"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(PageTitle left, PageTitle right) => !(left == right);
}
