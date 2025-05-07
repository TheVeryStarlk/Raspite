using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal readonly ref struct BufferWriter(IBufferWriter<byte> writer, bool littleEndian)
{
    public void Write(ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    public void Write(byte value)
    {
        var span = writer.GetSpan(sizeof(byte));
        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    public void Write(short value)
    {
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

    public void Write(int value)
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

    public void Write(long value)
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

    public void Write(float value)
    {
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

    public void Write(double value)
    {
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

    public void Write(string value)
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

        if (written > total)
        {
            throw new BinaryTagSerializerException("Failed to write the string.");
        }

        writer.Advance(total);
    }
}