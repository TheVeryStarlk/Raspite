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

    public sealed class Byte : NbtTag
    {
        public required byte Value { get; set; }
    }

    public sealed class Short : NbtTag
    {
        public required short Value { get; set; }
    }

    public sealed class Int : NbtTag
    {
        public required int Value { get; set; }
    }

    public sealed class Long : NbtTag
    {
        public required long Value { get; set; }
    }

    public sealed class Float : NbtTag
    {
        public required float Value { get; set; }
    }

    public sealed class Double : NbtTag
    {
        public required double Value { get; set; }
    }

    public sealed class ByteArray : NbtTag
    {
        public required byte[] Values { get; set; }
    }

    public sealed class String : NbtTag
    {
        public required string Value { get; set; }
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