namespace Raspite.Serializer.Tags;

public sealed record ListTag : CollectionTag<Tag>
{
    public override byte Identifier => 9;

    private ListTag()
    {
    }

    public static ListTag Create(Tag[] children, string name = "")
    {
        return new ListTag
        {
            Name = name,
            Children = children
        };
    }
}