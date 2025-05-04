using System.Buffers.Binary;
using System.Text;

namespace Raspite;

internal ref struct SpanWriter(Span<byte> span, bool littleEndian)
{
    private Span<byte> span = span;
    private int index;

    public void Write(ReadOnlySpan<byte> value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + value.Length,
            span.Length,
            "Reached the end of the buffer.");

        value.CopyTo(span[index..(index += value.Length)]);
    }

    public void Write(byte value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(byte),
            span.Length,
            "Reached the end of the buffer.");

        span[index++] = value;
    }

    public void Write(short value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(short),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[index..(index += sizeof(short))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt16LittleEndian(slice, value);
        }
        else
        {
            BinaryPrimitives.WriteInt16BigEndian(slice, value);
        }
    }

    public void Write(int value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(int),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[index..(index += sizeof(int))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt32LittleEndian(slice, value);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(slice, value);
        }
    }

    public void Write(long value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(long),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[index..(index += sizeof(long))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteInt64LittleEndian(slice, value);
        }
        else
        {
            BinaryPrimitives.WriteInt64BigEndian(slice, value);
        }
    }

    public void Write(float value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(float),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[index..(index += sizeof(float))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteSingleLittleEndian(slice, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleBigEndian(slice, value);
        }
    }

    public void Write(double value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + sizeof(double),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[index..(index += sizeof(double))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteDoubleLittleEndian(slice, value);
        }
        else
        {
            BinaryPrimitives.WriteDoubleBigEndian(slice, value);
        }
    }

    public void Write(string value)
    {
        var source = Encoding.UTF8.GetBytes(value);
        var length = (ushort) source.Length;

        BinaryTagSerializerException.ThrowIfGreaterThan(
            index + length + sizeof(ushort),
            span.Length,
            "Reached the end of the buffer.");

        BinaryTagSerializerException.ThrowIfGreaterThan(
            length,
            ushort.MaxValue,
            "String is too big.");

        var slice = span[index..(index += sizeof(ushort))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(slice, length);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(slice, length);
        }

        source.CopyTo(span[index..(index += value.Length)]);
    }
}