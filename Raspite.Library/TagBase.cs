namespace Raspite.Library;

public abstract record TagBase(string? Name = null)
{
    public sealed record End : TagBase;

    public sealed record Byte(byte Value, string? Name = null) : TagBase(Name);

    public sealed record Short(short Value, string? Name = null) : TagBase(Name);

    public sealed record Int(int Value, string? Name = null) : TagBase(Name);

    public sealed record Long(long Value, string? Name = null) : TagBase(Name);

    public sealed record Float(float Value, string? Name = null) : TagBase(Name);

    public sealed record Double(double Value, string? Name = null) : TagBase(Name);

    public sealed record ByteArray(byte[] Values, string? Name = null) : TagBase(Name);

    public sealed record String(string Value, string? Name = null) : TagBase(Name);

    public sealed record List(TagBase[] Children, string? Name = null) : TagBase(Name);

    public sealed record Compound(TagBase[] Children, string? Name = null) : TagBase(Name);

    public sealed record IntArray(int[] Values, string? Name = null) : TagBase(Name);

    public sealed record LongArray(long[] Values, string? Name = null) : TagBase(Name);
}