using System.Text;

namespace Tavenem.Wiki;

/// <summary>
/// An implementation of <see cref="TextReader"/> which wraps a <see cref="StringBuilder"/>.
/// </summary>
/// <param name="builder">The underlying <see cref="StringBuilder"/>.</param>
public class StringBuilderReader(StringBuilder builder) : TextReader
{
    /// <summary>
    /// The current position;
    /// </summary>
    public int Current { get; private set; }

    /// <summary>
    /// Returns the next available character without actually reading it from the input stream. The
    /// current position of the TextReader is not changed by this operation. The returned value is
    /// -1 if no further characters are available.
    /// </summary>
    /// <returns>
    /// The next available character without actually reading it from the input stream. The current
    /// position of the TextReader is not changed by this operation. The returned value is -1 if no
    /// further characters are available.
    /// </returns>
    public override int Peek() => builder.Length > Current
        ? builder[Current]
        : -1;

    /// <summary>
    /// Reads the next character from the input stream. The returned value is -1 if no further
    /// characters are available.
    /// </summary>
    /// <returns>
    /// The next character from the input stream. The returned value is -1 if no further characters
    /// are available.
    /// </returns>
    public override int Read() => builder.Length > Current
        ? builder[Current++]
        : -1;

    /// <summary>
    /// Reads all characters from the current position to the end of the
    /// TextReader, and returns them as one string.
    /// </summary>
    /// <returns>
    /// All characters from the current position to the end of the
    /// TextReader, as one string.
    /// </returns>
    public override string ReadToEnd()
        => builder.ToString(Current, builder.Length - Current);

    /// <summary>
    /// Reads a line. A line is defined as a sequence of characters followed by a carriage return
    /// ('\r'), a line feed ('\n'), or a carriage return immediately followed by a line feed. The
    /// resulting string does not contain the terminating carriage return and/or line feed. The
    /// returned value is null if the end of the input stream has been reached.
    /// </summary>
    /// <returns>
    /// The result of reading a line; or null if the end of the input stream has been reached.
    /// </returns>
    public override string? ReadLine()
    {
        var start = Current;
        while (true)
        {
            var ch = Read();
            if (ch == -1)
            {
                break;
            }
            if (ch is '\r' or '\n')
            {
                if (ch == '\r' && Peek() == '\n')
                {
                    Read();
                }

                return builder.ToString(start, Current - start);
            }
        }
        if (Current > start)
        {
            return builder.ToString(start, Current - start);
        }

        return null;
    }
}