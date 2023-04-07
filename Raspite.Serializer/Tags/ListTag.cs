namespace Raspite.Serializer.Tags;

public sealed class ListTag<T> : CollectionTag<T>
{
    internal override byte Type => 9;
}