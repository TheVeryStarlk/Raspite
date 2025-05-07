using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal static class BufferWriterExtensions
{
    public static void Write(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);
        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    public static void Write(this IBufferWriter<byte> writer, byte value)
    {
        var span = writer.GetSpan(sizeof(byte));
        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    public static void Write(this IBufferWriter<byte> writer, short value)
    {
        var span = writer.GetSpan(sizeof(short));
        BinaryPrimitives.WriteInt16LittleEndian(span, value);

        writer.Advance(sizeof(short));
    }

    public static void Write(this IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32LittleEndian(span, value);

        writer.Advance(sizeof(int));
    }

    public static void Write(this IBufferWriter<byte> writer, long value)
    {
        var span = writer.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64LittleEndian(span, value);

        writer.Advance(sizeof(long));
    }

    public static void Write(this IBufferWriter<byte> writer, float value)
    {
        var span = writer.GetSpan(sizeof(float));
        BinaryPrimitives.WriteSingleLittleEndian(span, value);

        writer.Advance(sizeof(float));
    }

    public static void Write(this IBufferWriter<byte> writer, double value)
    {
        var span = writer.GetSpan(sizeof(double));
        BinaryPrimitives.WriteDoubleLittleEndian(span, value);

        writer.Advance(sizeof(double));
    }

    public static void Write(this IBufferWriter<byte> writer, string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        var total = sizeof(ushort) + length;
        var span = writer.GetSpan(total);

        BinaryPrimitives.WriteUInt16LittleEndian(span, (ushort) length);

        var written = Encoding.UTF8.GetBytes(value, span[sizeof(ushort)..]);

        if (written > total)
        {
            throw new BinaryTagSerializerException("Failed to write the string.");
        }

        writer.Advance(total);
    }
}