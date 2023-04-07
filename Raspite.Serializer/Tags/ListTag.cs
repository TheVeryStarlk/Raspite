namespace Raspite.Serializer.Tags;

public sealed class ListTag<T> : CollectionTag<T> where T : Tag
{
    internal override byte Type => 9;
}