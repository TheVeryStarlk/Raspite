namespace Raspite.Library;

/// <summary>
/// Represents a Minecraft tag.
/// </summary>
public abstract class NbtTag
{
    /// <summary>
    /// Represents the name of the tag.
    /// </summary>
    /// <remarks>
    /// The name can be null when inside a <see cref="NbtTag.List"/>.
    /// </remarks>
    public string? Name { get; set; }

    public sealed class End : NbtTag
    {
    }

    public abstract class ValueBase : NbtTag
    {
        public required object Value { get; set; }
    }

    public sealed class Byte : ValueBase
    {
    }

    public sealed class Short : ValueBase
    {
    }

    public sealed class Int : ValueBase
    {
    }

    public sealed class Long : ValueBase
    {
    }

    public sealed class Float : ValueBase
    {
    }

    public sealed class Double : ValueBase
    {
    }

    public sealed class ByteArray : NbtTag
    {
        public required byte[] Values { get; set; }
    }

    public sealed class String : ValueBase
    {
    }

    public sealed class List : NbtTag
    {
        public required IEnumerable<NbtTag> Children { get; set; }
    }

    public sealed class Compound : NbtTag
    {
        public required IEnumerable<NbtTag> Children { get; set; }
    }

    public sealed class IntArray : NbtTag
    {
        public required int[] Values { get; set; }
    }

    public sealed class LongArray : NbtTag
    {
        public required long[] Values { get; set; }
    }
}