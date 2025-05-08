using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Raspite;

public ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    public readonly int Remaining => span.Length - position;

    private readonly ReadOnlySpan<byte> span = span;

    private int position;
    private bool nameless;

    public bool TryReadEndTag()
    {
        if (!TryReadByte(out var identifier))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(Tags.End, identifier, nameof(identifier));

        return true;
    }

    public bool TryReadByteTag(out byte value, out string name)
    {
        value = 0;
        name = string.Empty;

        return TryRead(Tags.Byte, out name) && TryReadByte(out value);
    }

    public bool TryReadShortTag(out short value, out string name)
    {
        value = 0;
        name = string.Empty;

        if (!TryRead(Tags.Short, out name) || sizeof(short) > Remaining)
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
        name = string.Empty;

        return TryRead(Tags.Integer, out name) && TryReadInteger(out value);
    }

    public bool TryReadLongTag(out long value, out string name)
    {
        value = 0;
        name = string.Empty;

        return TryRead(Tags.Long, out name) && TryReadLong(out value);
    }

    public bool TryReadFloatTag(out float value, out string name)
    {
        value = 0;
        name = string.Empty;

        if (!TryRead(Tags.Float, out name) || sizeof(float) > Remaining)
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
        name = string.Empty;

        if (!TryRead(Tags.Double, out name) || sizeof(double) > Remaining)
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
        name = string.Empty;

        return TryRead(Tags.String, out name) && TryReadString(out value);
    }

    public bool TryReadListTag(out byte identifier, out int length, out string name)
    {
        identifier = 0;
        length = 0;
        name = string.Empty;

        if (!TryRead(Tags.List, out name))
        {
            return false;
        }

        nameless = true;

        return TryReadByte(out identifier) && TryReadInteger(out length);
    }

    public bool TryReadCompoundTag(out string name)
    {
        name = string.Empty;

        nameless = false;

        return TryRead(Tags.Compound, out name);
    }

    public bool TryReadByteCollectionTag(out ReadOnlySpan<byte> value, out string name)
    {
        value = default;
        name = string.Empty;

        if (!TryRead(Tags.ByteCollection, out name) || !TryReadInteger(out var length) || length > Remaining)
        {
            return false;
        }

        value = span[position..(position += length)];
        return true;
    }

    public bool TryReadIntegerCollectionTag(out ReadOnlySpan<int> value, out string name)
    {
        value = default;
        name = string.Empty;

        if (!TryRead(Tags.IntegerCollection, out name) || !TryReadInteger(out var length))
        {
            return false;
        }

        var actual = length * sizeof(int);

        if (actual > Remaining)
        {
            return false;
        }

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            var slice = span[position..(position += actual)];
            value = MemoryMarshal.Cast<byte, int>(slice);

            return true;
        }

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

    public bool TryReadLongCollectionTag(out ReadOnlySpan<long> value, out string name)
    {
        value = default;
        name = string.Empty;

        if (!TryRead(Tags.LongCollection, out name) || !TryReadInteger(out var length))
        {
            return false;
        }

        var actual = length * sizeof(long);

        if (actual > Remaining)
        {
            return false;
        }

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            var slice = span[position..(position += actual)];
            value = MemoryMarshal.Cast<byte, long>(slice);

            return true;
        }

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

        if (nameless)
        {
            return true;
        }

        if (!TryReadByte(out var identifier))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(expected, identifier, nameof(identifier));

        return TryReadString(out name);
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