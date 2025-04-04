namespace Raspite.Serializer.Tags;

public sealed record ByteCollectionTag : CollectionTag<byte>
{
    public override byte Identifier => 7;

    private ByteCollectionTag()
    {
    }

    public static ByteCollectionTag Create(byte[] children, string name = "")
    {
        return new ByteCollectionTag
        {
            Name = name,
            Children = children
        };
    }
}