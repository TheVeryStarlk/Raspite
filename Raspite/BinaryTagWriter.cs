using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Raspite.Tags;

namespace Raspite;

public ref struct BinaryTagWriter(IBufferWriter<byte> buffer, bool littleEndian)
{
    public bool Nameless { get; set; }

    public void WriteEndTag()
    {
        WriteByte(Tag.End);
    }

    public void WriteByteTag(byte value, string name = "")
    {
        Write(Tag.Byte, name);
        WriteByte(value);
    }

    public void WriteShortTag(short value, string name = "")
    {
        Write(Tag.Short, name);

        var span = buffer.GetSpan(sizeof(short));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt16LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16BigEndian(span, value);
        }

        buffer.Advance(sizeof(short));
    }

    public void WriteIntegerTag(int value, string name = "")
    {
        Write(Tag.Integer, name);
        WriteInteger(value);
    }

    public void WriteLongTag(long value, string name = "")
    {
        Write(Tag.Long, name);
        WriteLong(value);
    }

    public void WriteFloatTag(float value, string name = "")
    {
        Write(Tag.Float, name);

        var span = buffer.GetSpan(sizeof(float));

        if (littleEndian)
        {
            BinaryPrimitives.WriteSingleLittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleBigEndian(span, value);
        }

        buffer.Advance(sizeof(float));
    }

    public void WriteDoubleTag(double value, string name = "")
    {
        Write(Tag.Double, name);

        var span = buffer.GetSpan(sizeof(double));

        if (littleEndian)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteDoubleBigEndian(span, value);
        }

        buffer.Advance(sizeof(double));
    }

    public void WriteStringTag(string value, string name = "")
    {
        Write(Tag.String, name);
        WriteString(value);
    }

    public void WriteListTag(byte identifier, int length, string name = "")
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(identifier, Tag.Longs);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Write(Tag.List, name);
        WriteByte(identifier);
        WriteInteger(length);

        Nameless = true;
    }

    public void WriteCompoundTag(string name = "")
    {
        Write(Tag.Compound, name);
        Nameless = false;
    }

    public void WriteBytesTag(ReadOnlySpan<byte> value, string name = "")
    {
        Write(Tag.Bytes, name);
        WriteInteger(value.Length);
        Write(value);
    }

    public void WriteIntegersTag(ReadOnlySpan<int> value, string name = "")
    {
        Write(Tag.Integers, name);
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

    public void WriteLongsTag(ReadOnlySpan<long> value, string name = "")
    {
        Write(Tag.Longs, name);
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

    private void Write(byte identifier, string name)
    {
        if (Nameless)
        {
            return;
        }

        WriteByte(identifier);
        WriteString(name);
    }

    private void Write(ReadOnlySpan<byte> value)
    {
        var span = buffer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        buffer.Advance(value.Length);
    }

    private void WriteByte(byte value)
    {
        var span = buffer.GetSpan(sizeof(byte));
        span[0] = value;

        buffer.Advance(sizeof(byte));
    }

    private void WriteInteger(int value)
    {
        var span = buffer.GetSpan(sizeof(int));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt32LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(span, value);
        }

        buffer.Advance(sizeof(int));
    }

    private void WriteLong(long value)
    {
        var span = buffer.GetSpan(sizeof(long));

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt64LittleEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteInt64BigEndian(span, value);
        }

        buffer.Advance(sizeof(long));
    }

    private void WriteString(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, ushort.MaxValue);

        var total = sizeof(ushort) + length;
        var span = buffer.GetSpan(total);

        if (littleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort) length);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, (ushort) length);
        }

        var written = Encoding.UTF8.GetBytes(value, span[sizeof(ushort)..]);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(written, total);

        buffer.Advance(total);
    }
}