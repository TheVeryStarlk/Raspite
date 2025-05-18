using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Raspite.Tags;

namespace Raspite;

public ref struct TagReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    public readonly int Remaining => span.Length - position;

    public bool Nameless { get; set; }

    private readonly ReadOnlySpan<byte> span = span;

    private int position;

    public bool TryPeek(out byte value)
    {
        value = 0;

        if (position == span.Length)
        {
            return false;
        }

        value = span[position];

        return true;
    }

    public bool TryReadEndTag()
    {
        if (!TryReadByte(out var identifier))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(Tag.End, identifier);

        return true;
    }

    public bool TryReadByteTag(out byte value, out string name)
    {
        value = 0;
        return TryRead(Tag.Byte, out name) && TryReadByte(out value);
    }

    public bool TryReadShortTag(out short value, out string name)
    {
        value = 0;

        if (!TryRead(Tag.Short, out name) || sizeof(short) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(short))];
        value = littleEndian ? BinaryPrimitives.ReadInt16LittleEndian(slice) : BinaryPrimitives.ReadInt16BigEndian(slice);

        return true;
    }

    public bool TryReadIntegerTag(out int value, out string name)
    {
        value = 0;
        return TryRead(Tag.Integer, out name) && TryReadInteger(out value);
    }

    public bool TryReadLongTag(out long value, out string name)
    {
        value = 0;
        return TryRead(Tag.Long, out name) && TryReadLong(out value);
    }

    public bool TryReadFloatTag(out float value, out string name)
    {
        value = 0;

        if (!TryRead(Tag.Float, out name) || sizeof(float) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(float))];
        value = littleEndian ? BinaryPrimitives.ReadSingleLittleEndian(slice) : BinaryPrimitives.ReadSingleBigEndian(slice);

        return true;
    }

    public bool TryReadDoubleTag(out double value, out string name)
    {
        value = 0;

        if (!TryRead(Tag.Double, out name) || sizeof(double) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(double))];
        value = littleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(slice) : BinaryPrimitives.ReadDoubleBigEndian(slice);

        return true;
    }

    public bool TryReadStringTag(out string value, out string name)
    {
        value = string.Empty;
        return TryRead(Tag.String, out name) && TryReadString(out value);
    }

    public bool TryReadListTag(out byte identifier, out int length, out string name)
    {
        identifier = 0;
        length = 0;

        if (!TryRead(Tag.List, out name) || !TryReadByte(out identifier) || !TryReadInteger(out length))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(identifier, Tag.Longs);

        Nameless = true;

        return true;
    }

    public bool TryReadCompoundTag(out string name)
    {
        if (!TryRead(Tag.Compound, out name))
        {
            return false;
        }

        Nameless = false;

        return true;
    }

    public bool TryReadBytesTag(out ReadOnlySpan<byte> value, out string name)
    {
        value = default;

        if (!TryRead(Tag.Bytes, out name) || !TryReadInteger(out var length) || length > Remaining)
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);
        value = span[position..(position += length)];

        return true;
    }

    public bool TryReadIntegersTag(out ReadOnlySpan<int> value, out string name)
    {
        value = default;

        if (!TryRead(Tag.Integers, out name) || !TryReadInteger(out var length) || length * sizeof(int) > Remaining)
        {
            return false;
        }

        // Fast path.
        if (BitConverter.IsLittleEndian == littleEndian)
        {
            return TryRead(length * sizeof(int), out value);
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var items = new int[length];

        for (var index = 0; index < length; index++)
        {
            if (!TryReadInteger(out var current))
            {
                return false;
            }

            items[index] = current;
        }

        value = items;

        return true;
    }

    public bool TryReadLongsTag(out ReadOnlySpan<long> value, out string name)
    {
        value = default;

        if (!TryRead(Tag.Longs, out name) || !TryReadInteger(out var length) || length * sizeof(long) > Remaining)
        {
            return false;
        }

        // Fast path.
        if (BitConverter.IsLittleEndian == littleEndian)
        {
            return TryRead(length * sizeof(long), out value);
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var items = new long[length];

        for (var index = 0; index < length; index++)
        {
            if (!TryReadLong(out var current))
            {
                return false;
            }

            items[index] = current;
        }

        value = items;

        return true;
    }

    private bool TryRead(byte expected, out string name)
    {
        name = string.Empty;

        if (Nameless)
        {
            return true;
        }

        if (!TryReadByte(out var identifier))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(expected, identifier);

        return TryReadString(out name);
    }

    private bool TryRead<T>(int length, out ReadOnlySpan<T> value) where T : struct
    {
        var slice = span[position..(position += length)];
        value = MemoryMarshal.Cast<byte, T>(slice);

        return true;
    }

    private bool TryReadByte(out byte value)
    {
        value = 0;

        if (sizeof(byte) > Remaining)
        {
            return false;
        }

        value = span[position++];

        return true;
    }

    private bool TryReadInteger(out int value)
    {
        value = 0;

        if (sizeof(int) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(int))];
        value = littleEndian ? BinaryPrimitives.ReadInt32LittleEndian(slice) : BinaryPrimitives.ReadInt32BigEndian(slice);

        return true;
    }

    private bool TryReadLong(out long value)
    {
        value = 0;

        if (sizeof(long) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(long))];
        value = littleEndian ? BinaryPrimitives.ReadInt64LittleEndian(slice) : BinaryPrimitives.ReadInt64BigEndian(slice);

        return true;
    }

    private bool TryReadString(out string value)
    {
        value = string.Empty;

        if (sizeof(ushort) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(ushort))];
        var length = littleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(slice) : BinaryPrimitives.ReadUInt16BigEndian(slice);

        if (length > Remaining)
        {
            return false;
        }

        value = Encoding.UTF8.GetString(span[position..(position += length)]);

        return true;
    }
}