using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Raspite.Tags;

namespace Raspite;

/// <summary>
/// Provides high-performance API for writing binary <see cref="Tag"/>s (NBTs).
/// </summary>
/// <param name="writer">The destination for writing the binary <see cref="Tag"/>s</param>
/// <param name="littleEndian">Whether to write as little-endian (<c>true</c>) or big-endian (<c>false</c>).</param>
/// <param name="variablePrefix">Whether to use Bedrock's variable-integer types <c>true</c> or use Java's <c>false</c> for prefixes.</param>
public ref struct BinaryTagWriter(IBufferWriter<byte> writer, bool littleEndian, bool variablePrefix)
{
    /// <summary>
    /// Whether to write the <see cref="Tag"/>'s identifier and name properties (<c>true</c>) or not (<c>false</c>).
    /// </summary>
    /// <remarks>
    /// Only <c>true</c> if inside a <see cref="ListTag"/>.
    /// </remarks>
    private bool nameless;

    /// <summary>
    /// Writes an end <see cref="Tag"/>.
    /// </summary>
    /// <remarks>
    /// Used only to end a <see cref="CompoundTag"/> or as an identifier in a <see cref="ListTag"/>.
    /// </remarks>
    public void WriteEndTag()
    {
        WriteByte(Tag.End);
    }

    /// <summary>
    /// Writes a <see cref="ByteTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteByteTag(byte value, string name = "")
    {
        Write(Tag.Byte, name);
        WriteByte(value);
    }

    /// <summary>
    /// Writes a <see cref="ShortTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
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
    /// Writes an <see cref="IntegerTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteIntegerTag(int value, string name = "")
    {
        Write(Tag.Integer, name);
        WriteInteger(value);
    }

    /// <summary>
    /// Writes a <see cref="LongTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteLongTag(long value, string name = "")
    {
        Write(Tag.Long, name);
        WriteLong(value);
    }

    /// <summary>
    /// Writes a <see cref="FloatTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
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
    /// Writes a <see cref="DoubleTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
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
    /// Writes a <see cref="ByteCollectionTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteByteCollectionTag(ReadOnlySpan<byte> value, string name = "")
    {
        Write(Tag.ByteCollection, name);
        WriteInteger(value.Length);
        Write(value);
    }

    /// <summary>
    /// Writes a <see cref="StringTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteStringTag(string value, string name = "")
    {
        Write(Tag.String, name);
        WriteString(value);
    }

    /// <summary>
    /// Writes a <see cref="ListTag"/>.
    /// </summary>
    /// <param name="identifier">The <see cref="Tag"/>s the <see cref="ListTag"/> contains.</param>
    /// <param name="length">The amount of tags inside the <see cref="ListTag"/>.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteListTag(byte identifier, int length, string name = "")
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Tag.LongCollection, identifier);

        Write(Tag.List, name);

        nameless = true;

        WriteByte(identifier);
        WriteInteger(length);
    }

    /// <summary>
    /// Writes a <see cref="CompoundTag"/>.
    /// </summary>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    public void WriteCompoundTag(string name = "")
    {
        nameless = false;
        Write(Tag.Compound, name);
    }

    /// <summary>
    /// Writes an <see cref="IntegerCollectionTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
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
    /// Writes a <see cref="LongCollectionTag"/>.
    /// </summary>
    /// <param name="value">The <see cref="Tag"/>'s value.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
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
    /// Writes a <see cref="Tag"/>.
    /// </summary>
    /// <param name="identifier">The <see cref="Tag"/>'s identifier.</param>
    /// <param name="name">The <see cref="Tag"/>'s name.</param>
    private void Write(byte identifier, string name)
    {
        if (nameless)
        {
            return;
        }

        WriteByte(identifier);
        WriteString(name);
    }

    /// <summary>
    /// Copies the <see cref="value"/> to the <see cref="IBufferWriter{T}"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlySpan{T}"/> to copy.</param>
    private void Write(ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    /// <summary>
    /// Writes a single <see cref="byte"/>.
    /// </summary>
    /// <param name="value">The <see cref="byte"/> to write.</param>
    private void WriteByte(byte value)
    {
        var span = writer.GetSpan(sizeof(byte));
        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    /// <summary>
    /// Writes a single <see cref="int"/>.
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
    /// Writes a single <see cref="long"/>.
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
    /// <param name="value">The <see cref="ushort"/>-prefixed <see cref="string"/> to write.</param>
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