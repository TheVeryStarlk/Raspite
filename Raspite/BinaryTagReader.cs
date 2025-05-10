using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Raspite;

/// <summary>
/// Provides a reader for reading named binary tags (NBTs).
/// </summary>
/// <param name="span">The <see cref="ReadOnlySpan{T}"/> to read the named binary tags (NBTs) from.</param>
/// <param name="littleEndian">Whether to read the named binary tags (NBTs) as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
public ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    /// <summary>
    /// The total number of <see cref="byte"/>s left in the <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public readonly int Remaining => span.Length - position;

    /// <summary>
    /// The <see cref="ReadOnlySpan{T}"/> to read the named binary tags (NBTs) from.
    /// </summary>
    private readonly ReadOnlySpan<byte> span = span;

    /// <summary>
    /// The current position of the reader.
    /// </summary>
    private int position;

    /// <summary>
    /// Whether to read the tag's identifier and name (<c>true</c>) or not (<c>false</c>).
    /// </summary>
    /// <remarks>
    /// This is only <c>true</c> inside a <see cref="Tags.List"/>.
    /// </remarks>
    private bool nameless;

    /// <summary>
    /// Tries to read an <see cref="Tags.End"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    public bool TryReadEndTag()
    {
        if (!TryReadByte(out var identifier))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNotEqual(Tags.End, identifier);

        return true;
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Byte"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadByteTag(out byte value, out string name)
    {
        value = 0;
        return TryRead(Tags.Byte, out name) && TryReadByte(out value);
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Short"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadShortTag(out short value, out string name)
    {
        value = 0;

        if (!TryRead(Tags.Short, out name) || sizeof(short) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(short))];
        value = littleEndian ? BinaryPrimitives.ReadInt16LittleEndian(slice) : BinaryPrimitives.ReadInt16BigEndian(slice);

        return true;
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Integer"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadIntegerTag(out int value, out string name)
    {
        value = 0;
        return TryRead(Tags.Integer, out name) && TryReadInteger(out value);
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Long"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadLongTag(out long value, out string name)
    {
        value = 0;
        return TryRead(Tags.Long, out name) && TryReadLong(out value);
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Float"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadFloatTag(out float value, out string name)
    {
        value = 0;

        if (!TryRead(Tags.Float, out name) || sizeof(float) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(float))];
        value = littleEndian ? BinaryPrimitives.ReadSingleLittleEndian(slice) : BinaryPrimitives.ReadSingleBigEndian(slice);

        return true;
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.Double"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadDoubleTag(out double value, out string name)
    {
        value = 0;

        if (!TryRead(Tags.Double, out name) || sizeof(double) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(double))];
        value = littleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(slice) : BinaryPrimitives.ReadDoubleBigEndian(slice);

        return true;
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.String"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadStringTag(out string value, out string name)
    {
        value = string.Empty;
        return TryRead(Tags.String, out name) && TryReadString(out value);
    }

    /// <summary>
    /// Tries to read the starting of a <see cref="Tags.List"/>.
    /// </summary>
    /// <param name="identifier">The tag's identifier that the <see cref="Tags.List"/> contains.</param>
    /// <param name="length">The number of tags inside the <see cref="Tags.List"/>.</param>
    /// <param name="name">The tag's name.</param>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadListTag(out byte identifier, out int length, out string name)
    {
        identifier = 0;
        length = 0;

        if (!TryRead(Tags.List, out name) || !TryReadByte(out identifier) || !TryReadInteger(out length))
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);
        nameless = true;

        return true;
    }

    /// <summary>
    /// Tries to read the starting of a <see cref="Tags.Compound"/>.
    /// </summary>
    /// <param name="name">The tag's name.</param>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadCompoundTag(out string name)
    {
        nameless = false;
        return TryRead(Tags.Compound, out name);
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.ByteCollection"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadByteCollectionTag(out ReadOnlySpan<byte> value, out string name)
    {
        value = default;

        if (!TryRead(Tags.ByteCollection, out name) || !TryReadInteger(out var length) || length > Remaining)
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);
        value = span[position..(position += length)];

        return true;
    }

    /// <summary>
    /// Tries to read an <see cref="Tags.IntegerCollection"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadIntegerCollectionTag(out ReadOnlySpan<int> value, out string name)
    {
        value = default;

        if (!TryRead(Tags.IntegerCollection, out name))
        {
            return false;
        }

        // Fast path.
        if (BitConverter.IsLittleEndian == littleEndian)
        {
            return TryRead(sizeof(int), out value);
        }

        if (!TryReadInteger(out var length) || length * sizeof(int) > Remaining)
        {
            return false;
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

    /// <summary>
    /// Tries to read an <see cref="Tags.LongCollection"/>.
    /// </summary>
    /// <returns>
    /// Whether the tag was read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
    public bool TryReadLongCollectionTag(out ReadOnlySpan<long> value, out string name)
    {
        value = default;

        if (!TryRead(Tags.LongCollection, out name))
        {
            return false;
        }

        // Fast path.
        if (BitConverter.IsLittleEndian == littleEndian)
        {
            return TryRead(sizeof(long), out value);
        }

        if (!TryReadInteger(out var length) || length * sizeof(long) > Remaining)
        {
            return false;
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

    /// <summary>
    /// Tries to read the identifier and name of a tag.
    /// </summary>
    /// <param name="expected">The expected tag identifier.</param>
    /// <param name="name">The tag's name.</param>
    /// <returns>
    /// Whether the tag's identifier and name were read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    /// <remarks>
    /// The tag will not have a name if it were inside <see cref="Tags.List"/>.
    /// </remarks>
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

        ArgumentOutOfRangeException.ThrowIfNotEqual(expected, identifier);

        return TryReadString(out name);
    }

    /// <summary>
    /// Tries to read a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="size">The size of the type.</param>
    /// <param name="value">The <see cref="ReadOnlySpan{T}"/> to read.</param>
    /// <typeparam name="T">The type of the <see cref="ReadOnlySpan{T}"/>.</typeparam>
    /// <returns>
    /// Whether was the <see cref="ReadOnlySpan{T}"/> read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
    private bool TryRead<T>(int size, out ReadOnlySpan<T> value) where T : struct
    {
        value = default;

        if (!TryReadInteger(out var length) || length * size > Remaining)
        {
            return false;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(length);

        var slice = span[position..(position += length * size)];
        value = MemoryMarshal.Cast<byte, T>(slice);

        return true;
    }

    /// <summary>
    /// Tries to read a <see cref="byte"/>.
    /// </summary>
    /// <param name="value">The <see cref="byte"/> to read.</param>
    /// <returns>
    /// Whether was the <see cref="byte"/> read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
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

    /// <summary>
    /// Tries to read a <see cref="int"/>.
    /// </summary>
    /// <param name="value">The <see cref="int"/> to read.</param>
    /// <returns>
    /// Whether was the <see cref="int"/> read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
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

    /// <summary>
    /// Tries to read a <see cref="long"/>.
    /// </summary>
    /// <param name="value">The <see cref="long"/> to read.</param>
    /// <returns>
    /// Whether was the <see cref="long"/> read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
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

    /// <summary>
    /// Tries to read a <see cref="ushort"/>-prefixed <see cref="string"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to read.</param>
    /// <returns>
    /// Whether was the <see cref="string"/> read successfully (<c>true</c>) or not (<c>false</c>).
    /// </returns>
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