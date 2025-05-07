using System.Buffers;
using System.Runtime.InteropServices;
using Raspite.Tags;

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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Tag.LongCollection, identifier);

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

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            writer.Write(MemoryMarshal.AsBytes(value));
            return;
        }

        foreach (var item in value)
        {
            writer.WriteInteger(item, littleEndian);
        }
    }

    public void WriteLongCollection(ReadOnlySpan<long> value, string name = "")
    {
        Write(Tag.LongCollection, name);

        writer.WriteInteger(value.Length, littleEndian);

        if (BitConverter.IsLittleEndian == littleEndian)
        {
            writer.Write(MemoryMarshal.AsBytes(value));
            return;
        }

        foreach (var item in value)
        {
            writer.WriteLong(item, littleEndian);
        }
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