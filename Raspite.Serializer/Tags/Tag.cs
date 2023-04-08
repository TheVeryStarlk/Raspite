namespace Raspite.Serializer.Tags;

public abstract class Tag
{
    public string Name { get; set; } = "";

    internal abstract byte Type { get; }
}

public abstract class Tag<T> : Tag
{
    public required T Value { get; set; }
}

public abstract class CollectionTag<T> : Tag
{
    public required T[] Children { get; set; }

    public T this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }
}