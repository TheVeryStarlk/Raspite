using System.Diagnostics;

namespace Raspite.Tags;

/// <summary>
/// Represents a named binary tag (NBT).
/// </summary>
/// <param name="name">The tag's name.</param>
[DebuggerDisplay("{Name}")]
public abstract class Tag(string name = "")
{
    /// <summary>
    /// The tag's identifier.
    /// </summary>
    public abstract byte Identifier { get; }

    /// <summary>
    /// The tag's name.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// The identifier of the end tag.
    /// </summary>
    /// <remarks>
    /// Used to close a <see cref="Tag.Compound"/>, or as an identifier in a <see cref="Tag.List"/>.
    /// </remarks>
    public const byte End = 0;

    /// <summary>
    /// The identifier of a <see cref="ByteTag"/>.
    /// </summary>
    public const byte Byte = 1;

    /// <summary>
    /// The identifier of a <see cref="ShortTag"/>.
    /// </summary>
    public const byte Short = 2;

    /// <summary>
    /// The identifier of a <see cref="IntegerTag"/>.
    /// </summary>
    public const byte Integer = 3;

    /// <summary>
    /// The identifier of a <see cref="LongTag"/>.
    /// </summary>
    public const byte Long = 4;

    /// <summary>
    /// The identifier of a <see cref="FloatTag"/>.
    /// </summary>
    public const byte Float = 5;

    /// <summary>
    /// The identifier of a <see cref="DoubleTag"/>.
    /// </summary>
    public const byte Double = 6;

    /// <summary>
    /// The identifier of a <see cref="ByteCollectionTag"/>.
    /// </summary>
    public const byte ByteCollection = 7;

    /// <summary>
    /// The identifier of a <see cref="StringTag"/>.
    /// </summary>
    public const byte String = 8;

    /// <summary>
    /// The identifier of a <see cref="ListTag"/>.
    /// </summary>
    public const byte List = 9;

    /// <summary>
    /// The identifier of a <see cref="CompoundTag"/>.
    /// </summary>
    public const byte Compound = 10;

    /// <summary>
    /// The identifier of a <see cref="IntegerCollectionTag"/>.
    /// </summary>
    public const byte IntegerCollection = 11;

    /// <summary>
    /// The identifier of a <see cref="LongCollectionTag"/>.
    /// </summary>
    public const byte LongCollection = 12;
}

/// <summary>
/// Represents a named binary tag (NBT) that holds a value.
/// </summary>
/// <param name="value">The tag's value.</param>
/// <param name="name">The tag's name.</param>
/// <typeparam name="T">The tag's value type.</typeparam>
[DebuggerDisplay("{Name} = {Value}")]
public abstract class Tag<T>(T value, string name = "") : Tag(name)
{
    /// <summary>
    /// The tag's value.
    /// </summary>
    public T Value { get; set; } = value;
}