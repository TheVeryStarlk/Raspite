using System.Buffers;

namespace Raspite;

internal readonly ref struct BinaryTagWriter(IBufferWriter<byte> writer)
{
    public void WriteCompound(string name)
    {
        writer.WriteByte(Tag.Compound);
        writer.WriteString(name);
    }

    public void WriteEnd()
    {
        writer.WriteByte(Tag.End);
    }

    public void WriteString(string name, string value)
    {
        writer.WriteByte(Tag.String);
        writer.WriteString(name);
        writer.WriteString(value);
    }
}

internal static class Tag
{
    public const byte End = 0;
    public const byte Byte = 1;
    public const byte Short = 2;
    public const byte Int = 3;
    public const byte Long = 4;
    public const byte Float = 5;
    public const byte Double = 6;
    public const byte ByteCollection = 7;
    public const byte String = 8;
    public const byte List = 9;
    public const byte Compound = 10;
    public const byte IntegerCollection = 11;
    public const byte LongCollection = 12;
}