using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal static class BufferWriterExtensions
{
    public static void Write(this IBufferWriter<byte> writer, ReadOnlySpan<byte> value)
    {
        var span = writer.GetSpan(value.Length);

        if (value.Length > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

        value.CopyTo(span[..value.Length]);

        writer.Advance(value.Length);
    }

    public static void Write(this IBufferWriter<byte> writer, byte value)
    {
        var span = writer.GetSpan(sizeof(byte));

        if (sizeof(byte) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

        span[0] = value;

        writer.Advance(sizeof(byte));
    }

    public static void Write(this IBufferWriter<byte> writer, short value, bool littleEndian)
    {
        var span = writer.GetSpan(sizeof(short));

        if (sizeof(short) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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

        if (sizeof(int) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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

        if (sizeof(long) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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

        if (sizeof(float) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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

        if (sizeof(double) > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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

        if (length > ushort.MaxValue)
        {
            throw new BinaryTagSerializerException("String is too big.");
        }

        var total = sizeof(ushort) + length;
        var span = writer.GetSpan(total);

        if (total > span.Length)
        {
            throw new BinaryTagSerializerException("Reached the end of the buffer.");
        }

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