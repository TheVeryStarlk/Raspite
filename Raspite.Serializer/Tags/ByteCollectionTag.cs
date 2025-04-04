namespace Raspite.Serializer.Tags;

public sealed record ByteCollectionTag(byte[] Children, string Name = "") : CollectionTag<byte>
{
    public override byte Identifier => 7;
}