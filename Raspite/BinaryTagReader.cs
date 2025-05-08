using System.Buffers.Binary;
using System.Text;

namespace Raspite;

public ref struct BinaryTagReader(ReadOnlySpan<byte> span, bool littleEndian)
{
    public readonly int Remaining => span.Length - position;

    private readonly ReadOnlySpan<byte> span = span;

    private int position;
    private bool nameless;

    public bool TryReadEndTag()
    {
        return TryReadByte(out var identifier) && identifier is Tags.End;
    }

    public bool TryReadByteTag(out byte value, out string name)
    {
        value = 0;
        name = string.Empty;

        return TryRead(Tags.Byte, out name) && TryReadByte(out value);
    }

    public bool TryReadCompoundTag(out string name)
    {
        name = string.Empty;
        return TryReadString(out name);
    }

    // This should store the identifier and throw if a different identifier was read.
    public bool TryReadListTag(out byte identifier, out int length, out string name)
    {
        identifier = 0;
        length = 0;
        name = string.Empty;

        if (!TryRead(Tags.List, out name))
        {
            return false;
        }

        nameless = true;

        return TryReadByte(out identifier) && TryReadInteger(out length);
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

    private bool TryReadInteger(out int value)
    {
        value = 0;

        if (sizeof(int) > Remaining)
        {
            return false;
        }

        var slice = span[position..(position += sizeof(int))];
        value = littleEndian ? BinaryPrimitives.ReadInt32LittleEndian(slice) : BinaryPrimitives.ReadInt32BigEndian(slice);

        return true;
    }

    private bool TryReadString(out string value)
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