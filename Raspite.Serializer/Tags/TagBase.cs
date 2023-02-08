namespace Raspite.Serializer.Tags;

/// <summary>
/// Represents a Minecraft tag.
/// </summary>
public abstract class TagBase
{
    /// <summary>
    /// Represents a Minecraft tag type/ID.
    /// </summary>
    public enum Type
    {
        End,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        ByteArray,
        String,
        List,
        Compound,
        IntArray,
        LongArray
    }

    /// <summary>
    /// Represents the name of the tag.
    /// </summary>
    /// <remarks>
    /// Tags do not have a name when inside a <see cref="ListTag"/>.
    /// </remarks>
    public string? Name { get; set; }

    public T? GetValue<T>()
    {
        var type = GetType();

        if (typeof(EndTag) == type)
        {
            throw new InvalidOperationException("End tags do not hold value.");
        }

        var value = type.GetProperty("Value")?.GetValue(this);
        return (T?) value;
    }

    public void SetValue<T>(T value)
    {
        var type = GetType();

        if (typeof(EndTag) == type)
        {
            throw new InvalidOperationException("End tags do not hold value.");
        }

        type.GetProperty("Value")?.SetValue(this, value);
    }
}

/// <inheritdoc />
public abstract class CollectionTagBase : TagBase
{
    public T First<T>(string? name = null) where T : TagBase
    {
        var collection = GetValue<IEnumerable<TagBase>>();

        return (T) collection!.First(tag =>
        {
            var typeMatches = typeof(T) == tag.GetType();

            return name is not null
                ? typeMatches && tag.Name == name
                : typeMatches;
        });
    }
}