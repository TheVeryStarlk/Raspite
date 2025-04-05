namespace Raspite.Serializer.Tags;

public sealed record IntegerCollectionTag(int[] Children, string Name = "") : CollectionTag<int>(Children, Name)
{
    public override byte Identifier => 11;
}