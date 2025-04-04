namespace Raspite.Serializer.Tags;

public sealed record IntegerCollectionTag(int[] Children, string Name = "") : CollectionTag<int>
{
    public override byte Identifier => 11;
}