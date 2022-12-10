namespace Raspite.Library;

/// <summary>
/// Represents a Minecraft tag.
/// </summary>
/// <param name="Name">The name of the tag.</param>
public abstract record NbtTag(string? Name = null)
{
    public sealed record End : NbtTag;

    public sealed record Byte(byte Value, string? Name = null) : NbtTag(Name);

    public sealed record Short(short Value, string? Name = null) : NbtTag(Name);

    public sealed record Int(int Value, string? Name = null) : NbtTag(Name);

    public sealed record Long(long Value, string? Name = null) : NbtTag(Name);

    public sealed record Float(float Value, string? Name = null) : NbtTag(Name);

    public sealed record Double(double Value, string? Name = null) : NbtTag(Name);

    public sealed record ByteArray(byte[] Values, string? Name = null) : NbtTag(Name);

    public sealed record String(string Value, string? Name = null) : NbtTag(Name);

    public sealed record List(NbtTag[] Children, string? Name = null) : NbtTag(Name);

    public sealed record Compound(NbtTag[] Children, string? Name = null) : NbtTag(Name);

    public sealed record IntArray(int[] Values, string? Name = null) : NbtTag(Name);

    public sealed record LongArray(long[] Values, string? Name = null) : NbtTag(Name);
}