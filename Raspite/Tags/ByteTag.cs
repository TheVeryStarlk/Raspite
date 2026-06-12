namespace Raspite.Tags;

public sealed class ByteTag(byte value, string name = "") : Tag<byte>(value, name)
{
    public ByteTag(bool value, string name = "") : this((byte) (value ? 1 : 0), name)
    {
    }

    public override byte Identifier => Byte;
}