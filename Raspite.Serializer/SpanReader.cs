using System.Buffers.Binary;
using System.Text;

namespace Raspite.Serializer;

internal ref struct SpanReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    private int position;

    private readonly ReadOnlySpan<byte> span = span;

    public ReadOnlySpan<byte> Read(int length)
    {
        return span[position..(position += length)];
    }

    public byte ReadByte()
    {
        return span[position++];
    }

    public bool ReadBoolean()
    {
        var current = ReadByte();

        if (current > 1)
        {
            throw new BinaryTagSerializerException("Invalid boolean value.");
        }

        return current is 0;
    }

    public short ReadShort()
    {
        return littleEndian
            ? BinaryPrimitives.ReadInt16LittleEndian(span[position..(position += sizeof(short))])
            : BinaryPrimitives.ReadInt16BigEndian(span[position..(position += sizeof(short))]);
    }

    public ushort ReadUnsignedShort()
    {
        return littleEndian
            ? BinaryPrimitives.ReadUInt16LittleEndian(span[position..(position += sizeof(ushort))])
            : BinaryPrimitives.ReadUInt16BigEndian(span[position..(position += sizeof(ushort))]);
    }

    public float ReadFloat()
    {
        return littleEndian
            ? BinaryPrimitives.ReadSingleLittleEndian(span[position..(position += sizeof(float))])
            : BinaryPrimitives.ReadSingleBigEndian(span[position..(position += sizeof(float))]);
    }

    public double ReadDouble()
    {
        return littleEndian
            ? BinaryPrimitives.ReadDoubleLittleEndian(span[position..(position += sizeof(double))])
            : BinaryPrimitives.ReadDoubleBigEndian(span[position..(position += sizeof(double))]);
    }

    public long ReadLong()
    {
        return littleEndian
            ? BinaryPrimitives.ReadInt64LittleEndian(span[position..(position += sizeof(long))])
            : BinaryPrimitives.ReadInt64BigEndian(span[position..(position += sizeof(long))]);
    }

    public string ReadString()
    {
        var length = ReadUnsignedShort();
        return Encoding.UTF8.GetString(Read(length));
    }
}