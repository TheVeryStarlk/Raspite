using System.Buffers;
using System.Globalization;

namespace Raspite;

/// <summary>
/// Writes stringified named binary tag (SNBT) tokens.
/// </summary>
/// <remarks>
/// Separators, newlines and indentation are written automatically between values.
/// </remarks>
public ref struct SnbtWriter(IBufferWriter<char> buffer, SnbtSerializerOptions options)
{
    /// <summary>
    /// Enough to hold any formatted number and its suffix.
    /// </summary>
    private const int MaximumNumberLength = 32;

    private static readonly SearchValues<char> doubleQuoteEscapes = CreateEscapes('"');

    private static readonly SearchValues<char> singleQuoteEscapes = CreateEscapes('\'');

    /// <summary>
    /// The number of currently open lists and compounds.
    /// </summary>
    public readonly int Depth => depth;

    private int depth;

    /// <summary>
    /// Whether the next value is the first in its container.
    /// </summary>
    private bool first = true;

    /// <summary>
    /// Whether a compound key was just written, so the next value follows it directly.
    /// </summary>
    private bool named;

    public void WriteName(string name)
    {
        WriteSeparator();

        if (name.Length > 0 && !name.AsSpan().ContainsAnyExcept(SnbtReader.Unquoted))
        {
            Write(name);
        }
        else
        {
            WriteQuoted(name);
        }

        Write(':');

        if (options.Indented)
        {
            Write(' ');
        }

        named = true;
    }

    public void WriteByteTag(byte value)
    {
        WriteSeparator();
        WriteNumber((sbyte) value, 'b');
    }

    public void WriteShortTag(short value)
    {
        WriteSeparator();
        WriteNumber(value, 's');
    }

    public void WriteIntegerTag(int value)
    {
        WriteSeparator();
        WriteNumber(value, '\0');
    }

    public void WriteLongTag(long value)
    {
        WriteSeparator();
        WriteNumber(value, 'L');
    }

    /// <remarks>
    /// Infinities are written as overflowing literals; <see cref="float.NaN"/> has no SNBT representation and is written as <c>NaNf</c> like vanilla does.
    /// </remarks>
    public void WriteFloatTag(float value)
    {
        WriteSeparator();

        if (float.IsFinite(value))
        {
            WriteNumber(value, 'f');
            return;
        }

        Write(float.IsNaN(value) ? "NaNf" : value > 0 ? "1e39f" : "-1e39f");
    }

    /// <remarks>
    /// Infinities are written as overflowing literals; <see cref="double.NaN"/> has no SNBT representation and is written as <c>NaNd</c> like vanilla does.
    /// </remarks>
    public void WriteDoubleTag(double value)
    {
        WriteSeparator();

        if (double.IsFinite(value))
        {
            WriteNumber(value, 'd');
            return;
        }

        Write(double.IsNaN(value) ? "NaNd" : value > 0 ? "1e309d" : "-1e309d");
    }

    /// <summary>
    /// Writes a string, quoting it only when it would otherwise be read as something else.
    /// </summary>
    public void WriteStringTag(string value)
    {
        WriteSeparator();

        // Unquoted strings starting with a digit, sign or dot are rejected by newer versions.
        var unquoted = value.Length > 0 && (char.IsAsciiLetter(value[0]) || value[0] == '_')
                                        && !value.AsSpan().ContainsAnyExcept(SnbtReader.Unquoted)
                                        && !value.Equals("true", StringComparison.OrdinalIgnoreCase)
                                        && !value.Equals("false", StringComparison.OrdinalIgnoreCase);

        if (unquoted)
        {
            Write(value);
        }
        else
        {
            WriteQuoted(value);
        }
    }

    public void WriteListStart()
    {
        WriteStart('[');
    }

    public void WriteListEnd()
    {
        WriteEnd(']');
    }

    public void WriteCompoundStart()
    {
        WriteStart('{');
    }

    public void WriteCompoundEnd()
    {
        WriteEnd('}');
    }

    public void WriteBytesTag(ReadOnlySpan<byte> value)
    {
        WriteArrayStart('B', value.IsEmpty);

        for (var index = 0; index < value.Length; index++)
        {
            WriteArraySeparator(index);
            WriteNumber((sbyte) value[index], 'b');
        }

        Write(']');
    }

    public void WriteIntegersTag(ReadOnlySpan<int> value)
    {
        WriteArrayStart('I', value.IsEmpty);

        for (var index = 0; index < value.Length; index++)
        {
            WriteArraySeparator(index);
            WriteNumber(value[index], '\0');
        }

        Write(']');
    }

    public void WriteLongsTag(ReadOnlySpan<long> value)
    {
        WriteArrayStart('L', value.IsEmpty);

        for (var index = 0; index < value.Length; index++)
        {
            WriteArraySeparator(index);
            WriteNumber(value[index], 'L');
        }

        Write(']');
    }

    private static SearchValues<char> CreateEscapes(char quote)
    {
        Span<char> values = stackalloc char[' ' + 2];

        for (var index = 0; index < ' '; index++)
        {
            values[index] = (char) index;
        }

        values[^2] = '\\';
        values[^1] = quote;

        return SearchValues.Create(values);
    }

    private void WriteSeparator()
    {
        if (named)
        {
            named = false;
            return;
        }

        if (depth == 0)
        {
            return;
        }

        if (!first)
        {
            Write(',');
        }

        first = false;

        if (options.Indented)
        {
            WriteLine();
        }
    }

    private void WriteStart(char value)
    {
        WriteSeparator();
        Write(value);

        depth++;
        first = true;
    }

    private void WriteEnd(char value)
    {
        if (depth == 0)
        {
            throw new InvalidOperationException("There is no open list or compound to end.");
        }

        depth--;

        // Empty containers stay on one line.
        if (!first && options.Indented)
        {
            WriteLine();
        }

        Write(value);
        first = false;
    }

    private void WriteArrayStart(char identifier, bool empty)
    {
        WriteSeparator();

        var span = buffer.GetSpan(4);

        span[0] = '[';
        span[1] = identifier;
        span[2] = ';';

        // Arrays always stay on one line.
        var space = options.Indented && !empty;

        if (space)
        {
            span[3] = ' ';
        }

        buffer.Advance(space ? 4 : 3);
    }

    private void WriteArraySeparator(int index)
    {
        if (index == 0)
        {
            return;
        }

        if (options.Indented)
        {
            Write(", ");
        }
        else
        {
            Write(',');
        }
    }

    private void WriteLine()
    {
        Write('\n');

        for (var index = 0; index < depth; index++)
        {
            Write(options.IndentString);
        }
    }

    private void WriteQuoted(string value)
    {
        var quote = options.SingleQuoteStrings ? '\'' : '"';
        var other = options.SingleQuoteStrings ? '"' : '\'';

        // Switch quotes when that avoids escaping.
        if (value.Contains(quote) && !value.Contains(other))
        {
            quote = other;
        }

        var escapes = quote == '"' ? doubleQuoteEscapes : singleQuoteEscapes;
        var remaining = value.AsSpan();

        Write(quote);

        while (true)
        {
            var index = remaining.IndexOfAny(escapes);

            if (index < 0)
            {
                Write(remaining);
                break;
            }

            Write(remaining[..index]);
            WriteEscape(remaining[index]);

            remaining = remaining[(index + 1)..];
        }

        Write(quote);
    }

    private void WriteEscape(char value)
    {
        var span = buffer.GetSpan(4);
        var length = 2;

        span[0] = '\\';

        switch (value)
        {
            case '\b':
                span[1] = 'b';
                break;

            case '\f':
                span[1] = 'f';
                break;

            case '\n':
                span[1] = 'n';
                break;

            case '\r':
                span[1] = 'r';
                break;

            case '\t':
                span[1] = 't';
                break;

            case < ' ':
                span[1] = 'x';

                var temporary = (byte) value;
                _ = temporary.TryFormat(span[2..], out _, "X2", CultureInfo.InvariantCulture);

                length = 4;
                break;

            default:
                span[1] = value;
                break;
        }

        buffer.Advance(length);
    }

    private void WriteNumber<T>(T value, char suffix) where T : struct, ISpanFormattable
    {
        var span = buffer.GetSpan(MaximumNumberLength);

        if (!value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format {value}.");
        }

        if (suffix != '\0')
        {
            span[written++] = suffix;
        }

        buffer.Advance(written);
    }

    private void Write(char value)
    {
        buffer.GetSpan(1)[0] = value;
        buffer.Advance(1);
    }

    private void Write(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return;
        }

        value.CopyTo(buffer.GetSpan(value.Length));
        buffer.Advance(value.Length);
    }
}