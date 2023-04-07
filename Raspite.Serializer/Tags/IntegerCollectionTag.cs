namespace Raspite.Serializer.Tags;

public sealed class IntegerCollectionTag : CollectionTag<int>
{
    internal override byte Type => 11;
}