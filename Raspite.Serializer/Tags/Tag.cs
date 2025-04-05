namespace Raspite.Serializer.Tags;

public abstract record Tag(string Name)
{
    public abstract byte Identifier { get; }
}

public abstract record Tag<T>(T Value, string Name = "") : Tag(Name);

public abstract record CollectionTag<T>(T[] Children, string Name = "") : Tag(Name)
{
    public T this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }
}