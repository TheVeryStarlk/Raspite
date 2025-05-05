using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal static class BufferWriterExtensions
{
    public static void Write(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);

        BinaryTagSerializerException.ThrowIfGreaterThan(
            value.Length,
            span.Length,
            "Reached the end of the buffer.");

        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    public static void Write(this IBufferWriter<byte> writer, byte value)
    {
        var span = writer.GetSpan(sizeof(byte));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(byte),
            span.Length,
            "Reached the end of the buffer.");

        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    public static void Write(this IBufferWriter<byte> writer, short value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(short));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(short),
            span.Length,
            "Reached the end of the buffer.");

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

    public static void Write(this IBufferWriter<byte> writer, int value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(int));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(int),
            span.Length,
            "Reached the end of the buffer.");

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

    public static void Write(this IBufferWriter<byte> writer, long value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(long));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(long),
            span.Length,
            "Reached the end of the buffer.");

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

    public static void Write(this IBufferWriter<byte> writer, float value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(float));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(float),
            span.Length,
            "Reached the end of the buffer.");

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

    public static void Write(this IBufferWriter<byte> writer, double value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(double));

        BinaryTagSerializerException.ThrowIfGreaterThan(
            sizeof(double),
            span.Length,
            "Reached the end of the buffer.");

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

    public static void Write(this IBufferWriter<byte> writer, string value, bool littleEndian)
    {
        var length = Encoding.UTF8.GetByteCount(value);

        BinaryTagSerializerException.ThrowIfGreaterThan(
            length,
            ushort.MaxValue,
            "String is too big.");

        var total = (ushort) (sizeof(ushort) + length);
        var span = writer.GetSpan(total);

        BinaryTagSerializerException.ThrowIfGreaterThan(
            total,
            span.Length,
            "Reached the end of the buffer.");

        if (littleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span, total);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, total);
        }

        var written = Encoding.UTF8.GetBytes(value, span[sizeof(ushort)..]);

        BinaryTagSerializerException.ThrowIfGreaterThan(
            written,
            total,
            "Could not write the string.");

        writer.Advance(total);
    }
}