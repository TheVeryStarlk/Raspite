namespace Raspite.Serializer.Tags;

public sealed record CompoundTag(Tag[] Children, string Name = "") : CollectionTag<Tag>
{
    public override byte Identifier => 10;

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