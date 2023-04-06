namespace Raspite.Serializer.Tags;

public abstract class Tag
{
    public string Name { get; set; } = string.Empty;

    internal abstract byte Type { get; }
}

public abstract class Tag<T> : Tag
{
    public required T Value { get; set; }
}

public abstract class CollectionTag<T> : Tag
{
    public required T[] Children { get; set; }
}