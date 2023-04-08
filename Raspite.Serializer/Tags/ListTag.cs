namespace Raspite.Serializer.Tags;

public sealed class ListTag<T> : CollectionTag<T> where T : Tag
{
    internal override byte Type => 9;

    public T First(string name)
    {
        var tag = Children.First(tag => tag.Name == name);
        return tag;
    }
}