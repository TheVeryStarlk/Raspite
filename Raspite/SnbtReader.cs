using System.Buffers;
using System.Globalization;
using System.Text;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Reads stringified named binary tag (SNBT) tokens.
/// </summary>
/// <remarks>
/// Every read skips leading whitespace.
/// </remarks>
public ref struct SnbtReader(ReadOnlySpan<char> span)
{
    /// <summary>
    /// The characters allowed in an unquoted string.
    /// </summary>
    internal static readonly SearchValues<char> Unquoted = SearchValues.Create("+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz");

    private static readonly SearchValues<char> hexadecimal = SearchValues.Create("0123456789ABCDEFabcdef");

    public readonly int Remaining => span.Length - position;

    private readonly ReadOnlySpan<char> span = span;

    private int position;

    public bool TryPeek(out char value)
    {
        SkipWhitespace();

        value = '\0';

        if (position == span.Length)
        {
            return false;
        }

        value = span[position];

        return true;
    }

    public bool TryRead(char expected)
    {
        if (!TryPeek(out var value) || value != expected)
        {
            return false;
        }

        position++;

        return true;
    }

    /// <summary>
    /// Reads the opening bracket of a list or an array.
    /// </summary>
    /// <param name="identifier">
    /// <see cref="Tag.Bytes"/>, <see cref="Tag.Integers"/> or <see cref="Tag.Longs"/> for arrays; otherwise, <see cref="Tag.List"/>.
    /// </param>
    public bool TryReadListStart(out byte identifier)
    {
        identifier = Tag.List;

        if (!TryRead('['))
        {
            return false;
        }

        SkipWhitespace();

        if (Remaining < 2 || span[position + 1] != ';')
        {
            return true;
        }

        identifier = span[position] switch
        {
            'B' => Tag.Bytes,
            'I' => Tag.Integers,
            'L' => Tag.Longs,
            _ => Tag.End
        };

        position += 2;

        return identifier is not Tag.End;
    }

    /// <summary>
    /// Reads a compound key and its trailing colon.
    /// </summary>
    public bool TryReadName(out string name)
    {
        name = string.Empty;

        if (!TryPeek(out var first))
        {
            return false;
        }

        if (first is '"' or '\'')
        {
            if (!TryReadQuoted(first, out name))
            {
                return false;
            }
        }
        else
        {
            var token = ReadUnquoted();

            if (token.IsEmpty)
            {
                return false;
            }

            name = new string(token);
        }

        return TryRead(':');
    }

    /// <summary>
    /// Reads a number, boolean or string, inferring its type the same way vanilla does.
    /// </summary>
    /// <remarks>
    /// Unquoted tokens that are not valid numbers (e.g. <c>123abc</c> or <c>300b</c>) are read as a <see cref="StringTag"/>.
    /// </remarks>
    public bool TryReadPrimitiveTag(out ITag tag, string name = "")
    {
        tag = EndTag.Instance;

        if (!TryPeek(out var first))
        {
            return false;
        }

        if (first is '"' or '\'')
        {
            if (!TryReadQuoted(first, out var value))
            {
                return false;
            }

            tag = new StringTag(value, name);
            return true;
        }

        var token = ReadUnquoted();

        if (token.IsEmpty)
        {
            return false;
        }

        tag = Infer(token, name);

        return true;
    }

    /// <summary>
    /// Reads an element of a <see cref="Tag.Bytes"/> array, where the <c>b</c> suffix is optional.
    /// </summary>
    public bool TryReadByteArrayItem(out byte value)
    {
        value = 0;

        if (!TryReadToken(out var token))
        {
            return false;
        }

        if (token[^1] is 'b' or 'B')
        {
            token = token[..^1];
        }

        if (!IsInteger(token) || !sbyte.TryParse(token, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var result))
        {
            return false;
        }

        value = (byte) result;

        return true;
    }

    /// <summary>
    /// Reads an element of a <see cref="Tag.Integers"/> array.
    /// </summary>
    public bool TryReadIntegerArrayItem(out int value)
    {
        value = 0;
        return TryReadToken(out var token) && IsInteger(token) && int.TryParse(token, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
    }

    /// <summary>
    /// Reads an element of a <see cref="Tag.Longs"/> array, where the <c>L</c> suffix is optional.
    /// </summary>
    public bool TryReadLongArrayItem(out long value)
    {
        value = 0;

        if (!TryReadToken(out var token))
        {
            return false;
        }

        if (token[^1] is 'l' or 'L')
        {
            token = token[..^1];
        }

        return IsInteger(token) && long.TryParse(token, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
    }

    private void SkipWhitespace()
    {
        while (position < span.Length && char.IsWhiteSpace(span[position]))
        {
            position++;
        }
    }

    private bool TryReadToken(out ReadOnlySpan<char> token)
    {
        SkipWhitespace();
        token = ReadUnquoted();

        return !token.IsEmpty;
    }

    private ReadOnlySpan<char> ReadUnquoted()
    {
        var remaining = span[position..];
        var length = remaining.IndexOfAnyExcept(Unquoted);

        if (length < 0)
        {
            length = remaining.Length;
        }

        position += length;

        return remaining[..length];
    }

    private bool TryReadQuoted(char quote, out string value)
    {
        value = string.Empty;

        var start = position + 1;
        var index = start;
        var length = 0;
        var escaped = false;

        while (true)
        {
            var offset = span[index..].IndexOfAny(quote, '\\');

            if (offset < 0)
            {
                return false;
            }

            index += offset;
            length += offset;

            if (span[index] == quote)
            {
                break;
            }

            if (!TryUnescape(span[index..], out var consumed, out var codePoint))
            {
                return false;
            }

            index += consumed;
            length += codePoint > char.MaxValue ? 2 : 1;
            escaped = true;
        }

        var content = span[start..index];
        position = index + 1;

        value = escaped
            ? string.Create(length, content, static (destination, source) => Unescape(source, destination))
            : new string(content);

        return true;
    }

    private static void Unescape(ReadOnlySpan<char> source, Span<char> destination)
    {
        while (true)
        {
            var offset = source.IndexOf('\\');

            if (offset < 0)
            {
                source.CopyTo(destination);
                return;
            }

            source[..offset].CopyTo(destination);
            destination = destination[offset..];

            // Already validated while scanning.
            TryUnescape(source[offset..], out var consumed, out var codePoint);

            if (codePoint > char.MaxValue)
            {
                destination = destination[new Rune(codePoint).EncodeToUtf16(destination)..];
            }
            else
            {
                destination[0] = (char) codePoint;
                destination = destination[1..];
            }

            source = source[(offset + consumed)..];
        }
    }

    private static bool TryUnescape(ReadOnlySpan<char> source, out int consumed, out uint codePoint)
    {
        consumed = 2;
        codePoint = 0;

        if (source.Length < 2)
        {
            return false;
        }

        switch (source[1])
        {
            case '\\' or '\'' or '"':
                codePoint = source[1];
                return true;

            case 'b':
                codePoint = '\b';
                return true;

            case 'f':
                codePoint = '\f';
                return true;

            case 'n':
                codePoint = '\n';
                return true;

            case 'r':
                codePoint = '\r';
                return true;

            case 's':
                codePoint = ' ';
                return true;

            case 't':
                codePoint = '\t';
                return true;

            case 'x':
                return TryReadHexadecimal(source, 2, out consumed, out codePoint);

            case 'u':
                return TryReadHexadecimal(source, 4, out consumed, out codePoint);

            case 'U':
                return TryReadHexadecimal(source, 8, out consumed, out codePoint) && Rune.IsValid(codePoint);

            default:
                return false;
        }
    }

    private static bool TryReadHexadecimal(ReadOnlySpan<char> source, int digits, out int consumed, out uint codePoint)
    {
        consumed = 2 + digits;
        codePoint = 0;

        if (source.Length < consumed)
        {
            return false;
        }

        var slice = source[2..consumed];

        return !slice.ContainsAnyExcept(hexadecimal) && uint.TryParse(slice, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out codePoint);
    }

    private static ITag Infer(ReadOnlySpan<char> token, string name)
    {
        if (token.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return new ByteTag(true, name);
        }

        if (token.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return new ByteTag(false, name);
        }

        var body = token[..^1];

        switch (token[^1])
        {
            case 'b' or 'B' when IsInteger(body) && sbyte.TryParse(body, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value):
                return new ByteTag((byte) value, name);

            case 's' or 'S' when IsInteger(body) && short.TryParse(body, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value):
                return new ShortTag(value, name);

            case 'l' or 'L' when IsInteger(body) && long.TryParse(body, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var value):
                return new LongTag(value, name);

            case 'f' or 'F' when IsDecimal(body, false) && float.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out var value):
                return new FloatTag(value, name);

            case 'd' or 'D' when IsDecimal(body, false) && double.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out var value):
                return new DoubleTag(value, name);
        }

        if (IsInteger(token) && int.TryParse(token, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var integer))
        {
            return new IntegerTag(integer, name);
        }

        if (IsDecimal(token, true) && double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var @double))
        {
            return new DoubleTag(@double, name);
        }

        return new StringTag(new string(token), name);
    }

    /// <summary>
    /// Matches <c>[-+]?(0|[1-9][0-9]*)</c>.
    /// </summary>
    private static bool IsInteger(ReadOnlySpan<char> value)
    {
        if (!value.IsEmpty && value[0] is '+' or '-')
        {
            value = value[1..];
        }

        if (value.IsEmpty)
        {
            return false;
        }

        return value[0] == '0' ? value.Length == 1 : !value.ContainsAnyExceptInRange('0', '9');
    }

    /// <summary>
    /// Matches <c>[-+]?([0-9]+[.]?|[0-9]*[.][0-9]+)(e[-+]?[0-9]+)?</c>, optionally requiring the decimal point.
    /// </summary>
    private static bool IsDecimal(ReadOnlySpan<char> value, bool point)
    {
        if (!value.IsEmpty && value[0] is '+' or '-')
        {
            value = value[1..];
        }

        var exponent = value.IndexOfAny('e', 'E');

        if (exponent >= 0)
        {
            var power = value[(exponent + 1)..];

            if (!power.IsEmpty && power[0] is '+' or '-')
            {
                power = power[1..];
            }

            if (power.IsEmpty || power.ContainsAnyExceptInRange('0', '9'))
            {
                return false;
            }

            value = value[..exponent];
        }

        var separator = value.IndexOf('.');

        if (separator < 0)
        {
            return !point && !value.IsEmpty && !value.ContainsAnyExceptInRange('0', '9');
        }

        var integral = value[..separator];
        var fractional = value[(separator + 1)..];

        return integral.Length + fractional.Length > 0
               && !integral.ContainsAnyExceptInRange('0', '9')
               && !fractional.ContainsAnyExceptInRange('0', '9');
    }
}
