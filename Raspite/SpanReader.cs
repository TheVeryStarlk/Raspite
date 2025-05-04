using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Raspite;

internal ref struct SpanReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    private ReadOnlySpan<byte> span = span;
    private int index;

    public bool TryRead(int length, out ReadOnlySpan<byte> value)
    {
        value = default;

        if (index + length > span.Length)
        {
            return false;
        }

        value = span.Slice(index, length);
        index += length;

        return true;
    }

    public bool TryRead(out byte value)
    {
        value = 0;

        if (index + sizeof(byte) > span.Length)
        {
            return false;
        }

        value = span[index++];
        return true;
    }

    public bool TryRead(out short value)
    {
        value = 0;

        if (index + sizeof(short) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(short))];

        value = littleEndian
            ? BinaryPrimitives.ReadInt16LittleEndian(slice)
            : BinaryPrimitives.ReadInt16BigEndian(slice);

        return true;
    }

    public bool TryRead(out ushort value)
    {
        value = 0;

        if (index + sizeof(ushort) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(ushort))];

        value = littleEndian
            ? BinaryPrimitives.ReadUInt16LittleEndian(slice)
            : BinaryPrimitives.ReadUInt16BigEndian(slice);

        return true;
    }

    public bool TryRead(out int value)
    {
        value = 0;

        if (index + sizeof(int) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(int))];

        value = littleEndian
            ? BinaryPrimitives.ReadInt32LittleEndian(slice)
            : BinaryPrimitives.ReadInt32BigEndian(slice);

        return true;
    }

    public bool TryRead(out long value)
    {
        value = 0;

        if (index + sizeof(long) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(long))];

        value = littleEndian
            ? BinaryPrimitives.ReadInt64LittleEndian(slice)
            : BinaryPrimitives.ReadInt64BigEndian(slice);

        return true;
    }

    public bool TryRead(out float value)
    {
        value = 0;

        if (index + sizeof(float) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(float))];

        value = littleEndian
            ? BinaryPrimitives.ReadSingleLittleEndian(slice)
            : BinaryPrimitives.ReadSingleBigEndian(slice);

        return true;
    }

    public bool TryRead(out double value)
    {
        value = 0;

        if (index + sizeof(double) > span.Length)
        {
            return false;
        }

        var slice = span[index..(index += sizeof(double))];

        value = littleEndian
            ? BinaryPrimitives.ReadDoubleLittleEndian(slice)
            : BinaryPrimitives.ReadDoubleBigEndian(slice);

        return true;
    }

    public bool TryRead([NotNullWhen(true)] out string? value)
    {
        value = string.Empty;

        if (!TryRead(out ushort length) || !TryRead(length, out var buffer))
        {
            return false;
        }

        value = Encoding.UTF8.GetString(buffer);
        return true;
    }
}