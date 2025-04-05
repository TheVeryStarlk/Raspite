namespace Raspite.Serializer.Tags;

public sealed record ByteCollectionTag(byte[] Children, string Name = "") : CollectionTag<byte>(Children, Name)
{
    public override byte Identifier => 7;
}