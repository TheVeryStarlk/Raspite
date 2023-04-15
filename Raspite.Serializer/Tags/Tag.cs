namespace Raspite.Serializer.Tags;

/// <summary>
/// Represents the base class of a Minecraft tag.
/// </summary>
public abstract class Tag
{
    /// <summary>
    /// Represents the tag's name.
    /// </summary>
    /// <remarks>
    /// Tags may not have a name when inside a <see cref="ListTag{T}"/>.
    /// </remarks>
    public string Name { get; set; } = "";

    /// <summary>
    /// Represents the tag's type. Used for serializing.
    /// </summary>
    internal abstract byte Type { get; }
}

/// <summary>
/// Represents a Minecraft tag that stores a value.
/// </summary>
/// <typeparam name="T">The value's type.</typeparam>
public abstract class Tag<T> : Tag
{
    /// <summary>
    /// Represents the tag's value.
    /// </summary>
    public required T Value { get; set; }
}

/// <summary>
/// Represents a Minecraft tag that stores other <see cref="Tag"/>(s).
/// </summary>
/// <typeparam name="T">The value's type.</typeparam>
public abstract class CollectionTag<T> : Tag
{
    /// <summary>
    /// Represents the stored tags.
    /// </summary>
    public required T[] Children { get; set; }

    public T this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }
}