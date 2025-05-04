namespace Raspite.Tags;

public sealed class ByteCollectionTag(byte[] value, string name = "") : Tag<byte[]>(value, name)
{
    public override byte Identifier => 7;
}