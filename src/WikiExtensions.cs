using System.Globalization;

namespace Tavenem.Wiki;

/// <summary>
/// Helper methods.
/// </summary>
public static class WikiExtensions
{
    /// <summary>
    /// Gets the index of the first character in this <see cref="string"/> which satisfies the
    /// given <paramref name="condition"/>.
    /// </summary>
    /// <param name="str">This <see cref="string"/>.</param>
    /// <param name="condition">A condition which a character in the <see cref="string"/> must
    /// satisfy.</param>
    /// <param name="startIndex">The index at which to begin searching.</param>
    /// <returns>
    /// The index of the first character in the <see cref="string"/> which satisfies the given
    /// <paramref name="condition"/>; or -1 if no characters satisfy the condition.
    /// </returns>
    public static int Find(this string str, Func<char, bool> condition, int startIndex = 0)
    {
        for (var i = startIndex; i < str.Length; i++)
        {
            if (condition(str[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Converts a <see cref="DateTimeOffset"/> to a formatted display string.
    /// </summary>
    /// <param name="timestamp">The <see cref="DateTimeOffset"/> to format.</param>
    /// <returns>A formatted string.</returns>
    public static string ToWikiDisplayString(this DateTimeOffset timestamp)
        => timestamp.ToUniversalTime().LocalDateTime.ToString("F", CultureInfo.CurrentCulture);

    /// <summary>
    /// Converts a <see cref="long"/> representing the number of ticks in a <see
    /// cref="DateTimeOffset"/> to a formatted display string.
    /// </summary>
    /// <param name="timestamp">The <see cref="long"/> to format.</param>
    /// <returns>A formatted string.</returns>
    public static string ToWikiDisplayString(this long timestamp)
        => new DateTimeOffset(timestamp, TimeSpan.Zero).LocalDateTime.ToString("F", CultureInfo.CurrentCulture);

    /// <summary>
    /// Converts the initial character of the specified string to upper case, leaving all others
    /// unchanged. Multi-codepoint Unicode characters are converted as a unit.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The specified string with its initial character converted to upper case.</returns>
    public static string ToWikiTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var si = new StringInfo(value);
        if (si.LengthInTextElements == 0)
        {
            return value;
        }
        else if (si.LengthInTextElements == 1)
        {
            return value.ToUpper(CultureInfo.CurrentCulture);
        }
        else
        {
            return si.SubstringByTextElements(0, 1).ToUpper(CultureInfo.CurrentCulture)
                + si.SubstringByTextElements(1);
        }
    }

    internal static int IndexOfUnescaped(this ReadOnlySpan<char> span, char target)
    {
        for (var i = 0; i < span.Length; i++)
        {
            if (span.IsUnescapedMatch(i, target))
            {
                return i;
            }
        }
        return -1;
    }

    internal static bool IsUnescapedMatch(this ReadOnlySpan<char> span, int index, char target)
    {
        if (index >= span.Length || index < 0 || span[index] != target)
        {
            return false;
        }
        var i = index - 1;
        var escapes = 0;
        while (i > 0 && span[i] == '\\')
        {
            escapes++;
        }
        return escapes == 0 || escapes % 2 == 1;
    }
}
