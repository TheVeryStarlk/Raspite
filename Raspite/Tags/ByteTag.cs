namespace Raspite.Tags;

public sealed class ByteTag(byte value, string name = "") : Tag<byte>(value, name)
{
    public override byte Identifier => 1;
}