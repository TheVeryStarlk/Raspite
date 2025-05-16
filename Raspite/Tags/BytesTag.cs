namespace Raspite.Tags;

public sealed class BytesTag(byte[] value, string name = "") : Tag<byte[]>(value, name)
{
    public override byte Identifier => Bytes;
}