namespace Raspite.Serializer.Tags;

public abstract record Tag
{
    public abstract byte Identifier { get; }

    public string Name { get; set; } = string.Empty;
}

public abstract record Tag<T> : Tag
{
    public required T Value { get; set; }
}

public abstract record CollectionTag<T> : Tag
{
    public required T[] Children { get; set; }

    public T this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }
}