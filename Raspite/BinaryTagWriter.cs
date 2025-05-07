using System.Buffers;
using System.Runtime.InteropServices;

namespace Raspite;

public ref struct BinaryTagWriter(IBufferWriter<byte> writer)
{
    private bool nameless;

    public void WriteEnd()
    {
        writer.WriteByte(Tag.End);
    }

    public void WriteByte(string name, byte value)
    {
        Write(Tag.Byte, name);
        writer.WriteByte(value);
    }

    public void WriteShort(string name, short value)
    {
        Write(Tag.Short, name);
        writer.WriteShort(value);
    }

    public void WriteInteger(string name, short value)
    {
        Write(Tag.Integer, name);
        writer.WriteInteger(value);
    }

    public void WriteLong(string name, short value)
    {
        Write(Tag.Long, name);
        writer.WriteLong(value);
    }

    public void WriteFloat(string name, short value)
    {
        Write(Tag.Float, name);
        writer.WriteFloat(value);
    }

    public void WriteDouble(string name, short value)
    {
        Write(Tag.Double, name);
        writer.WriteDouble(value);
    }

    public void WriteByteCollection(string name, ReadOnlySpan<byte> value)
    {
        Write(Tag.ByteCollection, name);

        writer.WriteInteger(value.Length);
        writer.Write(value);
    }

    public void WriteString(string name, string value)
    {
        Write(Tag.String, name);
        writer.WriteString(value);
    }

    public void WriteList(string name, byte identifier, int length)
    {
        Write(Tag.List, name);

        nameless = true;

        writer.WriteByte(identifier);
        writer.WriteInteger(length);
    }

    public void WriteCompound(string name)
    {
        nameless = false;
        Write(Tag.Compound, name);
    }

    public void WriteIntegerCollection(string name, ReadOnlySpan<int> value)
    {
        Write(Tag.IntegerCollection, name);

        writer.WriteInteger(value.Length);
        writer.Write(MemoryMarshal.AsBytes(value));
    }

    public void WriteLongCollection(string name, ReadOnlySpan<long> value)
    {
        Write(Tag.LongCollection, name);

        writer.WriteInteger(value.Length);
        writer.Write(MemoryMarshal.AsBytes(value));
    }

    private void Write(byte identifier, string name)
    {
        if (nameless)
        {
            return;
        }

        writer.WriteByte(identifier);
        writer.WriteString(name);
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