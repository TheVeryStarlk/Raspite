using System.Buffers.Binary;
using System.Text;

namespace Raspite;

public ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    public readonly int Remaining => span.Length - position;
    
    private readonly ReadOnlySpan<byte> span = span;

    private int position;
    private bool nameless;

    public bool TryReadByteTag(out byte value, out string name)
    {
        value = 0;
        name = string.Empty;

        return TryRead(Tag.Byte, out name) && !TryReadByte(out value);
    }

    private bool TryRead(byte identifier, out string name)
    {
        name = string.Empty;

        if (nameless)
        {
            return true;
        }

        if (!TryReadByte(out var expected))
        {
            return false;
        }

        return identifier == expected && TryReadString(out name);
    }

    private bool TryReadByte(out byte value)
    {
        value = 0;

        if (sizeof(byte) > Remaining)
        {
            return false;
        }

        value = span[position++];
        return true;
    }

    private bool TryReadString( out string value)
    {
        value = string.Empty;

        if (sizeof(ushort) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(ushort))];
        var length = littleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(slice) : BinaryPrimitives.ReadUInt16BigEndian(slice);

        if (length > Remaining)
        {
            return false;
        }

        value = Encoding.UTF8.GetString(span[position..(position += length)]);
        return true;
    }
}