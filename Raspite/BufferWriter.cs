using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal readonly ref struct BufferWriter(IBufferWriter<byte> writer)
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
        BinaryPrimitives.WriteInt16LittleEndian(span, value);

        writer.Advance(sizeof(short));
    }

    public void Write(int value)
    {
        var span = writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, value);

        writer.Advance(sizeof(int));
    }

    public void Write(long value)
    {
        var span = writer.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64LittleEndian(span, value);

        writer.Advance(sizeof(long));
    }

    public void Write(float value)
    {
        var span = writer.GetSpan(sizeof(float));
        BinaryPrimitives.WriteSingleLittleEndian(span, value);

        writer.Advance(sizeof(float));
    }

    public void Write(double value)
    {
        var span = writer.GetSpan(sizeof(double));
        BinaryPrimitives.WriteDoubleLittleEndian(span, value);

        writer.Advance(sizeof(double));
    }

    public void Write(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        var total = sizeof(ushort) + length;
        var span = writer.GetSpan(total);

        BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort) length);

        if (Encoding.UTF8.GetBytes(value, span[sizeof(ushort)..]) > total)
        {
            throw new BinaryTagSerializerException("Failed to write the string.");
        }

        writer.Advance(total);
    }
}