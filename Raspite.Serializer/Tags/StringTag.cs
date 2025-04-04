namespace Raspite.Serializer.Tags;

public sealed record StringTag : Tag<string>
{
    public override byte Identifier => 8;

    private StringTag()
    {
    }

    public static StringTag Create(string value, string name = "")
    {
        return new StringTag
        {
            Name = name,
            Value = value
        };
    }
}