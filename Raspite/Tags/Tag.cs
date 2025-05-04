namespace Raspite.Tags;

public abstract class Tag(string name = "")
{
    public abstract byte Identifier { get; }

    public string Name { get; set; } = name;
}

public abstract class Tag<T>(T value, string name = "") : Tag(name)
{
    public T Value => value;
}