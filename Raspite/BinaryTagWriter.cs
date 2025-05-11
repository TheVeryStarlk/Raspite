using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Provides a writer for writing named binary tags (NBTs).
/// </summary>
/// <param name="writer">The destination for writing the named binary tags (NBTs).</param>
/// <param name="littleEndian">Whether to write the named binary tags (NBTs) as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
public ref struct BinaryTagWriter(IBufferWriter<byte> writer, bool littleEndian)
{
    /// <summary>
    /// Whether to write the tag's identifier and name (<c>true</c>) or not (<c>false</c>).
    /// </summary>
    /// <remarks>
    /// This is only <c>true</c> inside a <see cref="List{T}"/>.
    /// </remarks>
    private bool nameless;

    /// <summary>
    /// The amount of tags that are inside a possibly current <see cref="List{T}"/>.
    /// </summary>
    private int left;

    /// <summary>
    /// Writes an <see cref="Tag.End"/>.
    /// </summary>
    /// <remarks>
    /// Used to close a <see cref="Tag.Compound"/>.
    /// </remarks>
    public void WriteEndTag()
    {
        if (left > 0)
        {
            nameless = true;
        }

        WriteByte(Tag.End);
    }

    /// <summary>
    /// Writes a <see cref="Tag.Byte"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteByteTag(byte value, string name = "")
    {
        Write(Tag.Byte, name);
        WriteByte(value);
    }

    /// <summary>
    /// Writes a <see cref="Tag.Short"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteShortTag(short value, string name = "")
    {
        Write(Tag.Short, name);

        var span = writer.GetSpan(sizeof(short));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt16LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16BigEndian(span, value);
        }

        writer.Advance(sizeof(short));
    }

    /// <summary>
    /// Writes a <see cref="Tag.Integer"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteIntegerTag(int value, string name = "")
    {
        Write(Tag.Integer, name);
        WriteInteger(value);
    }

    /// <summary>
    /// Writes a <see cref="Tag.Long"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteLongTag(long value, string name = "")
    {
        Write(Tag.Long, name);
        WriteLong(value);
    }

    /// <summary>
    /// Writes a <see cref="Tag.Float"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteFloatTag(float value, string name = "")
    {
        Write(Tag.Float, name);

        var span = writer.GetSpan(sizeof(float));

        if (littleEndian)
        {
            BinaryPrimitives.WriteSingleLittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleBigEndian(span, value);
        }

        writer.Advance(sizeof(float));
    }

    /// <summary>
    /// Writes a <see cref="Tag.Double"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteDoubleTag(double value, string name = "")
    {
        Write(Tag.Double, name);

        var span = writer.GetSpan(sizeof(double));

        if (littleEndian)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
        }

        writer.Advance(sizeof(double));
    }

    /// <summary>
    /// Writes a <see cref="Tag.String"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteStringTag(string value, string name = "")
    {
        Write(Tag.String, name);
        WriteString(value);
    }

    /// <summary>
    /// Writes the starting of a <see cref="Tag.List"/>.
    /// </summary>
    /// <param name="identifier">The tag's identifier that the <see cref="Tag.List"/> contains.</param>
    /// <param name="length">The number of tags inside the <see cref="Tag.List"/>.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteListTag(byte identifier, int length, string name = "")
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(identifier, Tag.LongCollection);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Write(Tag.List, name);
        WriteByte(identifier);
        WriteInteger(length);

        nameless = true;
        left = length;
    }

    /// <summary>
    /// Writes the starting of a <see cref="Tag.Compound"/>.
    /// </summary>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteCompoundTag(string name = "")
    {
        Write(Tag.Compound, name);
        nameless = false;
    }

    /// <summary>
    /// Writes a <see cref="Tag.ByteCollection"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteByteCollectionTag(ReadOnlySpan<byte> value, string name = "")
    {
        Write(Tag.ByteCollection, name);
        WriteInteger(value.Length);
        Write(value);
    }

    /// <summary>
    /// Writes a <see cref="Tag.IntegerCollection"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteIntegerCollectionTag(ReadOnlySpan<int> value, string name = "")
    {
        Write(Tag.IntegerCollection, name);
        WriteInteger(value.Length);

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            Write(MemoryMarshal.AsBytes(value));
            return;
        }

        foreach (var item in value)
        {
            WriteInteger(item);
        }
    }

    /// <summary>
    /// Writes a <see cref="Tag.LongCollection"/>.
    /// </summary>
    /// <param name="value">The tag's value.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    public void WriteLongCollectionTag(ReadOnlySpan<long> value, string name = "")
    {
        Write(Tag.LongCollection, name);
        WriteInteger(value.Length);

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            Write(MemoryMarshal.AsBytes(value));
            return;
        }

        foreach (var item in value)
        {
            WriteLong(item);
        }
    }

    /// <summary>
    /// Writes the identifier and name of a tag.
    /// </summary>
    /// <param name="identifier">The tag's identifier.</param>
    /// <param name="name">The tag's name.</param>
    /// <remarks>
    /// The tag's name will not be written if it were inside a <see cref="Tag.List"/>.
    /// </remarks>
    private void Write(byte identifier, string name)
    {
        if (nameless)
        {
            left -= 1;

            if (left > 0)
            {
                return;
            }

            nameless = false;
            left = 0;

            return;
        }

        WriteByte(identifier);
        WriteString(name);
    }

    /// <summary>
    /// Copies a <see cref="ReadOnlySpan{T}"/> to the <see cref="IBufferWriter{T}"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlySpan{T}"/> to copy.</param>
    private void Write(ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    /// <summary>
    /// Writes a <see cref="byte"/>.
    /// </summary>
    /// <param name="value">The <see cref="byte"/> to write.</param>
    private void WriteByte(byte value)
    {
        var span = writer.GetSpan(sizeof(byte));
        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    /// <summary>
    /// Writes an <see cref="int"/>.
    /// </summary>
    /// <param name="value">The <see cref="int"/> to write.</param>
    private void WriteInteger(int value)
    {
        var span = writer.GetSpan(sizeof(int));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt32LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(span, value);
        }

        writer.Advance(sizeof(int));
    }

    /// <summary>
    /// Writes a <see cref="long"/>.
    /// </summary>
    /// <param name="value">The <see cref="long"/> to write.</param>
    private void WriteLong(long value)
    {
        var span = writer.GetSpan(sizeof(long));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt64LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt64BigEndian(span, value);
        }

        writer.Advance(sizeof(long));
    }

    /// <summary>
    /// Writes a <see cref="ushort"/>-prefixed <see cref="string"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to write.</param>
    private void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        var total = sizeof(ushort) + length;
        var span = writer.GetSpan(total);

        if (littleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort) length);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, (ushort) length);
        }

        var written = Encoding.UTF8.GetBytes(value, span[sizeof(ushort)..]);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(written, total, nameof(written));

        writer.Advance(total);
    }
}