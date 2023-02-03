namespace Raspite.Library;

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

    protected object? InternalValue { get; set; }

    /// <summary>
    /// Tries to get the value of the tag.
    /// </summary>
    /// <example>
    /// <code>
    /// _ = compoundTag.TryGetValue&lt;IEnumerable&lt;TagBase&gt;&gt;(out var value);
    /// </code>
    /// </example>
    /// <param name="value">The tag's value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>Whether the operation has succeeded (true) or not (false).</returns>
    /// <exception cref="ArgumentOutOfRangeException">The tag is unknown.</exception>
    public bool TryGetValue<T>(out T? value)
    {
        try
        {
            value = InternalValue is IConvertible
                ? (T) Convert.ChangeType(InternalValue, typeof(T))
                : (T?) InternalValue;

            return true;
        }
        catch (InvalidCastException)
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Tries to set the value of the tag.
    /// </summary>
    /// <example>
    /// <code>
    /// _ = compoundTag.TrySetValue(Array.Empty&lt;TagBase&gt;());
    /// </code>
    /// </example>
    /// <param name="value">The value to set to the tag.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>Whether the operation has succeeded (true) or not (false).</returns>
    /// <exception cref="ArgumentOutOfRangeException">The tag is unknown.</exception>
    public bool TrySetValue<T>(T value) where T : notnull
    {
        var requestedType = typeof(T);

        if (requestedType != InternalValue?.GetType())
        {
            return false;
        }

        InternalValue = InternalValue is IConvertible
            ? (T) Convert.ChangeType(value, requestedType)
            : value;

        return true;
    }
}

/// <inheritdoc />
public abstract class CollectionTagBase : TagBase
{
    public T First<T>(string? name = null) where T : TagBase
    {
        var collection = (IEnumerable<TagBase>) InternalValue!;
        return (T) collection.First(tag =>
        {
            var typeMatches = typeof(T) == tag.GetType();

            return name is not null
                ? typeMatches && tag.Name == name
                : typeMatches;
        });
    }
}

/// <inheritdoc />
public abstract class EndTag : TagBase
{
}

/// <inheritdoc />
public sealed class ByteTag : TagBase
{
    public required byte Value
    {
        get => (byte) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class ShortTag : TagBase
{
    public required short Value { get; set; }
}

/// <inheritdoc />
public sealed class IntTag : TagBase
{
    public required int Value
    {
        get => (int) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class LongTag : TagBase
{
    public required long Value
    {
        get => (long) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class FloatTag : TagBase
{
    public required float Value
    {
        get => (float) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class DoubleTag : TagBase
{
    public required double Value
    {
        get => (double) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class ByteArrayTag : TagBase
{
    public required IEnumerable<byte> Value
    {
        get => (IEnumerable<byte>) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class StringTag : TagBase
{
    public required string Value
    {
        get => (string) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class ListTag : CollectionTagBase
{
    public required IEnumerable<TagBase> Value
    {
        get => (IEnumerable<TagBase>) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class CompoundTag : CollectionTagBase
{
    public required IEnumerable<TagBase> Value
    {
        get => (IEnumerable<TagBase>) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class IntArrayTag : TagBase
{
    public required IEnumerable<int> Value
    {
        get => (IEnumerable<int>) InternalValue!;
        set => InternalValue = value;
    }
}

/// <inheritdoc />
public sealed class LongArrayTag : TagBase
{
    public required IEnumerable<long> Value
    {
        get => (IEnumerable<long>) InternalValue!;
        set => InternalValue = value;
    }
}