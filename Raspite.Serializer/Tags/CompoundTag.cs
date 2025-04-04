namespace Raspite.Serializer.Tags;

public sealed record CompoundTag : CollectionTag<Tag>
{
    public override byte Identifier => 10;

    private CompoundTag()
    {
    }

    public static CompoundTag Create(Tag[] children, string name = "")
    {
        return new CompoundTag
        {
            Name = name,
            Children = children
        };
    }

    public T First<T>(string name = "") where T : Tag
    {
        var tag = Children.First(tag =>
        {
            var typeMatches = typeof(T) == tag.GetType();

            return !string.IsNullOrWhiteSpace(name)
                ? typeMatches && tag.Name == name
                : typeMatches;
        });

        return (T) tag;
    }
}