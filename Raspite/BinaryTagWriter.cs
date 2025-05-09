using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace Raspite;

public ref struct BinaryTagWriter(IBufferWriter<byte> writer, bool littleEndian)
{
    private bool nameless;

    public readonly void WriteEndTag()
    {
        WriteByte(Tags.End);
    }

    public readonly void WriteByteTag(byte value, string name = "")
    {
        Write(Tags.Byte, name);
        WriteByte(value);
    }

    public readonly void WriteShortTag(short value, string name = "")
    {
        Write(Tags.Short, name);

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

    public readonly void WriteIntegerTag(int value, string name = "")
    {
        Write(Tags.Integer, name);
        WriteInteger(value);
    }

    public readonly void WriteLongTag(long value, string name = "")
    {
        Write(Tags.Long, name);
        WriteLong(value);
    }

    public readonly void WriteFloatTag(float value, string name = "")
    {
        Write(Tags.Float, name);

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

    public readonly void WriteDoubleTag(double value, string name = "")
    {
        Write(Tags.Double, name);

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

    public readonly void WriteStringTag(string value, string name = "")
    {
        Write(Tags.String, name);
        WriteString(value);
    }

    public void WriteListTag(byte identifier, int length, string name = "")
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(identifier, Tags.LongCollection);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Write(Tags.List, name);
        WriteByte(identifier);
        WriteInteger(length);

        nameless = true;
    }

    public void WriteCompoundTag(string name = "")
    {
        Write(Tags.Compound, name);
        nameless = false;
    }

    public readonly void WriteByteCollectionTag(ReadOnlySpan<byte> value, string name = "")
    {
        Write(Tags.ByteCollection, name);
        WriteInteger(value.Length);
        Write(value);
    }

    public readonly void WriteIntegerCollectionTag(ReadOnlySpan<int> value, string name = "")
    {
        Write(Tags.IntegerCollection, name);
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

    public readonly void WriteLongCollectionTag(ReadOnlySpan<long> value, string name = "")
    {
        Write(Tags.LongCollection, name);
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

    private readonly void Write(byte identifier, string name)
    {
        if (nameless)
        {
            return;
        }

        WriteByte(identifier);
        WriteString(name);
    }

    private readonly void Write(ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    private readonly void WriteByte(byte value)
    {
        var span = writer.GetSpan(sizeof(byte));
        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    private readonly void WriteInteger(int value)
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

    private readonly void WriteLong(long value)
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

    private readonly void WriteString(string value)
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