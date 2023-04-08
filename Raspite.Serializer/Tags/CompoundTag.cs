namespace Raspite.Serializer.Tags;

public sealed class CompoundTag : CollectionTag<Tag>
{
    internal override byte Type => 10;

    public T First<T>(string? name = null) where T : Tag
    {
        var tag = Children.First(tag =>
        {
            var typeMatches = typeof(T) == tag.GetType();

            return name is not null
                ? typeMatches && tag.Name == name
                : typeMatches;
        });

        return (T) tag;
    }
}