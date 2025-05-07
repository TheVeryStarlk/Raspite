using System.Buffers;
using System.Runtime.InteropServices;

namespace Raspite;

public ref struct BinaryTagWriter(IBufferWriter<byte> writer, bool littleEndian, bool variablePrefix)
{
    private bool nameless;

    public void WriteEnd()
    {
        writer.WriteByte(Tag.End);
    }

    public void WriteByte(byte value, string name = "")
    {
        Write(Tag.Byte, name);
        writer.WriteByte(value);
    }

    public void WriteShort(short value, string name = "")
    {
        Write(Tag.Short, name);
        writer.WriteShort(value, littleEndian);
    }

    public void WriteInteger(int value, string name = "")
    {
        Write(Tag.Integer, name);
        writer.WriteInteger(value, littleEndian);
    }

    public void WriteLong(long value, string name = "")
    {
        Write(Tag.Long, name);
        writer.WriteLong(value, littleEndian);
    }

    public void WriteFloat(float value, string name = "")
    {
        Write(Tag.Float, name);
        writer.WriteFloat(value, littleEndian);
    }

    public void WriteDouble(double value, string name = "")
    {
        Write(Tag.Double, name);
        writer.WriteDouble(value, littleEndian);
    }

    public void WriteByteCollection(ReadOnlySpan<byte> value, string name = "")
    {
        Write(Tag.ByteCollection, name);

        writer.WriteInteger(value.Length, littleEndian);
        writer.Write(value);
    }

    public void WriteString(string value, string name = "")
    {
        Write(Tag.String, name);
        writer.WriteString(value, littleEndian);
    }

    public void WriteList(byte identifier, int length, string name = "")
    {
        Write(Tag.List, name);

        nameless = true;

        writer.WriteByte(identifier);
        writer.WriteInteger(length, littleEndian);
    }

    public void WriteCompound(string name = "")
    {
        nameless = false;
        Write(Tag.Compound, name);
    }

    public void WriteIntegerCollection(ReadOnlySpan<int> value, string name = "")
    {
        Write(Tag.IntegerCollection, name);

        writer.WriteInteger(value.Length, littleEndian);
        writer.Write(MemoryMarshal.AsBytes(value));
    }

    public void WriteLongCollection(ReadOnlySpan<long> value, string name = "")
    {
        Write(Tag.LongCollection, name);

        writer.WriteInteger(value.Length, littleEndian);
        writer.Write(MemoryMarshal.AsBytes(value));
    }

    private void Write(byte identifier, string name)
    {
        if (nameless)
        {
            return;
        }

        writer.WriteByte(identifier);
        writer.WriteString(name, littleEndian);
    }
}

internal static class Tag
{
    public const byte End = 0;
    public const byte Byte = 1;
    public const byte Short = 2;
    public const byte Integer = 3;
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