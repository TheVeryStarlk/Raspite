using System.Buffers.Binary;
using System.Text;

namespace Raspite.Serializer;

internal ref struct SpanWriter(Span<byte> span, bool littleEndian)
{
    private int position;

    private readonly Span<byte> span = span;

    public void Write(ReadOnlySpan<byte> value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            position + value.Length,
            span.Length,
            "Reached the end of the buffer.");

        value.CopyTo(span[position..(position += value.Length)]);
    }

    public void Write(byte value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            position + sizeof(byte),
            span.Length,
            "Reached the end of the buffer.");

        span[position++] = value;
    }

    public void Write(short value)
    {
        BinaryTagSerializerException.ThrowIfGreaterThan(
            position + sizeof(short),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[position..(position += sizeof(short))];

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
            position + sizeof(int),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[position..(position += sizeof(int))];

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
            position + sizeof(long),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[position..(position += sizeof(long))];

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
            position + sizeof(float),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[position..(position += sizeof(float))];

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
            position + sizeof(double),
            span.Length,
            "Reached the end of the buffer.");

        var slice = span[position..(position += sizeof(double))];

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
            position + length + sizeof(ushort),
            span.Length,
            "Reached the end of the buffer.");

        BinaryTagSerializerException.ThrowIfGreaterThan(
            length,
            ushort.MaxValue,
            "String is too big.");

        var slice = span[position..(position += sizeof(ushort))];

        if (littleEndian)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(slice, length);
        }
        else
        {
            BinaryPrimitives.WriteUInt16BigEndian(slice, length);
        }

        source.CopyTo(span[position..(position += value.Length)]);
    }
}